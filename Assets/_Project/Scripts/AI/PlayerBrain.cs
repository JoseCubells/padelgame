using System;
using Padel.Core;
using Padel.Simulation.Court;
using Padel.Simulation.Match;
using Padel.Simulation.Players;
using Padel.Simulation.Shots;

namespace Padel.AI
{
    /// <summary>
    /// Per-player AI (ADR-006): serves, reacts to the opponent's stroke after a reaction delay, runs to the
    /// interception point (with seeded prediction noise), times the swing with the same "ideal contact" the simulation
    /// grades against, and picks the shot by utility + softmax. Produces a <see cref="PlayerCommand"/> like a human.
    /// Its random stream is seeded per player, so AI-vs-AI matches are reproducible.
    /// </summary>
    public sealed class PlayerBrain : ICommandSource
    {
        private readonly MatchSimulation _sim;
        private readonly TeamBrain _team;
        private readonly AIProfile _profile;
        private Pcg32 _rng;

        private long _seenStrokeTick = long.MinValue;
        private long _reactAtTick;
        private Vec2 _noiseOffset;
        private float _timingOffset;
        private Vec2 _target;

        public PlayerBrain(MatchSimulation sim, TeamBrain team, AIProfile profile, ulong seed)
        {
            _sim = sim ?? throw new ArgumentNullException(nameof(sim));
            _team = team ?? throw new ArgumentNullException(nameof(team));
            _profile = profile ?? throw new ArgumentNullException(nameof(profile));
            _rng = new Pcg32(seed, 1013UL);
        }

        public AIProfile Profile => _profile;

        public PlayerCommand GetCommand(MatchState s, int p)
        {
            var cmd = new PlayerCommand();
            PlayerState me = s.Players[p];

            if (s.Phase == PointPhase.PreServe)
            {
                if (_sim.ServerIndex(s) == p && s.PhaseTime >= _profile.ServeDelay)
                {
                    cmd.Pressed = ShotIntent.Control;
                    cmd.Aim = new Vec2(Gaussian(0.3f), Gaussian(0.3f));
                }
                return cmd;
            }

            ObserveStroke(s);
            float dt = _sim.Dt;
            bool reacted = s.Tick >= _reactAtTick;
            int owner = _team.Owner(s, _profile.ReactionTime);

            if (owner == p && reacted)
            {
                Interception i = _team.FindInterception(s, p, 0f, serveReturn: s.Referee.IsServing);
                if (i.Point.SqrMagnitude > 0f || i.Found)
                {
                    // Stand so the ball passes beside the body, within comfortable reach, with prediction noise.
                    float side = me.Position.X >= i.Point.X ? 0.45f : -0.45f;
                    _target = i.Point + new Vec2(side, 0f) + _noiseOffset;
                }
                // Only evaluate the swing (a full prediction) when the ball is about to arrive.
                if (i.Found && i.Time <= 0.9f) TryStrike(s, p, ref cmd, dt);
            }
            else if (owner != p)
            {
                _target = _team.Slot(s, p);
            }

            cmd.Move = MoveTowards(me.Position, _target);
            if (me.Phase != PlayerPhase.Free) cmd.Move = Vec2.Zero;
            return cmd;
        }

        private void ObserveStroke(MatchState s)
        {
            if (s.LastStrokeTick == _seenStrokeTick) return;
            _seenStrokeTick = s.LastStrokeTick;
            _reactAtTick = s.LastStrokeTick + (long)MathF.Round(_profile.ReactionTime / _sim.Dt);
            _noiseOffset = new Vec2(Gaussian(_profile.PredictionNoise), Gaussian(_profile.PredictionNoise));
            // Pressure: a fast incoming ball makes the timing less reliable (GAMEPLAY_RESEARCH §5.4).
            float incoming = s.BallLive ? s.Ball.Velocity.Magnitude : 0f;
            float pressure = Math.Min(1f, Math.Max(0f, (incoming - 10f) / 15f));
            float multiplier = 1f + (_profile.PressureErrorMultiplier - 1f) * pressure;
            _timingOffset = Gaussian(_profile.TimingNoise * multiplier);
        }

