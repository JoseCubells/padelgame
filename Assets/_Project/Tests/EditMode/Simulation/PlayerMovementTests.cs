using NUnit.Framework;
using Padel.Core;
using Padel.Simulation.Court;
using Padel.Simulation.Players;

namespace Padel.Simulation.Tests
{
    public class PlayerMovementTests
    {
        private const float Dt = 1f / 120f;
        private static readonly CourtConfig Court = CourtConfig.Fip2026();
        private static readonly PlayerStats Stats = new PlayerStats();

        private static PlayerState At(float x, float z) => new PlayerState { Position = new Vec2(x, z) };

        private static void Run(ref PlayerState s, Vec2 move, float seconds, int halfSign = -1)
        {
            int steps = (int)(seconds / Dt + 0.5f);
            for (int i = 0; i < steps; i++) PlayerMovement.Step(ref s, Stats, move, halfSign, Court, Dt);
        }

        [Test]
        public void ReachesTopSpeedInAccelerationTime()
        {
            PlayerState s = At(0f, -7f);
            Run(ref s, new Vec2(1f, 0f), Stats.AccelerationTime * 0.5f);
            Assert.That(s.Velocity.X, Is.EqualTo(Stats.MaxSpeed * 0.5f).Within(0.1f));
            Run(ref s, new Vec2(1f, 0f), Stats.AccelerationTime * 0.5f);
            Assert.That(s.Velocity.X, Is.EqualTo(Stats.MaxSpeed).Within(0.05f));
        }

        [Test]
        public void BrakesFasterThanItAccelerates()
        {
            PlayerState s = At(-3f, -7f);
            Run(ref s, new Vec2(1f, 0f), 0.5f);
            Run(ref s, Vec2.Zero, Stats.BrakeTime + Dt); // + one tick: 0.12 s is 14.4 ticks
            Assert.That(s.Velocity.Magnitude, Is.LessThan(0.05f));
        }

        [Test]
        public void BackpedallingIsSlowerThanRunningForward()
        {
            PlayerState forward = At(0f, -8f), back = At(0f, -2f);
            Run(ref forward, new Vec2(0f, 1f), 0.6f);   // towards the net for a -Z player
            Run(ref back, new Vec2(0f, -1f), 0.6f);     // away from the net
            Assert.That(forward.Velocity.Magnitude, Is.EqualTo(Stats.MaxSpeed).Within(0.05f));
            Assert.That(back.Velocity.Magnitude, Is.EqualTo(Stats.BackpedalSpeed).Within(0.05f));
        }

        [Test]
        public void PlayersStayInsideTheirOwnHalf()
        {
            PlayerState s = At(0f, -2f);
            Run(ref s, new Vec2(0f, 1f), 3f);
            Assert.That(s.Position.Y, Is.EqualTo(-PlayerMovement.NetMargin).Within(1e-4f));
            Run(ref s, new Vec2(1f, 0f), 3f);
            Assert.That(s.Position.X, Is.EqualTo(Court.HalfWidth - PlayerMovement.WallMargin).Within(1e-4f));

            PlayerState other = At(0f, 5f);
            Run(ref other, new Vec2(0f, 1f), 3f, halfSign: 1); // +Z player moving away from the net
            Assert.That(other.Position.Y, Is.EqualTo(Court.HalfLength - PlayerMovement.WallMargin).Within(1e-4f));
        }

        [Test]
        public void MovementIsLockedDuringContactAndRecovery()
        {
            PlayerState s = At(0f, -7f);
            s.Phase = PlayerPhase.Recovery;
            Run(ref s, new Vec2(1f, 0f), 0.3f);
            Assert.That(s.Position.X, Is.EqualTo(0f).Within(1e-5f));
        }

        [Test]
        public void WindupAllowsOnlyMicroAdjustments()
        {
            PlayerState s = At(0f, -7f);
            s.Phase = PlayerPhase.Windup;
            Run(ref s, new Vec2(1f, 0f), 0.5f);
            Assert.That(s.Velocity.Magnitude, Is.LessThanOrEqualTo(Stats.WindupAdjustSpeed + 1e-4f));
        }

        [Test]
        public void SplitStepGivesAFasterStartOnlyWhenReady()
        {
            PlayerState plain = At(0f, -7f), boosted = At(0f, -7f);
            Assert.That(PlayerMovement.TrySplitStep(ref boosted, Stats), Is.True);
            Run(ref plain, new Vec2(1f, 0f), 0.2f);
            Run(ref boosted, new Vec2(1f, 0f), 0.2f);
            Assert.That(boosted.Position.X, Is.GreaterThan(plain.Position.X));

            PlayerState running = At(0f, -7f);
            running.Velocity = new Vec2(3f, 0f);
            Assert.That(PlayerMovement.TrySplitStep(ref running, Stats), Is.False);
        }
    }
}
