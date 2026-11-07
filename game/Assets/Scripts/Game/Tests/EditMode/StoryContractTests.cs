using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Ink;
using NUnit.Framework;
using Starflare.Blackjack;
using Starflare.Game;
using Starflare.State;

namespace Starflare.Tests
{
    /// <summary>
    /// Compiles the real story and checks it against <see cref="StoryContract"/>, so a rename in
    /// Ink or C# fails here rather than in a scene.
    /// </summary>
    public class StoryContractTests
    {
        const string StoryDir = "Assets/Resources/Story";

        static string _json;

        [OneTimeSetUp]
        public void CompileTheStory() => _json = CompileTheGamesStory();

        [Test]
        public void EveryVariableTheCodeUses_IsDeclared()
        {
            var story = new Ink.Runtime.Story(_json);
            foreach (string name in StoryContract.Variables)
                Assert.That(story.variablesState.GlobalVariableExistsWithName(name), Is.True, $"VAR {name} is missing from the story.");
        }

        [Test]
        public void EveryKnotTheCodeStarts_Exists()
        {
            var story = new Ink.Runtime.Story(_json);
            foreach (string knot in StoryContract.Knots.All)
                Assert.That(story.KnotContainerWithName(knot), Is.Not.Null, $"Knot {knot} is missing from the story.");
        }

        [Test]
        public void EveryExternalTheStoryDeclares_IsBound()
        {
            // The bridge validates the bindings as it's built, so this throws if an EXTERNAL is unbound.
            Assert.DoesNotThrow(() => new StoryBridge(_json, savedState: null, new PlayerState()));
        }

        [Test]
        public void Sync_PushesTheSharedNumbers()
        {
            var state = new PlayerState();
            state.SignRegister("Lou", RegisterBox.Ms);
            state.Borrow(50);
            var bridge = new StoryBridge(_json, null, state);

            bridge.Sync(state);
            Assert.That(bridge.Ink.variablesState[StoryContract.PlayerName], Is.EqualTo("Lou"));
            Assert.That(bridge.Ink.variablesState[StoryContract.Bankroll], Is.EqualTo(250));
            Assert.That(bridge.Ink.variablesState[StoryContract.Book], Is.EqualTo(50));
            Assert.That(bridge.Ink.variablesState[StoryContract.Day], Is.EqualTo(1));
            Assert.That(bridge.Ink.variablesState[StoryContract.NextSaturday], Is.EqualTo(6));
            Assert.That(bridge.Ink.variablesState[StoryContract.Hour], Is.EqualTo(18));
            Assert.That(bridge.Ink.variablesState[StoryContract.RegisterBox], Is.EqualTo("ms"));
        }

        [Test]
        public void Externals_ChangeThePlayerState()
        {
            string json = Compile(
                "EXTERNAL spend(amount)\nEXTERNAL borrow(amount)\nEXTERNAL repay(amount)\nEXTERNAL sleep()\n" +
                "~ borrow(50)\n~ spend(30)\n~ repay(20)\n~ sleep()\nDone.\n-> END\n", inline: true);
            var state = new PlayerState();
            var bridge = new StoryBridge(json, null, state);

            while (bridge.Ink.canContinue) bridge.Ink.Continue();
            Assert.That(state.BankrollNow, Is.EqualTo(200));
            Assert.That(state.Debts, Is.EqualTo(30));
            Assert.That(state.Clock.Day, Is.EqualTo(2));
        }

        [Test]
        public void WorkEarnAndLean_ChangeThePlayerState()
        {
            string json = Compile(
                "EXTERNAL earn(amount)\nEXTERNAL work(pay, hours)\nEXTERNAL lean(pole)\n" +
                "~ earn(40)\n~ work(60, 4)\n~ lean(\"crooked\")\nDone.\n-> END\n", inline: true);
            var state = new PlayerState();
            var bridge = new StoryBridge(json, null, state);

            while (bridge.Ink.canContinue) bridge.Ink.Continue();
            Assert.That(state.BankrollNow, Is.EqualTo(300));
            Assert.That(state.Clock.TimeLabel, Is.EqualTo("22:00"));
            Assert.That(state.Scales.Straight, Is.EqualTo(-1));
        }

