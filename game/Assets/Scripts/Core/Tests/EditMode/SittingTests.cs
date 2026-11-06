using NUnit.Framework;
using Starflare.State;

namespace Starflare.Tests
{
    /// <summary>Real time at a table becomes story time at the pace, up to the sitting's cap (Mike, 2026-11-05).</summary>
    public class SittingTests
    {
        [Test]
        public void ARealMinute_IsThirtyStoryMinutes_ByDefault()
        {
            var sitting = new Sitting(new ClockPace());
            sitting.Play(60);
            Assert.That(sitting.TakeMinutes(), Is.EqualTo(30));
        }

        [Test]
        public void Minutes_AreHandedOverOnce_AndPartMinutesWait()
        {
            var sitting = new Sitting(new ClockPace());
            sitting.Play(19);
            Assert.That(sitting.TakeMinutes(), Is.EqualTo(9), "9.5 story minutes: the half waits");
            sitting.Play(1);
            Assert.That(sitting.TakeMinutes(), Is.EqualTo(1));
            Assert.That(sitting.TakeMinutes(), Is.EqualTo(0));
        }

        /// <summary>Twenty real minutes at the default pace would be ten hours; the sitting stops at eight.</summary>
        [Test]
        public void ASitting_MovesTheClockAtMostItsCap_ThenPlayIsFree()
        {
            var sitting = new Sitting(new ClockPace());
            for (int i = 0; i < 20; i++) sitting.Play(60);
            Assert.That(sitting.ClockStopped, Is.True);
            Assert.That(sitting.TakeMinutes(), Is.EqualTo(8 * 60));
            sitting.Play(600);
            Assert.That(sitting.TakeMinutes(), Is.EqualTo(0));
        }

        [Test]
        public void ThePace_CanChangeMidSitting()
        {
            var pace = new ClockPace();
            var sitting = new Sitting(pace);
            sitting.Play(60);
            pace.StoryMinutesPerRealMinute = 60;
            sitting.Play(60);
            Assert.That(sitting.TakeMinutes(), Is.EqualTo(90));

            pace.MaxHoursPerSitting = 1;
            Assert.That(sitting.ClockStopped, Is.True);
            Assert.That(sitting.TakeMinutes(), Is.EqualTo(0), "a lowered cap never takes time back");
        }

        [Test]
        public void ANewSitting_StartsTheCapAgain()
        {
            var pace = new ClockPace { MaxHoursPerSitting = 1 };
            var first = new Sitting(pace);
            first.Play(600);
            Assert.That(first.TakeMinutes(), Is.EqualTo(60));

            var second = new Sitting(pace);
            second.Play(60);
            Assert.That(second.TakeMinutes(), Is.EqualTo(30));
        }
    }
}
