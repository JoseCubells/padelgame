using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Padel.Core;
using Padel.Simulation.Ball;
using Padel.Simulation.Court;

namespace Padel.Simulation.Tests
{
    public class BallPhysicsTests
    {
        private const float Dt = BallSimulator.DefaultTimeStep;
        private static readonly CourtGeometry Court = new CourtGeometry(CourtConfig.Fip2026());

        private static BallConfig NoAero(BallConfig b) => new BallConfig(
            b.Mass, b.Radius, b.Gravity, b.AirDensity, dragCoefficient: 0f, liftIntercept: 0f, liftSlope: 0f, liftMax: 0f,
            b.SpinDecayTime, b.InertiaFactor, b.MinBounceSpeed,
            b.Material(SurfaceKind.Floor), b.Material(SurfaceKind.Glass), b.Material(SurfaceKind.Mesh), b.Material(SurfaceKind.Net));

        private static List<BallEvent> Run(ref BallState s, BallConfig c, float seconds)
        {
            var all = new List<BallEvent>();
            int steps = (int)(seconds / Dt + 0.5f);
            for (int i = 0; i < steps; i++) BallSimulator.Step(ref s, c, Court, Dt, all);
            return all;
        }

        private static double Energy(in BallState s, BallConfig c)
        {
            double v2 = s.Velocity.SqrMagnitude;
            double w2 = s.Spin.SqrMagnitude;
            double inertia = c.InertiaFactor * c.Mass * c.Radius * c.Radius;
            return 0.5 * c.Mass * v2 + c.Mass * c.Gravity * s.Position.Y + 0.5 * inertia * w2;
        }

        [Test]
        public void FreeFlightWithoutAeroMatchesAnalyticParabola()
        {
            BallConfig c = NoAero(BallConfig.Fip2026Default());
            var s = new BallState(new Vec3(0f, 1f, -9f), new Vec3(0f, 5f, 3f), Vec3.Zero);
            Run(ref s, c, 0.5f);
            float t = 0.5f;
            float expectedY = 1f + 5f * t - 0.5f * c.Gravity * t * t;
            // Semi-implicit Euler vertical error bound: 1/2 * g * T * dt.
            float bound = 0.5f * c.Gravity * t * Dt + 1e-4f;
            Assert.That(s.Position.Y, Is.EqualTo(expectedY).Within(bound));
            Assert.That(s.Position.Z, Is.EqualTo(-9f + 3f * t).Within(1e-4f));
        }

        [Test]
        public void FipDropTestReboundsBetween135And145Centimetres()
        {
            // FIP 2026: dropped from 2.54 m onto a hard surface, rebound 1.35-1.45 m (heights of the ball bottom).
            BallConfig c = BallConfig.Fip2026Default();
            var s = new BallState(new Vec3(0f, 2.54f + c.Radius, -5f), Vec3.Zero, Vec3.Zero);
            var events = new List<BallEvent>();
            bool bounced = false;
            float apex = 0f;
            for (int i = 0; i < 600; i++)
            {
                events.Clear();
                BallSimulator.Step(ref s, c, Court, Dt, events);
                if (events.Any(e => e.Surface == SurfaceKind.Floor)) { if (bounced) break; bounced = true; }
                if (bounced) apex = System.Math.Max(apex, s.Position.Y - c.Radius);
            }
            TestContext.WriteLine($"FIP drop rebound: {apex:0.000} m");
            Assert.That(apex, Is.InRange(1.35f, 1.45f));
        }

        [Test]
        public void EnergyNeverIncreasesThroughBouncesOnAllSurfaces()
        {
            BallConfig c = BallConfig.Fip2026Default();
            var s = new BallState(new Vec3(1f, 1.2f, -2f), new Vec3(6f, 4f, 18f), new Vec3(-60f, 20f, 0f));
            double previous = Energy(s, c);
            double start = previous;
            for (int i = 0; i < 1200; i++)
            {
                BallSimulator.Step(ref s, c, Court, Dt, null);
                double e = Energy(s, c);
                Assert.That(e, Is.LessThanOrEqualTo(previous + 2e-3), $"step {i}");
                previous = e;
            }
            Assert.That(previous, Is.LessThan(start));
        }

