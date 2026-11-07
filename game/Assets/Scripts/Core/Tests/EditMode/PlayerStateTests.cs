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

        /// <summary>The bug this clock fixed: play through the afternoon and it's night.</summary>
        [Test]
        public void Rounds_RunTheClockOnIntoTheNextPart()
        {
            var state = new PlayerState();
            state.Sleep();
            state.PassTime(GameClock.AfternoonStart - GameClock.MorningStart + 5 * 60);
            for (int i = 0; i < 12; i++) state.ApplyRoundResult(0, 10);

            Assert.That(state.RoundsPlayed, Is.EqualTo(12));
            Assert.That(state.Clock.TimeLabel, Is.EqualTo("19:00"));
            Assert.That(state.Clock.Part, Is.EqualTo(PartOfDay.Night));
            Assert.That(state.Clock.Day, Is.EqualTo(2));
        }

        [TestCase(0, PartOfDay.BeforeDawn)]
        [TestCase(359, PartOfDay.BeforeDawn)]
        [TestCase(360, PartOfDay.Morning)]
        [TestCase(720, PartOfDay.Afternoon)]
        [TestCase(1079, PartOfDay.Afternoon)]
        [TestCase(1080, PartOfDay.Night)]
        [TestCase(1439, PartOfDay.Night)]
        public void Clock_ReadsThePartFromTheHour(int minuteOfDay, PartOfDay part)
        {
            Assert.That(new GameClock { MinuteOfDay = minuteOfDay }.Part, Is.EqualTo(part));
        }

        /// <summary>The date changes at midnight: past midnight on Saturday is Sunday (Mike, 2026-11-05).</summary>
        [Test]
        public void Clock_ChangesTheDateAtMidnight()
        {
            var clock = new GameClock { MinuteOfDay = 23 * 60 };
            clock.Advance(90);
            Assert.That(clock.Day, Is.EqualTo(2));
            Assert.That(clock.TimeLabel, Is.EqualTo("0:30"));
            Assert.That(clock.Part, Is.EqualTo(PartOfDay.BeforeDawn));
        }

        /// <summary>The phone counts through a trip that runs past midnight without a clock of its own.</summary>
        [TestCase(19 * 60 + 5, "19:05")]
        [TestCase(23 * 60 + 50 + 30, "0:20")]
        [TestCase(-10, "23:50")]
        public void Clock_LabelsAnyMinute(int minuteOfDay, string label)
        {
            Assert.That(GameClock.TimeLabelAt(minuteOfDay), Is.EqualTo(label));
        }

        [TestCase(7 * 60, 5 * 60, PartOfDay.Afternoon)]
        [TestCase(15 * 60, 3 * 60, PartOfDay.Night)]
        [TestCase(20 * 60, 4 * 60, PartOfDay.BeforeDawn)]
        [TestCase(1 * 60, 5 * 60, PartOfDay.Morning)]
        public void Clock_KnowsHowLongUntilTheNextPart(int minuteOfDay, int minutes, PartOfDay next)
        {
            var clock = new GameClock { MinuteOfDay = minuteOfDay };
            Assert.That(clock.NextPart, Is.EqualTo(next));
            Assert.That(clock.MinutesToNextPart, Is.EqualTo(minutes));
            clock.Advance(clock.MinutesToNextPart);
            Assert.That(clock.Part, Is.EqualTo(next));
        }

        [Test]
        public void Waiting_MovesTheClock_AndNoTimeIsNoChange()
        {
            var state = new PlayerState();
            int changes = 0;
            state.Changed += () => changes++;
            state.PassTime(0);
            Assert.That(changes, Is.EqualTo(0));
            state.PassTime(45);
            Assert.That(state.Clock.TimeLabel, Is.EqualTo("18:45"));
            Assert.That(changes, Is.EqualTo(1));
            Assert.Throws<ArgumentOutOfRangeException>(() => state.PassTime(-1));
        }

        [Test]
        public void Travel_TakesItsMinutes_ButStayingPutTakesNone()
        {
            var state = new PlayerState();
            state.MoveTo(state.Place, 30);
            Assert.That(state.Clock.TimeLabel, Is.EqualTo("18:00"));
            state.MoveTo("kasias-tap", 30);
            Assert.That(state.Clock.TimeLabel, Is.EqualTo("18:30"));
        }

        [Test]
        public void NewGame_StartsOnTheFirstNightWith200()
        {
            var state = new PlayerState();
            Assert.That(state.BankrollNow, Is.EqualTo(200));
            Assert.That(state.Clock.Day, Is.EqualTo(1));
            Assert.That(state.Clock.Part, Is.EqualTo(PartOfDay.Night));
            Assert.That(state.Clock.TimeLabel, Is.EqualTo("18:00"));
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
        public void Clock_ShowsTheTimeOfDay()
        {
            var clock = new GameClock();
            Assert.That(clock.TimeLabel, Is.EqualTo("6:00"));
            clock.Advance(24);
            Assert.That(clock.TimeLabel, Is.EqualTo("6:24"));
        }

        [Test]
        public void Sleep_StartsANewDay()
        {
            var clock = new GameClock();
            clock.Advance(100);
            clock.Sleep();

            Assert.That(clock.Day, Is.EqualTo(2));
            Assert.That(clock.Part, Is.EqualTo(PartOfDay.Morning));
            Assert.That(clock.TimeLabel, Is.EqualTo("6:00"));
        }

        /// <summary>Turning in at 2 a.m. after Saturday night: the date's already Sunday, so you wake Sunday.</summary>
        [Test]
        public void Sleep_BeforeDawn_WakesTheSameDate()
        {
            var clock = new GameClock { Day = 5, MinuteOfDay = 2 * 60 };
            clock.Sleep();
            Assert.That(clock.Day, Is.EqualTo(5));
            Assert.That(clock.TimeLabel, Is.EqualTo("6:00"));
        }

        [Test]
        public void Clock_StartsOnMayFourthInSpring()
        {
            var clock = new GameClock();
            Assert.That(clock.DateLabel, Is.EqualTo("Mon, May 4, Year 1"));
            Assert.That(clock.Season, Is.EqualTo(Season.Spring));
        }

        [TestCase(28, "Sun, May 31, Year 1", Season.Spring)]
        [TestCase(29, "Mon, Jun 1, Year 1", Season.Summer)]
        [TestCase(121, "Tue, Sep 1, Year 1", Season.Fall)]
        [TestCase(212, "Tue, Dec 1, Year 1", Season.Winter)]
        [TestCase(242, "Thu, Dec 31, Year 1", Season.Winter)]
        [TestCase(243, "Fri, Jan 1, Year 2", Season.Winter)]
        [TestCase(301, "Sun, Feb 28, Year 2", Season.Winter)]
        [TestCase(302, "Mon, Mar 1, Year 2", Season.Spring)]
        [TestCase(608, "Sat, Jan 1, Year 3", Season.Winter)]
        public void Clock_DatesFollowTheCalendar(int day, string date, Season season)
        {
            var clock = new GameClock { Day = day };
            Assert.That(clock.DateLabel, Is.EqualTo(date));
            Assert.That(clock.Season, Is.EqualTo(season));
        }

        [TestCase(1, DayOfWeek.Monday, 6)]
        [TestCase(5, DayOfWeek.Friday, 6)]
        [TestCase(6, DayOfWeek.Saturday, 13)]
        [TestCase(7, DayOfWeek.Sunday, 13)]
        [TestCase(243, DayOfWeek.Friday, 244)]
        public void Clock_KnowsTheWeekdayAndTheNextSaturday(int day, DayOfWeek weekday, int nextSaturday)
        {
            var clock = new GameClock { Day = day };
            Assert.That(clock.Weekday, Is.EqualTo(weekday));
            Assert.That(clock.NextSaturday, Is.EqualTo(nextSaturday));
        }

        [Test]
        public void State_SurvivesAJsonRoundTrip()
        {
            var state = new PlayerState();
            state.ApplyRoundResult(250, 24);
            state.Sleep();

            var loaded = JsonUtility.FromJson<PlayerState>(JsonUtility.ToJson(state));
            Assert.That(loaded.BankrollNow, Is.EqualTo(450));
            Assert.That(loaded.BestBankroll, Is.EqualTo(450));
            Assert.That(loaded.RoundsPlayed, Is.EqualTo(1));
            Assert.That(loaded.Clock.Day, Is.EqualTo(2));
        }

        /// <summary>Kasia's bar-back shift (A1.3): sixty in cash, and about four hours.</summary>
        [Test]
        public void AShift_PaysInCash_AndTakesItsHours()
        {
            var state = new PlayerState();
            state.ApplyRoundResult(0, 30);
            state.WorkShift(60, 4);
            Assert.That(state.BankrollNow, Is.EqualTo(260));
            Assert.That(state.BestBankroll, Is.EqualTo(260));
            Assert.That(state.Clock.TimeLabel, Is.EqualTo("22:30"));
            Assert.That(state.Clock.Day, Is.EqualTo(1));
        }

        [Test]
        public void Leaning_MovesOneScale_TowardThatPole()
        {
            var state = new PlayerState();
            state.Lean("crooked");
            state.Lean("crooked");
            state.Lean("generous");
            state.Lean("flaky");
            Assert.That(state.Scales.Straight, Is.EqualTo(-2));
            Assert.That(state.Scales.Generous, Is.EqualTo(1));
            Assert.That(state.Scales.Reliable, Is.EqualTo(-1));
            Assert.That(state.Scales.Ruthless, Is.EqualTo(0));

            var loaded = JsonUtility.FromJson<PlayerState>(JsonUtility.ToJson(state));
            Assert.That(loaded.Scales.Straight, Is.EqualTo(-2));
        }

        [Test]
        public void EveryPole_IsAScaleStep()
        {
            var state = new PlayerState();
            foreach (string pole in Scales.Poles) state.Lean(pole);
            Assert.That(state.Scales.Generous + state.Scales.Reliable + state.Scales.Ruthless + state.Scales.Straight, Is.EqualTo(0));
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

        [Test]
        public void RetickingTheBox_NeedsASignedRegister_AndKeepsTheName()
        {
            var state = new PlayerState();
            Assert.Throws<InvalidOperationException>(() => state.RetickBox(RegisterBox.Mr));

            state.SignRegister("Sam", RegisterBox.Ms);
            int changes = 0;
            state.Changed += () => changes++;
            state.RetickBox(RegisterBox.Mr);
            Assert.That(state.Box, Is.EqualTo(RegisterBox.Mr));
            Assert.That(state.Name, Is.EqualTo("Sam"));
            Assert.That(changes, Is.EqualTo(1));
            Assert.Throws<ArgumentException>(() => state.RetickBox(RegisterBox.Unticked));
        }

        [Test]
        public void Changed_FiresOncePerSettledChange()
        {
            var state = new PlayerState();
            int changes = 0;
            state.Changed += () => changes++;

            state.SignRegister("Sam", RegisterBox.Mr);
            state.ApplyRoundResult(10, 5);
            state.Spend(5);
            state.Earn(5);
            state.Borrow(20);
            state.Repay(20);
            state.Sleep();
            state.MoveTo("kasias-tap");
            state.WorkShift(60, 4);
            state.Lean("straight");
            state.PassTime(60);
            Assert.That(changes, Is.EqualTo(11));
        }

        [Test]
        public void Changed_DoesNotFireForARefusedChange_OrStayingPut()
        {
            var state = new PlayerState();
            int changes = 0;
            state.Changed += () => changes++;

            Assert.Throws<InvalidOperationException>(() => state.Spend(1000));
            Assert.Throws<ArgumentOutOfRangeException>(() => state.Repay(1));
            Assert.Throws<ArgumentException>(() => state.SignRegister("", RegisterBox.Mr));
            state.MoveTo(state.Place);
            Assert.Throws<ArgumentException>(() => state.Lean("lucky"));
            Assert.That(changes, Is.EqualTo(0));
        }

        [Test]
        public void Changed_IsNotSaved()
        {
            var state = new PlayerState();
            state.Changed += () => throw new Exception("should not be serialised");
            Assert.That(JsonUtility.ToJson(state), Does.Not.Contain("Changed"));
        }
    }
}
