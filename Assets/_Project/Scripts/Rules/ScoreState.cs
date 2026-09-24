using System;
using System.Collections.Generic;
using Padel.Core;

namespace Padel.Rules
{
    /// <summary>
    /// Complete, serializable score of a match. Mutated only through <see cref="ScoreKeeper"/>.
    /// Team-indexed arrays use <c>(int)TeamId</c>.
    /// </summary>
    [Serializable]
    public sealed class ScoreState
    {
        internal PlayerSlot[] ServeOrder;
        internal int[] RightReceiver = new int[2];
        internal int[] Sets = new int[2];
        internal int[] Games = new int[2];
        internal int[] Points = new int[2];
        internal List<SetResult> CompletedSetList = new List<SetResult>();

        public int PlayersPerTeam { get; internal set; }
        /// <summary>Index in the serve order of the server of the current game, or of the first server of the current tie-break.</summary>
        public int ServerOrderIndex { get; internal set; }
        public bool InTiebreak { get; internal set; }
        public bool IsSuperTiebreak { get; internal set; }
        public int GamesPlayedInSet { get; internal set; }
        /// <summary>False while team A plays on the negative-Z half; toggles on every change of ends.</summary>
        public bool EndsSwapped { get; internal set; }
        /// <summary>Side chosen by the receiving team for the current decisive point (FIP O4). Right by default.</summary>
        public ServeSide DecisivePointSide { get; internal set; }
        public TeamId? Winner { get; internal set; }

        public IReadOnlyList<PlayerSlot> ServeOrderView => ServeOrder;
        public IReadOnlyList<SetResult> CompletedSets => CompletedSetList;
        public int CurrentSetNumber => CompletedSetList.Count + 1;
        public bool IsOver => Winner.HasValue;

        public int SetsWon(TeamId team) => Sets[(int)team];
        public int GamesInSet(TeamId team) => Games[(int)team];
        /// <summary>Raw points in the current game (0,1,2,3 = 0/15/30/40) or tie-break points.</summary>
        public int PointsInGame(TeamId team) => Points[(int)team];
        public int PointsPlayedInGame => Points[0] + Points[1];

        public ScoreState Clone()
        {
            var copy = (ScoreState)MemberwiseClone();
            copy.ServeOrder = (PlayerSlot[])ServeOrder.Clone();
            copy.RightReceiver = (int[])RightReceiver.Clone();
            copy.Sets = (int[])Sets.Clone();
            copy.Games = (int[])Games.Clone();
            copy.Points = (int[])Points.Clone();
            copy.CompletedSetList = new List<SetResult>(CompletedSetList);
            return copy;
        }
    }
}
