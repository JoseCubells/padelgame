using Padel.Core;

namespace Padel.Rules.Tests
{
    internal static class ScoreTestUtil
    {
        /// <summary>Awards points from a string such as "AABAB" and returns the last change.</summary>
        public static ScoreChange Play(ScoreState state, ScoringConfig config, string points)
        {
            ScoreChange last = default;
            foreach (char c in points)
            {
                last = ScoreKeeper.AwardPoint(state, config, c == 'A' ? TeamId.A : TeamId.B);
            }
            return last;
        }

        /// <summary>Wins a game from 0-0 with four straight points.</summary>
        public static ScoreChange WinGame(ScoreState state, ScoringConfig config, TeamId team)
        {
            string p = team == TeamId.A ? "AAAA" : "BBBB";
            return Play(state, config, p);
        }

        /// <summary>Plays games alternating winners according to the string, e.g. "AABAB".</summary>
        public static ScoreChange WinGames(ScoreState state, ScoringConfig config, string games)
        {
            ScoreChange last = default;
            foreach (char c in games)
            {
                last = WinGame(state, config, c == 'A' ? TeamId.A : TeamId.B);
            }
            return last;
        }

        /// <summary>Reaches 6-6 in the current set with alternating games (A,B,A,B...).</summary>
        public static ScoreChange ReachSixAll(ScoreState state, ScoringConfig config) =>
            WinGames(state, config, "ABABABABABAB");
    }
}
