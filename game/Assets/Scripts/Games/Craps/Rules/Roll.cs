using System;

namespace Starflare.Craps
{
    /// <summary>One throw of two dice.</summary>
    public readonly struct Roll : IEquatable<Roll>
    {
        public readonly int Die1;
        public readonly int Die2;

        public Roll(int die1, int die2)
        {
            if (die1 < 1 || die1 > 6) throw new ArgumentOutOfRangeException(nameof(die1));
            if (die2 < 1 || die2 > 6) throw new ArgumentOutOfRangeException(nameof(die2));
            Die1 = die1;
            Die2 = die2;
        }

        public int Total => Die1 + Die2;

        public bool Equals(Roll other) => Die1 == other.Die1 && Die2 == other.Die2;
        public override bool Equals(object obj) => obj is Roll other && Equals(other);
        public override int GetHashCode() => Die1 * 7 + Die2;
        public override string ToString() => $"{Die1}+{Die2}={Total}";
    }
}
