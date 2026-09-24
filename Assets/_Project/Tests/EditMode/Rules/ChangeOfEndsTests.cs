using NUnit.Framework;
using Padel.Core;

namespace Padel.Rules.Tests
{
    public class ChangeOfEndsTests
    {
        [Test]
        public void EndsChangeAfterOddGamesOfTheSet()
        {
            var config = ScoringConfig.StarPoint();
            var state = ScoreKeeper.NewMatch(config, MatchSetup.Singles());
            bool[] expected = { true, false, true, false, true };
            foreach (bool change in expected)
            {
                Assert.That(ScoreTestUtil.WinGame(state, config, TeamId.A).ChangeEnds, Is.EqualTo(change));
            }
            Assert.That(state.EndsSwapped, Is.True);
        }

        [Test]
        public void EvenSetEndsWithoutChangeThenChangesAfterFirstGameOfNextSet()
        {
            var config = ScoringConfig.StarPoint();
            var state = ScoreKeeper.NewMatch(config, MatchSetup.Singles());
            ScoreChange change = ScoreTestUtil.WinGames(state, config, "ABABABABAA"); // 6-4, 10 games
            Assert.That(change.SetWinner, Is.EqualTo(TeamId.A));
            Assert.That(change.ChangeEnds, Is.False);
            Assert.That(ScoreTestUtil.WinGame(state, config, TeamId.B).ChangeEnds, Is.True);
        }

        [Test]
        public void TiebreakChangesEndsEverySixPointsAndAtItsEnd()
        {
            var config = ScoringConfig.StarPoint();
            var state = ScoreKeeper.NewMatch(config, MatchSetup.Singles());
            ScoreTestUtil.ReachSixAll(state, config);
            bool before = state.EndsSwapped;

            for (int i = 1; i <= 12; i++)
            {
                ScoreChange c = ScoreTestUtil.Play(state, config, i % 2 == 0 ? "B" : "A");
                Assert.That(c.ChangeEnds, Is.EqualTo(i % 6 == 0), $"point {i}");
            }
            Assert.That(state.EndsSwapped, Is.EqualTo(before)); // two swaps

            ScoreTestUtil.Play(state, config, "A");
            ScoreChange end = ScoreTestUtil.Play(state, config, "A"); // 8-6 ends the tie-break (game 13)
            Assert.That(end.SetWinner, Is.EqualTo(TeamId.A));
            Assert.That(end.ChangeEnds, Is.True);
        }

        [Test]
        public void TiebreakEndingOnAMultipleOfSixSwapsOnlyOnce()
        {
            var config = ScoringConfig.StarPoint();
            var state = ScoreKeeper.NewMatch(config, MatchSetup.Singles());
            ScoreTestUtil.ReachSixAll(state, config);
            ScoreTestUtil.Play(state, config, "ABABAB"); // 3-3, swap at 6 points
            bool before = state.EndsSwapped;
            ScoreTestUtil.Play(state, config, "BAAA");  // 6-4 after 10 points
            ScoreChange end = ScoreTestUtil.Play(state, config, "BA"); // 7-5 after 12 points
            Assert.That(end.SetWinner, Is.EqualTo(TeamId.A));
            Assert.That(state.EndsSwapped, Is.EqualTo(!before));
        }
    }
}
