using System;

namespace Starflare.Slots
{
    /// <summary>What the cabinet's bulbs are doing.</summary>
    public enum LightPattern
    {
        /// <summary>Waiting for a pull: a slow chase round the marquee, one bulb in four.</summary>
        Idle,

        /// <summary>The reels turning: a fast chase, one bulb in three.</summary>
        Spinning,

        /// <summary>A pull that paid: every bulb flashing together.</summary>
        Win,

        /// <summary>The top award: halves flashing against each other, fast.</summary>
        Jackpot,
    }

    /// <summary>
    /// Which bulbs are lit at a moment, as view state. Plain arithmetic on the time since the
    /// pattern began, so the lights can be tested without a screen.
    /// </summary>
    public static class LightShow
    {
        public const double IdleStep = 0.35, SpinStep = 0.06, WinStep = 0.12, JackpotStep = 0.08;

        public static bool IsLit(LightPattern pattern, int bulb, int bulbs, double seconds)
        {
            if (bulbs < 1) throw new ArgumentOutOfRangeException(nameof(bulbs));
            long step;
            switch (pattern)
            {
                case LightPattern.Spinning:
                    step = (long)(seconds / SpinStep);
                    return Mod(bulb - step, 3) == 0;
                case LightPattern.Win:
                    step = (long)(seconds / WinStep);
                    return step % 2 == 0;
                case LightPattern.Jackpot:
                    step = (long)(seconds / JackpotStep);
                    return (bulb % 2 == 0) == (step % 2 == 0);
                default:
                    step = (long)(seconds / IdleStep);
                    return Mod(bulb - step, 4) == 0;
            }
        }

        static long Mod(long value, long by) => ((value % by) + by) % by;
    }
}
