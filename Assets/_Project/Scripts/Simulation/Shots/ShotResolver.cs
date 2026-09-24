using Padel.Simulation.Players;

namespace Padel.Simulation.Shots
{
    /// <summary>
    /// Maps intent + context to a technique with an ordered rule table: first matching rule wins
    /// (GAMEPLAY_SPEC §5.2). Thresholds are [P] tuning values.
    /// </summary>
    public static class ShotResolver
    {
        /// <summary>Contact height above which a ball is played as an overhead (m) [P].</summary>
        public const float OverheadHeight = 2.2f;
        /// <summary>Charge fraction from which an overhead attack becomes a smash instead of a víbora [P].</summary>
        public const float SmashCharge = 0.5f;

        public static ShotType Resolve(in ShotContext c)
        {
            if (c.IsServe) return ShotType.Serve;

            bool overhead = !c.Bounced && c.ContactHeight >= OverheadHeight;
            if (overhead)
            {
                switch (c.Intent)
                {
                    case ShotIntent.Attack: return c.Charge >= SmashCharge ? ShotType.Smash : ShotType.Vibora;
                    case ShotIntent.Control: return ShotType.Bandeja;
                    default: return ShotType.Lob;
                }
            }

            if (!c.Bounced)
            {
                if (c.Touch) return ShotType.DropVolley;
                return c.Intent == ShotIntent.Lob ? ShotType.Lob : ShotType.Volley;
            }

            if (c.Touch) return ShotType.Chiquita;
            switch (c.Intent)
            {
                case ShotIntent.Lob: return ShotType.Lob;
                case ShotIntent.Control: return ShotType.Slice;
                default: return c.OffOwnWall ? ShotType.WallExit : ShotType.Drive;
            }
        }
    }
}
