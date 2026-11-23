using System;

namespace Starflare.Bowling
{
    /// <summary>
    /// One lane tonight, as it stands: its oil (worn by every ball), the pins up, and the seeded
    /// racks. Everybody who bowls on it bowls through this, you and the regulars alike.
    /// </summary>
    public sealed class LaneState
    {
        readonly Random _racks;

        public readonly LaneOil Oil;
        public readonly Hand Hand;
        public PinDeck Deck { get; private set; }

        public LaneState(LaneOil oil, Hand hand, int seed)
        {
            Oil = oil ?? throw new ArgumentNullException(nameof(oil));
            Hand = hand;
            _racks = new Random(seed);
            Rack();
        }

        /// <summary>A fresh rack of ten.</summary>
        public void Rack() => Deck = PinDeck.Rack(_racks);

        /// <summary>
        /// Rolls a throw down the lane into what's standing: the lane picks up the oil it crossed,
        /// the fallen pins are swept and the rest stay where they stand.
        /// </summary>
        public Delivery Bowl(Throw t, BowlingBall ball)
        {
            var path = BallSim.Roll(t, Oil, ball, Hand);
            var before = Deck;
            var pins = PinSim.Hit(path, before);
            Oil.Pass(path);
            Deck = before.After(pins);
            return new Delivery(t, path, before, pins);
        }
    }
}
