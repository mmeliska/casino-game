using System;

namespace Starflare.Bowling
{
    /// <summary>
    /// The four inputs of a throw, as the meters set them: the board the ball starts on, the board
    /// it crosses at the arrows, the hook dial and the speed. Boards are counted from your own
    /// gutter, whichever hand you bowl with.
    /// </summary>
    public readonly struct Throw : IEquatable<Throw>
    {
        public const int MinStand = 1, MaxStand = 39;
        public const double MinAim = 5, MaxAim = 35;
        public const double MinHook = 0, MaxHook = 100;
        public const double MinSpeed = 12, MaxSpeed = 20;

        /// <summary>The board the ball starts on at the foul line.</summary>
        public readonly int Stand;

        /// <summary>The board it rolls over at the arrows, 15 ft (between arrows allowed).</summary>
        public readonly double Aim;

        /// <summary>0 rolls end over end and never turns; 100 is a cranker's ball.</summary>
        public readonly double Hook;

        /// <summary>Miles an hour off the hand.</summary>
        public readonly double Speed;

        public Throw(int stand, double aim, double hook, double speed)
        {
            Stand = stand;
            Aim = aim;
            Hook = hook;
            Speed = speed;
        }

        /// <summary>The same throw with every input inside its range.</summary>
        public Throw Clamp() => new Throw(
            Math.Max(MinStand, Math.Min(MaxStand, Stand)),
            Math.Max(MinAim, Math.Min(MaxAim, Aim)),
            Math.Max(MinHook, Math.Min(MaxHook, Hook)),
            Math.Max(MinSpeed, Math.Min(MaxSpeed, Speed)));

        public bool Equals(Throw other) => Stand == other.Stand && Aim.Equals(other.Aim) && Hook.Equals(other.Hook) && Speed.Equals(other.Speed);
        public override bool Equals(object obj) => obj is Throw other && Equals(other);
        public override int GetHashCode() => ((Stand * 397 ^ Aim.GetHashCode()) * 397 ^ Hook.GetHashCode()) * 397 ^ Speed.GetHashCode();
        public override string ToString() => $"stand {Stand}, aim {Aim:0.#}, hook {Hook:0}, {Speed:0.#} mph";
    }
}
