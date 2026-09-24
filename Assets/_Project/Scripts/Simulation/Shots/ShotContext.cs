using Padel.Simulation.Court;
using Padel.Simulation.Players;

namespace Padel.Simulation.Shots
{
    /// <summary>Situation of a stroke used to pick the concrete technique (GAMEPLAY_SPEC §5.2).</summary>
    public struct ShotContext
    {
        public ShotIntent Intent;
        public bool Touch;
        /// <summary>0..1 fraction of the maximum charge time.</summary>
        public float Charge;
        public bool IsServe;
        public float ContactHeight;
        /// <summary>The ball has bounced on the hitter's floor since the last stroke (false = volley).</summary>
        public bool Bounced;
        /// <summary>After bouncing, the ball came off the hitter's own back/side glass.</summary>
        public bool OffOwnWall;
        public CourtZone Zone;
    }
}
