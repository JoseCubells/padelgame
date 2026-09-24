using System;
using System.Collections.Generic;
using Padel.Core;
using Padel.Rules;
using Padel.Simulation.Ball;
using Padel.Simulation.Court;
using Padel.Simulation.Players;
using Padel.Simulation.Shots;

namespace Padel.Simulation.Match
{
    /// <summary>
    /// Deterministic match step (ARCHITECTURE §4): players, strokes, ball, referee and score advance together at a
    /// fixed tick from a state and one <see cref="PlayerCommand"/> per player. Holds only immutable config and scratch
    /// buffers; all mutable data lives in <see cref="MatchState"/>. The same code runs 1v1 and 2v2.
    /// </summary>
    public sealed class MatchSimulation
    {
        /// <summary>Lowest and highest ball-centre heights a player can strike (m) [P].</summary>
        public const float MinContactHeight = 0.1f;
        public const float MaxContactHeight = 3.3f;
        private const float PredictionHorizon = 1.5f;
        /// <summary>Extra reach allowed on late swings (m) [P].</summary>
        public const float LateReachBonus = 0.25f;

        private readonly MatchConfig _config;
        private readonly TrajectoryPredictor _predictor = new TrajectoryPredictor();
        private readonly ShotSolver _solver;
        private readonly List<BallEvent> _ballEvents = new List<BallEvent>(8);
        private readonly List<BallState> _samples = new List<BallState>(256);
        private readonly List<PredictedEvent> _predictedEvents = new List<PredictedEvent>(16);

        public MatchConfig Config => _config;
        public float Dt => _config.TimeStep;

        public MatchSimulation(MatchConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _solver = new ShotSolver(config.Ball, config.Court, config.TimeStep);
        }

        public MatchState CreateInitialState()
        {
            int n = _config.PlayerCount;
            var state = new MatchState
            {
                Rng = new Pcg32(_config.Seed),
                Players = new PlayerState[n],
                Strokes = new StrokeState[n],
                Score = ScoreKeeper.NewMatch(_config.Scoring, _config.Setup),
                Referee = new PointReferee(),
            };
            StartPoint(state, null);
            return state;
        }

        public int ServerIndex(MatchState s) => s.IndexOf(ScoreKeeper.CurrentServer(s.Score));
        public ServeSide CurrentServeSide(MatchState s) => ScoreKeeper.CurrentServeSide(s.Score, _config.Scoring);

        public void Step(MatchState s, PlayerCommand[] commands, List<MatchEvent> events)
        {
            if (commands == null || commands.Length != s.Players.Length) throw new ArgumentException("One command per player is required.", nameof(commands));
            float dt = Dt;
            s.Tick++;
            s.PhaseTime += dt;

            switch (s.Phase)
            {
                case PointPhase.MatchOver:
                    MovePlayers(s, commands, dt, lockServer: false);
                    return;
                case PointPhase.ServePause:
                    if (s.PhaseTime >= _config.FaultPause) RearmServe(s);
                    break;
                case PointPhase.PointOver:
                    if (s.PhaseTime >= _config.PointPause) StartPoint(s, events);
                    break;
            }

            HandlePresses(s, commands, events);
            MovePlayers(s, commands, dt, lockServer: s.Phase == PointPhase.PreServe || s.Phase == PointPhase.ServeToss);
            AdvanceStrokes(s, commands, events);

            if (s.BallLive)
            {
                _ballEvents.Clear();
                BallSimulator.Step(ref s.Ball, _config.Ball, _config.Court, dt, _ballEvents);
                ProcessBallEvents(s, events);
            }

            if (s.Phase == PointPhase.ServeToss) TryServeContact(s, events);

            if ((s.Phase == PointPhase.Rally || s.Phase == PointPhase.ServeToss) && s.PhaseTime > _config.RallyTimeout)
            {
                HandleCall(s, new RefereeCall { Kind = CallKind.Let, Reason = PointReason.RallyTimeout }, events);
            }
        }

        // ------------------------------------------------------------------ strokes

        private void HandlePresses(MatchState s, PlayerCommand[] commands, List<MatchEvent> events)
        {
            int server = ServerIndex(s);
            for (int p = 0; p < s.Players.Length; p++)
            {
                PlayerCommand cmd = commands[p];
                if (cmd.Pressed == ShotIntent.None) continue;

                if (s.Phase == PointPhase.PreServe)
                {
                    if (p == server) StartServeToss(s, p, cmd, events);
                    continue;
                }
                if (s.Phase != PointPhase.Rally && s.Phase != PointPhase.ServeToss) continue;
                if (s.Strokes[p].Active || s.Players[p].Phase != PlayerPhase.Free || !s.BallLive) continue;
                BeginStroke(s, p, cmd);
            }
        }

