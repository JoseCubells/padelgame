using System;
using Padel.Core;

namespace Padel.Rules
{
    /// <summary>A player identified by team and index inside the team (0 or 1).</summary>
    [Serializable]
    public struct PlayerSlot : IEquatable<PlayerSlot>
    {
        public TeamId Team;
        public int Index;

        public PlayerSlot(TeamId team, int index)
        {
            Team = team;
            Index = index;
        }

        public bool Equals(PlayerSlot other) => Team == other.Team && Index == other.Index;
        public override bool Equals(object obj) => obj is PlayerSlot other && Equals(other);
        public override int GetHashCode() => HashCode.Combine((int)Team, Index);
        public override string ToString() => $"{Team}{Index + 1}";
    }
}
