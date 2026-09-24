using System;
using Padel.Core;
using Padel.Simulation.Court;

namespace Padel.Simulation.Players
{
    /// <summary>
    /// Custom court-plane movement (ADR: no CharacterController, no NavMesh; GAMEPLAY_SPEC §4). Separate acceleration,
    /// braking and reversal rates give a sporty feel; backpedalling is slower; players stay inside their own half.
    /// </summary>
    public static class PlayerMovement
    {
        /// <summary>Distance kept from the walls and the net (body radius) [P].</summary>
        public const float WallMargin = 0.25f;
        public const float NetMargin = 0.30f;

        /// <param name="halfSign">-1 if the player's team currently plays on the negative-Z half, +1 otherwise.</param>
        public static void Step(ref PlayerState s, PlayerStats stats, Vec2 move, int halfSign, CourtConfig court, float dt)
        {
            Vec2 forward = new Vec2(0f, -halfSign); // towards the net
            Vec2 target = DesiredVelocity(s, stats, move, forward);

            Vec2 delta = target - s.Velocity;
            float rate;
            if (Vec2.Dot(target, s.Velocity) < 0f) rate = stats.ReverseDeceleration;
            else if (target.SqrMagnitude < s.Velocity.SqrMagnitude) rate = stats.Deceleration;
            else rate = stats.Acceleration * (s.BoostTime > 0f ? stats.SplitStepBoost : 1f);

            s.Velocity += delta.ClampMagnitude(rate * dt);
            s.Position += s.Velocity * dt;
            Confine(ref s, halfSign, court);

            s.Facing = forward;
            s.PhaseTime += dt;
            if (s.BoostTime > 0f) s.BoostTime = Math.Max(0f, s.BoostTime - dt);
        }

        /// <summary>Called when an opponent strikes the ball: a ready (almost still) player gets a start boost.</summary>
        public static bool TrySplitStep(ref PlayerState s, PlayerStats stats)
        {
            if (s.Phase != PlayerPhase.Free || s.Velocity.Magnitude > stats.ReadySpeed) return false;
            s.BoostTime = stats.SplitStepDuration;
            return true;
        }

        private static Vec2 DesiredVelocity(in PlayerState s, PlayerStats stats, Vec2 move, Vec2 forward)
        {
            Vec2 dir = move.ClampMagnitude(1f);
            switch (s.Phase)
            {
                case PlayerPhase.Contact:
                case PlayerPhase.Recovery:
                    return Vec2.Zero; // locked during the stroke (GAMEPLAY_SPEC §4)
                case PlayerPhase.Windup:
                    return dir * stats.WindupAdjustSpeed;
            }

            float magnitude = dir.Magnitude;
            if (magnitude < 1e-4f) return Vec2.Zero;
            float backward = -Vec2.Dot(dir / magnitude, forward);
            backward = backward < 0f ? 0f : (backward > 1f ? 1f : backward);
            float limit = stats.MaxSpeed + (stats.BackpedalSpeed - stats.MaxSpeed) * backward;
            return dir * limit;
        }

        private static void Confine(ref PlayerState s, int halfSign, CourtConfig court)
        {
            float maxX = court.HalfWidth - WallMargin;
            float nearZ = NetMargin;
            float farZ = court.HalfLength - WallMargin;

            if (s.Position.X > maxX) { s.Position.X = maxX; if (s.Velocity.X > 0f) s.Velocity.X = 0f; }
            if (s.Position.X < -maxX) { s.Position.X = -maxX; if (s.Velocity.X < 0f) s.Velocity.X = 0f; }

            float depth = s.Position.Y * halfSign; // distance from the net into the own half
            float depthVelocity = s.Velocity.Y * halfSign;
            if (depth < nearZ) { depth = nearZ; if (depthVelocity < 0f) s.Velocity.Y = 0f; }
            if (depth > farZ) { depth = farZ; if (depthVelocity > 0f) s.Velocity.Y = 0f; }
            s.Position.Y = depth * halfSign;
        }
    }
}
