using System;
using System.Linq;
using NUnit.Framework;
using Starflare.Bowling;
using Starflare.Game;
using Starflare.State;

namespace Starflare.Tests
{
    /// <summary>
    /// Millstone Lanes' bowling calendar and what the venue hands the lane (bowling spec): the
    /// league's Tuesday nights, the short Tuesdays, the lane by the night, Juan's standing, and
    /// tonight's regulars and sub-night teams.
    /// </summary>
    public class LanesTests
    {
        static GameClock At(DayOfWeek weekday, int hour, int minute = 0)
        {
            int day = 7 + (int)weekday;
            return new GameClock { Day = day, MinuteOfDay = hour * 60 + minute };
        }

        [TestCase(DayOfWeek.Tuesday, 18, 59, false)]
        [TestCase(DayOfWeek.Tuesday, 19, 0, true)]
        [TestCase(DayOfWeek.Tuesday, 21, 59, true)]
        [TestCase(DayOfWeek.Tuesday, 22, 0, false)]
        [TestCase(DayOfWeek.Wednesday, 20, 0, false)]
        public void TheLeague_HasTheLanes_TuesdaysSevenToTen(DayOfWeek day, int hour, int minute, bool on)
        {
            Assert.That(MillstoneLanes.LeagueOn(At(day, hour, minute)), Is.EqualTo(on));
        }

        [Test]
        public void ATeamsShort_AboutOneTuesdayInThree_AndOnlyBetweenSixThirtyAndSevenThirty()
        {
            int shortOnes = 0, tuesdays = 0;
            for (int day = 2; day < 2 + 7 * 300; day += 7)
            {
                tuesdays++;
                if (MillstoneLanes.ShortTuesday(day)) shortOnes++;
            }
            Assert.That(shortOnes / (double)tuesdays, Is.InRange(0.25, 0.42));

            int a = Enumerable.Range(0, 100).Select(i => 2 + 7 * i).First(MillstoneLanes.ShortTuesday);
            Assert.That(MillstoneLanes.ShortTonight(new GameClock { Day = a, MinuteOfDay = 18 * 60 + 45 }), Is.True);
            Assert.That(MillstoneLanes.ShortTonight(new GameClock { Day = a, MinuteOfDay = 18 * 60 + 29 }), Is.False);
            Assert.That(MillstoneLanes.ShortTonight(new GameClock { Day = a, MinuteOfDay = 19 * 60 + 30 }), Is.False);
            Assert.That(MillstoneLanes.ShortTonight(new GameClock { Day = a, MinuteOfDay = 18 * 60 + 45 }),
                Is.EqualTo(MillstoneLanes.ShortTonight(new GameClock { Day = a, MinuteOfDay = 18 * 60 + 45 })), "the same on a reload");
        }

        [Test]
        public void JuansStanding_CountsSubNightsDouble_AndOpenBowlingNever()
        {
            Assert.That(MillstoneLanes.Standing(subNights: 1, challengesWon: 0), Is.EqualTo(MillstoneLanes.Warmer));
            Assert.That(MillstoneLanes.Standing(subNights: 0, challengesWon: 2), Is.EqualTo(MillstoneLanes.Warmer));
            Assert.That(MillstoneLanes.Standing(subNights: 3, challengesWon: 0), Is.EqualTo(MillstoneLanes.Easy));
            Assert.That(MillstoneLanes.Standing(0, 0), Is.EqualTo(0));
        }

        [Test]
        public void TheLane_IsTheSameAllNight_AndSometimesLaneSix()
        {
            var lanes = Enumerable.Range(1, 120).Select(MillstoneLanes.LaneTonight).ToList();
            Assert.That(lanes.All(l => l >= 1 && l <= 6), Is.True);
            Assert.That(lanes.Distinct().Count(), Is.EqualTo(6));
            Assert.That(MillstoneLanes.LaneTonight(40), Is.EqualTo(MillstoneLanes.LaneTonight(40)));
        }

        [Test]
        public void AnOpenNight_HasChuyAndTwoFaces_OnALaneWornByTheClock()
        {
            var rules = new BowlingRules();
            var early = MillstoneLanes.Tonight(At(DayOfWeek.Wednesday, 17), false, rules);
            var late = MillstoneLanes.Tonight(At(DayOfWeek.Wednesday, 22, 30), false, rules);
            Assert.That(early.Regulars.Select(b => b.Average), Is.EqualTo(new[] { 185, 160, 135 }));
            Assert.That(early.Regulars[0].Name, Is.EqualTo("Chuy"));
            Assert.That(early.Regulars.Select(b => b.Name), Is.Unique);
            Assert.That(early.Match, Is.Null);
            Assert.That(late.Oil.UnitsAt(10, 20), Is.LessThan(early.Oil.UnitsAt(10, 20)), "The evening's balls have worn the track.");
        }

        [Test]
        public void ASubNight_PutsChuyOnYourTeam_AgainstFour()
        {
            var setup = MillstoneLanes.Tonight(At(DayOfWeek.Tuesday, 18, 45), true, new BowlingRules());
            Assert.That(setup.Match, Is.Not.Null);
            Assert.That(setup.Match.Mates[0].Name, Is.EqualTo("Chuy"));
            Assert.That(setup.Match.Mates.Count, Is.EqualTo(3));
            Assert.That(setup.Match.Opponents.Count, Is.EqualTo(4));
            Assert.That(setup.Match.Mates.Concat(setup.Match.Opponents).Select(b => b.Name), Is.Unique);
            Assert.That(setup.Match.Mates.Concat(setup.Match.Opponents).All(b => b.Average >= 130 && b.Average <= 190), Is.True);
            Assert.That(setup.Match.Games, Is.EqualTo(3));
            Assert.That(setup.Match.SidePot, Is.EqualTo(5));
        }

        [Test]
        public void TheCheckpoints_LandOnTheirNights()
        {
            var state = new PlayerState();
            var story = StoryBridge.Load(null, state);
            Checkpoints.ShortABowler.Apply(state, story);
            Assert.That(MillstoneLanes.ShortTonight(state.Clock), Is.True, "the sub ask is about to fire");
            Assert.That(state.Place, Is.EqualTo(MillstoneLanes.PlaceId));

            state = new PlayerState();
            story = StoryBridge.Load(null, state);
            Checkpoints.LeagueToJuan.Apply(state, story);
            Assert.That(MillstoneLanes.Standing(story.Int(StoryContract.SubNights), story.Int(StoryContract.ChallengesWon)), Is.EqualTo(MillstoneLanes.Easy));
            Assert.That(state.Clock.Weekday, Is.EqualTo(DayOfWeek.Wednesday));
        }
    }
}