        private void BeginStroke(MatchState s, int p, PlayerCommand cmd)
        {
            long ideal = PredictIdealContactTick(s, p, out float idealHeight, out bool bounceFirst);
            if (ideal < 0) return; // the ball will not come near this player: ignore the press

            ShotDefinition provisional = ProvisionalShot(s, cmd, idealHeight, bounceFirst);
            long windupTicks = Ticks(provisional.WindupTime);
            long swingReady = s.Tick + windupTicks;
            // Early presses (up to MaxEarlyPress) hold the swing until the ball arrives and are graded early;
            // late presses swing late and may miss (ADR-007 input buffer / charge).
            if (ideal - swingReady > Ticks(_config.MaxEarlyPress)) return;
            long contact = Math.Max(swingReady, ideal);

            s.Strokes[p] = new StrokeState
            {
                Active = true, Intent = cmd.Pressed, Touch = cmd.Touch, Aim = cmd.Aim,
                PressTick = s.Tick, ContactTick = contact, IdealTick = ideal,
                TimingOffset = (swingReady - ideal) * Dt,
            };
            s.Players[p].Phase = PlayerPhase.Windup;
            s.Players[p].PhaseTime = 0f;
        }

        /// <summary>
        /// Technique expected for a press, from the predicted contact. Its windup decides when the swing is ready.
        /// Public so AI controllers time presses with exactly the same windup (single source of truth).
        /// </summary>
        public ShotDefinition ProvisionalShot(MatchState s, PlayerCommand cmd, float contactHeight, bool bouncesFirst)
        {
            var context = new ShotContext
            {
                Intent = cmd.Pressed, Touch = cmd.Touch, Bounced = bouncesFirst || s.BouncedSinceHit,
                ContactHeight = contactHeight, Charge = cmd.Held ? 1f : 0f,
            };
            return _config.Shots.Get(ShotResolver.Resolve(context));
        }

        /// <summary>
        /// Predicts the tick at which the ball passes closest to player <paramref name="p"/> at a hittable height on
        /// their half, using the exact simulation (ADR-004). Returns -1 if it never gets within stretch reach.
        /// Public so AI controllers use the same notion of "ideal contact" as the timing grader.
        /// </summary>
        public long PredictIdealContactTick(MatchState s, int p, out float height, out bool bouncesFirst)
        {
            height = 0f;
            bouncesFirst = false;
            if (!s.BallLive) return -1;

            _predictor.Predict(s.Ball, _config.Ball, _config.Court, Dt, PredictionHorizon, _samples, _predictedEvents);
            TeamId team = s.TeamOf(p);
            int half = s.HalfSign(team);
            // A player who presses while running keeps sliding until braked: evaluate reach where they will stop.
            PlayerState player = s.Players[p];
            float speed = player.Velocity.Magnitude;
            Vec2 pos = player.Position + (speed > 1e-4f
                ? player.Velocity * (speed / (2f * _config.Players[p].Deceleration))
                : Vec2.Zero);
            float reach = _config.Players[p].StretchReach;

            int best = -1;
            float bestDistance = float.MaxValue;
            for (int k = 0; k < _samples.Count; k++)
            {
                Vec3 b = _samples[k].Position;
                if (b.Y < MinContactHeight || b.Y > MaxContactHeight) continue;
                if ((b.Z < 0f ? -1 : 1) != half) continue;
                float d = (new Vec2(b.X, b.Z) - pos).Magnitude;
                if (d < bestDistance) { bestDistance = d; best = k; }
            }
            if (best < 0 || bestDistance > reach) return -1;

            height = _samples[best].Position.Y;
            float bestTime = (best + 1) * Dt;
            for (int i = 0; i < _predictedEvents.Count; i++)
            {
                PredictedEvent e = _predictedEvents[i];
                if (e.Time > bestTime) break;
                if (e.Event.Kind == BallEventKind.Contact && e.Event.Surface == SurfaceKind.Floor && s.TeamAtHalf(e.Event.Position.Z) == team)
                    bouncesFirst = true;
            }
            return s.Tick + best + 1;
        }