        private void TryStrike(MatchState s, int p, ref PlayerCommand cmd, float dt)
        {
            if (s.Players[p].Phase != PlayerPhase.Free || s.Strokes[p].Active) return;
            long ideal = _sim.PredictIdealContactTick(s, p, out float height, out bool bouncesFirst);
            if (ideal < 0) return;
            bool willBounce = bouncesFirst || s.BouncedSinceHit;
            if (s.Referee.IsServing && !willBounce) return; // the serve must bounce before the return

            (ShotIntent intent, bool touch, Vec2 aim) = ChooseShot(s, p, height, willBounce);
            var press = new PlayerCommand
            {
                Pressed = intent,
                Held = intent == ShotIntent.Attack && height >= ShotResolver.OverheadHeight,
                Touch = touch,
                Aim = aim,
            };
            ShotDefinition shot = _sim.ProvisionalShot(s, press, height, bouncesFirst);
            float timeToIdeal = (ideal - s.Tick) * dt;
            if (timeToIdeal > shot.WindupTime + _timingOffset) return;

            cmd.Pressed = press.Pressed;
            cmd.Held = press.Held;
            cmd.Touch = press.Touch;
            cmd.Aim = press.Aim;
        }

        /// <summary>Utility over (intent, aim) candidates, then softmax with the profile temperature (ADR-006).</summary>
        private (ShotIntent, bool, Vec2) ChooseShot(MatchState s, int p, float height, bool bounced)
        {
            float myDepth = Math.Abs(s.Players[p].Position.Y);
            float oppDepth = 0f, oppX = 0f;
            int opponents = 0;
            for (int o = 0; o < s.Players.Length; o++)
            {
                if (s.TeamOf(o) == s.TeamOf(p)) continue;
                oppDepth += Math.Abs(s.Players[o].Position.Y);
                oppX += s.Players[o].Position.X;
                opponents++;
            }
            oppDepth /= opponents;
            oppX /= opponents;
            bool opponentsAtNet = oppDepth < 5f;
            bool overhead = !bounced && height >= ShotResolver.OverheadHeight;

            // Candidates: intent x aim-x. Aim-x in court coordinates is relative to the opponent's view; we only need
            // "away from the opponents' centre".
            Span<float> utilities = stackalloc float[9];
            ShotIntent[] intents = { ShotIntent.Attack, ShotIntent.Control, ShotIntent.Lob };
            float[] aimsX = { -0.75f, 0f, 0.75f };
            for (int i = 0; i < 3; i++)
            for (int a = 0; a < 3; a++)
            {
                ShotIntent intent = intents[i];
                float targetX = aimsX[a] * (_sim.Config.Court.Config.HalfWidth - 1.2f);
                float safety, pressure, position;
                switch (intent)
                {
                    case ShotIntent.Attack:
                        safety = overhead ? (myDepth < 4f ? 0.5f : 0.1f) : (bounced ? 0.6f : 0.7f);
                        pressure = overhead ? 1f : (opponentsAtNet ? 0.5f : 0.7f);
                        position = 0.3f;
                        break;
                    case ShotIntent.Control:
                        safety = overhead ? 0.9f : 0.8f;
                        pressure = 0.4f;
                        position = overhead ? 0.7f : 0.4f;
                        break;
                    default:
                        safety = overhead ? 0.3f : 0.7f;
                        pressure = opponentsAtNet ? 0.8f : 0.1f;
                        position = opponentsAtNet ? 1f : 0.2f;
                        break;
                }
                // Hitting away from the opponents adds pressure; the centre is safer.
                float away = Math.Abs(targetX - oppX) / 8f;
                pressure += 0.4f * away;
                safety -= a == 1 ? 0f : 0.1f;
                utilities[i * 3 + a] = _profile.SafetyWeight * safety + _profile.PressureWeight * pressure + _profile.PositionWeight * position;
            }

            int choice = Softmax(utilities, _profile.Temperature);
            ShotIntent chosen = intents[choice / 3];
            float aimX = aimsX[choice % 3];
            // Aim is interpreted in court coordinates by the simulation (x across the court).
            Vec2 aim = new Vec2(aimX + Gaussian(_profile.AimNoise), Gaussian(_profile.AimNoise));
            return (chosen, false, aim);
        }

        private int Softmax(Span<float> utilities, float temperature)
        {
            float max = float.MinValue;
            for (int i = 0; i < utilities.Length; i++) max = Math.Max(max, utilities[i]);
            float sum = 0f;
            Span<float> w = stackalloc float[utilities.Length];
            for (int i = 0; i < utilities.Length; i++) { w[i] = MathF.Exp((utilities[i] - max) / temperature); sum += w[i]; }
            float r = _rng.NextFloat() * sum;
            for (int i = 0; i < w.Length; i++) { r -= w[i]; if (r <= 0f) return i; }
            return w.Length - 1;
        }

        private static Vec2 MoveTowards(Vec2 from, Vec2 to)
        {
            Vec2 d = to - from;
            float dist = d.Magnitude;
            if (dist < 0.08f) return Vec2.Zero;
            return d / dist * Math.Min(1f, dist / 0.6f);
        }

        private float Gaussian(float sigma) => sigma <= 0f ? 0f : _rng.NextGaussian() * sigma;
    }
}
