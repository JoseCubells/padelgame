using NUnit.Framework;

namespace Padel.Core.Tests
{
    public class VecTests
    {
        [Test]
        public void CrossProductFollowsRightHandRuleOnBasis()
        {
            Vec3 x = new Vec3(1f, 0f, 0f);
            Vec3 y = new Vec3(0f, 1f, 0f);
            Assert.That(Vec3.Cross(x, y), Is.EqualTo(new Vec3(0f, 0f, 1f)));
        }

        [Test]
        public void NormalizedZeroVectorIsZero()
        {
            Assert.That(Vec3.Zero.Normalized, Is.EqualTo(Vec3.Zero));
            Assert.That(Vec2.Zero.Normalized, Is.EqualTo(Vec2.Zero));
        }

        [Test]
        public void ClampMagnitudeLimitsLongVectorsOnly()
        {
            Assert.That(new Vec2(3f, 4f).ClampMagnitude(10f), Is.EqualTo(new Vec2(3f, 4f)));
            Vec2 clamped = new Vec2(3f, 4f).ClampMagnitude(1f);
            Assert.That(clamped.Magnitude, Is.EqualTo(1f).Within(1e-6f));
        }

        [Test]
        public void CourtPlaneMapsToWorldXZ()
        {
            Assert.That(new Vec2(2f, -5f).ToWorld(1.5f), Is.EqualTo(new Vec3(2f, 1.5f, -5f)));
        }
    }
}
