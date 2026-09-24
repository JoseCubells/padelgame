using NUnit.Framework;
using Padel.Core;

namespace Padel.Rules.Tests
{
    public class SetAndTiebreakTests
    {
        [Test]
        public void SetIsWonAtSixWithTwoGameLead()
        {
            var config = ScoringConfig.StarPoint();
            var state = ScoreKeeper.NewMatch(config, MatchSetup.Singles());
            ScoreChange change = ScoreTestUtil.WinGames(state, config, "ABABABABAA"); // 6-4
            Assert.That(change.SetWinner, Is.EqualTo(TeamId.A));
            Assert.That(state.CompletedSets[0].GamesA, Is.EqualTo(6));
            Assert.That(state.CompletedSets[0].GamesB, Is.EqualTo(4));
            Assert.That(state.GamesInSet(TeamId.A), Is.EqualTo(0));
        }

        [Test]
        public void SixFiveIsNotASetButSevenFiveIs()
        {
            var config = ScoringConfig.StarPoint();
            var state = ScoreKeeper.NewMatch(config, MatchSetup.Singles());
            ScoreChange change = ScoreTestUtil.WinGames(state, config, "ABABABABABA"); // 6-5
            Assert.That(change.SetWinner, Is.Null);
            change = ScoreTestUtil.WinGame(state, config, TeamId.A); // 7-5
            Assert.That(change.SetWinner, Is.EqualTo(TeamId.A));
        }

        [Test]
        public void SixAllStartsATiebreakWonAtSevenByTwo()
        {
            var config = ScoringConfig.StarPoint();
            var state = ScoreKeeper.NewMatch(config, MatchSetup.Singles());
            ScoreChange change = ScoreTestUtil.ReachSixAll(state, config);
            Assert.That(change.TiebreakStarted, Is.True);
            Assert.That(state.InTiebreak, Is.True);

            change = ScoreTestUtil.Play(state, config, "ABABABABABAB"); // 6-6 in tie-break
            Assert.That(change.SetWinner, Is.Null);
            change = ScoreTestUtil.Play(state, config, "A"); // 7-6: not enough
            Assert.That(change.SetWinner, Is.Null);
            change = ScoreTestUtil.Play(state, config, "A"); // 8-6
            Assert.That(change.SetWinner, Is.EqualTo(TeamId.A));

            SetResult set = state.CompletedSets[0];
            Assert.That(set.GamesA, Is.EqualTo(7));
            Assert.That(set.GamesB, Is.EqualTo(6));
            Assert.That(set.TiebreakPointsA, Is.EqualTo(8));
            Assert.That(set.TiebreakPointsB, Is.EqualTo(6));
            Assert.That(state.InTiebreak, Is.False);
        }

        [Test]
        public void TiebreakHasNoDecisivePoint()
        {
            var config = ScoringConfig.GoldenPoint();
            var state = ScoreKeeper.NewMatch(config, MatchSetup.Singles());
            ScoreTestUtil.ReachSixAll(state, config);
            ScoreTestUtil.Play(state, config, "AAABBB");
            Assert.That(ScoreKeeper.IsDecisivePoint(state, config), Is.False);
        }

        [Test]
        public void BestOfThreeMatchEndsAtTwoSets()
        {
            var config = ScoringConfig.StarPoint();
            var state = ScoreKeeper.NewMatch(config, MatchSetup.Doubles());
            ScoreTestUtil.WinGames(state, config, "AAAAAA");
            ScoreChange change = ScoreTestUtil.WinGames(state, config, "BBBBBB");
            Assert.That(change.SetWinner, Is.EqualTo(TeamId.B));
            Assert.That(state.IsOver, Is.False);
            change = ScoreTestUtil.WinGames(state, config, "AAAAAA");
            Assert.That(change.MatchWinner, Is.EqualTo(TeamId.A));
            Assert.That(state.Winner, Is.EqualTo(TeamId.A));
            Assert.That(state.CompletedSets.Count, Is.EqualTo(3));
        }

        [Test]
        public void AdvantageFinalSetHasNoTiebreak()
        {
            var config = new ScoringConfig(2, finalSet: FinalSetMode.AdvantageSet);
            var state = ScoreKeeper.NewMatch(config, MatchSetup.Singles());
            ScoreTestUtil.WinGames(state, config, "AAAAAA");
            ScoreTestUtil.WinGames(state, config, "BBBBBB");

            ScoreChange change = ScoreTestUtil.ReachSixAll(state, config);
            Assert.That(change.TiebreakStarted, Is.False);
            Assert.That(state.InTiebreak, Is.False);
            ScoreTestUtil.WinGames(state, config, "AB"); // 7-7
            change = ScoreTestUtil.WinGames(state, config, "AA"); // 9-7
            Assert.That(change.MatchWinner, Is.EqualTo(TeamId.A));
            Assert.That(state.CompletedSets[2].GamesA, Is.EqualTo(9));
        }

        [Test]
        public void SuperTiebreakReplacesTheDecidingSet()
        {
            var config = new ScoringConfig(2, finalSet: FinalSetMode.SuperTiebreak);
            var state = ScoreKeeper.NewMatch(config, MatchSetup.Doubles());
            ScoreTestUtil.WinGames(state, config, "AAAAAA");
            ScoreChange change = ScoreTestUtil.WinGames(state, config, "BBBBBB");
            Assert.That(change.TiebreakStarted, Is.True);
            Assert.That(state.InTiebreak, Is.True);
            Assert.That(state.IsSuperTiebreak, Is.True);

            change = ScoreTestUtil.Play(state, config, "ABABABABABABABABAB"); // 9-9
            Assert.That(change.MatchWinner, Is.Null);
            change = ScoreTestUtil.Play(state, config, "BB"); // 9-11
            Assert.That(change.MatchWinner, Is.EqualTo(TeamId.B));
            Assert.That(state.CompletedSets[2].WasSuperTiebreak, Is.True);
            Assert.That(state.CompletedSets[2].TiebreakPointsB, Is.EqualTo(11));
        }
    }
}