        /// <summary>The bar-back shift (A1.3), played through: keep the twenties, take the pay.</summary>
        [Test]
        public void BarBackShift_KeepingTheTwenties_PaysAndLeansCrooked()
        {
            var state = new PlayerState();
            var bridge = new StoryBridge(_json, null, state);
            var ink = bridge.Ink;
            ink.ChoosePathString(StoryContract.Knots.BarBackJob);
            while (ink.canContinue) ink.Continue();
            ink.ChooseChoiceIndex(0); // tie on the apron
            while (ink.canContinue) ink.Continue();
            Assert.That(ink.currentChoices.Count, Is.EqualTo(2));
            ink.ChooseChoiceIndex(1); // thumb out two twenties
            while (ink.canContinue) ink.Continue();

            Assert.That(state.BankrollNow, Is.EqualTo(300));
            Assert.That(state.Scales.Straight, Is.EqualTo(-1));
            Assert.That(bridge.Flag(StoryContract.DidBarBack), Is.True);
        }

        /// <summary>Kasia's nod toward the back door asks about the wedding: yes books the next Saturday (A1.4).</summary>
        [Test]
        public void BackRoomInvite_SayingYes_BooksTheNextSaturday()
        {
            var (_, bridge) = WeddingStory(RegisterBox.Mr, day: 2, hour: 10);
            Play(bridge, StoryContract.Knots.BackRoomInvite);
            bridge.Ink.ChooseChoiceIndex(0); // say you'll be there
            Play(bridge);

            Assert.That(bridge.Int(StoryContract.WeddingDay), Is.EqualTo(6));
            Assert.That(bridge.Int(StoryContract.WeddingAskedDay), Is.EqualTo(2));
            Assert.That(bridge.Flag(StoryContract.BackRoomOpen), Is.True);
        }

        /// <summary>A no and then a no-show: one more chance, then it's over without you.</summary>
        [Test]
        public void Wedding_SayingNoThenMissingIt_EndsIt()
        {
            var (state, bridge) = WeddingStory(RegisterBox.Mr, day: 2, hour: 10);
            Play(bridge, StoryContract.Knots.BackRoomInvite);
            bridge.Ink.ChooseChoiceIndex(1); // weddings aren't your thing
            Play(bridge);
            Assert.That(bridge.Flag(StoryContract.WeddingOver), Is.False);

            state.Clock.Day = 3;
            bridge.Sync(state);
            Play(bridge, StoryContract.Knots.WeddingAsk);
            bridge.Ink.ChooseChoiceIndex(0); // say you'll be there
            Play(bridge);
            Assert.That(bridge.Int(StoryContract.WeddingDay), Is.EqualTo(6));

            state.Clock.Day = 7;
            bridge.Sync(state);
            Play(bridge, StoryContract.Knots.WeddingMissed);
            Assert.That(bridge.Flag(StoryContract.WeddingOver), Is.True);
            Assert.That(bridge.Flag(StoryContract.WorkedWedding), Is.False);
            Assert.That(bridge.Ink.currentChoices, Is.Empty);
        }

        /// <summary>A first no-show: Kasia rebooks for the Saturday after, once.</summary>
        [Test]
        public void Wedding_MissedOnce_Rebooks()
        {
            var (_, bridge) = WeddingStory(RegisterBox.Mr, day: 7, hour: 20);
            bridge.Set(StoryContract.WeddingDay, 6);
            Play(bridge, StoryContract.Knots.WeddingMissed);
            bridge.Ink.ChooseChoiceIndex(0); // say you'll be there
            Play(bridge);

            Assert.That(bridge.Int(StoryContract.WeddingDay), Is.EqualTo(13));
            Assert.That(bridge.Int(StoryContract.WeddingMisses), Is.EqualTo(1));
            Assert.That(bridge.Flag(StoryContract.WeddingOver), Is.False);
        }

