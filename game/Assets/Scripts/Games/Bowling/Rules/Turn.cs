using System;

namespace Starflare.Bowling
{
    /// <summary>
    /// A rotation in the lane's frame, as a unit quaternion: how the ball is turned. Doubles and
    /// no engine, like <see cref="Vec3"/>, so the ball's spin on screen can be tested.
    /// </summary>
    public readonly struct Turn
    {
        public readonly double W, X, Y, Z;

        public Turn(double w, double x, double y, double z)
        {
            double n = Math.Sqrt(w * w + x * x + y * y + z * z);
            if (n <= 0) { W = 1; X = Y = Z = 0; return; }
            W = w / n;
            X = x / n;
            Y = y / n;
            Z = z / n;
        }

        public static readonly Turn None = new Turn(1, 0, 0, 0);

        /// <summary>A turn of this many radians about an axis, by the right-hand rule.</summary>
        public static Turn About(Vec3 axis, double radians)
        {
            double len = axis.Length;
            if (len <= 0 || radians == 0) return None;
            double s = Math.Sin(radians / 2) / len;
            return new Turn(Math.Cos(radians / 2), axis.X * s, axis.Y * s, axis.Z * s);
        }

        /// <summary>Spinning at this rate (radians a second about each axis) for this long.</summary>
        public static Turn Spun(Vec3 spin, double seconds) => About(spin, spin.Length * seconds);

        /// <summary>The turn that takes one direction to another, the short way round.</summary>
        public static Turn Between(Vec3 from, Vec3 to)
        {
            var a = Unit(from);
            var b = Unit(to);
            var axis = Vec3.Cross(a, b);
            double dot = a.X * b.X + a.Y * b.Y + a.Z * b.Z;
            if (axis.Length < 1e-9) return dot > 0 ? None : About(Math.Abs(a.X) < 0.9 ? Vec3.Cross(a, new Vec3(1, 0, 0)) : Vec3.Cross(a, new Vec3(0, 1, 0)), Math.PI);
            return About(axis, Math.Atan2(axis.Length, dot));
        }

        /// <summary>This turn, then <paramref name="then"/>.</summary>
        public Turn Then(Turn then) => then * this;

        public Turn Inverse => new Turn(W, -X, -Y, -Z);

        public static Turn operator *(Turn a, Turn b) => new Turn(
            a.W * b.W - a.X * b.X - a.Y * b.Y - a.Z * b.Z,
            a.W * b.X + a.X * b.W + a.Y * b.Z - a.Z * b.Y,
            a.W * b.Y - a.X * b.Z + a.Y * b.W + a.Z * b.X,
            a.W * b.Z + a.X * b.Y - a.Y * b.X + a.Z * b.W);

        /// <summary>A direction, turned.</summary>
        public Vec3 Rotate(Vec3 v)
        {
            // v + 2w(q×v) + 2q×(q×v), with q the turn's axis part.
            var q = new Vec3(X, Y, Z);
            var t = Vec3.Cross(q, v) * 2;
            return v + t * W + Vec3.Cross(q, t);
        }

        static Vec3 Unit(Vec3 v)
        {
            double len = v.Length;
            return len > 0 ? v * (1 / len) : new Vec3(1, 0, 0);
        }
    }
}
