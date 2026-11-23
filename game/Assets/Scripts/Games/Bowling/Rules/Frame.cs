using System.Collections.Generic;

namespace Starflare.Bowling
{
    /// <summary>
    /// One frame on the sheet: its balls, and its score once the balls it counts have been
    /// thrown. The tenth can hold three.
    /// </summary>
    public sealed class Frame
    {
        readonly List<int> _balls = new List<int>(3);

        public readonly int Number;

        public Frame(int number) => Number = number;

        public IReadOnlyList<int> Balls => _balls;
        public bool IsTenth => Number == ScoreSheet.Frames;
        public bool IsStrike => _balls.Count > 0 && _balls[0] == ScoreSheet.Pins;
        public bool IsSpare => !IsStrike && _balls.Count > 1 && _balls[0] + _balls[1] == ScoreSheet.Pins;

        /// <summary>The running total through this frame, once it can be counted.</summary>
        public int? Total { get; internal set; }

        internal void Add(int pins) => _balls.Add(pins);
    }
}
