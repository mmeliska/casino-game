using NUnit.Framework;
using Starflare.Game;
using Starflare.State;

namespace Starflare.Tests
{
    /// <summary>
    /// The Flare (A1.6, A1.7, R1) against the real story: which beat each visit plays, the word
    /// at the pit, and what the choices in Gus's office and at the pit leave behind.
    /// </summary>
    public class FlareTests
    {
        static string _json;

        [OneTimeSetUp]
        public void CompileTheStory() => _json = StoryContractTests.CompileTheGamesStory();

        [Test]
        public void TheFirstArrival_IsCoffeeWithGus()
        {
            var (state, story) = At(Checkpoints.AtTheFlare);
            Assert.That(TheFlare.ArrivalKnot(story, state.Clock.Day, newVisit: true), Is.EqualTo(StoryContract.Knots.FlareFirst));
        }

        /// <summary>A1.7 a beat a visit: Pete at the door, Jalen at the bar, then Gus's office.</summary>
        [Test]
        public void EachLaterVisit_PlaysTheNextBeat()
        {
            var (state, story) = At(Checkpoints.MetGus);
            Assert.That(TheFlare.ArrivalKnot(story, state.Clock.Day, true), Is.EqualTo(StoryContract.Knots.PeteFirst));
            (state, story) = At(Checkpoints.MetPete);
            Assert.That(TheFlare.ArrivalKnot(story, state.Clock.Day, true), Is.EqualTo(StoryContract.Knots.JalenFirst));
            (state, story) = At(Checkpoints.MetJalen);
            Assert.That(TheFlare.ArrivalKnot(story, state.Clock.Day, true), Is.EqualTo(StoryContract.Knots.FlareDecline));
            (state, story) = At(Checkpoints.ARegular);
            Assert.That(TheFlare.ArrivalKnot(story, state.Clock.Day, true), Is.Null);
        }

        /// <summary>Coming back the same day plays nothing new, and two beats never share a day.</summary>
        [Test]
        public void TheSameDay_PlaysNoNewBeat()
        {
            var (state, story) = At(Checkpoints.MetGus);
            Assert.That(TheFlare.ArrivalKnot(story, state.Clock.Day, newVisit: false), Is.Null);

            story.Set(StoryContract.MetPete, true);
            story.Set(StoryContract.MetPeteDay, state.Clock.Day);
            Assert.That(TheFlare.ArrivalKnot(story, state.Clock.Day, newVisit: true), Is.Null, "Jalen waits for another day");
        }

        [Test]
        public void GettingUp_MeetsPeteFirst_ThenHisWord()
        {
            var (state, story) = At(Checkpoints.AtTheFlare);
            story.Set(StoryContract.MetGus, true);
            Assert.That(TheFlare.GetUpKnot(story, state.Clock.Day), Is.EqualTo(StoryContract.Knots.PeteFirst));
            story.Set(StoryContract.MetPete, true);
            Assert.That(TheFlare.GetUpKnot(story, state.Clock.Day), Is.EqualTo(StoryContract.Knots.FlareGetUp));
        }

        /// <summary>The name scene: after the notice, on a later day, a few sittings in.</summary>
        [Test]
        public void PetesNameScene_WaitsForTheNotice_ALaterDay_AndAFewSittings()
        {
            var (state, story) = At(Checkpoints.KeptGusSecret);
            Assert.That(TheFlare.GetUpKnot(story, state.Clock.Day), Is.EqualTo(StoryContract.Knots.FlareGetUp), "one sitting short");

            story.Set(StoryContract.FlareSittings, TheFlare.SittingsForYourName);
            Assert.That(TheFlare.GetUpKnot(story, state.Clock.Day), Is.EqualTo(StoryContract.Knots.PeteName));
            Assert.That(TheFlare.GetUpKnot(story, story.Int(StoryContract.SawNoticeDay)), Is.EqualTo(StoryContract.Knots.FlareGetUp), "not the notice's own night");

            story.Set(StoryContract.PeteSaidName, true);
            Assert.That(TheFlare.GetUpKnot(story, state.Clock.Day), Is.EqualTo(StoryContract.Knots.FlareGetUp));
        }

        [Test]
        public void CoffeeWithGus_MeetsHim_AndTheJarCostsFive()
        {
            var (state, story) = At(Checkpoints.AtTheFlare);
            Play(story, StoryContract.Knots.FlareFirst);
            Choose(story, 0); // tell him your name
            Choose(story, 0); // rub the boot
            string text = Choose(story, 0); // the busboys' jar
            Assert.That(text, Does.Contain("saving for school"));
            Assert.That(story.Flag(StoryContract.MetGus), Is.True);
            Assert.That(story.Int(StoryContract.MetGusDay), Is.EqualTo(state.Clock.Day));
            Assert.That(story.Flag(StoryContract.RubbedBoot), Is.True);
            Assert.That(story.Flag(StoryContract.TippedJar), Is.True);
            Assert.That(state.BankrollNow, Is.EqualTo(Bankroll(Checkpoints.AtTheFlare) - 5));
            Assert.That(state.Scales.Generous, Is.EqualTo(1));
        }

