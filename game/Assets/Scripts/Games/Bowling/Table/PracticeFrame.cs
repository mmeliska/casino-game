using System;

namespace Starflare.Bowling
{
    /// <summary>
    /// A coached frame: the inputs a teacher locks the meters to, a ball each on a fresh rack,
    /// like Juan's first frame with you (bowling spec, "Juan's first frame"). It's for show: the
    /// house ball, no fee, no round, and the clock doesn't move. You still drag and tap through
    /// it; whatever you stop, the meters snap to his numbers.
    /// </summary>
    public sealed class PracticeFrame
    {
        public Throw[] Balls { get; }

        public PracticeFrame(params Throw[] balls)
        {
            if (balls == null || balls.Length == 0) throw new ArgumentException("A practice frame needs at least one ball.", nameof(balls));
            Balls = balls;
        }

        /// <summary>
        /// Juan's: the seventh board up the second arrow with nothing on it, which finds the head
        /// pin flush and splits; then six boards left with something on it, into the pocket.
        /// </summary>
        public static PracticeFrame Juans() => new PracticeFrame(new Throw(7, 10, 0, 16), new Throw(13, 10, 70, 16));
    }
}
