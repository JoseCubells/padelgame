using System;

namespace Padel.Simulation.Ball
{
    /// <summary>
    /// Bounce response of one court surface (ADR-004). Normal restitution decreases linearly from
    /// <see cref="RestitutionLow"/> at <see cref="SpeedLow"/> to <see cref="RestitutionHigh"/> at <see cref="SpeedHigh"/>
    /// (impact normal speed, m/s). Values are tuning data [P] until calibrated with reference video (KI-005).
    /// </summary>
    public readonly struct SurfaceMaterial
    {
        public readonly float RestitutionLow;
        public readonly float RestitutionHigh;
        public readonly float SpeedLow;
        public readonly float SpeedHigh;
        /// <summary>Coulomb sliding friction coefficient at the contact.</summary>
        public readonly float Friction;
        /// <summary>Random tilt of the bounce normal in degrees (seeded). Models the irregular mesh.</summary>
        public readonly float NormalJitterDegrees;

        public SurfaceMaterial(float restitutionLow, float restitutionHigh, float speedLow, float speedHigh, float friction, float normalJitterDegrees = 0f)
        {
            if (restitutionLow < 0f || restitutionLow > 1f) throw new ArgumentOutOfRangeException(nameof(restitutionLow));
            if (restitutionHigh < 0f || restitutionHigh > 1f) throw new ArgumentOutOfRangeException(nameof(restitutionHigh));
            if (speedHigh <= speedLow) throw new ArgumentOutOfRangeException(nameof(speedHigh));
            if (friction < 0f) throw new ArgumentOutOfRangeException(nameof(friction));

            RestitutionLow = restitutionLow;
            RestitutionHigh = restitutionHigh;
            SpeedLow = speedLow;
            SpeedHigh = speedHigh;
            Friction = friction;
            NormalJitterDegrees = normalJitterDegrees;
        }

        public float RestitutionAt(float normalSpeed)
        {
            float t = (normalSpeed - SpeedLow) / (SpeedHigh - SpeedLow);
            t = t < 0f ? 0f : (t > 1f ? 1f : t);
            return RestitutionLow + (RestitutionHigh - RestitutionLow) * t;
        }
    }
}
