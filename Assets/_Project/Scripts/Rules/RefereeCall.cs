using Padel.Core;

namespace Padel.Rules
{
    public enum CallKind
    {
        /// <summary>Play continues.</summary>
        None = 0,
        /// <summary>Serve fault; the server serves again (second serve).</summary>
        Fault = 1,
        /// <summary>Serve let; the same serve is repeated.</summary>
        Let = 2,
        /// <summary>The point is over; see <see cref="RefereeCall.Winner"/> and <see cref="RefereeCall.Reason"/>.</summary>
        Point = 3,
    }

    /// <summary>Why a point ended or a serve was called (shown to the player as a learning aid, GAMEPLAY_SPEC §6).</summary>
    public enum PointReason
    {
        None = 0,
        DoubleFault,
        ServeOutOfBox,
        ServeHitOwnWall,
        ServeHitMeshAfterBounce,
        ServeLeftCourt,
        ReceiverVolleyedServe,
        DoubleBounce,
        HitOpponentWallBeforeBounce,
        OutBeforeBounce,
        OutAfterBounce,
        DidNotCrossNet,
        HitOwnMesh,
        DoubleHit,
    }

    public struct RefereeCall
    {
        public CallKind Kind;
        public TeamId Winner;
        public PointReason Reason;

        public static RefereeCall Continue => default;
        public static RefereeCall PointTo(TeamId winner, PointReason reason) =>
            new RefereeCall { Kind = CallKind.Point, Winner = winner, Reason = reason };

        public override string ToString() => Kind == CallKind.Point ? $"Point {Winner} ({Reason})" : $"{Kind} ({Reason})";
    }
}
