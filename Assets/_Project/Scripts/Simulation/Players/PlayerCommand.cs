using System;
using Padel.Core;

namespace Padel.Simulation.Players
{
    /// <summary>
    /// Everything a controller (human input adapter, AI, scripted test, future network) can ask of a player in one
    /// simulation tick (ADR-007). Directions are court-relative: X across, Y along the court (world Z).
    /// </summary>
    [Serializable]
    public struct PlayerCommand
    {
        /// <summary>Desired movement, magnitude 0..1.</summary>
        public Vec2 Move;
        /// <summary>Shot family pressed on this tick (edge), or None.</summary>
        public ShotIntent Pressed;
        /// <summary>True while the shot button is held (charging).</summary>
        public bool Held;
        /// <summary>Aim direction at contact, magnitude 0..1 (0 = default target).</summary>
        public Vec2 Aim;
        /// <summary>Touch modifier (drop shot / chiquita).</summary>
        public bool Touch;

        public static PlayerCommand Idle => default;
    }
}
