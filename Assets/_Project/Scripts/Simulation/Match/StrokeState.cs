using System;
using Padel.Core;
using Padel.Simulation.Players;
using Padel.Simulation.Shots;

namespace Padel.Simulation.Match
{
    /// <summary>A pending or executing stroke of one player.</summary>
    [Serializable]
    public struct StrokeState
    {
        public bool Active;
        public ShotIntent Intent;
        public bool Touch;
        public Vec2 Aim;
        public long PressTick;
        /// <summary>Tick at which the racket meets the ball (press + windup).</summary>
        public long ContactTick;
        /// <summary>Predicted tick of the ideal contact at press time; timing quality compares against it.</summary>
        public long IdealTick;
        /// <summary>Ticks the button has been held since the press.</summary>
        public int HeldTicks;
        public bool Released;
        public bool IsServe;
        /// <summary>Swing readiness minus ideal contact (s): negative = pressed early, positive = late.</summary>
        public float TimingOffset;
        /// <summary>Recovery time of the executed shot (s); 0 until a shot is executed.</summary>
        public float Recovery;
    }
}
