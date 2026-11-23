using System;

namespace Starflare.Bowling
{
    /// <summary>
    /// The ball's spin and the lever from its centre to the lane: x across, y down the lane, z up.
    /// Doubles, for the same reason as <see cref="Vec2"/>.
    /// </summary>
    public readonly struct Vec3 : IEquatable<Vec3>
    {
        public readonly double X;
        public readonly double Y;
        public readonly double Z;

        public Vec3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static readonly Vec3 Zero = new Vec3(0, 0, 0);

        public double Length => Math.Sqrt(X * X + Y * Y + Z * Z);

        public static Vec3 Cross(Vec3 a, Vec3 b) =>
            new Vec3(a.Y * b.Z - a.Z * b.Y, a.Z * b.X - a.X * b.Z, a.X * b.Y - a.Y * b.X);

        public static Vec3 operator +(Vec3 a, Vec3 b) => new Vec3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Vec3 operator -(Vec3 a, Vec3 b) => new Vec3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static Vec3 operator *(Vec3 a, double k) => new Vec3(a.X * k, a.Y * k, a.Z * k);
        public static Vec3 operator *(double k, Vec3 a) => new Vec3(a.X * k, a.Y * k, a.Z * k);

        /// <summary>The lane-plane part, dropping z.</summary>
        public Vec2 Flat => new Vec2(X, Y);

        public bool Equals(Vec3 other) => X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
        public override bool Equals(object obj) => obj is Vec3 other && Equals(other);
        public override int GetHashCode() => (X.GetHashCode() * 397 ^ Y.GetHashCode()) * 397 ^ Z.GetHashCode();
        public override string ToString() => $"({X:0.000}, {Y:0.000}, {Z:0.000})";
    }
}
