using System.Collections.Generic;
using NUnit.Framework;
using Starflare.Game;
using Starflare.State;

namespace Starflare.Tests
{
    /// <summary>
    /// Plays every debug checkpoint onto a new game against the real story, so a renamed flag
    /// or a rule that now refuses a beat's step fails here instead of in the panel.
    /// </summary>
    public class CheckpointTests
    {
        static string _json;

        static readonly string[] BuiltPlaces = { SlackWaterMotel.PlaceId, KasiasTap.PlaceId, MillstoneLanes.PlaceId };

        [OneTimeSetUp]
        public void CompileTheStory() => _json = StoryContractTests.CompileTheGamesStory();

        static IEnumerable<Checkpoint> All => Checkpoints.All;

        [TestCaseSource(nameof(All))]
        public void EveryCheckpoint_AppliesToANewGame(Checkpoint checkpoint)
        {
            var (state, _) = Apply(checkpoint);
            Assert.That(state.HasSigned, Is.True);
            Assert.That(BuiltPlaces, Does.Contain(state.Place));
        }

        [Test]
        public void CheckpointNames_AreUnique()
        {
            var names = new HashSet<string>();
            foreach (var checkpoint in Checkpoints.All)
                Assert.That(names.Add(checkpoint.Name), Is.True, $"Two checkpoints are called {checkpoint.Name}.");
        }

        [Test]
        public void SatWithRosa_CarriesEveryBeatBeforeIt()
        {
            var (state, story) = Apply(Checkpoints.SatWithRosa);
            Assert.That(story.Flag(StoryContract.CheckedIn), Is.True);
            Assert.That(story.Flag(StoryContract.MetKasia), Is.True);
            Assert.That(story.Flag(StoryContract.MetJuan), Is.True);
            Assert.That(story.Flag(StoryContract.CarryingCoupons), Is.False);
            Assert.That(story.Flag(StoryContract.DidBarBack), Is.True);
            Assert.That(story.Flag(StoryContract.BackRoomOpen), Is.True);
            Assert.That(story.Flag(StoryContract.SatWithRosa), Is.True);
            Assert.That(state.BankrollNow, Is.EqualTo(PlayerState.StartingBankroll + 60));
            Assert.That(state.Clock.Day, Is.EqualTo(2));
            Assert.That(state.Scales.Straight, Is.EqualTo(1));
            Assert.That(state.Place, Is.EqualTo(KasiasTap.PlaceId));
        }

        /// <summary>Kasia's coupon job plays on walking in only after day 1 (KasiasTap.Enter).</summary>
        [Test]
        public void NightOneDone_IsTheNextDay()
        {
            var (state, _) = Apply(Checkpoints.NightOneDone);
            Assert.That(state.Clock.Day, Is.GreaterThan(1));
        }

        [Test]
        public void Broke_HasNothingAndOwesNothing()
        {
            var (state, _) = Apply(Checkpoints.Broke);
            Assert.That(state.BankrollNow, Is.Zero);
            Assert.That(state.Debts, Is.Zero);
        }

        [Test]
        public void ACheckpoint_SurvivesTheSave()
        {
            var (state, story) = Apply(Checkpoints.CarryingCoupons);
            var reloaded = new StoryBridge(_json, story.SaveJson(), state);
            Assert.That(reloaded.Flag(StoryContract.CarryingCoupons), Is.True);
        }

        static (PlayerState state, StoryBridge story) Apply(Checkpoint checkpoint)
        {
            var state = new PlayerState();
            var story = new StoryBridge(_json, null, state);
            checkpoint.Apply(state, story);
            return (state, story);
        }
    }
}
