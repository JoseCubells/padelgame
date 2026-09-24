using System;

namespace Padel.Simulation.Shots
{
    /// <summary>
    /// Immutable data describing one technique (ADR-004/007, GAMEPLAY_SPEC §5.3). Everything that is tuning lives
    /// here, never in code. Values in <see cref="ShotCatalog.Default"/> are [P] starting points from the research
    /// taxonomy, to be tuned in playtest.
    /// </summary>
    public sealed class ShotDefinition
    {
        public ShotType Type { get; }
        public TrajectoryMode Mode { get; }
        /// <summary>Apex mode: apex height above the higher of contact and landing point (m).</summary>
        public float ApexAboveContact { get; }
        /// <summary>Speed mode: launch speed at full charge (m/s); uncharged uses <see cref="MinSpeed"/>.</summary>
        public float MaxSpeed { get; }
        public float MinSpeed { get; }
        /// <summary>Target landing depth range, measured from the net into the opponent's half (m).</summary>
        public float TargetDepthMin { get; }
        public float TargetDepthMax { get; }
        /// <summary>Spin in rad/s: positive = topspin, negative = backspin (slice).</summary>
        public float TopSpin { get; }
        /// <summary>Side spin in rad/s around the vertical axis (víbora, bandeja).</summary>
        public float SideSpin { get; }
        /// <summary>Timing windows in seconds (half-width around the ideal contact).</summary>
        public float PerfectWindow { get; }
        public float GoodWindow { get; }
        /// <summary>Execution error (1 sigma) for a poor contact: direction in degrees, depth in metres.</summary>
        public float PoorAimErrorDegrees { get; }
        public float PoorDepthError { get; }
        public float WindupTime { get; }
        public float RecoveryTime { get; }

        public ShotDefinition(
            ShotType type, TrajectoryMode mode, float apexAboveContact, float minSpeed, float maxSpeed,
            float targetDepthMin, float targetDepthMax, float topSpin, float sideSpin,
            float perfectWindow, float goodWindow, float poorAimErrorDegrees, float poorDepthError,
            float windupTime, float recoveryTime)
        {
            if (targetDepthMax < targetDepthMin) throw new ArgumentOutOfRangeException(nameof(targetDepthMax));
            if (mode == TrajectoryMode.Speed && (minSpeed <= 0f || maxSpeed < minSpeed)) throw new ArgumentOutOfRangeException(nameof(maxSpeed));
            if (mode == TrajectoryMode.Apex && apexAboveContact < 0f) throw new ArgumentOutOfRangeException(nameof(apexAboveContact));
            if (perfectWindow <= 0f || goodWindow < perfectWindow) throw new ArgumentOutOfRangeException(nameof(goodWindow));

            Type = type;
            Mode = mode;
            ApexAboveContact = apexAboveContact;
            MinSpeed = minSpeed;
            MaxSpeed = maxSpeed;
            TargetDepthMin = targetDepthMin;
            TargetDepthMax = targetDepthMax;
            TopSpin = topSpin;
            SideSpin = sideSpin;
            PerfectWindow = perfectWindow;
            GoodWindow = goodWindow;
            PoorAimErrorDegrees = poorAimErrorDegrees;
            PoorDepthError = poorDepthError;
            WindupTime = windupTime;
            RecoveryTime = recoveryTime;
        }
    }
}
