using NUnit.Framework;
using Starflare.Game;
using Starflare.State;

namespace Starflare.Tests
{
    /// <summary>
    /// Raffy's envelope comes due (A1.5) against the real story: when each scene plays, and what
    /// each of the four ends leaves on the state and the story.
    /// </summary>
    public class RaffysEnvelopeTests
    {
        static string _json;

        [OneTimeSetUp]
        public void CompileTheStory() => _json = StoryContractTests.CompileTheGamesStory();

        [Test]
        public void Ray_AsksFromTheDueDay_OnlyOnceTheFlareIsOnTheMap()
        {
            var (state, story) = At(Checkpoints.EnvelopeDue);
            Assert.That(RaffysEnvelope.RayAsksToday(story, state.Clock.Day), Is.True);
            Assert.That(RaffysEnvelope.RayAsksToday(story, RaffysEnvelope.DueDay - 1), Is.False);

            story.Set(StoryContract.MetMitch, false);
            Assert.That(RaffysEnvelope.RayAsksToday(story, state.Clock.Day), Is.False);

            story.Set(StoryContract.MetMitch, true);
            story.Set(StoryContract.HasEnvelope, false);
            Assert.That(RaffysEnvelope.RayAsksToday(story, state.Clock.Day), Is.False, "no envelope, nothing to ask about");
        }

        [Test]
        public void RayAsks_SendsYouToJuans_AndDotWarnsYou()
        {
            var (_, story) = At(Checkpoints.EnvelopeDue);
            string text = Play(story, StoryContract.Knots.RayAsks);
            Assert.That(text, Does.Contain("RAY: Buddy!"));
            text = Choose(story, 1); // a beer
            Assert.That(text, Does.Contain("Juan's had a new kid sweeping lanes"));
            Assert.That(story.Flag(StoryContract.RayAsking), Is.True);
            Assert.That(RaffysEnvelope.RaffyHidingToday(story), Is.True);
            Assert.That(RaffysEnvelope.RayAtTheBar(story), Is.True);

            Assert.That(Play(story, StoryContract.Knots.Office), Does.Contain("not a post office"));
            Assert.That(Play(story, StoryContract.Knots.Office), Does.Not.Contain("post office"), "once");
        }

        /// <summary>Returning it moves no money: the cash was never yours. Raffy stays in hiding.</summary>
        [Test]
        public void Returned_AtTheTap_KeepsTheDrive()
        {
            var (state, story) = At(Checkpoints.EnvelopeDue);
            int cash = state.BankrollNow;
            Play(story, StoryContract.Knots.RayAsks);
            Assert.That(Choose(story, 2), Does.Contain("The drive stays in your pocket."));
            Choose(story, 0); // that's all there was
            Assert.That(story.Text(StoryContract.EnvelopeSettled), Is.EqualTo("returned"));
            Assert.That(story.Flag(StoryContract.HasEnvelope), Is.False);
            Assert.That(story.Flag(StoryContract.HasDrive), Is.True);
            Assert.That(state.BankrollNow, Is.EqualTo(cash));
            Assert.That(RaffysEnvelope.KasiaHeardFromHisMother(story, state.Clock.Day + 1), Is.False);
        }

        [Test]
        public void Helped_CostsFiveHundred_AndRaffyPaysItBackWithTheFifty()
        {
            var (state, story) = At(Checkpoints.RayAsking);
            int cash = state.BankrollNow;
            Play(story, StoryContract.Knots.RaffyHiding);
            Choose(story, 0); // cover the $500
            Assert.That(story.Text(StoryContract.EnvelopeSettled), Is.EqualTo("helped"));
            Assert.That(state.BankrollNow, Is.EqualTo(cash - 500));
            Assert.That(RaffysEnvelope.RaffyPaysToday(story, state.Clock.Day), Is.False, "not the same day");

            for (int payment = 0; payment < 4; payment++)
            {
                Assert.That(RaffysEnvelope.RaffyPaysToday(story, state.Clock.Day + payment + 1), Is.True);
                story.Set(StoryContract.Day, state.Clock.Day + payment + 1);
                Play(story, StoryContract.Knots.RaffyPays);
                Choose(story, 0); // take it
            }
            Assert.That(story.Int(StoryContract.RaffyOwes), Is.EqualTo(100));
            story.Set(StoryContract.Day, state.Clock.Day + 5);
            Assert.That(Play(story, StoryContract.Knots.RaffyPays), Does.Contain("This one's mine."));
            Assert.That(story.Flag(StoryContract.RaffyPaidOff), Is.True);
            Assert.That(state.BankrollNow, Is.EqualTo(cash + 50));
            Assert.That(RaffysEnvelope.RaffyPaysToday(story, state.Clock.Day + 6), Is.False);
        }

        [Test]
        public void Helped_StopPaying_StillBringsTheFifty()
        {
            var (state, story) = At(Checkpoints.HelpedRaffy);
            int cash = state.BankrollNow;
            Play(story, StoryContract.Knots.RaffyPays);
            Choose(story, 1); // he can stop
            story.Set(StoryContract.Day, state.Clock.Day + 1);
            Assert.That(Play(story, StoryContract.Knots.RaffyPays), Does.Contain("just the fifty"));
            Assert.That(state.BankrollNow, Is.EqualTo(cash + 50));
        }

