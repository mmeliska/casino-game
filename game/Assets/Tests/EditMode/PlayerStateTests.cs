using System;
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
            Assert.That(state.BestBankroll, Is.EqualTo(300));

            state.ApplyRoundResult(-150, 0);
            Assert.That(state.BankrollNow, Is.EqualTo(150));
            Assert.That(state.BestBankroll, Is.EqualTo(300));
        }

        [Test]
        public void BestBankroll_CountsOnlyYourOwnMoney()
        {
            var state = new PlayerState { Debts = 100 };
            state.ApplyRoundResult(50, 0);
            Assert.That(state.OwnMoney, Is.EqualTo(150));
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
            Assert.That(state.Clock.Part, Is.EqualTo(PartOfDay.Night));
        }

        [Test]
        public void NewGame_StartsOnTheFirstNightWith200()
        {
            var state = new PlayerState();
            Assert.That(state.BankrollNow, Is.EqualTo(200));
            Assert.That(state.Clock.Day, Is.EqualTo(1));
            Assert.That(state.Clock.Part, Is.EqualTo(PartOfDay.Night));
        }

        [Test]
        public void Borrowing_AddsCashAndDebt_ButNotBestBankroll()
        {
            var state = new PlayerState();
            state.ApplyRoundResult(-200, 0);
            state.Borrow(50);

            Assert.That(state.BankrollNow, Is.EqualTo(50));
            Assert.That(state.Debts, Is.EqualTo(50));
            Assert.That(state.OwnMoney, Is.EqualTo(0));
            Assert.That(state.BestBankroll, Is.EqualTo(PlayerState.StartingBankroll));

            state.ApplyRoundResult(100, 0);
            state.Repay(50);
            Assert.That(state.BankrollNow, Is.EqualTo(100));
            Assert.That(state.Debts, Is.EqualTo(0));
        }

        [Test]
        public void Repay_NeedsTheCashAndAnOpenDebt()
        {
            var state = new PlayerState();
            Assert.Throws<ArgumentOutOfRangeException>(() => state.Repay(10));

            state.Borrow(50);
            state.ApplyRoundResult(-240, 0);
            Assert.Throws<InvalidOperationException>(() => state.Repay(50));
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
            Assert.That(loaded.BankrollNow, Is.EqualTo(450));
            Assert.That(loaded.BestBankroll, Is.EqualTo(450));
            Assert.That(loaded.RoundsPlayed, Is.EqualTo(1));
            Assert.That(loaded.Clock.Day, Is.EqualTo(2));
        }

        [Test]
        public void NewGame_StartsUnsignedAtTheMotel()
        {
            var state = new PlayerState();
            Assert.That(state.HasSigned, Is.False);
            Assert.That(state.Place, Is.EqualTo("slack-water"));
        }

        [Test]
        public void SigningTheRegister_KeepsTheTrimmedNameAndBox()
        {
            var state = new PlayerState();
            state.SignRegister("  Sam Novak ", RegisterBox.PreferNotToAnswer);

            Assert.That(state.HasSigned, Is.True);
            Assert.That(state.Name, Is.EqualTo("Sam Novak"));
            Assert.That(state.Box, Is.EqualTo(RegisterBox.PreferNotToAnswer));

            var loaded = JsonUtility.FromJson<PlayerState>(JsonUtility.ToJson(state));
            Assert.That(loaded.Name, Is.EqualTo("Sam Novak"));
            Assert.That(loaded.Box, Is.EqualTo(RegisterBox.PreferNotToAnswer));
        }

        [Test]
        public void SigningTheRegister_NeedsANameAndABox_AndHappensOnce()
        {
            var state = new PlayerState();
            Assert.Throws<ArgumentException>(() => state.SignRegister("   ", RegisterBox.Ms));
            Assert.Throws<ArgumentException>(() => state.SignRegister("Sam", RegisterBox.Unticked));

            state.SignRegister("Sam", RegisterBox.Ms);
            Assert.Throws<InvalidOperationException>(() => state.SignRegister("Alex", RegisterBox.Mr));
            Assert.That(state.Name, Is.EqualTo("Sam"));
        }
    }
}
