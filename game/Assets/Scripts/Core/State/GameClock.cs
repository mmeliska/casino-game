using System;

namespace Starflare.State
{
    /// <summary>Morning, afternoon and night keep their old numbers in saves; before dawn came later.</summary>
    public enum PartOfDay { Morning, Afternoon, Night, BeforeDawn }

    public enum Season { Spring, Summer, Fall, Winter }

    /// <summary>
    /// The in-game clock. It runs straight through the hours, and the part of the day is read
    /// from the hour: before dawn from midnight, morning from 6:00, afternoon from noon, night
    /// from 6:00 p.m. The date changes at midnight, so playing past midnight on Saturday makes
    /// it Sunday (Mike, 2026-11-05). Time moves only on what you do: play (see <see cref="Sitting"/>),
    /// travel, jobs, waiting and sleep.
    ///
    /// Days fall on a calendar: day 1 is May 4 of year 1, the night you arrive, and year 2
    /// starts the next Jan 1. Years have 365 days, no leap years. Seasons follow the months:
    /// spring is March to May, summer June to August, fall September to November, winter
    /// December to February.
    /// </summary>
    [Serializable]
    public sealed class GameClock
    {
        public const int MinutesPerDay = 24 * 60;
        public const int MinutesPerPart = 6 * 60;
        public const int DaysPerYear = 365;

        public const int MorningStart = 6 * 60;
        public const int AfternoonStart = 12 * 60;
        public const int NightStart = 18 * 60;

        static readonly int[] DaysInMonth = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        static readonly string[] MonthNames = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

        /// <summary>Day 1's place in year 1, counting Jan 1 as 0: May 4.</summary>
        const int ArrivalDayOfYear = 31 + 28 + 31 + 30 + 3;

        public int Day = 1;

        /// <summary>Minutes since midnight.</summary>
        public int MinuteOfDay = MorningStart;

        public PartOfDay Part =>
            MinuteOfDay < MorningStart ? PartOfDay.BeforeDawn
            : MinuteOfDay < AfternoonStart ? PartOfDay.Morning
            : MinuteOfDay < NightStart ? PartOfDay.Afternoon
            : PartOfDay.Night;

        /// <summary>Where each part of the day starts, minutes after midnight.</summary>
        public static int StartOf(PartOfDay part) => part switch
        {
            PartOfDay.BeforeDawn => 0,
            PartOfDay.Morning => MorningStart,
            PartOfDay.Afternoon => AfternoonStart,
            _ => NightStart,
        };

        /// <summary>The part that comes after this one: night runs into before dawn.</summary>
        public PartOfDay NextPart => Part switch
        {
            PartOfDay.BeforeDawn => PartOfDay.Morning,
            PartOfDay.Morning => PartOfDay.Afternoon,
            PartOfDay.Afternoon => PartOfDay.Night,
            _ => PartOfDay.BeforeDawn,
        };

        /// <summary>How long until the next part of the day starts, for "until night".</summary>
        public int MinutesToNextPart => ((StartOf(NextPart) - MinuteOfDay) % MinutesPerDay + MinutesPerDay) % MinutesPerDay;

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

        /// <summary>Clock time as h:mm, on a 24-hour clock.</summary>
        public string TimeLabel => TimeLabelAt(MinuteOfDay);

        /// <summary>
        /// Any time as h:mm, wrapping past midnight; the phone counts through a trip with it
        /// before the trip is taken.
        /// </summary>
        public static string TimeLabelAt(int minuteOfDay)
        {
            int minute = (minuteOfDay % MinutesPerDay + MinutesPerDay) % MinutesPerDay;
            return $"{minute / 60}:{minute % 60:00}";
        }

        /// <summary>The date, e.g. "May 4, Year 1".</summary>
        public string DateLabel => $"{MonthNames[Month - 1]} {DayOfMonth}, Year {Year}";

        /// <summary>Moves the clock on, past midnight into the next day if it gets there.</summary>
        public void Advance(int minutes)
        {
            if (minutes < 0) throw new ArgumentOutOfRangeException(nameof(minutes));
            int total = MinuteOfDay + minutes;
            Day += total / MinutesPerDay;
            MinuteOfDay = total % MinutesPerDay;
        }

        /// <summary>To the next 6:00 a.m.: the same date if you turn in before dawn.</summary>
        public void Sleep()
        {
            if (MinuteOfDay >= MorningStart) Day++;
            MinuteOfDay = MorningStart;
        }
    }
}
