using System;

namespace Padel.Rules
{
    /// <summary>How the deciding set is played (FIP 2026 rules, docs/research/PADEL_RULES_RESEARCH.md P4/P8).</summary>
    public enum FinalSetMode
    {
        /// <summary>Normal set with a tie-break at games-all.</summary>
        Tiebreak,
        /// <summary>Normal set without tie-break: play on until a two-game lead.</summary>
        AdvantageSet,
        /// <summary>The deciding set is replaced by a match tie-break (default 10 points).</summary>
        SuperTiebreak,
    }

    /// <summary>
    /// Immutable scoring configuration. Covers CLASSIC, GOLDEN_POINT, STAR_POINT and CUSTOM_ARCADE
    /// scoring through <see cref="MaxAdvantages"/> and the set/tie-break parameters.
    /// </summary>
    public sealed class ScoringConfig
    {
        /// <summary>Value of <see cref="MaxAdvantages"/> meaning "advantage can be lost forever" (classic scoring).</summary>
        public const int UnlimitedAdvantages = -1;

        /// <summary>
        /// Number of advantages that can be lost before the next deuce becomes a single decisive point.
        /// 0 = golden point, 2 = Star Point (FIP 2026), -1 = classic unlimited advantage.
        /// </summary>
        public int MaxAdvantages { get; }
        public int GamesPerSet { get; }
        public int SetsToWin { get; }
        public int TiebreakPoints { get; }
        public FinalSetMode FinalSet { get; }
        public int SuperTiebreakPoints { get; }
        /// <summary>Change ends every N points inside a (super) tie-break (FIP P9: 6).</summary>
        public int TiebreakChangeEndsEvery { get; }

        public ScoringConfig(
            int maxAdvantages,
            int gamesPerSet = 6,
            int setsToWin = 2,
            int tiebreakPoints = 7,
            FinalSetMode finalSet = FinalSetMode.Tiebreak,
            int superTiebreakPoints = 10,
            int tiebreakChangeEndsEvery = 6)
        {
            if (maxAdvantages < UnlimitedAdvantages) throw new ArgumentOutOfRangeException(nameof(maxAdvantages));
            if (gamesPerSet < 1) throw new ArgumentOutOfRangeException(nameof(gamesPerSet));
            if (setsToWin < 1) throw new ArgumentOutOfRangeException(nameof(setsToWin));
            if (tiebreakPoints < 1) throw new ArgumentOutOfRangeException(nameof(tiebreakPoints));
            if (superTiebreakPoints < 1) throw new ArgumentOutOfRangeException(nameof(superTiebreakPoints));
            if (tiebreakChangeEndsEvery < 1) throw new ArgumentOutOfRangeException(nameof(tiebreakChangeEndsEvery));

            MaxAdvantages = maxAdvantages;
            GamesPerSet = gamesPerSet;
            SetsToWin = setsToWin;
            TiebreakPoints = tiebreakPoints;
            FinalSet = finalSet;
            SuperTiebreakPoints = superTiebreakPoints;
            TiebreakChangeEndsEvery = tiebreakChangeEndsEvery;
        }

        /// <summary>FIP 2026 official default: Star Point, best of 3 sets, tie-break at 6-6 (P2, P4-P7).</summary>
        public static ScoringConfig StarPoint() => new ScoringConfig(maxAdvantages: 2);

        /// <summary>Golden point ("no advantage"), kept by FIP 2026 as an alternative (P3).</summary>
        public static ScoringConfig GoldenPoint() => new ScoringConfig(maxAdvantages: 0);

        /// <summary>Classic unlimited advantage.</summary>
        public static ScoringConfig Classic() => new ScoringConfig(maxAdvantages: UnlimitedAdvantages);
    }
}
