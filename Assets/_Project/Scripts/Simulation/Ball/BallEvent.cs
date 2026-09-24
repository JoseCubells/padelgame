using Padel.Core;
using Padel.Simulation.Court;

namespace Padel.Simulation.Ball
{
    public enum BallEventKind
    {
        /// <summary>Contact with a surface (floor, glass, mesh or net).</summary>
        Contact = 0,
        /// <summary>The ball centre crossed the net plane above the net.</summary>
        CrossedNet = 1,
        /// <summary>The ball left the enclosure over a wall.</summary>
        LeftCourt = 2,
    }

    /// <summary>Event produced during a simulation step; consumed by rules (point outcome) and presentation (audio/VFX).</summary>
    public struct BallEvent
    {
        public BallEventKind Kind;
        public SurfaceKind Surface;
        public Vec3 Position;
        public Vec3 Normal;
        /// <summary>Impact normal speed (m/s) for contacts; 0 otherwise. Drives sound and VFX intensity.</summary>
        public float ImpactSpeed;

        public override string ToString() =>
            Kind == BallEventKind.Contact ? $"{Kind}:{Surface} at {Position} ({ImpactSpeed:0.0} m/s)" : $"{Kind} at {Position}";
    }
}
