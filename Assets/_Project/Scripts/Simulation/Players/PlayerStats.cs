using System;

namespace Padel.Simulation.Players
{
    /// <summary>
    /// Immutable movement and reach parameters of a player (GAMEPLAY_SPEC §4). All values are [P] tuning data
    /// pending playtest; no biomechanical padel data was found (GAMEPLAY_RESEARCH §4, gap).
    /// </summary>
    public sealed class PlayerStats
    {
        public float MaxSpeed { get; }
        public float BackpedalSpeed { get; }
        public float AccelerationTime { get; }
        public float BrakeTime { get; }
        public float ReverseTime { get; }
        public float Reach { get; }
        public float StretchReach { get; }
        public float SplitStepBoost { get; }
        public float SplitStepDuration { get; }
        /// <summary>Speed below which the player counts as "ready" for a split-step.</summary>
        public float ReadySpeed { get; }
        /// <summary>Allowed movement speed during a swing windup (micro-adjustments).</summary>
        public float WindupAdjustSpeed { get; }
        public bool LeftHanded { get; }

        public PlayerStats(
            float maxSpeed = 5.5f, float backpedalSpeed = 4f, float accelerationTime = 0.25f, float brakeTime = 0.12f,
            float reverseTime = 0.30f, float reach = 0.9f, float stretchReach = 1.3f, float splitStepBoost = 1.3f,
            float splitStepDuration = 0.2f, float readySpeed = 0.5f, float windupAdjustSpeed = 0.3f, bool leftHanded = false)
        {
            if (maxSpeed <= 0f || backpedalSpeed <= 0f) throw new ArgumentOutOfRangeException(nameof(maxSpeed));
            if (accelerationTime <= 0f || brakeTime <= 0f || reverseTime <= 0f) throw new ArgumentOutOfRangeException(nameof(accelerationTime));
            if (reach <= 0f || stretchReach < reach) throw new ArgumentOutOfRangeException(nameof(stretchReach));

            MaxSpeed = maxSpeed;
            BackpedalSpeed = backpedalSpeed;
            AccelerationTime = accelerationTime;
            BrakeTime = brakeTime;
            ReverseTime = reverseTime;
            Reach = reach;
            StretchReach = stretchReach;
            SplitStepBoost = splitStepBoost;
            SplitStepDuration = splitStepDuration;
            ReadySpeed = readySpeed;
            WindupAdjustSpeed = windupAdjustSpeed;
            LeftHanded = leftHanded;
        }

        public float Acceleration => MaxSpeed / AccelerationTime;
        public float Deceleration => MaxSpeed / BrakeTime;
        public float ReverseDeceleration => 2f * MaxSpeed / ReverseTime;
    }
}
