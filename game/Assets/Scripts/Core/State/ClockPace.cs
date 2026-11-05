namespace Starflare.State
{
    /// <summary>
    /// How fast play moves the clock: story minutes per real minute at a table or mini-game, and
    /// the most one sitting can move it. Starting values to tune (Mike, 2026-11-05); the debug
    /// panel changes them while you play. Not saved: it's the build's pace, not the player's.
    /// </summary>
    public sealed class ClockPace
    {
        public const int DefaultStoryMinutesPerRealMinute = 30;
        public const int DefaultMaxHoursPerSitting = 8;

        public int StoryMinutesPerRealMinute = DefaultStoryMinutesPerRealMinute;
        public int MaxHoursPerSitting = DefaultMaxHoursPerSitting;

        public int MaxMinutesPerSitting => MaxHoursPerSitting * 60;
    }
}
