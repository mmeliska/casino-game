using NUnit.Framework;
using Starflare.State;

namespace Starflare.Tests
{
    /// <summary>Heat as the GDD counts it (Mike, 2026-11-20): points a night per illegal place, a step cooled each week.</summary>
    public class HeatTests
    {
        const string BackLot = "millstone-lanes";
        const string BackRoom = "kasias-tap";

        /// <summary>A new game at night on day 1, as you arrive.</summary>
        static PlayerState Arrived() => new PlayerState();

        [TestCase(0, HeatStep.None)]
        [TestCase(1, HeatStep.Low)]
        [TestCase(3, HeatStep.Low)]
        [TestCase(4, HeatStep.Medium)]
        [TestCase(7, HeatStep.Medium)]
        [TestCase(8, HeatStep.High)]
        [TestCase(30, HeatStep.High)]
        public void Steps_AreThresholdsOnThePoints(int points, HeatStep step)
        {
            Assert.That(Heat.StepAt(points), Is.EqualTo(step));
        }

        [Test]
        public void ANewGame_HasNoHeat()
        {
            Assert.That(Arrived().HeatStep, Is.EqualTo(HeatStep.None));
        }

        [Test]
        public void APlace_CountsOnceANight_HoweverManySittings()
        {
            var state = Arrived();
            state.PlayIllegalGame(BackLot, 1);
            state.PassTime(60);
            state.PlayIllegalGame(BackLot, 1);
            Assert.That(state.Heat.Points, Is.EqualTo(1));
        }

        [Test]
        public void TwoPlaces_TheSameNight_EachCount()
        {
            var state = Arrived();
            state.PlayIllegalGame(BackRoom, 1);
            state.PlayIllegalGame(BackLot, 1);
            Assert.That(state.Heat.Points, Is.EqualTo(2));
        }

        /// <summary>The late game runs past midnight: that's still the same night, not a new one.</summary>
        [Test]
        public void PastMidnight_IsStillTheSameNight()
        {
            var state = Arrived();
            state.PassTime(GameClock.MinutesPerDay - state.Clock.MinuteOfDay - 60);
            state.PlayIllegalGame(BackLot, 1);
            state.PassTime(2 * 60);
            Assert.That(state.Clock.Part, Is.EqualTo(PartOfDay.BeforeDawn));
            state.PlayIllegalGame(BackLot, 1);
            Assert.That(state.Heat.Points, Is.EqualTo(1));
        }

        [Test]
        public void TheNextNight_CountsAgain()
        {
            var state = Arrived();
            state.PlayIllegalGame(BackLot, 1);
            state.Sleep();
            state.PassTime(GameClock.NightStart - GameClock.MorningStart);
            state.PlayIllegalGame(BackLot, 1);
            Assert.That(state.Heat.Points, Is.EqualTo(2));
        }

        [Test]
        public void Counting_RaisesChanged_ButNotASecondTime()
        {
            var state = Arrived();
            int changes = 0;
            state.Changed += () => changes++;
            state.PlayIllegalGame(BackLot, 1);
            state.PlayIllegalGame(BackLot, 1);
            Assert.That(changes, Is.EqualTo(1));
        }

        /// <summary>"One step per week of licensed-only play", to the top of the step below.</summary>
        [TestCase(9, 7)]
        [TestCase(5, 3)]
        [TestCase(2, 0)]
        public void AWeekWithoutIllegalPlay_DropsAStep(int points, int after)
        {
            var state = Arrived();
            state.SetHeatPoints(points);
            for (int i = 0; i < Heat.NightsToCoolAStep - 1; i++) state.Sleep();
            Assert.That(state.Heat.Points, Is.EqualTo(points), "Six nights isn't a week.");

            state.Sleep();
            Assert.That(state.Heat.Points, Is.EqualTo(after));
        }

        [Test]
        public void TwoWeeks_DropTwoSteps()
        {
            var state = Arrived();
            state.SetHeatPoints(9);
            state.PassTime(2 * Heat.NightsToCoolAStep * GameClock.MinutesPerDay);
            Assert.That(state.Heat.Points, Is.EqualTo(Heat.MediumFrom - 1));
            Assert.That(state.HeatStep, Is.EqualTo(HeatStep.Low));
        }

        [Test]
        public void IllegalPlay_StartsTheWeekAgain()
        {
            var state = Arrived();
            state.SetHeatPoints(5);
            for (int i = 0; i < 5; i++) state.Sleep();
            state.PassTime(GameClock.NightStart - GameClock.MorningStart);
            state.PlayIllegalGame(BackLot, 1);
            for (int i = 0; i < Heat.NightsToCoolAStep - 1; i++) state.Sleep();
            Assert.That(state.Heat.Points, Is.EqualTo(6));
        }

        [Test]
        public void Rounds_LetHeatCoolToo()
        {
            var state = Arrived();
            state.SetHeatPoints(2);
            state.ApplyRoundResult(0, Heat.NightsToCoolAStep * GameClock.MinutesPerDay);
            Assert.That(state.HeatStep, Is.EqualTo(HeatStep.None));
        }
    }
}
