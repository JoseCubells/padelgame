using Padel.Core;

namespace Padel.Rules
{
    /// <summary>What happened as a consequence of awarding one point. Consumed by match flow and presentation.</summary>
    public struct ScoreChange
    {
        public TeamId PointWinner;
        public TeamId? GameWinner;
        public TeamId? SetWinner;
        public TeamId? MatchWinner;
        public bool TiebreakStarted;
        public bool ChangeEnds;
        /// <summary>The next point is a single decisive point (golden point or Star Point).</summary>
        public bool DecisivePointNext;
    }
}
