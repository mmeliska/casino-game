using System;

namespace Starflare.Bowling
{
    /// <summary>
    /// How many balls a lane has seen since Juan oiled it, from the clock: the venue's helper at
    /// sit-down, so the lane is computed, not saved. He oils every afternoon at 5 (proposed); from
    /// then a few open bowlers an hour, more in the evening, and the league's sixty an hour from 7
    /// to 10 on a Tuesday. The lanes see nobody from 11 p.m. to noon.
    /// </summary>
    public static class LaneWear
    {
        public const int OiledAt = 17 * 60;

        /// <summary>Balls an hour on one lane, by the hour that starts at <paramref name="hour"/>.</summary>
        public static int BallsAnHour(DayOfWeek day, int hour)
        {
            if (day == DayOfWeek.Tuesday && hour >= 19 && hour < 22) return 60;
            if (hour >= 17 && hour < 23) return 10;
            if (hour >= 12 && hour < 17) return 5;
            return 0;
        }

        /// <summary>Balls since the last oiling, at this weekday and minute of the day.</summary>
        public static int BallsSinceOiling(DayOfWeek weekday, int minuteOfDay)
        {
            var day = minuteOfDay >= OiledAt ? weekday : (DayOfWeek)(((int)weekday + 6) % 7);
            int minute = OiledAt;
            int elapsed = minuteOfDay >= OiledAt ? minuteOfDay - OiledAt : minuteOfDay + 24 * 60 - OiledAt;
            double balls = 0;
            while (elapsed > 0)
            {
                int hour = minute / 60;
                int step = Math.Min(elapsed, 60 - minute % 60);
                balls += BallsAnHour(day, hour) * step / 60.0;
                elapsed -= step;
                minute += step;
                if (minute >= 24 * 60)
                {
                    minute -= 24 * 60;
                    day = (DayOfWeek)(((int)day + 1) % 7);
                }
            }
            return (int)Math.Round(balls);
        }
    }
}
