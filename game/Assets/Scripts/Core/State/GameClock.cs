using System;

namespace Starflare.State
{
    public enum PartOfDay { Morning, Afternoon, Night }

    public enum Season { Spring, Summer, Fall, Winter }

    /// <summary>
    /// The in-game clock. Time runs on what you do, not the real clock: a round takes a few
    /// minutes but only up to the end of the current part of the day, after which play is
    /// unlimited. Days pass only when you move, sleep, travel or take a job.
    ///
    /// Days fall on a calendar: day 1 is May 4 of year 1, the night you arrive, and year 2
    /// starts the next Jan 1. Years have 365 days, no leap years. Seasons follow the months:
    /// spring is March to May, summer June to August, fall September to November, winter
    /// December to February.
    /// </summary>
    [Serializable]
    public sealed class GameClock
    {
        public const int MinutesPerPart = 360;
        public const int DaysPerYear = 365;
        const int MorningStartHour = 6;

        static readonly int[] DaysInMonth = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        static readonly string[] MonthNames = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

        /// <summary>Day 1's place in year 1, counting Jan 1 as 0: May 4.</summary>
        const int ArrivalDayOfYear = 31 + 28 + 31 + 30 + 3;

        public int Day = 1;
        public PartOfDay Part = PartOfDay.Morning;
        public int MinutesIntoPart;

        /// <summary>Days since Jan 1 of year 1.</summary>
        int DaysSinceYearOne => ArrivalDayOfYear + Day - 1;

        public int Year => DaysSinceYearOne / DaysPerYear + 1;

        /// <summary>The month, 1 for January to 12 for December.</summary>
        public int Month => MonthAndDay.month;

        public int DayOfMonth => MonthAndDay.day;

        // March to May is spring (Season 0), so the season is the month counted from March, in threes.
        public Season Season => (Season)((Month + 9) % 12 / 3);

        (int month, int day) MonthAndDay
        {
            get
            {
                int day = DaysSinceYearOne % DaysPerYear, month = 0;
                while (day >= DaysInMonth[month]) day -= DaysInMonth[month++];
                return (month + 1, day + 1);
            }
        }
        public bool PartIsOver => MinutesIntoPart >= MinutesPerPart;

        /// <summary>Clock time as h:mm, from 6:00 in the morning.</summary>
        public string TimeLabel
        {
            get
            {
                int minutes = MorningStartHour * 60 + (int)Part * MinutesPerPart + MinutesIntoPart;
                int hour = minutes / 60 % 24;
                return $"{hour}:{minutes % 60:00}";
            }
        }

        /// <summary>The date, e.g. "May 4, Year 1".</summary>
        public string DateLabel => $"{MonthNames[Month - 1]} {DayOfMonth}, Year {Year}";

        /// <summary>Playing at a table: fills the current part of the day and stops there.</summary>
        public void AdvanceForPlay(int minutes)
        {
            if (minutes < 0) throw new ArgumentOutOfRangeException(nameof(minutes));
            MinutesIntoPart = Math.Min(MinutesPerPart, MinutesIntoPart + minutes);
        }

        public void Sleep()
        {
            Day++;
            Part = PartOfDay.Morning;
            MinutesIntoPart = 0;
        }
    }
}
