using NUnit.Framework;
using Padel.Core;

namespace Padel.Rules.Tests
{
    public class ServeRotationTests
    {
        [Test]
        public void DoublesServeOrderIsA1B1A2B2()
        {
            var config = ScoringConfig.StarPoint();
            var state = ScoreKeeper.NewMatch(config, new MatchSetup(2, TeamId.A, firstServerA: 1, firstServerB: 0));
            var expected = new[]
            {
                new PlayerSlot(TeamId.A, 1), new PlayerSlot(TeamId.B, 0),
                new PlayerSlot(TeamId.A, 0), new PlayerSlot(TeamId.B, 1),
                new PlayerSlot(TeamId.A, 1),
            };
            foreach (PlayerSlot server in expected)
            {
                Assert.That(ScoreKeeper.CurrentServer(state), Is.EqualTo(server));
                ScoreTestUtil.WinGame(state, config, TeamId.B);
            }
        }

        [Test]
        public void SinglesAlternatesServerEveryGame()
        {
            var config = ScoringConfig.StarPoint();
            var state = ScoreKeeper.NewMatch(config, MatchSetup.Singles(TeamId.B));
            Assert.That(ScoreKeeper.CurrentServer(state).Team, Is.EqualTo(TeamId.B));
            ScoreTestUtil.WinGame(state, config, TeamId.A);
            Assert.That(ScoreKeeper.CurrentServer(state).Team, Is.EqualTo(TeamId.A));
            ScoreTestUtil.WinGame(state, config, TeamId.A);
            Assert.That(ScoreKeeper.CurrentServer(state).Team, Is.EqualTo(TeamId.B));
        }

        [Test]
        public void EveryGameStartsFromTheRightAndAlternates()
        {
            var config = ScoringConfig.StarPoint();
            var state = ScoreKeeper.NewMatch(config, MatchSetup.Singles());
            ServeSide[] expected = { ServeSide.Right, ServeSide.Left, ServeSide.Right, ServeSide.Left };
            foreach (ServeSide side in expected)
            {
                Assert.That(ScoreKeeper.CurrentServeSide(state, config), Is.EqualTo(side));
                ScoreTestUtil.Play(state, config, "A");
            }
            Assert.That(state.PointsPlayedInGame, Is.EqualTo(0)); // game over after four points
            Assert.That(ScoreKeeper.CurrentServeSide(state, config), Is.EqualTo(ServeSide.Right));
        }

        [Test]
        public void DoublesReceiverKeepsTheirSide()
        {
            var config = ScoringConfig.StarPoint();
            var state = ScoreKeeper.NewMatch(config, new MatchSetup(2, TeamId.A, rightReceiverB: 1));
            Assert.That(ScoreKeeper.CurrentReceiver(state, config), Is.EqualTo(new PlayerSlot(TeamId.B, 1)));
            ScoreTestUtil.Play(state, config, "A");
            Assert.That(ScoreKeeper.CurrentReceiver(state, config), Is.EqualTo(new PlayerSlot(TeamId.B, 0)));
        }

        [Test]
        public void TiebreakServingPatternIsOneThenTwoEach()
        {
            // FIP P7: due server serves 1 point from the right; then 2 points per server in order, starting from the left.
            var config = ScoringConfig.StarPoint();
            var state = ScoreKeeper.NewMatch(config, MatchSetup.Doubles());
            ScoreTestUtil.ReachSixAll(state, config); // 12 games: rotation back to index 0 (A1)
            Assert.That(state.InTiebreak, Is.True);

            PlayerSlot a1 = new PlayerSlot(TeamId.A, 0), b1 = new PlayerSlot(TeamId.B, 0);
            PlayerSlot a2 = new PlayerSlot(TeamId.A, 1), b2 = new PlayerSlot(TeamId.B, 1);
            PlayerSlot[] servers = { a1, b1, b1, a2, a2, b2, b2, a1, a1 };
            ServeSide[] sides =
            {
                ServeSide.Right, ServeSide.Left, ServeSide.Right, ServeSide.Left, ServeSide.Right,
                ServeSide.Left, ServeSide.Right, ServeSide.Left, ServeSide.Right,
            };
            for (int i = 0; i < servers.Length; i++)
            {
                Assert.That(ScoreKeeper.CurrentServer(state), Is.EqualTo(servers[i]), $"point {i}");
                Assert.That(ScoreKeeper.CurrentServeSide(state, config), Is.EqualTo(sides[i]), $"point {i}");
                ScoreTestUtil.Play(state, config, i % 2 == 0 ? "A" : "B");
            }
        }

        [Test]
        public void TeamThatReceivedFirstInTiebreakServesFirstNextSet()
        {
            var config = ScoringConfig.StarPoint();
            var state = ScoreKeeper.NewMatch(config, MatchSetup.Doubles());
            ScoreTestUtil.ReachSixAll(state, config);
            Assert.That(ScoreKeeper.CurrentServer(state).Team, Is.EqualTo(TeamId.A)); // A1 opens the tie-break
            ScoreTestUtil.Play(state, config, "AAAAAAA");
            Assert.That(state.CompletedSets.Count, Is.EqualTo(1));
            Assert.That(ScoreKeeper.CurrentServer(state), Is.EqualTo(new PlayerSlot(TeamId.B, 0)));
        }

        [Test]
        public void ReceiversChooseTheSideOnTheDecisivePoint()
        {
            var config = ScoringConfig.GoldenPoint();
            var state = ScoreKeeper.NewMatch(config, MatchSetup.Doubles());
            Assert.Throws<System.InvalidOperationException>(() =>
                ScoreKeeper.ChooseDecisivePointSide(state, config, ServeSide.Left));

            ScoreTestUtil.Play(state, config, "AAABBB"); // six points played: parity says Right
            ScoreKeeper.ChooseDecisivePointSide(state, config, ServeSide.Left);
            Assert.That(ScoreKeeper.CurrentServeSide(state, config), Is.EqualTo(ServeSide.Left));
            Assert.That(ScoreKeeper.CurrentReceiver(state, config), Is.EqualTo(new PlayerSlot(TeamId.B, 1)));

            ScoreTestUtil.Play(state, config, "A");
            Assert.That(state.DecisivePointSide, Is.EqualTo(ServeSide.Right)); // reset for the next game
        }
    }
}
