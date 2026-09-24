using NUnit.Framework;
using Padel.Simulation.Match;
using Padel.Simulation.Players;

namespace Padel.AI.Tests
{
    public class AIMatchTests
    {
        private static MatchStats Play(int perTeam, AIProfile a, AIProfile b, ulong seed, int points)
        {
            var sim = new MatchSimulation(MatchConfig.Default(perTeam, seed));
            MatchState state = sim.CreateInitialState();
            ICommandSource[] bots = HeadlessMatch.Bots(sim, a, b, seed);
            MatchStats stats = HeadlessMatch.Run(sim, state, bots, points, 120L * 60 * 60);
            TestContext.WriteLine($"{perTeam}v{perTeam} {a.Name} vs {b.Name} seed {seed}: {stats}");
            return stats;
        }

        [TestCase(1)]
        [TestCase(2)]
        public void MediumBotsPlayRealRallies(int perTeam)
        {
            MatchStats stats = Play(perTeam, AIProfile.Medium(), AIProfile.Medium(), 11UL, 24);
            Assert.That(stats.Points, Is.EqualTo(24));
            Assert.That(stats.MeanStrokesPerPoint, Is.GreaterThanOrEqualTo(3.0), "bots should sustain rallies");
            Assert.That(stats.Faults, Is.LessThan(stats.Points), "serve faults should be occasional");
        }

        [Test]
        public void AIvsAIIsReproducibleFromTheSeed()
        {
            MatchStats x = Play(2, AIProfile.Medium(), AIProfile.Hard(), 5UL, 12);
            MatchStats y = Play(2, AIProfile.Medium(), AIProfile.Hard(), 5UL, 12);
            Assert.That(y.Ticks, Is.EqualTo(x.Ticks));
            Assert.That(y.PointsWonA, Is.EqualTo(x.PointsWonA));
            Assert.That(y.Strokes, Is.EqualTo(x.Strokes));
        }

        [Test]
        public void HardBeatsEasyOverManyPoints()
        {
            int hard = 0, easy = 0;
            foreach (ulong seed in new ulong[] { 1, 2 })
            {
                MatchStats s = Play(1, AIProfile.Hard(), AIProfile.Easy(), seed, 24);
                hard += s.PointsWonA;
                easy += s.PointsWonB;
            }
            Assert.That(hard, Is.GreaterThan(easy));
        }
    }
}
