using System;
using System.Collections.Generic;
using Padel.Core;
using Padel.Simulation.Ball;
using Padel.Simulation.Court;

namespace Padel.Simulation.Shots
{
    /// <summary>Result of solving a stroke's launch.</summary>
    public struct ShotSolution
    {
        public BallState Launch;
        /// <summary>Where the ball first touches something (normally the floor) with this launch.</summary>
        public Vec3 FirstContact;
        public SurfaceKind FirstContactSurface;
        /// <summary>Horizontal distance between the first contact and the requested target (m).</summary>
        public float Error;
        /// <summary>First contact is the floor, on the target's half, after crossing the net.</summary>
        public bool Valid;
    }

    /// <summary>
    /// Inverse ballistics by shooting (GAMEPLAY_RESEARCH §3.4): an analytic no-drag guess followed by corrections
    /// using the real <see cref="BallSimulator"/> (drag, Magnus, net), so the ball lands where the solver says.
    /// A stroke is a gameplay event that sets velocity and spin, not a physical racket collision (ADR-004).
    /// Holds scratch buffers; not thread-safe.
    /// </summary>
    public sealed class ShotSolver
    {
        private const int MaxIterations = 10;
        private const float Tolerance = 0.02f;
        private const float NetClearance = 0.3f;
        private const float Horizon = 5f;

        private readonly TrajectoryPredictor _predictor = new TrajectoryPredictor();
        private readonly List<PredictedEvent> _events = new List<PredictedEvent>(16);
        private readonly BallConfig _ball;
        private readonly CourtGeometry _court;
        private readonly float _dt;

        public ShotSolver(BallConfig ball, CourtGeometry court, float dt = BallSimulator.DefaultTimeStep)
        {
            _ball = ball ?? throw new ArgumentNullException(nameof(ball));
            _court = court ?? throw new ArgumentNullException(nameof(court));
            _dt = dt;
        }

        /// <param name="charge">0..1; interpolates MinSpeed..MaxSpeed in speed mode.</param>
        public ShotSolution Solve(ShotDefinition shot, Vec3 contact, Vec2 target, float charge = 0f)
        {
            Vec2 from = new Vec2(contact.X, contact.Z);
            Vec2 horizontal = target - from;
            float distance = horizontal.Magnitude;
            Vec2 dir = distance > 1e-4f ? horizontal / distance : new Vec2(0f, 1f);
            Vec3 spin = SpinFor(shot, dir);

            return shot.Mode == TrajectoryMode.Apex
                ? SolveApex(shot, contact, target, spin)
                : SolveSpeed(shot, contact, target, spin, Lerp(shot.MinSpeed, shot.MaxSpeed, Clamp01(charge)));
        }

        /// <summary>Topspin axis is up × direction (for +Z travel: +X); side spin is around the vertical axis.</summary>
        public static Vec3 SpinFor(ShotDefinition shot, Vec2 direction)
        {
            Vec3 d = direction.ToWorld();
            return Vec3.Cross(Vec3.Up, d) * shot.TopSpin + Vec3.Up * shot.SideSpin;
        }

        private ShotSolution SolveApex(ShotDefinition shot, Vec3 contact, Vec2 target, Vec3 spin)
        {
            float g = _ball.Gravity;
            float landingY = _ball.Radius;
            float apex = Math.Max(contact.Y, landingY) + shot.ApexAboveContact;
            apex = Math.Max(apex, _court.Config.NetHeightPosts + NetClearance);

            ShotSolution best = default;
            best.Error = float.MaxValue;
            for (int attempt = 0; attempt < 4; attempt++)
            {
                float vy = MathF.Sqrt(2f * g * Math.Max(apex - contact.Y, 0.01f));
                float flight = vy / g + MathF.Sqrt(2f * (apex - landingY) / g);
                Vec2 vh = (target - new Vec2(contact.X, contact.Z)) / flight;

                bool hitNet = false;
                for (int i = 0; i < MaxIterations; i++)
                {
                    var launch = new BallState(contact, new Vec3(vh.X, vy, vh.Y), spin);
                    ShotSolution s = Evaluate(launch, target);
                    if (s.Error < best.Error || (s.Valid && !best.Valid)) best = s;
                    if (s.FirstContactSurface == SurfaceKind.Net) { hitNet = true; break; }
                    if (s.Valid && s.Error < Tolerance) return s;

                    Vec2 landed = new Vec2(s.FirstContact.X, s.FirstContact.Z);
                    vh += (target - landed) / flight;
                }
                if (!hitNet) break;
                apex += 0.4f; // clear the net with a higher arc and try again
            }
            return best;
        }

