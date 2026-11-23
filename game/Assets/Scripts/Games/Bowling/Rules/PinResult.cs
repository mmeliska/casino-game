using System.Collections.Generic;

namespace Starflare.Bowling
{
    /// <summary>One moment of the hit, for the deck's close-up.</summary>
    public readonly struct PinFrame
    {
        public readonly double Time;
        public readonly Vec2 Ball;
        public readonly bool BallGone;
        public readonly Vec2[] Pins;
        public readonly bool[] Down;

        public PinFrame(double time, Vec2 ball, bool ballGone, Vec2[] pins, bool[] down)
        {
            Time = time;
            Ball = ball;
            BallGone = ballGone;
            Pins = pins;
            Down = down;
        }
    }

    /// <summary>What a ball did to the deck: which pins fell, where the rest stand, and the motion.</summary>
    public sealed class PinResult
    {
        /// <summary>Per pin (index 0 the head pin): knocked down by this ball.</summary>
        public readonly bool[] Fell;

        /// <summary>Where every pin ended up; the fallen ones are swept before the next ball.</summary>
        public readonly Vec2[] FinalPositions;

        public readonly IReadOnlyList<PinFrame> Frames;

        public PinResult(bool[] fell, Vec2[] finalPositions, IReadOnlyList<PinFrame> frames)
        {
            Fell = fell;
            FinalPositions = finalPositions;
            Frames = frames;
        }

        public int Count
        {
            get
            {
                int n = 0;
                foreach (bool f in Fell) if (f) n++;
                return n;
            }
        }

        /// <summary>Nothing touched: a gutter ball, or a ball through a gap.</summary>
        public static PinResult Nothing(PinDeck deck)
        {
            var positions = new Vec2[PinDeck.Pins];
            for (int i = 0; i < PinDeck.Pins; i++) positions[i] = deck.PositionOf(i);
            return new PinResult(new bool[PinDeck.Pins], positions, new PinFrame[0]);
        }
    }
}
