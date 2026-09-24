using System;

namespace Padel.Core
{
    /// <summary>
    /// Engine-free 2D vector on the court plane: X across the court, Y maps to world Z (along the court).
    /// </summary>
    [Serializable]
    public struct Vec2 : IEquatable<Vec2>
    {
        public float X;
        public float Y;

        public Vec2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static Vec2 Zero => new Vec2(0f, 0f);

        public float SqrMagnitude => X * X + Y * Y;
        public float Magnitude => MathF.Sqrt(SqrMagnitude);

        public Vec2 Normalized
        {
            get
            {
                float m = Magnitude;
                return m > 1e-9f ? this / m : Zero;
            }
        }

        /// <summary>Returns the vector scaled down to <paramref name="maxLength"/> if it is longer.</summary>
        public Vec2 ClampMagnitude(float maxLength)
        {
            float sqr = SqrMagnitude;
            if (sqr <= maxLength * maxLength) return this;
            return this * (maxLength / MathF.Sqrt(sqr));
        }

        public static Vec2 operator +(Vec2 a, Vec2 b) => new Vec2(a.X + b.X, a.Y + b.Y);
        public static Vec2 operator -(Vec2 a, Vec2 b) => new Vec2(a.X - b.X, a.Y - b.Y);
        public static Vec2 operator -(Vec2 a) => new Vec2(-a.X, -a.Y);
        public static Vec2 operator *(Vec2 a, float s) => new Vec2(a.X * s, a.Y * s);
        public static Vec2 operator *(float s, Vec2 a) => new Vec2(a.X * s, a.Y * s);
        public static Vec2 operator /(Vec2 a, float s) => new Vec2(a.X / s, a.Y / s);

        public static float Dot(Vec2 a, Vec2 b) => a.X * b.X + a.Y * b.Y;

        /// <summary>Court-plane vector to world vector (Y up = <paramref name="height"/>).</summary>
        public Vec3 ToWorld(float height = 0f) => new Vec3(X, height, Y);

        public bool Equals(Vec2 other) => X.Equals(other.X) && Y.Equals(other.Y);
        public override bool Equals(object obj) => obj is Vec2 other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(X, Y);
        public override string ToString() => $"({X:0.###}, {Y:0.###})";
    }
}
