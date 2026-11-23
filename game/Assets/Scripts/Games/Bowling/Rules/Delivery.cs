namespace Starflare.Bowling
{
    /// <summary>One ball: the throw, what the lane did with it and what it knocked down.</summary>
    public sealed class Delivery
    {
        public readonly Throw Throw;
        public readonly BallPath Path;
        public readonly PinDeck Before;
        public readonly PinResult Pins;

        public Delivery(Throw t, BallPath path, PinDeck before, PinResult pins)
        {
            Throw = t;
            Path = path;
            Before = before;
            Pins = pins;
        }

        public int Count => Pins.Count;
    }
}