        /// <summary>The lie: the $2,000 is yours, until Ray finds Raffy and puts it, with interest, on you.</summary>
        [Test]
        public void Lied_RayNamesYou_AndPutsTheEnvelopeOnYourDebt()
        {
            var (state, story) = At(Checkpoints.RayAsking);
            int cash = state.BankrollNow;
            Play(story, StoryContract.Knots.RaffyHiding);
            Choose(story, 2); // it's settled
            Assert.That(state.BankrollNow, Is.EqualTo(cash + 2000));
            Assert.That(RaffysEnvelope.RayNamesYouToday(story, state.Clock.Day), Is.False);
            Assert.That(RaffysEnvelope.RaffyPaysToday(story, state.Clock.Day + 1), Is.True, "a payment or two before Ray finds him");

            var (later, found) = At(Checkpoints.ToldRaffySettled);
            Assert.That(RaffysEnvelope.RayNamesYouToday(found, later.Clock.Day), Is.True);
            Assert.That(Play(found, StoryContract.Knots.RayNamesYou), Does.Contain("We need to talk about an envelope."));
            Assert.That(later.RayOwed, Is.EqualTo(2200));
            Assert.That(later.KasiasBook, Is.EqualTo(0));
            Assert.That(RaffysEnvelope.RaffyPaysToday(found, later.Clock.Day + 1), Is.False, "Raffy's gone");
            Assert.That(RaffysEnvelope.KasiaHeardFromHisMother(found, later.Clock.Day + 1), Is.True);
            Assert.That(found.Ink.EvaluateFunction("dot_warns_bridge"), Is.EqualTo(false));
        }

        [Test]
        public void Kept_InYourRoom_IsYoursAndRaffyIsGone()
        {
            var (state, story) = At(Checkpoints.RayAsking);
            int cash = state.BankrollNow;
            Assert.That(RaffysEnvelope.CanKeepIt(story), Is.True);
            Play(story, StoryContract.Knots.KeepEnvelope);
            Assert.That(state.BankrollNow, Is.EqualTo(cash + 2000));
            Assert.That(story.Text(StoryContract.EnvelopeSettled), Is.EqualTo("kept"));
            Assert.That(RaffysEnvelope.RaffyHidingToday(story), Is.False);
            Assert.That(RaffysEnvelope.KasiaHeardFromHisMother(story, state.Clock.Day), Is.False, "a later day");
            Assert.That(RaffysEnvelope.KasiaHeardFromHisMother(story, state.Clock.Day + 1), Is.True);
            Assert.That(RaffysEnvelope.CanPlugInTheDrive(story), Is.True);
            Assert.That(Play(story, StoryContract.Knots.PlugInDrive), Does.Contain("REVARIS"));
            Assert.That(RaffysEnvelope.CanPlugInTheDrive(story), Is.False);
        }

        [Test]
        public void GivingRaffyUp_ToRay_StopsDotsBridgeWarning()
        {
            var (_, story) = At(Checkpoints.RayAsking);
            Play(story, StoryContract.Knots.RaffyHiding);
            Choose(story, 1); // you'll take Ray the envelope
            Play(story, StoryContract.Knots.RayReturn);
            Assert.That(story.Ink.currentChoices.Count, Is.EqualTo(2));
            Choose(story, 1); // the kid's at Juan's
            Assert.That(story.Flag(StoryContract.GaveUpRaffy), Is.True);
            Assert.That(story.Ink.EvaluateFunction("dot_warns_bridge"), Is.EqualTo(false));
        }

        /// <summary>A version 4 save has no Ray part of what you owe; it reads as none.</summary>
        [Test]
        public void VersionFour_MigratesForward_OwingRayNothing()
        {
            const string versionFour = @"{
                ""Version"": 4,
                ""BankrollNow"": 300,
                ""BestBankroll"": 320,
                ""Debts"": 50,
                ""Name"": ""Pat"",
                ""Box"": 1,
                ""Place"": ""kasias-tap"",
                ""Clock"": { ""Day"": 6, ""MinuteOfDay"": 1200 }
            }";
            var state = SaveStore.Parse(versionFour);
            Assert.That(state.Version, Is.EqualTo(PlayerState.CurrentVersion));
            Assert.That(state.Debts, Is.EqualTo(50));
            Assert.That(state.RayOwed, Is.EqualTo(0));
            Assert.That(state.KasiasBook, Is.EqualTo(50));
        }

        static (PlayerState state, StoryBridge story) At(Checkpoint checkpoint)
        {
            var state = new PlayerState();
            var story = new StoryBridge(_json, null, state);
            checkpoint.Apply(state, story);
            if (state.BankrollNow < 1000) state.Earn(1000 - state.BankrollNow); // enough to cover Raffy's $500
            story.Sync(state);
            return (state, story);
        }

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
