using NUnit.Framework;
using Padel.Core;
using Padel.Simulation.Ball;
using Padel.Simulation.Court;
using Padel.Simulation.Players;
using Padel.Simulation.Shots;

namespace Padel.Simulation.Tests
{
    public class ShotTests
    {
        private static readonly CourtGeometry Court = new CourtGeometry(CourtConfig.Fip2026());
        private static readonly BallConfig Ball = BallConfig.Fip2026Default();
        private static readonly ShotCatalog Catalog = ShotCatalog.Default();

        // ---------- ShotResolver ----------

        private static ShotContext Ground(ShotIntent intent) =>
            new ShotContext { Intent = intent, Bounced = true, ContactHeight = 0.8f, Zone = CourtZone.Back };

        [Test]
        public void ServeContextAlwaysResolvesToServe()
        {
            var c = Ground(ShotIntent.Attack);
            c.IsServe = true;
            Assert.That(ShotResolver.Resolve(c), Is.EqualTo(ShotType.Serve));
        }

        [TestCase(ShotIntent.Attack, ShotType.Drive)]
        [TestCase(ShotIntent.Control, ShotType.Slice)]
        [TestCase(ShotIntent.Lob, ShotType.Lob)]
        public void GroundStrokesFollowTheIntent(ShotIntent intent, ShotType expected)
        {
            Assert.That(ShotResolver.Resolve(Ground(intent)), Is.EqualTo(expected));
        }

        [Test]
        public void AttackOffOwnWallIsAWallExitAndTouchIsAChiquita()
        {
            var c = Ground(ShotIntent.Attack);
            c.OffOwnWall = true;
            Assert.That(ShotResolver.Resolve(c), Is.EqualTo(ShotType.WallExit));
            c = Ground(ShotIntent.Attack);
            c.Touch = true;
            Assert.That(ShotResolver.Resolve(c), Is.EqualTo(ShotType.Chiquita));
        }

        [TestCase(ShotIntent.Control, 0f, ShotType.Bandeja)]
        [TestCase(ShotIntent.Attack, 0.2f, ShotType.Vibora)]
        [TestCase(ShotIntent.Attack, 0.9f, ShotType.Smash)]
        [TestCase(ShotIntent.Lob, 0f, ShotType.Lob)]
        public void OverheadsDependOnIntentAndCharge(ShotIntent intent, float charge, ShotType expected)
        {
            var c = new ShotContext { Intent = intent, Charge = charge, Bounced = false, ContactHeight = 2.6f, Zone = CourtZone.Net };
            Assert.That(ShotResolver.Resolve(c), Is.EqualTo(expected));
        }

        [Test]
        public void LowBallWithoutBounceIsAVolleyOrDropVolley()
        {
            var c = new ShotContext { Intent = ShotIntent.Attack, Bounced = false, ContactHeight = 1.2f, Zone = CourtZone.Net };
            Assert.That(ShotResolver.Resolve(c), Is.EqualTo(ShotType.Volley));
            c.Touch = true;
            Assert.That(ShotResolver.Resolve(c), Is.EqualTo(ShotType.DropVolley));
        }

        // ---------- ShotTiming ----------

        [Test]
        public void TimingWindowsClassifyContact()
        {
            ShotDefinition drive = Catalog.Get(ShotType.Drive);
            Assert.That(ShotTiming.Evaluate(0.01f, drive), Is.EqualTo(ShotQuality.Perfect));
            Assert.That(ShotTiming.Evaluate(-0.1f, drive), Is.EqualTo(ShotQuality.Good));
            Assert.That(ShotTiming.Evaluate(-0.3f, drive), Is.EqualTo(ShotQuality.Early));
            Assert.That(ShotTiming.Evaluate(0.3f, drive), Is.EqualTo(ShotQuality.Late));
        }