        [Test]
        public void SmashAtFortyMetresPerSecondDoesNotTunnelThroughTheBackGlass()
        {
            BallConfig c = BallConfig.Fip2026Default();
            var s = new BallState(new Vec3(0f, 1.5f, 5f), new Vec3(0f, 0f, 40f), Vec3.Zero);
            var events = new List<BallEvent>();
            for (int i = 0; i < 60; i++)
            {
                BallSimulator.Step(ref s, c, Court, Dt, events);
                Assert.That(s.Position.Z, Is.LessThanOrEqualTo(10f - c.Radius + 1e-4f));
            }
            Assert.That(events.Any(e => e.Kind == BallEventKind.Contact && e.Surface == SurfaceKind.Glass), Is.True);
            Assert.That(s.Velocity.Z, Is.LessThan(0f));
        }

        [Test]
        public void GlassReturnsTheBallWithItsRestitution()
        {
            BallConfig c = NoAero(BallConfig.Fip2026Default());
            var s = new BallState(new Vec3(0f, 1.5f, 9.5f), new Vec3(0f, 0f, 15f), Vec3.Zero);
            List<BallEvent> events = Run(ref s, c, 0.05f);
            BallEvent glass = events.First(e => e.Surface == SurfaceKind.Glass);
            float ratio = -s.Velocity.Z / glass.ImpactSpeed;
            SurfaceMaterial m = c.Material(SurfaceKind.Glass);
            Assert.That(ratio, Is.EqualTo(m.RestitutionAt(glass.ImpactSpeed)).Within(1e-3f));
        }

        [Test]
        public void TopspinKicksForwardAndBackspinChecksOnTheFloor()
        {
            BallConfig c = NoAero(BallConfig.Fip2026Default());
            float Bounce(float spinX)
            {
                var s = new BallState(new Vec3(0f, 0.1f, 2f), new Vec3(0f, -6f, 12f), new Vec3(spinX, 0f, 0f));
                var events = new List<BallEvent>();
                while (!events.Any(e => e.Surface == SurfaceKind.Floor)) BallSimulator.Step(ref s, c, Court, Dt, events);
                return s.Velocity.Z;
            }
            // For motion towards +Z, topspin (top of the ball moving forward) is +X angular velocity.
            float top = Bounce(250f);
            float flat = Bounce(0f);
            float back = Bounce(-250f);
            TestContext.WriteLine($"post-bounce vz: topspin {top:0.00}, flat {flat:0.00}, backspin {back:0.00}");
            Assert.That(top, Is.GreaterThan(flat));
            Assert.That(flat, Is.GreaterThan(back));
        }

        [Test]
        public void TopspinDipsAndBackspinFloatsInFlight()
        {
            BallConfig c = BallConfig.Fip2026Default();
            float LandingZ(float spinX)
            {
                var s = new BallState(new Vec3(0f, 1f, -9f), new Vec3(0f, 6f, 11f), new Vec3(spinX, 0f, 0f));
                var events = new List<BallEvent>();
                while (!events.Any(e => e.Kind == BallEventKind.Contact))
                    BallSimulator.Step(ref s, c, Court, Dt, events);
                Assert.That(events.First(e => e.Kind == BallEventKind.Contact).Surface, Is.EqualTo(SurfaceKind.Floor));
                return s.Position.Z;
            }
            TestContext.WriteLine($"landing z: topspin {LandingZ(200f):0.00}, flat {LandingZ(0f):0.00}, backspin {LandingZ(-200f):0.00}");
            Assert.That(LandingZ(0f), Is.GreaterThan(0f));
            Assert.That(LandingZ(200f), Is.LessThan(LandingZ(0f)));
            Assert.That(LandingZ(-200f), Is.GreaterThan(LandingZ(0f)));
        }

        [Test]
        public void LowBallHitsTheNetAndStaysOnItsSide()
        {
            BallConfig c = BallConfig.Fip2026Default();
            var s = new BallState(new Vec3(0f, 0.5f, -2f), new Vec3(0f, 0f, 10f), Vec3.Zero);
            List<BallEvent> events = Run(ref s, c, 0.4f);
            Assert.That(events.Any(e => e.Surface == SurfaceKind.Net), Is.True);
            Assert.That(events.Any(e => e.Kind == BallEventKind.CrossedNet), Is.False);
            Assert.That(s.Position.Z, Is.LessThan(0f));
        }

