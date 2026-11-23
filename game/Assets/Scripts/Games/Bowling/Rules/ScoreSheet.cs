using System;
using System.Collections.Generic;

namespace Starflare.Bowling
{
    /// <summary>
    /// Standard tenpins: a strike counts the next two balls, a spare the next one, a strike or a
    /// spare in the tenth earns its extra balls, 300 is perfect. No foul line: there's no foot to
    /// foul with.
    /// </summary>
    public sealed class ScoreSheet
    {
        public const int Frames = 10;
        public const int Pins = 10;

        readonly List<Frame> _frames = new List<Frame>(Frames);
        readonly List<int> _balls = new List<int>(21);

        public ScoreSheet()
        {
            for (int i = 1; i <= Frames; i++) _frames.Add(new Frame(i));
        }

        public IReadOnlyList<Frame> FrameList => _frames;

        /// <summary>Every ball so far, in order.</summary>
        public IReadOnlyList<int> Balls => _balls;

        /// <summary>The frame the next ball goes in, 1 to 10 (10 once the game's over).</summary>
        public int CurrentFrame { get; private set; } = 1;

        /// <summary>The next ball's place in its frame: 0 for the first.</summary>
        public int BallInFrame => _frames[CurrentFrame - 1].Balls.Count;

        public bool IsComplete { get; private set; }

        /// <summary>The next ball faces a full rack: a new frame, or the tenth after a strike or spare.</summary>
        public bool FreshRack
        {
            get
            {
                if (IsComplete) return false;
                var frame = _frames[CurrentFrame - 1];
                if (frame.Balls.Count == 0) return true;
                if (!frame.IsTenth) return false;
                // In the tenth: fresh after a strike, or after a spare.
                return Standing(frame) == Pins;
            }
        }

        /// <summary>The pins standing for the next ball.</summary>
        public int PinsStanding => IsComplete ? 0 : Standing(_frames[CurrentFrame - 1]);

        /// <summary>The score so far: the last frame that can be counted.</summary>
        public int Score
        {
            get
            {
                int score = 0;
                foreach (var f in _frames)
                    if (f.Total.HasValue) score = f.Total.Value;
                return score;
            }
        }

        /// <summary>Pins down for the next ball. Can't be more than are standing.</summary>
        public void Record(int pins)
        {
            if (IsComplete) throw new InvalidOperationException("The game's over.");
            if (pins < 0 || pins > PinsStanding) throw new ArgumentOutOfRangeException(nameof(pins), $"{pins} of {PinsStanding} standing.");
            var frame = _frames[CurrentFrame - 1];
            frame.Add(pins);
            _balls.Add(pins);
            Advance(frame);
            Count();
        }

        void Advance(Frame frame)
        {
            if (!frame.IsTenth)
            {
                if (frame.IsStrike || frame.Balls.Count == 2) CurrentFrame++;
                return;
            }
            int n = frame.Balls.Count;
            if (n == 3 || (n == 2 && !frame.IsStrike && !frame.IsSpare)) IsComplete = true;
        }

        static int Standing(Frame frame)
        {
            var b = frame.Balls;
            if (!frame.IsTenth) return b.Count == 0 ? Pins : Pins - b[0];
            // The tenth: the rack is reset after a strike, and after a spare.
            int standing = Pins;
            foreach (int pins in b)
            {
                standing -= pins;
                if (standing == 0) standing = Pins;
            }
            return standing;
        }

        void Count()
        {
            int total = 0, at = 0;
            foreach (var frame in _frames) frame.Total = null;
            foreach (var frame in _frames)
            {
                if (frame.IsTenth)
                {
                    if (!IsComplete) return;
                    foreach (int pins in frame.Balls) total += pins;
                    frame.Total = total;
                    return;
                }
                if (frame.IsStrike)
                {
                    if (at + 2 >= _balls.Count) return;
                    total += Pins + _balls[at + 1] + _balls[at + 2];
                    at += 1;
                }
                else if (frame.Balls.Count == 2)
                {
                    if (frame.IsSpare)
                    {
                        if (at + 2 >= _balls.Count) return;
                        total += Pins + _balls[at + 2];
                    }
                    else total += frame.Balls[0] + frame.Balls[1];
                    at += 2;
                }
                else return;
                frame.Total = total;
            }
        }

        /// <summary>
        /// The mark in a frame's box for one ball, the way a sheet writes it: X, /, - for a miss,
        /// or the count. Empty if not thrown.
        /// </summary>
        public string Mark(int frameNumber, int ball)
        {
            var frame = _frames[frameNumber - 1];
            var b = frame.Balls;
            if (ball >= b.Count) return "";
            int pins = b[ball];
            if (!frame.IsTenth)
            {
                if (ball == 0) return pins == Pins ? "X" : pins == 0 ? "-" : pins.ToString();
                return b[0] + pins == Pins ? "/" : pins == 0 ? "-" : pins.ToString();
            }
            // The tenth: a strike on a fresh rack is X, finishing a rack is /.
            int standing = Pins;
            for (int i = 0; i < ball; i++)
            {
                standing -= b[i];
                if (standing == 0) standing = Pins;
            }
            if (pins == Pins && standing == Pins) return "X";
            if (pins == standing && standing != Pins) return "/";
            return pins == 0 ? "-" : pins.ToString();
        }
    }
}
