using System;

namespace Starflare.State
{
    /// <summary>
    /// One sitting at a table or a mini-game, from sitting down to getting up. Real time played
    /// becomes story time at the <see cref="ClockPace"/>, up to the sitting's cap; after that you
    /// play on and the clock waits until you get up (Mike, 2026-11-05). The game counts real time
    /// in (only while you're playing, not while a teacher talks over it) and hands the story
    /// minutes to the player state with each settled round.
    /// </summary>
    public sealed class Sitting
    {
        /// <summary>
        /// The most real time one frame counts. The first frame back from the background reports
        /// the whole time away, and no time passes while the app is closed.
        /// </summary>
        public const float MaxFrameSeconds = 0.5f;

        readonly ClockPace _pace;
        double _storyMinutes;
        int _handedOver;

        public Sitting(ClockPace pace) => _pace = pace ?? throw new ArgumentNullException(nameof(pace));

        /// <summary>Story minutes this sitting has moved the clock, counting what's not handed over yet.</summary>
        public int StoryMinutes => (int)Math.Min(_storyMinutes, _pace.MaxMinutesPerSitting);

        /// <summary>The cap's reached: play is free from here until you get up.</summary>
        public bool ClockStopped => _storyMinutes >= _pace.MaxMinutesPerSitting;

        /// <summary>Real seconds of play since the last call.</summary>
        public void Play(double realSeconds)
        {
            if (realSeconds < 0) throw new ArgumentOutOfRangeException(nameof(realSeconds));
            if (ClockStopped) return;
            _storyMinutes = Math.Min(_pace.MaxMinutesPerSitting, _storyMinutes + realSeconds / 60 * _pace.StoryMinutesPerRealMinute);
        }

        /// <summary>The whole story minutes played since the last hand-over, for the clock.</summary>
        public int TakeMinutes()
        {
            int minutes = Math.Max(0, StoryMinutes - _handedOver);
            _handedOver += minutes;
            return minutes;
        }
    }
}
