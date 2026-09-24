using System.Linq;
using NUnit.Framework;
using Padel.Core;
using Padel.Rules;
using Padel.Simulation.Court;

namespace Padel.Simulation.Tests
{
    public class CourtTests
    {
        [Test]
        public void DefaultCourtIsFipCompliant()
        {
            Assert.That(CourtValidator.Validate(CourtConfig.Fip2026()), Is.Empty);
        }

        [TestCase(20.2f, 10f, 0.88f, 6.95f)]
        [TestCase(20f, 9.9f, 0.88f, 6.95f)]
        [TestCase(20f, 10f, 0.90f, 6.95f)]
        [TestCase(20f, 10f, 0.88f, 7.0f)]
        public void OutOfToleranceCourtIsReported(float length, float width, float net, float serviceLine)
        {
            var config = new CourtConfig(length: length, width: width, netHeightCenter: net,
                netHeightPosts: 0.92f, serviceLineFromNet: serviceLine);
            Assert.That(CourtValidator.Validate(config), Is.Not.Empty);
        }

        [Test]
        public void LengthWithinHalfPercentToleranceIsAccepted()
        {
            Assert.That(CourtValidator.IsFipCompliant(new CourtConfig(length: 20.09f)), Is.True);
        }

        [Test]
        public void NetIsLowestInTheCentreAndHighestAtThePosts()
        {
            var geo = new CourtGeometry(CourtConfig.Fip2026());
            Assert.That(geo.NetHeightAt(0f), Is.EqualTo(0.88f).Within(1e-5f));
            Assert.That(geo.NetHeightAt(5f), Is.EqualTo(0.92f).Within(1e-5f));
            Assert.That(geo.NetHeightAt(-2.5f), Is.EqualTo(0.90f).Within(1e-5f));
        }

        [TestCase(SideWallVariant.FullGlass)]
        [TestCase(SideWallVariant.Stepped)]
        public void WallsFullyEncloseTheCourtUpToTheirHeight(SideWallVariant variant)
        {
            var geo = new CourtGeometry(new CourtConfig(sideWalls: variant));
            // Sample points on each wall plane below the minimum enclosure height (3 m): each must hit exactly one panel.
            for (float along = -9.9f; along <= 9.9f; along += 0.3f)
            {
                for (float y = 0.05f; y < 2.95f; y += 0.3f)
                {
                    foreach (float x in new[] { -5f, 5f })
                    {
                        var p = new Vec3(x, y, along);
                        int hits = geo.Panels.Count(panel => panel.Axis == PanelAxis.X && panel.PlaneCoord == x && panel.ContainsProjected(p));
                        Assert.That(hits, Is.GreaterThanOrEqualTo(1), $"gap in side wall at {p}");
                    }
                }
            }
            for (float along = -4.9f; along <= 4.9f; along += 0.3f)
            {
                for (float y = 0.05f; y < 3.95f; y += 0.3f)
                {
                    foreach (float z in new[] { -10f, 10f })
                    {
                        var p = new Vec3(along, y, z);
                        Assert.That(geo.Panels.Any(panel => panel.Axis == PanelAxis.Z && panel.PlaneCoord == z && panel.ContainsProjected(p)),
                            $"gap in back wall at {p}");
                    }
                }
            }
        }

        [Test]
        public void BackWallIsGlassToThreeMetresThenMesh()
        {
            var geo = new CourtGeometry(CourtConfig.Fip2026());
            CourtPanel glass = geo.Panels.First(p => p.Axis == PanelAxis.Z && p.PlaneCoord == 10f && p.ContainsProjected(new Vec3(0f, 2f, 10f)));
            CourtPanel mesh = geo.Panels.First(p => p.Axis == PanelAxis.Z && p.PlaneCoord == 10f && p.ContainsProjected(new Vec3(0f, 3.5f, 10f)));
            Assert.That(glass.Surface, Is.EqualTo(SurfaceKind.Glass));
            Assert.That(mesh.Surface, Is.EqualTo(SurfaceKind.Mesh));
            Assert.That(glass.InwardNormal, Is.EqualTo(new Vec3(0f, 0f, -1f)));
        }

        [Test]
        public void SideWallCentreIsMeshAndEndsAreGlass()
        {
            var geo = new CourtGeometry(CourtConfig.Fip2026());
            SurfaceKind At(float z, float y) =>
                geo.Panels.First(p => p.Axis == PanelAxis.X && p.PlaneCoord == 5f && p.ContainsProjected(new Vec3(5f, y, z))).Surface;
            Assert.That(At(0f, 1f), Is.EqualTo(SurfaceKind.Mesh));
            Assert.That(At(7f, 1f), Is.EqualTo(SurfaceKind.Glass));
            Assert.That(At(-9f, 2.5f), Is.EqualTo(SurfaceKind.Glass));
            Assert.That(At(9f, 3.5f), Is.EqualTo(SurfaceKind.Mesh));
        }

        [Test]
        public void ServeFromTheRightGoesDiagonally()
        {
            var geo = new CourtGeometry(CourtConfig.Fip2026());
            // Server on the -Z half, right side: stands at +X, target box is x<0 on the +Z half.
            Assert.That(CourtGeometry.ServerXSign(-1, ServeSide.Right), Is.EqualTo(1));
            Assert.That(geo.IsInServiceBox(-2f, 4f, -1, ServeSide.Right), Is.True);
            Assert.That(geo.IsInServiceBox(2f, 4f, -1, ServeSide.Right), Is.False);
            Assert.That(geo.IsInServiceBox(-2f, 7.5f, -1, ServeSide.Right), Is.False);
            Assert.That(geo.IsInServiceBox(-2f, -4f, -1, ServeSide.Right), Is.False);
            // Server on the +Z half serving from the left: stands at +X, target x<0 on the -Z half.
            Assert.That(CourtGeometry.ServerXSign(1, ServeSide.Left), Is.EqualTo(1));
            Assert.That(geo.IsInServiceBox(-3f, -6f, 1, ServeSide.Left), Is.True);
        }

        [Test]
        public void ServiceLinesAreGood()
        {
            var geo = new CourtGeometry(CourtConfig.Fip2026());
            Assert.That(geo.IsInServiceBox(-2f, 6.95f + 0.02f, -1, ServeSide.Right), Is.True);
            Assert.That(geo.IsInServiceBox(0.02f, 3f, -1, ServeSide.Right), Is.True);
            Assert.That(geo.IsInServiceBox(-2f, 6.95f + 0.04f, -1, ServeSide.Right), Is.False);
        }

        [TestCase(1f, CourtZone.Net)]
        [TestCase(-5f, CourtZone.Transition)]
        [TestCase(9f, CourtZone.Back)]
        public void ZonesFollowDepthFromNet(float z, CourtZone zone)
        {
            Assert.That(new CourtGeometry(CourtConfig.Fip2026()).ZoneOf(z), Is.EqualTo(zone));
        }
    }
}