        private void AdvanceStrokes(MatchState s, PlayerCommand[] commands, List<MatchEvent> events)
        {
            for (int p = 0; p < s.Players.Length; p++)
            {
                ref PlayerState player = ref s.Players[p];
                ref StrokeState stroke = ref s.Strokes[p];

                if (player.Phase == PlayerPhase.Contact)
                {
                    player.Phase = PlayerPhase.Recovery;
                    player.PhaseTime = 0f;
                }
                else if (player.Phase == PlayerPhase.Recovery && player.PhaseTime >= RecoveryTime(stroke))
                {
                    player.Phase = PlayerPhase.Free;
                    player.PhaseTime = 0f;
                    stroke = default;
                }

                if (!stroke.Active || stroke.IsServe) continue;
                if (!stroke.Released)
                {
                    if (commands[p].Held) stroke.HeldTicks++;
                    else stroke.Released = true;
                }
                if (s.Tick < stroke.ContactTick) continue;

                if (CanReach(s, p)) ExecuteStroke(s, p, events);
                else events?.Add(new MatchEvent { Kind = MatchEventKind.Whiff, Tick = s.Tick, Player = p });
                stroke.Active = false;
                player.Phase = PlayerPhase.Contact;
                player.PhaseTime = 0f;
            }
        }

        private float RecoveryTime(in StrokeState stroke) =>
            stroke.Recovery > 0f ? stroke.Recovery : _config.Shots.Get(ShotType.Drive).RecoveryTime;

        private bool CanReach(MatchState s, int p)
        {
            if (!s.BallLive || (s.Phase != PointPhase.Rally && s.Phase != PointPhase.ServeToss)) return false;
            Vec3 b = s.Ball.Position;
            if (b.Y < MinContactHeight || b.Y > MaxContactHeight) return false;
            if (s.TeamAtHalf(b.Z) != s.TeamOf(p)) return false;
            float d = (new Vec2(b.X, b.Z) - s.Players[p].Position).Magnitude;
            // A slightly late swing still meets the ball behind the body (graded late) [P].
            float lateBonus = s.Strokes[p].TimingOffset > 0f ? LateReachBonus : 0f;
            return d <= _config.Players[p].StretchReach + lateBonus;
        }

        private void ExecuteStroke(MatchState s, int p, List<MatchEvent> events)
        {
            StrokeState stroke = s.Strokes[p];
            TeamId team = s.TeamOf(p);
            int opponentHalf = -s.HalfSign(team);
            Vec3 ball = s.Ball.Position;

            var context = new ShotContext
            {
                Intent = stroke.Intent,
                Touch = stroke.Touch,
                Charge = Math.Min(1f, stroke.HeldTicks * Dt / _config.MaxChargeTime),
                IsServe = stroke.IsServe,
                ContactHeight = ball.Y,
                Bounced = s.BouncedSinceHit,
                OffOwnWall = s.OffWallSinceBounce,
                Zone = _config.Court.ZoneOf(ball.Z),
            };
            ShotType type = ShotResolver.Resolve(context);
            ShotDefinition shot = _config.Shots.Get(type);

            ShotQuality quality = stroke.IsServe
                ? ShotQuality.Good
                : ShotTiming.Evaluate(stroke.TimingOffset, shot);

            Vec2 target = stroke.IsServe ? ServeTarget(s, stroke.Aim, shot) : RallyTarget(stroke.Aim, shot, opponentHalf);
            Vec2 contactXZ = new Vec2(ball.X, ball.Z);
            target = ShotTiming.ApplyError(contactXZ, target, shot, quality, ref s.Rng);

            s.Strokes[p].Recovery = shot.RecoveryTime;
            ShotSolution solution = _solver.Solve(shot, ball, target, context.Charge);
            s.Ball = solution.Launch;
            s.LastHitter = p;
            s.LastShot = type;
            s.LastStrokeTick = s.Tick;
            s.BouncedSinceHit = false;
            s.OffWallSinceBounce = false;

            events?.Add(new MatchEvent { Kind = MatchEventKind.Stroke, Tick = s.Tick, Player = p, Shot = type, Quality = quality, Position = ball });

            RefereeCall call = s.Referee.Process(RallyEvent.Hit(team));
            if (call.Kind != CallKind.None) { HandleCall(s, call, events); return; }
            if (s.Phase == PointPhase.ServeToss) { s.Phase = PointPhase.Rally; s.PhaseTime = 0f; }

            for (int o = 0; o < s.Players.Length; o++)
            {
                if (s.TeamOf(o) == team) continue;
                if (PlayerMovement.TrySplitStep(ref s.Players[o], _config.Players[o]))
                    events?.Add(new MatchEvent { Kind = MatchEventKind.SplitStep, Tick = s.Tick, Player = o });
            }
        }

