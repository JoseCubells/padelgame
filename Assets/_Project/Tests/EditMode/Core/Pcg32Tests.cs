using NUnit.Framework;

namespace Padel.Core.Tests
{
    public class Pcg32Tests
    {
        [Test]
        public void MatchesPcgReferenceSequence_Seed42Stream54()
        {
            // Reference output of pcg32-demo (pcg-c) for pcg32_srandom_r(42, 54), round 1.
            var rng = new Pcg32(42UL, 54UL);
            uint[] expected = { 0xa15c02b7u, 0x7b47f409u, 0xba1d3330u, 0x83d2f293u, 0xbfa4784bu, 0xcbed606eu };
            foreach (uint value in expected)
            {
                Assert.That(rng.NextUInt(), Is.EqualTo(value));
            }
        }

        [Test]
        public void SameSeedProducesSameSequence()
        {
            var a = new Pcg32(1234UL);
            var b = new Pcg32(1234UL);
            for (int i = 0; i < 1000; i++)
            {
                Assert.That(a.NextUInt(), Is.EqualTo(b.NextUInt()));
            }
        }

        [Test]
        public void CopyingTheStructForksTheSequence()
        {
            var original = new Pcg32(7UL);
            original.NextUInt();
            Pcg32 copy = original;
            Assert.That(copy.NextUInt(), Is.EqualTo(original.NextUInt()));
        }

        [Test]
        public void NextFloatStaysInUnitInterval()
        {
            var rng = new Pcg32(99UL);
            for (int i = 0; i < 100000; i++)
            {
                float f = rng.NextFloat();
                Assert.That(f, Is.GreaterThanOrEqualTo(0f).And.LessThan(1f));
            }
        }

        [Test]
        public void GaussianHasApproximatelyZeroMeanAndUnitVariance()
        {
            var rng = new Pcg32(2026UL);
            const int n = 200000;
            double sum = 0, sumSq = 0;
            for (int i = 0; i < n; i++)
            {
                double g = rng.NextGaussian();
                sum += g;
                sumSq += g * g;
            }
            double mean = sum / n;
            double variance = sumSq / n - mean * mean;
            Assert.That(mean, Is.EqualTo(0.0).Within(0.01));
            Assert.That(variance, Is.EqualTo(1.0).Within(0.02));
        }
    }
}
