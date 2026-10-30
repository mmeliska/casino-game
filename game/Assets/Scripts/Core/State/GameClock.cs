using System;

namespace Starflare.State
{
    public enum PartOfDay { Morning, Afternoon, Night }

    public enum Season { Spring, Summer, Fall, Winter }

    /// <summary>
    /// The in-game clock. Time runs on what you do, not the real clock: a round takes a few
    /// minutes but only up to the end of the current part of the day, after which play is
    /// unlimited. Days pass only when you move, sleep, travel or take a job.
    /// </summary>
    [Serializable]
    public sealed class GameClock
    {
        /// <summary>GDD starting value; tune once the prototype runs.</summary>
        public const int DaysPerSeason = 28;
        public const int MinutesPerPart = 360;
        const int MorningStartHour = 6;

        public int Day = 1;
        public PartOfDay Part = PartOfDay.Morning;
        public int MinutesIntoPart;

        public Season Season => (Season)((Day - 1) / DaysPerSeason % 4);
        public int DayOfSeason => (Day - 1) % DaysPerSeason + 1;
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