        /// <summary>Keeping Gus's secret from Pete ("a good view") is what builds his trust.</summary>
        [Test]
        public void TheNotice_KeptFromPete_BuildsGusTrust()
        {
            var (_, story) = At(Checkpoints.MetJalen);
            Assert.That(Play(story, StoryContract.Knots.FlareDecline), Does.Contain("shutoff date"));
            Choose(story, 0); // you didn't see anything
            Choose(story, 0); // Dot runs the place like a ship
            string pete = Choose(story, 1); // a good view
            Assert.That(pete, Does.Contain("Say nothing."));
            Assert.That(story.Flag(StoryContract.SawNotice), Is.True);
            Assert.That(story.Flag(StoryContract.GusAskedAfterDot), Is.True);
            Assert.That(story.Flag(StoryContract.KeptGusSecret), Is.True);
            Assert.That(story.Flag(StoryContract.ToldPete), Is.False);
            Assert.That(story.Int(StoryContract.GusTrust), Is.EqualTo(1));
        }

        [Test]
        public void TheNotice_ToldToPete_CostsGusTrust()
        {
            var (state, story) = At(Checkpoints.MetJalen);
            int straight = state.Scales.Straight;
            Play(story, StoryContract.Knots.FlareDecline);
            Choose(story, 2); // Pete should hear it from him
            Choose(story, 1); // she's fine
            Choose(story, 0); // tell Pete
            Assert.That(story.Flag(StoryContract.ToldPete), Is.True);
            Assert.That(story.Flag(StoryContract.KeptGusSecret), Is.False);
            Assert.That(story.Int(StoryContract.GusTrust), Is.EqualTo(-1));
            Assert.That(state.Scales.Straight, Is.EqualTo(straight + 2));
        }

        /// <summary>R1: Dot asks after him once A1.7's under way (you've met Pete), once, and changes the subject.</summary>
        [Test]
        public void DotAsksAfterGus_OnceYouveMetPete_Once()
        {
            var (_, story) = At(Checkpoints.MetGus);
            Assert.That(Play(story, StoryContract.Knots.Office), Does.Not.Contain("red jacket"), "not yet");
            (_, story) = At(Checkpoints.MetPete);
            string first = Play(story, StoryContract.Knots.Office);
            Assert.That(first, Does.Contain("The big fella in the red jacket."));
            Assert.That(story.Flag(StoryContract.DotAskedAfterGus), Is.True);
            Assert.That(Play(story, StoryContract.Knots.Office), Does.Not.Contain("red jacket"));
        }

        /// <summary>Once Dot has asked, Gus's window offers telling him so.</summary>
        [Test]
        public void GusWindow_OffersDotsQuestion_OnlyOnceSheAsked()
        {
            var (_, story) = At(Checkpoints.MetJalen);
            Play(story, StoryContract.Knots.FlareDecline);
            Choose(story, 0);
            Assert.That(story.Ink.currentChoices.Count, Is.EqualTo(2));

            (_, story) = At(Checkpoints.MetJalen);
            story.Set(StoryContract.DotAskedAfterGus, true);
            Play(story, StoryContract.Knots.FlareDecline);
            Choose(story, 0);
            Assert.That(story.Ink.currentChoices.Count, Is.EqualTo(3));
            Choose(story, 2); // she asked how he was
            Assert.That(story.Int(StoryContract.GusTrust), Is.EqualTo(1));
        }

        /// <summary>Pete's name scene: the name from Dot's register, and Jalen's "He doesn't do that."</summary>
        [Test]
        public void PeteName_SaysYourName()
        {
            var (_, story) = At(Checkpoints.KeptGusSecret);
            string text = Play(story, StoryContract.Knots.PeteName);
            Assert.That(text, Does.Contain("Lou. Your rating says"));
            Assert.That(text, Does.Contain("Excuse me, ma'am. He doesn't do that."));
            Assert.That(story.Flag(StoryContract.PeteSaidName), Is.True);
            Assert.That(Play(story, StoryContract.Knots.FlareGetUp), Does.Contain("Lou"));
        }

        [Test]
        public void PeteFirst_CallsYouKid()
        {
            var (_, story) = At(Checkpoints.MetGus);
            Assert.That(Play(story, StoryContract.Knots.PeteFirst), Does.Contain("Kid. Your rating says"));
            Assert.That(story.Flag(StoryContract.MetPete), Is.True);
        }

        static (PlayerState state, StoryBridge story) At(Checkpoint checkpoint)
        {
            var state = new PlayerState();
            var story = new StoryBridge(_json, null, state);
            checkpoint.Apply(state, story);
            story.Sync(state);
            return (state, story);
        }

        static int Bankroll(Checkpoint checkpoint) => At(checkpoint).state.BankrollNow;

        static string Play(StoryBridge story, string knot)
        {
            story.Ink.ChoosePathString(knot);
            return Continue(story);
        }

        static string Choose(StoryBridge story, int index)
        {
            story.Ink.ChooseChoiceIndex(index);
            return Continue(story);
        }

        static string Continue(StoryBridge story)
        {
            var text = new System.Text.StringBuilder();
            while (story.Ink.canContinue) text.Append(story.Ink.Continue());
            return text.ToString();
        }
    }
}
