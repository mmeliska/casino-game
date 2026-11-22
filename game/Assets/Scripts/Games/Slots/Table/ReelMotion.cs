using System;

namespace Starflare.Slots
{
    /// <summary>
    /// One reel turning, as view state: a small kick up as the handle goes, a steady spin, then
    /// a landing that runs a few symbols past and settles back onto its stop. The stop is
    /// already decided (<see cref="SlotSpin"/>); this only shows it. Position is the strip
    /// index in the window's top row, fractional while it turns; it falls as the symbols move
    /// down.
    /// </summary>
    public sealed class ReelMotion
    {
        /// <summary>Symbols a second at full spin.</summary>
        public const double Speed = 24;

        /// <summary>How long the kick up lasts, and how far it goes, in symbols.</summary>
        public const double KickSeconds = 0.12, Kick = 0.3;

        /// <summary>Symbols the landing covers after the spin, so it starts at full speed.</summary>
        public const double Run = 3;

        /// <summary>How far past its stop a landing swings before settling (the ease's overshoot).</summary>
        const double Overshoot = 1.0;

        /// <summary>The landing ease's speed at its start, per unit of distance and time, so it matches <see cref="Speed"/>.</summary>
        const double LandingSlope = 3 * (Overshoot + 1) - 2 * Overshoot;

        enum Phase { Resting, Kicking, Spinning, Landing }

        readonly int _length;
        Phase _phase = Phase.Resting;
        double _elapsed, _stopAt, _from, _landingTime, _landingSeconds;
        int _target;

        public ReelMotion(int length, int restingAt = 0)
        {
            if (length < 1) throw new ArgumentOutOfRangeException(nameof(length));
            _length = length;
            Position = Wrap(restingAt);
            _target = (int)Position;
        }

        public double Position { get; private set; }
        public bool Turning => _phase != Phase.Resting;

        /// <summary>At full speed, where the symbols blur.</summary>
        public bool Blurred => _phase == Phase.Spinning;

        /// <summary>The landing's length, from a spin at full speed.</summary>
        public static double LandingSeconds => LandingSlope * Run / Speed;

        /// <summary>Strip index of the symbol in the window's top row, rounded down.</summary>
        public int Top => (int)Math.Floor(Wrap(Position));

        /// <summary>How far the top symbol has moved down out of the window, 0 to 1.</summary>
        public double Offset => Wrap(Position) - Top;

        /// <summary>Sets the reel turning, to land on <paramref name="stop"/> once it's spun for <paramref name="seconds"/>.</summary>
        public void Spin(int stop, double seconds)
        {
            if (stop < 0 || stop >= _length) throw new ArgumentOutOfRangeException(nameof(stop));
            Position = Wrap(Position);
            _from = Position;
            _target = stop;
            _elapsed = 0;
            _stopAt = Math.Max(seconds, KickSeconds);
            _phase = Phase.Kicking;
        }

        /// <summary>Lands sooner, when you tap to stop; never later than it would have.</summary>
        public void StopWithin(double seconds)
        {
            if (_phase == Phase.Kicking || _phase == Phase.Spinning)
                _stopAt = Math.Min(_stopAt, _elapsed + Math.Max(0, seconds));
        }

        /// <summary>Puts the reel straight onto its stop.</summary>
        public void Settle()
        {
            Position = _target;
            _phase = Phase.Resting;
        }

        public void Advance(double seconds)
        {
            if (_phase == Phase.Resting) return;
            _elapsed += seconds;

            if (_phase == Phase.Kicking)
            {
                if (_elapsed < KickSeconds && _elapsed < _stopAt)
                {
                    Position = _from + Kick * Math.Sin(Math.PI * _elapsed / KickSeconds);
                    return;
                }
                Position = _from;
                _phase = Phase.Spinning;
                seconds = _elapsed - KickSeconds;
            }

            if (_phase == Phase.Spinning)
            {
                Position -= Speed * Math.Max(0, seconds);
                if (_elapsed < _stopAt) return;
                // The strip jumps to a few symbols above the stop as the landing starts. At full
                // spin it's a blur, so the jump doesn't show; keeping the fraction keeps it smooth.
                double fraction = Wrap(Position) - Math.Floor(Wrap(Position));
                _from = _target + Run + fraction;
                Position = _from;
                _landingSeconds = LandingSlope * (Run + fraction) / Speed;
                _landingTime = 0;
                _phase = Phase.Landing;
                return;
            }

            _landingTime += seconds;
            double t = Math.Min(1, _landingTime / _landingSeconds);
            Position = _from + (_target - _from) * Land(t);
            if (t >= 1) Settle();
        }

        /// <summary>Ease out with a swing past the stop and back (an ease-out-back), 0 to 1.</summary>
        static double Land(double t)
        {
            double u = t - 1;
            return 1 + (Overshoot + 1) * u * u * u + Overshoot * u * u;
        }

        double Wrap(double position)
        {
            double wrapped = position % _length;
            return wrapped < 0 ? wrapped + _length : wrapped;
        }
    }
}
