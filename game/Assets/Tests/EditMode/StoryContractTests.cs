using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Ink;
using NUnit.Framework;
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
                "EXTERNAL earn(amount)\nEXTERNAL work(pay)\nEXTERNAL lean(pole)\n" +
                "~ earn(40)\n~ work(60)\n~ lean(\"crooked\")\nDone.\n-> END\n", inline: true);
            var state = new PlayerState();
            var bridge = new StoryBridge(json, null, state);

            while (bridge.Ink.canContinue) bridge.Ink.Continue();
            Assert.That(state.BankrollNow, Is.EqualTo(300));
            Assert.That(state.Clock.PartIsOver, Is.True);
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
