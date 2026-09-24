using System;
using Padel.Core;

namespace Padel.Simulation.Players
{
    /// <summary>Mutable per-player simulation state. Court-plane position/velocity (Y = world Z).</summary>
    [Serializable]
    public struct PlayerState
    {
        public Vec2 Position;
        public Vec2 Velocity;
        /// <summary>Unit direction the player faces on the court plane.</summary>
        public Vec2 Facing;
        public PlayerPhase Phase;
        /// <summary>Seconds elapsed in the current phase.</summary>
        public float PhaseTime;
        /// <summary>Seconds of split-step acceleration boost remaining.</summary>
        public float BoostTime;
    }
}