        private ShotSolution SolveSpeed(ShotDefinition shot, Vec3 contact, Vec2 target, Vec3 spin, float speed)
        {
            Vec2 from = new Vec2(contact.X, contact.Z);
            float yaw = MathF.Atan2(target.X - from.X, target.Y - from.Y);
            ShotSolution best = default;
            best.Error = float.MaxValue;

            for (int yawIteration = 0; yawIteration < 4; yawIteration++)
            {
                // Bisection on pitch: landing distance grows with pitch in the low-angle range used by overheads.
                float lo = -70f * MathF.PI / 180f, hi = 25f * MathF.PI / 180f;
                float targetDistance = (target - from).Magnitude;
                ShotSolution s = default;
                for (int i = 0; i < 24; i++)
                {
                    float pitch = 0.5f * (lo + hi);
                    s = Evaluate(Launch(contact, speed, yaw, pitch, spin), target);
                    if (s.Error < best.Error || (s.Valid && !best.Valid)) best = s;
                    float landed = (new Vec2(s.FirstContact.X, s.FirstContact.Z) - from).Magnitude;
                    bool tooShort = s.FirstContactSurface == SurfaceKind.Net || landed < targetDistance;
                    if (tooShort) lo = pitch; else hi = pitch;
                }
                if (best.Valid && best.Error < Tolerance) return best;

                // Correct yaw for lateral drift (side spin).
                Vec2 landedAt = new Vec2(s.FirstContact.X, s.FirstContact.Z);
                float landedYaw = MathF.Atan2(landedAt.X - from.X, landedAt.Y - from.Y);
                float targetYaw = MathF.Atan2(target.X - from.X, target.Y - from.Y);
                yaw += targetYaw - landedYaw;
            }
            return best;
        }

        private static BallState Launch(Vec3 contact, float speed, float yaw, float pitch, Vec3 spin)
        {
            float cosP = MathF.Cos(pitch);
            var v = new Vec3(MathF.Sin(yaw) * cosP, MathF.Sin(pitch), MathF.Cos(yaw) * cosP) * speed;
            return new BallState(contact, v, spin);
        }

        private ShotSolution Evaluate(BallState launch, Vec2 target)
        {
            _predictor.Predict(launch, _ball, _court, _dt, Horizon, null, _events);
            var result = new ShotSolution { Launch = launch, Error = float.MaxValue };
            bool crossed = false;
            for (int i = 0; i < _events.Count; i++)
            {
                BallEvent e = _events[i].Event;
                if (e.Kind == BallEventKind.CrossedNet) { crossed = true; continue; }
                if (e.Kind == BallEventKind.LeftCourt)
                {
                    result.FirstContact = e.Position;
                    result.FirstContactSurface = SurfaceKind.Mesh;
                    break;
                }
                result.FirstContact = e.Position;
                result.FirstContactSurface = e.Surface;
                break;
            }
            Vec2 landed = new Vec2(result.FirstContact.X, result.FirstContact.Z);
            result.Error = (landed - target).Magnitude;
            bool sameHalf = (result.FirstContact.Z < 0f) == (target.Y < 0f);
            result.Valid = crossed && sameHalf && result.FirstContactSurface == SurfaceKind.Floor;
            return result;
        }

        private static float Clamp01(float v) => v < 0f ? 0f : (v > 1f ? 1f : v);
        private static float Lerp(float a, float b, float t) => a + (b - a) * t;
    }
}