        [Test]
        public void HighBallCrossesTheNet()
        {
            BallConfig c = BallConfig.Fip2026Default();
            var s = new BallState(new Vec3(0f, 1.5f, -2f), new Vec3(0f, 2f, 10f), Vec3.Zero);
            List<BallEvent> events = Run(ref s, c, 0.4f);
            Assert.That(events.Count(e => e.Kind == BallEventKind.CrossedNet), Is.EqualTo(1));
            Assert.That(events.Any(e => e.Surface == SurfaceKind.Net), Is.False);
        }

        [Test]
        public void LobOverTheBackWallLeavesTheCourt()
        {
            BallConfig c = BallConfig.Fip2026Default();
            var s = new BallState(new Vec3(0f, 2f, 4f), new Vec3(0f, 11f, 13f), Vec3.Zero);
            List<BallEvent> events = Run(ref s, c, 1.5f);
            Assert.That(events.Any(e => e.Kind == BallEventKind.LeftCourt), Is.True);
            Assert.That(events.Any(e => e.Kind == BallEventKind.Contact && e.Surface != SurfaceKind.Floor), Is.False);
        }

        [Test]
        public void MeshBouncesAreIrregularButDeterministic()
        {
            BallConfig c = BallConfig.Fip2026Default();
            Vec3 Reflect(float x)
            {
                var s = new BallState(new Vec3(4.5f, 1.5f, x), new Vec3(8f, 0f, 0f), Vec3.Zero); // side mesh (centre)
                var events = new List<BallEvent>();
                while (!events.Any(e => e.Surface == SurfaceKind.Mesh)) BallSimulator.Step(ref s, c, Court, Dt, events);
                return s.Velocity;
            }
            Vec3 a1 = Reflect(1.0f), a2 = Reflect(1.0f), b = Reflect(2.37f);
            Assert.That(a1, Is.EqualTo(a2));
            Assert.That(a1, Is.Not.EqualTo(b));
            Assert.That(a1.X, Is.LessThan(0f));
        }

        [Test]
        public void BallEventuallySettlesWithoutEventSpam()
        {
            BallConfig c = BallConfig.Fip2026Default();
            var s = new BallState(new Vec3(0f, 1f, -5f), new Vec3(0f, 2f, 1f), Vec3.Zero);
            List<BallEvent> events = Run(ref s, c, 8f);
            Assert.That(s.Velocity.Y, Is.EqualTo(0f));
            Assert.That(s.Position.Y, Is.EqualTo(c.Radius).Within(1e-3f));
            Assert.That(events.Count(e => e.Surface == SurfaceKind.Floor), Is.LessThan(30));
        }

        [Test]
        public void SimulationIsBitwiseDeterministic()
        {
            BallConfig c = BallConfig.Fip2026Default();
            BallState RunOnce()
            {
                var s = new BallState(new Vec3(-3f, 1f, -8f), new Vec3(7f, 6f, 20f), new Vec3(-40f, 15f, 5f));
                Run(ref s, c, 5f);
                return s;
            }
            BallState x = RunOnce(), y = RunOnce();
            Assert.That(x.Position, Is.EqualTo(y.Position));
            Assert.That(x.Velocity, Is.EqualTo(y.Velocity));
            Assert.That(x.Spin, Is.EqualTo(y.Spin));
        }

        [Test]
        public void PredictorMatchesTheSimulationExactly()
        {
            BallConfig c = BallConfig.Fip2026Default();
            var start = new BallState(new Vec3(2f, 1f, -7f), new Vec3(-3f, 5f, 17f), new Vec3(-50f, 0f, 10f));
            var predicted = new List<PredictedEvent>();
            BallState end = new TrajectoryPredictor().Predict(start, c, Court, Dt, 3f, null, predicted);

            BallState s = start;
            List<BallEvent> actual = Run(ref s, c, 3f);
            Assert.That(predicted.Count, Is.EqualTo(actual.Count));
            for (int i = 0; i < actual.Count; i++)
            {
                Assert.That(predicted[i].Event.Position, Is.EqualTo(actual[i].Position));
            }
            Assert.That(end.Position, Is.EqualTo(s.Position));
        }
    }
}
