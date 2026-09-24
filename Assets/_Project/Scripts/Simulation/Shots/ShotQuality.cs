using System;
using Padel.Core;

namespace Padel.Simulation.Shots
{
    public enum ShotQuality
    {
        Perfect = 0,
        Good = 1,
        Early = 2,
        Late = 3,
    }

    /// <summary>Timing evaluation and execution error (GAMEPLAY_SPEC §5.4).</summary>
    public static class ShotTiming
    {
        /// <param name="offsetSeconds">Actual contact minus ideal contact: negative = early, positive = late.</param>
        public static ShotQuality Evaluate(float offsetSeconds, ShotDefinition shot)
        {
            float a = Math.Abs(offsetSeconds);
            if (a <= shot.PerfectWindow) return ShotQuality.Perfect;
            if (a <= shot.GoodWindow) return ShotQuality.Good;
            return offsetSeconds < 0f ? ShotQuality.Early : ShotQuality.Late;
        }

        /// <summary>Scale applied to the "poor" error sigmas [P].</summary>
        public static float ErrorScale(ShotQuality quality)
        {
            switch (quality)
            {
                case ShotQuality.Perfect: return 0f;
                case ShotQuality.Good: return 0.4f;
                default: return 1f;
            }
        }

        /// <summary>
        /// Perturbs the intended landing target around the contact point with seeded Gaussian noise (direction and depth).
        /// </summary>
        public static Vec2 ApplyError(Vec2 contact, Vec2 target, ShotDefinition shot, ShotQuality quality, ref Pcg32 rng)
        {
            float scale = ErrorScale(quality);
            if (scale <= 0f) return target;

            Vec2 delta = target - contact;
            float distance = delta.Magnitude;
            if (distance < 1e-4f) return target;
            Vec2 dir = delta / distance;

            float angle = rng.NextGaussian() * shot.PoorAimErrorDegrees * scale * (MathF.PI / 180f);
            float depth = rng.NextGaussian() * shot.PoorDepthError * scale;
            float cos = MathF.Cos(angle), sin = MathF.Sin(angle);
            Vec2 rotated = new Vec2(dir.X * cos - dir.Y * sin, dir.X * sin + dir.Y * cos);
            return contact + rotated * (distance + depth);
        }
    }
}
