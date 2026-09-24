using Padel.Core;

namespace Padel.Rules
{
    public enum RallyEventKind
    {
        /// <summary>A player of <see cref="RallyEvent.Team"/> hit the ball.</summary>
        Hit = 0,
        /// <summary>The ball bounced on the floor of the half owned by <see cref="RallyEvent.Team"/>.</summary>
        FloorBounce = 1,
        /// <summary>The ball touched a wall (glass or mesh) of the half owned by <see cref="RallyEvent.Team"/>.</summary>
        WallContact = 2,
        /// <summary>The ball touched the net.</summary>
        NetContact = 3,
        /// <summary>The ball crossed over the net into the half owned by <see cref="RallyEvent.Team"/>.</summary>
        CrossedNet = 4,
        /// <summary>The ball left the court over the walls (no outside play in the MVP).</summary>
        LeftCourt = 5,
    }

    /// <summary>
    /// Rule-level event. The simulation translates physical ball events into these using the court and the current
    /// ends, so <see cref="PointReferee"/> reasons about teams and halves, never about coordinates or physics.
    /// </summary>
    public struct RallyEvent
    {
        public RallyEventKind Kind;
        public TeamId Team;
        /// <summary>For <see cref="RallyEventKind.WallContact"/>: true for metal mesh, false for glass/solid wall.</summary>
        public bool IsMesh;
        /// <summary>For <see cref="RallyEventKind.FloorBounce"/> during a serve: inside the target service box.</summary>
        public bool InServiceBox;

        public static RallyEvent Hit(TeamId team) => new RallyEvent { Kind = RallyEventKind.Hit, Team = team };
        public static RallyEvent Bounce(TeamId half, bool inServiceBox = false) =>
            new RallyEvent { Kind = RallyEventKind.FloorBounce, Team = half, InServiceBox = inServiceBox };
        public static RallyEvent Glass(TeamId half) => new RallyEvent { Kind = RallyEventKind.WallContact, Team = half };
        public static RallyEvent Mesh(TeamId half) => new RallyEvent { Kind = RallyEventKind.WallContact, Team = half, IsMesh = true };
        public static RallyEvent Net() => new RallyEvent { Kind = RallyEventKind.NetContact };
        public static RallyEvent Crossed(TeamId intoHalf) => new RallyEvent { Kind = RallyEventKind.CrossedNet, Team = intoHalf };
        public static RallyEvent Out() => new RallyEvent { Kind = RallyEventKind.LeftCourt };

        public override string ToString() => $"{Kind}({Team}{(IsMesh ? ",mesh" : "")}{(InServiceBox ? ",box" : "")})";
    }
}
