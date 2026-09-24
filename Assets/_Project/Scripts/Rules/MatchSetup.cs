using System;
using Padel.Core;

namespace Padel.Rules
{
    /// <summary>
    /// Who plays and in which order at the start of the match. The same model serves 1v1 (one player per team)
    /// and 2v2 (two players per team), so no scoring logic is duplicated between modes.
    /// </summary>
    public sealed class MatchSetup
    {
        public int PlayersPerTeam { get; }
        public TeamId FirstServingTeam { get; }
        /// <summary>Per team: index of the player who serves first for that team in the serve order.</summary>
        public int FirstServerA { get; }
        public int FirstServerB { get; }
        /// <summary>Per team: index of the player who receives on the right (drive) side.</summary>
        public int RightReceiverA { get; }
        public int RightReceiverB { get; }

        public MatchSetup(
            int playersPerTeam,
            TeamId firstServingTeam = TeamId.A,
            int firstServerA = 0,
            int firstServerB = 0,
            int rightReceiverA = 0,
            int rightReceiverB = 0)
        {
            if (playersPerTeam != 1 && playersPerTeam != 2) throw new ArgumentOutOfRangeException(nameof(playersPerTeam));
            ValidateIndex(firstServerA, playersPerTeam, nameof(firstServerA));
            ValidateIndex(firstServerB, playersPerTeam, nameof(firstServerB));
            ValidateIndex(rightReceiverA, playersPerTeam, nameof(rightReceiverA));
            ValidateIndex(rightReceiverB, playersPerTeam, nameof(rightReceiverB));

            PlayersPerTeam = playersPerTeam;
            FirstServingTeam = firstServingTeam;
            FirstServerA = firstServerA;
            FirstServerB = firstServerB;
            RightReceiverA = rightReceiverA;
            RightReceiverB = rightReceiverB;
        }

        public static MatchSetup Singles(TeamId firstServingTeam = TeamId.A) => new MatchSetup(1, firstServingTeam);
        public static MatchSetup Doubles(TeamId firstServingTeam = TeamId.A) => new MatchSetup(2, firstServingTeam);

        public int FirstServer(TeamId team) => team == TeamId.A ? FirstServerA : FirstServerB;
        public int RightReceiver(TeamId team) => team == TeamId.A ? RightReceiverA : RightReceiverB;

        private static void ValidateIndex(int index, int playersPerTeam, string name)
        {
            if (index < 0 || index >= playersPerTeam) throw new ArgumentOutOfRangeException(name);
        }
    }
}
