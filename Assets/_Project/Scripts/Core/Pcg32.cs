using System;

namespace Padel.Core
{
    /// <summary>
    /// Deterministic PCG32 random generator (PCG-XSH-RR, O'Neill 2014, https://www.pcg-random.org/).
    /// Lives inside simulation state so a match is reproducible from its seed. Never use
    /// UnityEngine.Random or System.Random inside the simulation (ADR-004, ADR-008).
    /// </summary>
    [Serializable]
    public struct Pcg32
    {
        private const ulong Multiplier = 6364136223846793005UL;

        public ulong State;
        public ulong Increment;

        public Pcg32(ulong seed, ulong stream = 54UL)
        {
            State = 0UL;
            Increment = (stream << 1) | 1UL;
            NextUInt();
            State += seed;
            NextUInt();
        }

        public uint NextUInt()
        {
            ulong old = State;
            State = unchecked(old * Multiplier + Increment);
            uint xorShifted = (uint)(((old >> 18) ^ old) >> 27);
            int rot = (int)(old >> 59);
            return (xorShifted >> rot) | (xorShifted << ((-rot) & 31));
        }

        /// <summary>Uniform float in [0, 1).</summary>
        public float NextFloat() => (NextUInt() >> 8) * (1f / 16777216f);

        /// <summary>Uniform float in [min, max).</summary>
        public float Range(float min, float max) => min + (max - min) * NextFloat();

        /// <summary>Standard normal sample (Box-Muller).</summary>
        public float NextGaussian()
        {
            float u1 = 1f - NextFloat(); // (0, 1]
            float u2 = NextFloat();
            return MathF.Sqrt(-2f * MathF.Log(u1)) * MathF.Cos(2f * MathF.PI * u2);
        }
    }
}