        private Vec2 RallyTarget(Vec2 aim, ShotDefinition shot, int opponentHalf)
        {
            float depthT = Clamp(0.5f + 0.5f * aim.Y, 0f, 1f);
            float depth = shot.TargetDepthMin + (shot.TargetDepthMax - shot.TargetDepthMin) * depthT;
            float x = Clamp(aim.X, -1f, 1f) * (_config.Court.Config.HalfWidth - 1.2f);
            return new Vec2(x, depth * opponentHalf);
        }

        private Vec2 ServeTarget(MatchState s, Vec2 aim, ShotDefinition shot)
        {
            int serverHalf = s.HalfSign(s.TeamOf(ServerIndex(s)));
            var box = _config.Court.ServiceBox(serverHalf, CurrentServeSide(s));
            float centreX = 0.5f * (box.minX + box.maxX);
            float x = centreX + Clamp(aim.X, -1f, 1f) * 1.5f;
            float depthT = Clamp(0.5f + 0.5f * aim.Y, 0f, 1f);
            float depth = shot.TargetDepthMin + (shot.TargetDepthMax - shot.TargetDepthMin) * depthT;
            return new Vec2(x, depth * -serverHalf);
        }

        // ------------------------------------------------------------------ serve

        private void StartServeToss(MatchState s, int server, PlayerCommand cmd, List<MatchEvent> events)
        {
            Vec2 pos = s.Players[server].Position;
            int half = s.HalfSign(s.TeamOf(server));
            s.Ball = new BallState(new Vec3(pos.X, _config.ServeTossHeight, pos.Y - 0.35f * half), Vec3.Zero, Vec3.Zero);
            s.BallLive = true;
            s.TossBounced = false;
            s.Phase = PointPhase.ServeToss;
            s.PhaseTime = 0f;
            s.Strokes[server] = new StrokeState { Active = true, IsServe = true, Intent = ShotIntent.Control, Aim = cmd.Aim, PressTick = s.Tick };
            s.Players[server].Phase = PlayerPhase.Windup;
            s.Players[server].PhaseTime = 0f;
            events?.Add(new MatchEvent { Kind = MatchEventKind.ServeTossed, Tick = s.Tick, Player = server });
        }

        private void TryServeContact(MatchState s, List<MatchEvent> events)
        {
            int server = ServerIndex(s);
            if (!s.Strokes[server].IsServe || !s.Strokes[server].Active) return;
            // Contact at the top of the toss rebound: at or below the waist (FIP S1).
            if (!s.TossBounced || s.Ball.Velocity.Y > 0f) return;
            ExecuteStroke(s, server, events);
            s.Strokes[server].Active = false;
            s.Players[server].Phase = PlayerPhase.Contact;
            s.Players[server].PhaseTime = 0f;
        }

        // ------------------------------------------------------------------ rules

        private void ProcessBallEvents(MatchState s, List<MatchEvent> events)
        {
            for (int i = 0; i < _ballEvents.Count; i++)
            {
                BallEvent be = _ballEvents[i];
                events?.Add(new MatchEvent { Kind = MatchEventKind.Ball, Tick = s.Tick, BallEvent = be, Position = be.Position });
                if (!TryTranslate(s, be, out RallyEvent re)) continue;

                RefereeCall call = s.Referee.Process(re);
                if (call.Kind != CallKind.None)
                {
                    HandleCall(s, call, events);
                    return;
                }
            }
        }

        private bool TryTranslate(MatchState s, BallEvent be, out RallyEvent re)
        {
            re = default;
            TeamId nextHitter = s.LastHitter >= 0 ? s.TeamOf(s.LastHitter).Other() : s.TeamOf(ServerIndex(s)).Other();
            switch (be.Kind)
            {
                case BallEventKind.CrossedNet:
                    re = RallyEvent.Crossed(s.TeamAtHalf(s.Ball.Velocity.Z >= 0f ? 1f : -1f));
                    return true;
                case BallEventKind.LeftCourt:
                    re = RallyEvent.Out();
                    return true;
            }

            TeamId half = s.TeamAtHalf(be.Position.Z);
            switch (be.Surface)
            {
                case SurfaceKind.Floor:
                    if (s.Phase == PointPhase.ServeToss && !s.TossBounced && s.LastHitter < 0)
                    {
                        s.TossBounced = true; // toss bounce before the serve contact
                    }
                    if (half == nextHitter) s.BouncedSinceHit = true;
                    bool inBox = false;
                    if (s.Referee.IsServing)
                    {
                        int serverHalf = s.HalfSign(s.TeamOf(ServerIndex(s)));
                        inBox = _config.Court.IsInServiceBox(be.Position.X, be.Position.Z, serverHalf, CurrentServeSide(s));
                    }
                    re = RallyEvent.Bounce(half, inBox);
                    return true;
                case SurfaceKind.Glass:
                case SurfaceKind.Mesh:
                    if (half == nextHitter && s.BouncedSinceHit) s.OffWallSinceBounce = true;
                    re = be.Surface == SurfaceKind.Mesh ? RallyEvent.Mesh(half) : RallyEvent.Glass(half);
                    return true;
                case SurfaceKind.Net:
                    re = RallyEvent.Net();
                    return true;
            }
            return false;
        }