        [Test]
        public void PerfectShotsHaveNoErrorAndPoorShotsScatter()
        {
            ShotDefinition drive = Catalog.Get(ShotType.Drive);
            var rng = new Pcg32(5UL);
            Vec2 contact = new Vec2(0f, -8f), target = new Vec2(2f, 7f);
            Assert.That(ShotTiming.ApplyError(contact, target, drive, ShotQuality.Perfect, ref rng), Is.EqualTo(target));

            double sumSq = 0;
            for (int i = 0; i < 2000; i++)
            {
                Vec2 t = ShotTiming.ApplyError(contact, target, drive, ShotQuality.Late, ref rng);
                sumSq += (t - target).SqrMagnitude;
            }
            double rms = System.Math.Sqrt(sumSq / 2000);
            Assert.That(rms, Is.GreaterThan(0.5).And.LessThan(4.0));
        }

        // ---------- ShotSolver ----------

        [Test]
        public void ApexShotsLandOnTargetAcrossAGridOfCases(
            [Values(ShotType.Drive, ShotType.Slice, ShotType.Lob, ShotType.Volley, ShotType.Bandeja, ShotType.WallExit)] ShotType type)
        {
            var solver = new ShotSolver(Ball, Court);
            ShotDefinition shot = Catalog.Get(type);
            int cases = 0;
            foreach (float cx in new[] { -3f, 0f, 3f })
                foreach (float cz in new[] { -8.5f, -5f, -2.5f })
                    foreach (float tx in new[] { -3.5f, 0f, 3.5f })
                    {
                        float height = type == ShotType.Bandeja ? 2.4f : 0.9f;
                        float depth = 0.5f * (shot.TargetDepthMin + shot.TargetDepthMax);
                        ShotSolution s = solver.Solve(shot, new Vec3(cx, height, cz), new Vec2(tx, depth));
                        Assert.That(s.Valid, Is.True, $"{type} from ({cx},{cz}) to ({tx},{depth}) first contact {s.FirstContactSurface} at {s.FirstContact}");
                        Assert.That(s.Error, Is.LessThan(0.05f), $"{type} from ({cx},{cz}) to ({tx},{depth})");
                        cases++;
                    }
            Assert.That(cases, Is.EqualTo(27));
        }

        [Test]
        public void ServeFromTheRightLandsInTheDiagonalBox()
        {
            var solver = new ShotSolver(Ball, Court);
            ShotDefinition serve = Catalog.Get(ShotType.Serve);
            // Server on -Z half, right side stands at +X; target: diagonal box on the +Z half (x < 0).
            ShotSolution s = solver.Solve(serve, new Vec3(2.5f, 0.9f, -9.6f), new Vec2(-2.5f, 5.5f));
            Assert.That(s.Valid, Is.True);
            Assert.That(Court.IsInServiceBox(s.FirstContact.X, s.FirstContact.Z, -1, Padel.Rules.ServeSide.Right), Is.True);
        }

        [TestCase(ShotType.Smash, 1f)]
        [TestCase(ShotType.Vibora, 0f)]
        public void SpeedShotsLandOnTargetAtTheirSpeed(ShotType type, float charge)
        {
            var solver = new ShotSolver(Ball, Court);
            ShotDefinition shot = Catalog.Get(type);
            foreach (float tx in new[] { -3f, 0f, 3f })
            {
                ShotSolution s = solver.Solve(shot, new Vec3(1f, 2.7f, -2.5f), new Vec2(tx, 7f), charge);
                float expectedSpeed = charge >= 1f ? shot.MaxSpeed : shot.MinSpeed;
                Assert.That(s.Valid, Is.True, $"{type} to x={tx}: {s.FirstContactSurface} at {s.FirstContact}");
                Assert.That(s.Error, Is.LessThan(0.10f), $"{type} to x={tx}");
                Assert.That(s.Launch.Velocity.Magnitude, Is.EqualTo(expectedSpeed).Within(1e-3f));
            }
        }

        [Test]
        public void SliceCarriesBackspinAndDriveTopspin()
        {
            Vec2 dir = new Vec2(0f, 1f);
            Assert.That(ShotSolver.SpinFor(Catalog.Get(ShotType.Drive), dir).X, Is.GreaterThan(0f));
            Assert.That(ShotSolver.SpinFor(Catalog.Get(ShotType.Slice), dir).X, Is.LessThan(0f));
        }
    }
}
