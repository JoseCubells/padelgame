using NUnit.Framework;
using Padel.Core;

namespace Padel.Rules.Tests
{
    public class GameScoringTests
    {
        private static ScoreState NewSingles(ScoringConfig config) => ScoreKeeper.NewMatch(config, MatchSetup.Singles());

        [Test]
        public void FourStraightPointsWinTheGame()
        {
            var config = ScoringConfig.StarPoint();
            var state = NewSingles(config);

            ScoreChange change = ScoreTestUtil.Play(state, config, "AAA");
            Assert.That(change.GameWinner, Is.Null);
            Assert.That(state.PointsInGame(TeamId.A), Is.EqualTo(3)); // 40

            change = ScoreTestUtil.Play(state, config, "A");
            Assert.That(change.GameWinner, Is.EqualTo(TeamId.A));
            Assert.That(state.GamesInSet(TeamId.A), Is.EqualTo(1));
            Assert.That(state.PointsInGame(TeamId.A), Is.EqualTo(0));
        }

        [Test]
        public void FortyFifteenThenWin()
        {
            var config = ScoringConfig.Classic();
            var state = NewSingles(config);
            ScoreChange change = ScoreTestUtil.Play(state, config, "AABAA");
            Assert.That(change.GameWinner, Is.EqualTo(TeamId.A));
        }

        [Test]
        public void ClassicAdvantageCanBeLostIndefinitely()
        {
            var config = ScoringConfig.Classic();
            var state = NewSingles(config);
            ScoreTestUtil.Play(state, config, "AAABBB");
            for (int i = 0; i < 20; i++)
            {
                ScoreChange c1 = ScoreTestUtil.Play(state, config, "A");
                ScoreChange c2 = ScoreTestUtil.Play(state, config, "B");
                Assert.That(c1.GameWinner, Is.Null);
                Assert.That(c2.GameWinner, Is.Null);
                Assert.That(ScoreKeeper.IsDecisivePoint(state, config), Is.False);
            }
            Assert.That(ScoreTestUtil.Play(state, config, "BB").GameWinner, Is.EqualTo(TeamId.B));
        }

        [Test]
        public void GoldenPointDecidesTheGameAtFirstDeuce()
        {
            var config = ScoringConfig.GoldenPoint();
            var state = NewSingles(config);

            ScoreChange change = ScoreTestUtil.Play(state, config, "AAABBB");
            Assert.That(change.DecisivePointNext, Is.True);
            Assert.That(ScoreKeeper.IsDecisivePoint(state, config), Is.True);

            change = ScoreTestUtil.Play(state, config, "B");
            Assert.That(change.GameWinner, Is.EqualTo(TeamId.B));
        }

        [Test]
        public void StarPointFollowsFip2026Sequence()
        {
            // deuce 1 -> adv 1 (A) -> deuce 2 -> adv 2 (A) -> deuce 3 = Star Point (FIP 2026, P2).
            var config = ScoringConfig.StarPoint();
            var state = NewSingles(config);

            ScoreTestUtil.Play(state, config, "AAABBB");
            Assert.That(ScoreKeeper.DeuceNumber(state), Is.EqualTo(1));
            Assert.That(ScoreKeeper.IsDecisivePoint(state, config), Is.False);

            Assert.That(ScoreTestUtil.Play(state, config, "A").GameWinner, Is.Null); // advantage 1
            Assert.That(ScoreTestUtil.Play(state, config, "B").GameWinner, Is.Null); // deuce 2
            Assert.That(ScoreKeeper.DeuceNumber(state), Is.EqualTo(2));
            Assert.That(ScoreKeeper.IsDecisivePoint(state, config), Is.False);

            Assert.That(ScoreTestUtil.Play(state, config, "A").GameWinner, Is.Null); // advantage 2
            ScoreChange change = ScoreTestUtil.Play(state, config, "B");            // deuce 3
            Assert.That(change.GameWinner, Is.Null);
            Assert.That(change.DecisivePointNext, Is.True);
            Assert.That(ScoreKeeper.DeuceNumber(state), Is.EqualTo(3));

            change = ScoreTestUtil.Play(state, config, "B"); // Star Point
            Assert.That(change.GameWinner, Is.EqualTo(TeamId.B));
        }

        [Test]
        public void StarPointGameNeverExceedsElevenPoints()
        {
            var config = ScoringConfig.StarPoint();
            var state = NewSingles(config);
            ScoreChange change = ScoreTestUtil.Play(state, config, "AAABBBABABA");
            Assert.That(change.GameWinner, Is.EqualTo(TeamId.A));
        }

        [Test]
        public void StarPointAdvantageCanStillBeConverted()
        {
            var config = ScoringConfig.StarPoint();
            var state = NewSingles(config);
            ScoreChange change = ScoreTestUtil.Play(state, config, "AAABBBAA");
            Assert.That(change.GameWinner, Is.EqualTo(TeamId.A));
        }

        [TestCase(0, 1)]
        [TestCase(1, 2)]
        [TestCase(2, 3)]
        [TestCase(3, 4)]
        public void CustomMaxAdvantagesSetsTheDecisiveDeuce(int maxAdvantages, int decisiveDeuce)
        {
            var config = new ScoringConfig(maxAdvantages);
            var state = NewSingles(config);
            ScoreTestUtil.Play(state, config, "AAABBB");
            while (ScoreKeeper.DeuceNumber(state) < decisiveDeuce)
            {
                Assert.That(ScoreKeeper.IsDecisivePoint(state, config), Is.False);
                ScoreTestUtil.Play(state, config, "AB");
            }
            Assert.That(ScoreKeeper.IsDecisivePoint(state, config), Is.True);
        }

        [Test]
        public void AwardingAPointAfterMatchEndThrows()
        {
            var config = new ScoringConfig(2, gamesPerSet: 1, setsToWin: 1);
            var state = NewSingles(config);
            // Sets still need a two-game lead, so 1-0 is not enough even with one game per set.
            Assert.That(ScoreTestUtil.WinGame(state, config, TeamId.A).MatchWinner, Is.Null);
            ScoreChange change = ScoreTestUtil.WinGame(state, config, TeamId.A);
            Assert.That(change.MatchWinner, Is.EqualTo(TeamId.A));
            Assert.Throws<System.InvalidOperationException>(() => ScoreKeeper.AwardPoint(state, config, TeamId.B));
        }
    }
}