        /// <summary>
        /// Ticked Ms.: the bride's brother asks first. Looking past him at the maid of honor
        /// brings her offer, which Kasia carries quietly (Mike, 2026-11-06).
        /// </summary>
        [Test]
        public void Wedding_Ms_LookingPastTheBrother_BringsTheMaidOfHonorsQuietOffer()
        {
            var (state, bridge) = WeddingStory(RegisterBox.Ms, day: 6, hour: 18);
            Play(bridge, StoryContract.Knots.Wedding);
            bridge.Ink.ChooseChoiceIndex(0); // take the tray
            string first = Play(bridge);
            Assert.That(first, Does.Contain("The bride's brother asked if you'd dance. I'll cover your tables."));
            Assert.That(bridge.Ink.currentChoices.Count, Is.EqualTo(4));
            Assert.That(bridge.Ink.currentChoices[2].text, Is.EqualTo("Look past him at the maid of honor."));

            bridge.Ink.ChooseChoiceIndex(2); // look past him
            string second = Play(bridge);
            Assert.That(second, Does.Contain("The maid of honor asked me to ask. Quietly."));
            Assert.That(bridge.Ink.currentChoices[3].text, Is.EqualTo("Tell Kasia you're not that kind of gal."));
            bridge.Ink.ChooseChoiceIndex(0); // apron off before Kasia finishes
            string dance = Play(bridge);
            Assert.That(dance, Does.Contain("Asked Kasia to ask. Easier that way."));
            Assert.That(dance, Does.Contain("MAID OF HONOR: Thanks. I wasn't sure anybody would say yes."));

            Assert.That(bridge.Ink.variablesState[StoryContract.DancedWith], Is.EqualTo("maid_of_honor"));
            Assert.That(bridge.Flag(StoryContract.DanceKeen), Is.True);
            Assert.That(bridge.Flag(StoryContract.WorkedWedding), Is.True);
            Assert.That(bridge.Flag(StoryContract.WeddingOver), Is.True);
            Assert.That(state.BankrollNow, Is.EqualTo(PlayerState.StartingBankroll + 120));
            Assert.That(state.Clock.Day, Is.EqualTo(7));
            Assert.That(state.Clock.TimeLabel, Is.EqualTo("1:00"));
        }

        /// <summary>Ticked Mr.: on the clock to the maid of honor ends it; the bride's brother never asks.</summary>
        [Test]
        public void Wedding_Mr_OnTheClock_FinishesTheShiftWithoutASecondOffer()
        {
            var (state, bridge) = WeddingStory(RegisterBox.Mr, day: 6, hour: 21);
            Play(bridge, StoryContract.Knots.Wedding);
            bridge.Ink.ChooseChoiceIndex(1); // take the bus tub
            string busser = Play(bridge);
            Assert.That(busser, Does.Contain("Starflare"));
            Assert.That(busser, Does.Contain("Bridgehead"));
            Assert.That(busser, Does.Contain("The maid of honor asked"));
            bridge.Ink.ChooseChoiceIndex(3); // on the clock
            Assert.That(Play(bridge), Does.Not.Contain("bride's brother"));

            Assert.That(bridge.Ink.currentChoices, Is.Empty);
            Assert.That(bridge.Ink.variablesState[StoryContract.DancedWith], Is.EqualTo(""));
            Assert.That(bridge.Ink.variablesState[StoryContract.WeddingRole], Is.EqualTo("busser"));
            Assert.That(state.BankrollNow, Is.EqualTo(PlayerState.StartingBankroll + 95));
            Assert.That(state.Clock.TimeLabel, Is.EqualTo("1:00"));
        }

        /// <summary>Ticked Mr.: the quiet offer can be turned down as "not that kind of guy", and the shift finishes.</summary>
        [Test]
        public void Wedding_Mr_NotThatKindOfGuy_FinishesTheShift()
        {
            var (_, bridge) = WeddingStory(RegisterBox.Mr, day: 6, hour: 18);
            Play(bridge, StoryContract.Knots.Wedding);
            bridge.Ink.ChooseChoiceIndex(0); // take the tray
            Play(bridge);
            bridge.Ink.ChooseChoiceIndex(2); // look past her at the bride's brother
            Assert.That(Play(bridge), Does.Contain("The bride's brother asked me to ask. Quietly."));
            Assert.That(bridge.Ink.currentChoices[3].text, Is.EqualTo("Tell Kasia you're not that kind of guy."));
            bridge.Ink.ChooseChoiceIndex(3);
            Assert.That(Play(bridge), Does.Contain("Fair enough. I'll tell him you're on the clock."));

            Assert.That(bridge.Ink.variablesState[StoryContract.DancedWith], Is.EqualTo(""));
            Assert.That(bridge.Flag(StoryContract.WorkedWedding), Is.True);
        }

