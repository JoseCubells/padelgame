using System;
using System.Collections.Generic;
using Padel.Core;
using Padel.Rules;
using Padel.Simulation.Ball;
using Padel.Simulation.Court;
using Padel.Simulation.Match;
using Padel.Simulation.Shots;

namespace Padel.AI
{
    public enum Formation
    {
        Back = 0,
        Transition = 1,
        Net = 2,
    }

    /// <summary>Where and when a player can meet the ball.</summary>
    public struct Interception
    {
        public bool Found;
        public Vec2 Point;
        public float Height;
        public float Time;
        public bool AfterBounce;
    }

    /// <summary>
    /// Pair-level reasoning (ADR-006): formation, who takes the ball (shortest interception time), and positional
    /// slots keeping partners about 3.5 m apart ("the rope"). Works unchanged for 1v1 (team of one).
    /// Uses the exact ball simulation for prediction, so the AI does not cheat with privileged physics.
    /// </summary>
    public sealed class TeamBrain
    {
        public const float RopeHalfWidth = 1.75f;
        private const float Horizon = 2.5f;

        private readonly MatchSimulation _sim;
        private readonly TeamId _team;
        private readonly TrajectoryPredictor _predictor = new TrajectoryPredictor();
        private readonly List<BallState> _samples = new List<BallState>(320);
        private readonly List<PredictedEvent> _events = new List<PredictedEvent>(16);
        private long _cachedTick = -1;
        private long _predictedTick = -1;
        private int _cachedOwner = -1;
        private Formation _formation = Formation.Back;

        public TeamBrain(MatchSimulation sim, TeamId team)
        {
            _sim = sim ?? throw new ArgumentNullException(nameof(sim));
            _team = team;
        }

        public TeamId Team => _team;
        public Formation Formation => _formation;

        /// <summary>Formation depth from the net (m) [P].</summary>
        public static float Depth(Formation f) => f == Formation.Net ? 3.2f : (f == Formation.Transition ? 6.2f : 8.8f);

        /// <summary>True if the ball is travelling towards (or is on) our side and we must play it next.</summary>
        public bool BallIsOurs(MatchState s)
        {
            if (!s.BallLive) return false;
            if (s.Phase != PointPhase.Rally && s.Phase != PointPhase.ServeToss) return false;
            return s.LastHitter >= 0 && s.TeamOf(s.LastHitter) != _team;
        }

        /// <summary>Player of this team who should take the ball this tick, or -1.</summary>
        public int Owner(MatchState s, float reactionMargin)
        {
            if (_cachedTick == s.Tick) return _cachedOwner;
            UpdateFormation(s);
            int owner = -1;
            if (BallIsOurs(s))
            {
                if (s.Referee.IsServing)
                {
                    owner = s.IndexOf(ScoreKeeper.CurrentReceiver(s.Score, _sim.Config.Scoring)); // only the receiver returns
                }
                else if (TeammateSwinging(s, out int swinging))
                {
                    owner = swinging; // never switch owner once a stroke is under way (avoids double hits)
                }
                else
                {
                    float best = float.MaxValue;
                    for (int p = 0; p < s.Players.Length; p++)
                    {
                        if (s.TeamOf(p) != _team) continue;
                        Interception i = FindInterception(s, p, reactionMargin, serveReturn: false);
                        float score = i.Found ? i.Time : 100f + (i.Point - s.Players[p].Position).Magnitude;
                        if (score < best) { best = score; owner = p; }
                    }
                }
            }
            _cachedTick = s.Tick;
            _cachedOwner = owner;
            return owner;
        }

        private bool TeammateSwinging(MatchState s, out int player)
        {
            for (int p = 0; p < s.Players.Length; p++)
            {
                if (s.TeamOf(p) == _team && s.Strokes[p].Active) { player = p; return true; }
            }
            player = -1;
            return false;
        }

