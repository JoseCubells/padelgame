using System;
using Padel.Simulation.Court;

namespace Padel.Simulation.Ball
{
    /// <summary>
    /// Immutable ball and aerodynamics parameters (ADR-004; values and sources in GAMEPLAY_RESEARCH §2.3-2.4).
    /// </summary>
    public sealed class BallConfig
    {
        public float Mass { get; }
        public float Radius { get; }
        public float Gravity { get; }
        public float AirDensity { get; }
        public float DragCoefficient { get; }
        /// <summary>Lift coefficient model C_L(S) = clamp(LiftIntercept + LiftSlope * S, 0, LiftMax) with S = r|w|/|v| [P].</summary>
        public float LiftIntercept { get; }
        public float LiftSlope { get; }
        public float LiftMax { get; }
        /// <summary>Exponential spin decay time constant in seconds [P, no source].</summary>
        public float SpinDecayTime { get; }
        /// <summary>Moment of inertia factor alpha, I = alpha * m * r^2 (thin shell 2/3).</summary>
        public float InertiaFactor { get; }
        /// <summary>Below this rebound normal speed the ball stops bouncing and rolls/slides on the surface.</summary>
        public float MinBounceSpeed { get; }

        private readonly SurfaceMaterial[] _materials;

        public BallConfig(
            float mass, float radius, float gravity, float airDensity, float dragCoefficient,
            float liftIntercept, float liftSlope, float liftMax, float spinDecayTime, float inertiaFactor,
            float minBounceSpeed, SurfaceMaterial floor, SurfaceMaterial glass, SurfaceMaterial mesh, SurfaceMaterial net)
        {
            if (mass <= 0f) throw new ArgumentOutOfRangeException(nameof(mass));
            if (radius <= 0f) throw new ArgumentOutOfRangeException(nameof(radius));
            if (dragCoefficient < 0f) throw new ArgumentOutOfRangeException(nameof(dragCoefficient));
            if (spinDecayTime <= 0f) throw new ArgumentOutOfRangeException(nameof(spinDecayTime));
            if (inertiaFactor <= 0f || inertiaFactor > 1f) throw new ArgumentOutOfRangeException(nameof(inertiaFactor));

            Mass = mass;
            Radius = radius;
            Gravity = gravity;
            AirDensity = airDensity;
            DragCoefficient = dragCoefficient;
            LiftIntercept = liftIntercept;
            LiftSlope = liftSlope;
            LiftMax = liftMax;
            SpinDecayTime = spinDecayTime;
            InertiaFactor = inertiaFactor;
            MinBounceSpeed = minBounceSpeed;
            _materials = new SurfaceMaterial[4];
            _materials[(int)SurfaceKind.Floor] = floor;
            _materials[(int)SurfaceKind.Glass] = glass;
            _materials[(int)SurfaceKind.Mesh] = mesh;
            _materials[(int)SurfaceKind.Net] = net;
        }

        public float CrossSectionArea => (float)Math.PI * Radius * Radius;

        /// <summary>k = 1/2 * rho * A / m, so that drag acceleration = k * C_D * |v| * v.</summary>
        public float AeroFactor => 0.5f * AirDensity * CrossSectionArea / Mass;

        public SurfaceMaterial Material(SurfaceKind surface) => _materials[(int)surface];

        /// <summary>
        /// FIP 2026 ball (mid-range 57.7 g, 6.56 cm). Floor restitution is calibrated *in simulation, including drag*
        /// so a 2.54 m drop rebounds inside the FIP 1.35-1.45 m window (test: BallCalibrationTests).
        /// Glass/mesh/net and aerodynamic values are [P] tennis-literature approximations pending KI-005.
        /// </summary>
        public static BallConfig Fip2026Default() => new BallConfig(
            mass: 0.0577f,
            radius: 0.0328f,
            gravity: 9.81f,
            airDensity: 1.2f,
            dragCoefficient: 0.55f,
            liftIntercept: 0.057f,
            liftSlope: 0.364f,
            liftMax: 0.30f,
            spinDecayTime: 4f,
            inertiaFactor: 2f / 3f,
            minBounceSpeed: 0.15f,
            floor: new SurfaceMaterial(0.772f, 0.65f, 8f, 30f, friction: 0.6f),
            glass: new SurfaceMaterial(0.72f, 0.65f, 8f, 35f, friction: 0.28f),
            mesh: new SurfaceMaterial(0.40f, 0.30f, 5f, 30f, friction: 0.5f, normalJitterDegrees: 15f),
            net: new SurfaceMaterial(0.15f, 0.10f, 5f, 30f, friction: 0.8f));
    }
}
