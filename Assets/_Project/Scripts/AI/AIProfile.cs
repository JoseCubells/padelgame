using System;

namespace Padel.AI
{
    /// <summary>
    /// Difficulty and personality as data (ADR-006, GAMEPLAY_RESEARCH §5.4). All values [P] pending tuning.
    /// </summary>
    public sealed class AIProfile
    {
        public string Name { get; }
        /// <summary>Delay after an opponent's stroke before the AI reacts to the new trajectory (s).</summary>
        public float ReactionTime { get; }
        /// <summary>Sigma of the error on the predicted interception point (m).</summary>
        public float PredictionNoise { get; }
        /// <summary>Sigma of the press-timing error (s).</summary>
        public float TimingNoise { get; }
        /// <summary>Sigma of the aim error (aim units, -1..1).</summary>
        public float AimNoise { get; }
        /// <summary>Softmax temperature of the shot choice: low = best option almost always.</summary>
        public float Temperature { get; }
        /// <summary>Utility weights: safety, pressure, positional gain.</summary>
        public float SafetyWeight { get; }
        public float PressureWeight { get; }
        public float PositionWeight { get; }
        /// <summary>Delay before serving (s).</summary>
        public float ServeDelay { get; }
        /// <summary>Timing error multiplier at maximum pressure (fast incoming ball, long run) — research table §5.4.</summary>
        public float PressureErrorMultiplier { get; }

        public AIProfile(string name, float reactionTime, float predictionNoise, float timingNoise, float aimNoise,
            float temperature, float safetyWeight, float pressureWeight, float positionWeight, float pressureErrorMultiplier,
            float serveDelay = 1f)
        {
            if (reactionTime < 0f || predictionNoise < 0f || timingNoise < 0f || aimNoise < 0f) throw new ArgumentOutOfRangeException(nameof(reactionTime));
            if (temperature <= 0f) throw new ArgumentOutOfRangeException(nameof(temperature));
            Name = name;
            ReactionTime = reactionTime;
            PredictionNoise = predictionNoise;
            TimingNoise = timingNoise;
            AimNoise = aimNoise;
            Temperature = temperature;
            SafetyWeight = safetyWeight;
            PressureWeight = pressureWeight;
            PositionWeight = positionWeight;
            ServeDelay = serveDelay;
            PressureErrorMultiplier = pressureErrorMultiplier;
        }

        public static AIProfile Easy() => new AIProfile("Easy", 0.35f, 0.6f, 0.09f, 0.35f, 1.0f, 1.2f, 0.4f, 0.6f, 2.0f);
        public static AIProfile Medium() => new AIProfile("Medium", 0.25f, 0.3f, 0.05f, 0.2f, 0.5f, 1.0f, 0.8f, 0.8f, 1.5f);
        public static AIProfile Hard() => new AIProfile("Hard", 0.15f, 0.1f, 0.025f, 0.1f, 0.15f, 0.9f, 1.2f, 1.0f, 1.2f);
    }
}
