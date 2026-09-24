using System;

namespace Padel.Rules
{
    /// <summary>Final score of a completed set. Tie-break points are 0 when no tie-break was played.</summary>
    [Serializable]
    public struct SetResult
    {
        public int GamesA;
        public int GamesB;
        public int TiebreakPointsA;
        public int TiebreakPointsB;
        public bool WasSuperTiebreak;

        public override string ToString() =>
            TiebreakPointsA + TiebreakPointsB > 0
                ? $"{GamesA}-{GamesB} ({TiebreakPointsA}-{TiebreakPointsB})"
                : $"{GamesA}-{GamesB}";
    }
}
