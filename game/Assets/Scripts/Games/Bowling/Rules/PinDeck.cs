using System;
using System.Collections.Generic;

namespace Starflare.Bowling
{
    /// <summary>
    /// The ten pins: where they stand and which are still up. A fresh rack puts each on its spot
    /// with a seeded jitter of up to an eighth of an inch, so two identical throws don't knock down
    /// identical pins. After a ball the fallen ones are swept and the standing ones stay where they
    /// ended up for the next.
    /// </summary>
    public sealed class PinDeck
    {
        public const int Pins = 10;

        /// <summary>4.766 in across at the belly.</summary>
        public const double PinRadius = 4.766 / 2 * LaneGeometry.MetresPerInch;

        /// <summary>3 lb 6 oz.</summary>
        public const double PinMass = 1.53;

        /// <summary>The rack's jitter, up to an eighth of an inch off each spot.</summary>
        public const double Jitter = 0.125 * LaneGeometry.MetresPerInch;

        const double Spacing = 12 * LaneGeometry.MetresPerInch;
        const double RowDepth = 10.39 * LaneGeometry.MetresPerInch;

        /// <summary>
        /// The spots, pin 1 first: the head pin on board 20 at 60 ft, then rows behind it. Across is
        /// the lane's x, so the 3, 6 and 10 are on board 1's side, a right-hander's pocket side.
        /// </summary>
        public static readonly IReadOnlyList<Vec2> Spots = new[]
        {
            Spot(0, 0),
            Spot(-0.5, 1), Spot(0.5, 1),
            Spot(-1, 2), Spot(0, 2), Spot(1, 2),
            Spot(-1.5, 3), Spot(-0.5, 3), Spot(0.5, 3), Spot(1.5, 3),
        };

        static Vec2 Spot(double across, int row) =>
            new Vec2(across * Spacing, LaneGeometry.Y(LaneGeometry.HeadPinFt) + row * RowDepth);

        readonly Vec2[] _positions;
        readonly bool[] _standing;

        /// <summary>
        /// The luck in this rack's pin action: how the pins tilt and turn as they're hit, which a
        /// flat sim can't show. Drawn with the rack, so the same rack falls the same way.
        /// </summary>
        public readonly int ActionSeed;

        PinDeck(Vec2[] positions, bool[] standing, int actionSeed)
        {
            _positions = positions;
            _standing = standing;
            ActionSeed = actionSeed;
        }

        /// <summary>A fresh rack, each pin within the jitter of its spot.</summary>
        public static PinDeck Rack(Random rng)
        {
            if (rng == null) throw new ArgumentNullException(nameof(rng));
            var positions = new Vec2[Pins];
            for (int i = 0; i < Pins; i++)
            {
                double angle = rng.NextDouble() * 2 * Math.PI;
                double r = Math.Sqrt(rng.NextDouble()) * Jitter;
                positions[i] = Spots[i] + new Vec2(Math.Cos(angle) * r, Math.Sin(angle) * r);
            }
            return new PinDeck(positions, Full(), rng.Next());
        }

        /// <summary>A rack exactly on the spots, its pin action set by a seed: for tests and Juan's lesson.</summary>
        public static PinDeck Exact(int actionSeed = 0)
        {
            var positions = new Vec2[Pins];
            for (int i = 0; i < Pins; i++) positions[i] = Spots[i];
            return new PinDeck(positions, Full(), actionSeed);
        }

        /// <summary>Only these pins (1 to 10) standing, on their spots: a leave to shoot at.</summary>
        public static PinDeck Leave(params int[] pins) => Leave(0, pins);

        /// <summary>A leave on its spots with its own pin action.</summary>
        public static PinDeck Leave(int actionSeed, int[] pins)
        {
            var deck = Exact(actionSeed);
            for (int i = 0; i < Pins; i++) deck._standing[i] = false;
            foreach (int pin in pins) deck._standing[pin - 1] = true;
            return deck;
        }

        static bool[] Full()
        {
            var standing = new bool[Pins];
            for (int i = 0; i < Pins; i++) standing[i] = true;
            return standing;
        }

        /// <summary>Where a pin (index 0 for the head pin) stands, or last stood.</summary>
        public Vec2 PositionOf(int index) => _positions[index];

        public bool IsStanding(int index) => _standing[index];

        public int StandingCount
        {
            get
            {
                int n = 0;
                foreach (bool up in _standing) if (up) n++;
                return n;
            }
        }

        /// <summary>The pins (1 to 10) still up, for the leave's name.</summary>
        public List<int> StandingPins()
        {
            var pins = new List<int>();
            for (int i = 0; i < Pins; i++) if (_standing[i]) pins.Add(i + 1);
            return pins;
        }

        /// <summary>The deck after a ball: the fallen swept, the standing where they ended up.</summary>
        public PinDeck After(PinResult result)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));
            var positions = new Vec2[Pins];
            var standing = new bool[Pins];
            for (int i = 0; i < Pins; i++)
            {
                standing[i] = _standing[i] && !result.Fell[i];
                positions[i] = standing[i] ? result.FinalPositions[i] : _positions[i];
            }
            return new PinDeck(positions, standing, unchecked(ActionSeed * 31 + 17));
        }

        /// <summary>The same deck seen from the other hand: mirrored across the head pin's board.</summary>
        public PinDeck Mirrored()
        {
            var positions = new Vec2[Pins];
            for (int i = 0; i < Pins; i++) positions[i] = new Vec2(-_positions[i].X, _positions[i].Y);
            return new PinDeck(positions, (bool[])_standing.Clone(), ActionSeed);
        }
    }
}