        /// <summary>
        /// Earliest point on our half where the ball is at a playable height and player <paramref name="p"/> can
        /// get there in time (running at 90 % of top speed after <paramref name="reactionMargin"/>).
        /// </summary>
        public Interception FindInterception(MatchState s, int p, float reactionMargin, bool serveReturn)
        {
            var config = _sim.Config;
            if (_predictedTick != s.Tick)
            {
                _predictor.Predict(s.Ball, config.Ball, config.Court, config.TimeStep, Horizon, _samples, _events);
                _predictedTick = s.Tick;
            }
            int half = s.HalfSign(_team);
            Vec2 pos = s.Players[p].Position;
            float speed = config.Players[p].MaxSpeed * 0.9f;
            float reach = config.Players[p].Reach;

            int bounces = s.BouncedSinceHit ? 1 : 0;
            int eventIndex = 0;
            var fallback = new Interception();
            float fallbackLateness = float.MaxValue;

            for (int k = 0; k < _samples.Count; k++)
            {
                float t = (k + 1) * config.TimeStep;
                while (eventIndex < _events.Count && _events[eventIndex].Time <= t)
                {
                    BallEvent e = _events[eventIndex].Event;
                    if (e.Kind == BallEventKind.Contact && e.Surface == SurfaceKind.Floor && (e.Position.Z < 0f ? -1 : 1) == half) bounces++;
                    eventIndex++;
                }
                if (bounces >= 2) break; // point already lost past this sample

                Vec3 b = _samples[k].Position;
                if ((b.Z < 0f ? -1 : 1) != half) continue;
                float depth = Math.Abs(b.Z);
                bool afterBounce = bounces == 1;
                bool playable =
                    (afterBounce && b.Y >= 0.45f && b.Y <= 1.5f) ||
                    (!afterBounce && !serveReturn && !s.Referee.IsServing && depth < 5.5f && b.Y >= 0.7f && b.Y <= 1.7f) ||
                    (!afterBounce && !serveReturn && !s.Referee.IsServing && depth < 6.5f && b.Y >= 2.3f && b.Y <= 3.0f);
                if (!playable) continue;

                Vec2 point = new Vec2(b.X, b.Z);
                float need = Math.Max(0f, (point - pos).Magnitude - reach * 0.7f) / speed + reactionMargin;
                if (need <= t)
                {
                    return new Interception { Found = true, Point = point, Height = b.Y, Time = t, AfterBounce = afterBounce };
                }
                float lateness = need - t;
                if (lateness < fallbackLateness)
                {
                    fallbackLateness = lateness;
                    fallback = new Interception { Found = false, Point = point, Height = b.Y, Time = t, AfterBounce = afterBounce };
                }
            }
            return fallback;
        }

        /// <summary>Positional slot for a player not taking the ball.</summary>
        public Vec2 Slot(MatchState s, int p)
        {
            int half = s.HalfSign(_team);
            float depth = Depth(_formation);
            float ballX = s.BallLive ? s.Ball.Position.X : 0f;
            if (s.PlayersPerTeam == 1) return new Vec2(Clamp(ballX * 0.5f, -3f, 3f), depth * half);

            // Player 0 covers the right (drive) side, player 1 the left, as seen facing the net.
            int rightSign = -half;
            int side = (p % s.PlayersPerTeam) == 0 ? rightSign : -rightSign;
            float centre = Clamp(ballX * 0.35f, -1.5f, 1.5f);
            return new Vec2(centre + side * RopeHalfWidth, depth * half);
        }

        private void UpdateFormation(MatchState s)
        {
            if (s.Phase == PointPhase.PreServe || s.Phase == PointPhase.ServeToss)
            {
                bool serving = s.TeamOf(_sim.ServerIndex(s)) == _team;
                _formation = serving ? Formation.Transition : Formation.Back;
                return;
            }
            if (s.LastHitter < 0) return;

            bool weHit = s.TeamOf(s.LastHitter) == _team;
            ShotType shot = s.LastShot;
            if (weHit)
            {
                switch (shot)
                {
                    case ShotType.Lob:
                    case ShotType.Volley:
                    case ShotType.DropVolley:
                    case ShotType.Bandeja:
                    case ShotType.Vibora:
                    case ShotType.Smash:
                    case ShotType.Serve:
                        _formation = Formation.Net;
                        break;
                    case ShotType.Chiquita:
                        _formation = Formation.Transition;
                        break;
                }
            }
            else if (shot == ShotType.Lob)
            {
                _formation = Formation.Back; // the lob pushes us back to defend behind the glass
            }
        }

        private static float Clamp(float v, float min, float max) => v < min ? min : (v > max ? max : v);
    }
}
