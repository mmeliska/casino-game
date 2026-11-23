using System;

namespace Starflare.Bowling
{
    /// <summary>
    /// A point or a velocity on the lane, in metres: x across (positive toward board 1, a
    /// right-hander's right), y down the lane from the foul line. The sim's own arithmetic in
    /// doubles, no engine types, so the same throw gives the same path on every platform.
    /// </summary>
    public readonly struct Vec2 : IEquatable<Vec2>
    {
        public readonly double X;
        public readonly double Y;

        public Vec2(double x, double y)
        {
            X = x;
            Y = y;
        }

        public static readonly Vec2 Zero = new Vec2(0, 0);

        public double Length => Math.Sqrt(X * X + Y * Y);
        public double LengthSquared => X * X + Y * Y;
        public Vec2 Normalized { get { double l = Length; return l > 0 ? new Vec2(X / l, Y / l) : Zero; } }

        public static double Dot(Vec2 a, Vec2 b) => a.X * b.X + a.Y * b.Y;

        /// <summary>The z of the 3D cross product of the two, for curvature.</summary>
        public static double Cross(Vec2 a, Vec2 b) => a.X * b.Y - a.Y * b.X;

        public static Vec2 operator +(Vec2 a, Vec2 b) => new Vec2(a.X + b.X, a.Y + b.Y);
        public static Vec2 operator -(Vec2 a, Vec2 b) => new Vec2(a.X - b.X, a.Y - b.Y);
        public static Vec2 operator -(Vec2 a) => new Vec2(-a.X, -a.Y);
        public static Vec2 operator *(Vec2 a, double k) => new Vec2(a.X * k, a.Y * k);
        public static Vec2 operator *(double k, Vec2 a) => new Vec2(a.X * k, a.Y * k);
        public static Vec2 operator /(Vec2 a, double k) => new Vec2(a.X / k, a.Y / k);

        public bool Equals(Vec2 other) => X.Equals(other.X) && Y.Equals(other.Y);
        public override bool Equals(object obj) => obj is Vec2 other && Equals(other);
        public override int GetHashCode() => X.GetHashCode() * 397 ^ Y.GetHashCode();
        public override string ToString() => $"({X:0.0000}, {Y:0.0000})";
    }
}
