using System;
using System.Collections.Generic;

namespace Starflare.Bowling
{
    /// <summary>
    /// How the ball is turned at each moment of its roll, from the spin the sim recorded: what
    /// the drawn ball shows, so a skidding ball visibly turns slower than it travels, a rolling
    /// one turns with its speed, and a hook's axis sweeps the finger holes round. The ball leaves
    /// the hand with its holes up and back toward the bowler (<see cref="Holes"/>).
    /// </summary>
    public sealed class BallSpin
    {
        /// <summary>Where the finger holes face on the ball itself: its own x. Its surface picture centres them there.</summary>
        public static readonly Vec3 Holes = new Vec3(1, 0, 0);

        /// <summary>
        /// The three holes on the ball's own surface, as latitude and longitude in radians (the
        /// thumb below, two fingers above), and how wide each is: a 24 mm hole on a 108 mm ball.
        /// The stand-in surfaces and tools/import_bowling.py stamp them at these places.
        /// </summary>
        public static readonly (double lat, double lon)[] HolePlaces = { (-0.3, 0), (0.15, -0.16), (0.15, 0.16) };
        public const double HoleRadius = 0.11;

        /// <summary>At release the holes face up and back toward the bowler.</summary>
        public static readonly Turn AtRelease = Turn.Between(Holes, new Vec3(0, -0.5, 0.85));

        readonly List<BallSample> _samples;
        readonly Turn[] _turns;

        public BallSpin(BallPath path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            _samples = new List<BallSample>(path.Samples);
            _turns = new Turn[_samples.Count];
            var turn = AtRelease;
            for (int i = 0; i < _samples.Count; i++)
            {
                _turns[i] = turn;
                if (i + 1 < _samples.Count)
                {
                    var a = _samples[i];
                    var b = _samples[i + 1];
                    var spin = (a.Spin + b.Spin) * 0.5;
                    turn = turn.Then(Turn.Spun(spin, b.Time - a.Time));
                }
            }
        }

        /// <summary>The ball's turn this long after release; past the end, still spinning as it was.</summary>
        public Turn At(double time)
        {
            if (_samples.Count == 0) return AtRelease;
            if (time <= _samples[0].Time) return _turns[0];
            int lo = 0, hi = _samples.Count - 1;
            if (time >= _samples[hi].Time) return _turns[hi].Then(Turn.Spun(_samples[hi].Spin, time - _samples[hi].Time));
            while (hi - lo > 1)
            {
                int mid = (lo + hi) / 2;
                if (_samples[mid].Time <= time) lo = mid; else hi = mid;
            }
            return _turns[lo].Then(Turn.Spun(_samples[lo].Spin, time - _samples[lo].Time));
        }
    }
}