        private void HandleCall(MatchState s, RefereeCall call, List<MatchEvent> events)
        {
            s.LastCall = call;
            events?.Add(new MatchEvent { Kind = MatchEventKind.Call, Tick = s.Tick, Call = call });
            ClearStrokes(s);

            if (call.Kind == CallKind.Fault || call.Kind == CallKind.Let)
            {
                s.Phase = PointPhase.ServePause;
                s.PhaseTime = 0f;
                s.BallLive = false;
                return;
            }

            ScoreChange change = ScoreKeeper.AwardPoint(s.Score, _config.Scoring, call.Winner);
            events?.Add(new MatchEvent { Kind = MatchEventKind.Score, Tick = s.Tick, Score = change, Call = call });
            s.Phase = change.MatchWinner.HasValue ? PointPhase.MatchOver : PointPhase.PointOver;
            s.PhaseTime = 0f;
        }

        private void RearmServe(MatchState s)
        {
            s.Referee.StartServe();
            ResetFormation(s);
            s.Phase = PointPhase.PreServe;
            s.PhaseTime = 0f;
        }

        private void StartPoint(MatchState s, List<MatchEvent> events)
        {
            s.Referee.StartPoint(s.TeamOf(ServerIndex(s)));
            ResetFormation(s);
            s.Phase = PointPhase.PreServe;
            s.PhaseTime = 0f;
            s.LastCall = default;
            events?.Add(new MatchEvent { Kind = MatchEventKind.PointStarted, Tick = s.Tick, Player = ServerIndex(s) });
        }

        /// <summary>Serve formation (GAMEPLAY_SPEC §6): server behind the line, receiver diagonal at the back,
        /// partners at the net / service line. Positions are [P].</summary>
        private void ResetFormation(MatchState s)
        {
            ClearStrokes(s);
            int server = ServerIndex(s);
            TeamId serverTeam = s.TeamOf(server);
            int serverHalf = s.HalfSign(serverTeam);
            ServeSide side = CurrentServeSide(s);
            int serverX = CourtGeometry.ServerXSign(serverHalf, side);
            int receiverX = side == ServeSide.Right ? serverHalf : -serverHalf; // diagonal box x-sign
            int receiver = s.IndexOf(ScoreKeeper.CurrentReceiver(s.Score, _config.Scoring));

            for (int p = 0; p < s.Players.Length; p++)
            {
                Vec2 pos;
                int half = s.HalfSign(s.TeamOf(p));
                if (p == server) pos = new Vec2(serverX * 2.5f, serverHalf * 9.3f);
                else if (p == receiver) pos = new Vec2(receiverX * 2.5f, -serverHalf * 9.0f);
                else if (s.TeamOf(p) == serverTeam) pos = new Vec2(-serverX * 2.5f, serverHalf * 3.0f);
                else pos = new Vec2(-receiverX * 2.5f, half * 6.5f);

                s.Players[p] = new PlayerState { Position = pos, Facing = new Vec2(0f, -half) };
            }

            Vec2 hand = s.Players[server].Position;
            s.Ball = new BallState(new Vec3(hand.X, _config.ServeTossHeight, hand.Y), Vec3.Zero, Vec3.Zero);
            s.BallLive = false;
            s.LastHitter = -1;
            s.LastStrokeTick = -1;
            s.BouncedSinceHit = false;
            s.OffWallSinceBounce = false;
            s.TossBounced = false;
        }

        private static void ClearStrokes(MatchState s)
        {
            for (int p = 0; p < s.Strokes.Length; p++) s.Strokes[p] = default;
        }

        private void MovePlayers(MatchState s, PlayerCommand[] commands, float dt, bool lockServer)
        {
            int server = ServerIndex(s);
            for (int p = 0; p < s.Players.Length; p++)
            {
                Vec2 move = lockServer && p == server ? Vec2.Zero : commands[p].Move;
                PlayerMovement.Step(ref s.Players[p], _config.Players[p], move, s.HalfSign(s.TeamOf(p)), _config.Court.Config, dt);
            }
        }

        private long Ticks(float seconds) => (long)MathF.Round(seconds / Dt);
        private static float Clamp(float v, float min, float max) => v < min ? min : (v > max ? max : v);
    }
}
