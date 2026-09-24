using System;
using Padel.Core;

namespace Padel.Simulation.Ball
{
    /// <summary>Ball kinematic state. Spin is angular velocity in rad/s (axis = direction, magnitude = rate).</summary>
    [Serializable]
    public struct BallState
    {
        public Vec3 Position;
        public Vec3 Velocity;
        public Vec3 Spin;

        public BallState(Vec3 position, Vec3 velocity, Vec3 spin)
        {
            Position = position;
            Velocity = velocity;
            Spin = spin;
        }
    }
}