        /// <summary>No answer on the register: both ask at once; pick one or neither.</summary>
        [Test]
        public void Wedding_PreferNotToAnswer_OffersBothAtOnce()
        {
            var (_, bridge) = WeddingStory(RegisterBox.PreferNotToAnswer, day: 6, hour: 18);
            Play(bridge, StoryContract.Knots.Wedding);
            bridge.Ink.ChooseChoiceIndex(0); // take the tray
            Assert.That(Play(bridge), Does.Contain("Both asked if you'd dance."));
            Assert.That(bridge.Ink.currentChoices.Count, Is.EqualTo(3));
            bridge.Ink.ChooseChoiceIndex(1); // the bride's brother
            Play(bridge);

            Assert.That(bridge.Ink.variablesState[StoryContract.DancedWith], Is.EqualTo("brides_brother"));
        }

        /// <summary>A signed-in player on <paramref name="day"/> at <paramref name="hour"/>, the story synced to them.</summary>
        static (PlayerState state, StoryBridge bridge) WeddingStory(RegisterBox box, int day, int hour)
        {
            var state = new PlayerState();
            state.SignRegister("Lou", box);
            state.Clock.Day = day;
            state.Clock.MinuteOfDay = hour * 60;
            var bridge = new StoryBridge(_json, null, state);
            bridge.Sync(state);
            return (state, bridge);
        }

        /// <summary>Starts <paramref name="knot"/>, or carries on from a choice, up to the next choice or the end; returns the text.</summary>
        static string Play(StoryBridge bridge, string knot = null)
        {
            if (knot != null) bridge.Ink.ChoosePathString(knot);
            var text = new System.Text.StringBuilder();
            while (bridge.Ink.canContinue) text.Append(bridge.Ink.Continue());
            return text.ToString();
        }

        /// <summary>Every "# point:" in Kasia's back room names a control the blackjack table has.</summary>
        [Test]
        public void EveryPointInTheBackRoom_NamesABlackjackControl()
        {
            string source = File.ReadAllText(Path.Combine(StoryDir, "kasias_tap.ink"));
            var points = Regex.Matches(source, @"#\s*point:\s*(\w+)");
            Assert.That(points.Count, Is.GreaterThan(0));
            foreach (Match point in points)
                Assert.That(BlackjackControls.All, Does.Contain(point.Groups[1].Value), $"# point: {point.Groups[1].Value}");
        }

        [Test]
        public void FlagNames_AreTheStorysTrueFalseVariables()
        {
            var flags = new List<string>(new StoryBridge(_json, null, new PlayerState()).FlagNames());
            Assert.That(flags, Does.Contain(StoryContract.CheckedIn));
            Assert.That(flags, Does.Contain(StoryContract.SatWithRosa));
            Assert.That(flags, Does.Not.Contain(StoryContract.Bankroll));
            Assert.That(flags, Does.Not.Contain(StoryContract.PlayerName));
        }

        [Test]
        public void ASavedStoryState_ComesBack()
        {
            var state = new PlayerState();
            var first = new StoryBridge(_json, null, state);
            first.Set(StoryContract.MetKasia, true);
            var second = new StoryBridge(_json, first.SaveJson(), state);
            Assert.That(second.Flag(StoryContract.MetKasia), Is.True);
        }

        /// <summary>The game's story, compiled from source; other fixtures that need it use this too.</summary>
        internal static string CompileTheGamesStory() => Compile(Path.Combine(StoryDir, "northlight.ink"));

        static string Compile(string pathOrSource, bool inline = false)
        {
            var errors = new List<string>();
            string source = inline ? pathOrSource : File.ReadAllText(pathOrSource);
            var compiler = new Compiler(source, new Compiler.Options
            {
                sourceFilename = inline ? "inline.ink" : Path.GetFileName(pathOrSource),
                fileHandler = new FolderFileHandler(inline ? "." : Path.GetDirectoryName(pathOrSource)),
                errorHandler = (message, type) => { if (type == ErrorType.Error) errors.Add(message); },
            });
            var story = compiler.Compile();
            Assert.That(errors, Is.Empty, "The story has compile errors.");
            return story.ToJson();
        }

        /// <summary>Resolves INCLUDEs against the story's folder.</summary>
        sealed class FolderFileHandler : IFileHandler
        {
            readonly string _folder;
            public FolderFileHandler(string folder) => _folder = folder;
            public string ResolveInkFilename(string includeName) => Path.Combine(_folder, includeName);
            public string LoadInkFileContents(string fullFilename) => File.ReadAllText(fullFilename);
        }
    }
}
