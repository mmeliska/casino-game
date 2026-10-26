using NUnit.Framework;
using Starflare.State;
using UnityEngine;

namespace Starflare.Tests
{
    public class PlayerStateTests
    {
        [Test]
        public void BestBankroll_OnlyGoesUp()
        {
            var state = new PlayerState();
            state.ApplyRoundResult(100, 0);
            Assert.That(state.BestBankroll, Is.EqualTo(600));

            state.ApplyRoundResult(-300, 0);
            Assert.That(state.BankrollNow, Is.EqualTo(300));
            Assert.That(state.BestBankroll, Is.EqualTo(600));
        }

        [Test]
        public void BestBankroll_CountsOnlyYourOwnMoney()
        {
            var state = new PlayerState { Debts = 200 };
            state.ApplyRoundResult(100, 0);
            Assert.That(state.OwnMoney, Is.EqualTo(400));
            Assert.That(state.BestBankroll, Is.EqualTo(PlayerState.StartingBankroll));
        }

        [Test]
        public void Rounds_FillThePartOfTheDay_ThenPlayIsFree()
        {
            var state = new PlayerState();
            for (int i = 0; i < 20; i++) state.ApplyRoundResult(0, 24);

            Assert.That(state.RoundsPlayed, Is.EqualTo(20));
            Assert.That(state.Clock.MinutesIntoPart, Is.EqualTo(GameClock.MinutesPerPart));
            Assert.That(state.Clock.PartIsOver, Is.True);
            Assert.That(state.Clock.Day, Is.EqualTo(1));
            Assert.That(state.Clock.Part, Is.EqualTo(PartOfDay.Morning));
        }

        [Test]
        public void Clock_ShowsTimeFromSixInTheMorning()
        {
            var clock = new GameClock();
            Assert.That(clock.TimeLabel, Is.EqualTo("6:00"));
            clock.AdvanceForPlay(24);
            Assert.That(clock.TimeLabel, Is.EqualTo("6:24"));
        }

        [Test]
        public void Sleep_StartsANewDay_AndSeasonsTurnEvery28Days()
        {
            var clock = new GameClock();
            clock.AdvanceForPlay(100);
            for (int i = 0; i < GameClock.DaysPerSeason; i++) clock.Sleep();

            Assert.That(clock.Day, Is.EqualTo(29));
            Assert.That(clock.Season, Is.EqualTo(Season.Summer));
            Assert.That(clock.DayOfSeason, Is.EqualTo(1));
            Assert.That(clock.MinutesIntoPart, Is.EqualTo(0));
        }

        [Test]
        public void State_SurvivesAJsonRoundTrip()
        {
            var state = new PlayerState();
            state.ApplyRoundResult(250, 24);
            state.Clock.Sleep();

            var loaded = JsonUtility.FromJson<PlayerState>(JsonUtility.ToJson(state));
            Assert.That(loaded.BankrollNow, Is.EqualTo(750));
            Assert.That(loaded.BestBankroll, Is.EqualTo(750));
            Assert.That(loaded.RoundsPlayed, Is.EqualTo(1));
            Assert.That(loaded.Clock.Day, Is.EqualTo(2));
        }
    }
}
