using System.Collections.Generic;
using System.Linq;
using Ink;
using NUnit.Framework;
using Starflare.Blackjack;
using Starflare.Cards;
using Starflare.Game;
using Starflare.State;
using UnityEngine;
using UnityEngine.UI;

namespace Starflare.Tests
{
    /// <summary>
    /// Rosa's first hand in Kasia's back room (A1.3), at the table alone: a practice hand that
    /// doesn't count, coached by pointing at the real buttons, and a scene over the table that
    /// steps aside while you press the one it points at.
    /// </summary>
    public class CoachTests
    {
        readonly List<GameObject> _made = new List<GameObject>();

        [TearDown]
        public void CleanUp()
        {
            foreach (var go in _made) Object.Destroy(go);
            _made.Clear();
        }

        [Test]
        public void APracticeHand_DoesNotCount_AndALockedTableTakesOnlyItsControl()
        {
            var state = new PlayerState();
            var table = MakeTable(state);
            var taken = new List<string>();
            table.ActionTaken += taken.Add;

            // The coach locks everything while she talks, then points at one control at a time.
            table.Lock("");
            table.Lock(BlackjackControls.Deal);
            Click(table, "Clear Button");
            Assert.That(taken, Is.Empty, "a control the coach isn't pointing at does nothing");
            Click(table, "Deal Button");

            table.Lock("");
            table.Lock(BlackjackControls.NoInsurance);
            Click(table, "Insurance Button");
            Click(table, "No insurance Button");
            table.Lock("");
            table.Lock(BlackjackControls.Stand);
            Click(table, "Stand Button");

            Assert.That(taken, Is.EqualTo(new[] { BlackjackControls.Deal, BlackjackControls.NoInsurance, BlackjackControls.Stand }));
            Assert.That(state.BankrollNow, Is.EqualTo(PlayerState.StartingBankroll), "a practice hand pays nothing");
            Assert.That(state.RoundsPlayed, Is.EqualTo(0));
            Assert.That(state.Clock.TimeLabel, Is.EqualTo("18:00"), "no time passes while she coaches");
        }

        [Test]
        public void ASceneOverTheTable_StepsAsideAtAPoint_AndCarriesOnWhenYouPressIt()
        {
            var state = new PlayerState();
            var table = MakeTable(state);
            var go = Make("Story");
            var player = go.AddComponent<StoryPlayer>();
            player.Init(Compile("=== lesson ===\nROSA: Deal yourself in. # point: deal\nROSA: Good.\n-> END\n"));
            bool done = false;
            player.PlayOver("lesson", table, () => done = true);

            Tap(go); // finish typing the line
            Tap(go); // read: she steps aside
            var help = go.GetComponentsInChildren<Button>(true).First(b => b.name == "Help Button");
            Assert.That(help.gameObject.activeSelf, Is.True);
            Click(table, "Clear Button");
            Assert.That(done, Is.False);

            Click(table, "Deal Button");
            Assert.That(help.gameObject.activeSelf, Is.False);
            Tap(go);
            Tap(go);
            Assert.That(done, Is.True);
        }

        /// <summary>
        /// Over a table the teacher is a face in a square beside the line, not a full figure;
        /// narration has no face. A scene on its own keeps the full pose and no square.
        /// </summary>
        [Test]
        public void OverATable_TheSpeakerIsAFaceBesideTheLine()
        {
            var table = MakeTable(new PlayerState());
            var go = Make("Story");
            var player = go.AddComponent<StoryPlayer>();
            player.Init(Compile("=== lesson ===\n# pose: rosa_neutral\nROSA: First hand's practice.\nThe lamp hums.\n-> END\n"));
            player.PlayOver("lesson", table, () => { });

            Assert.That(Shown(go, "Face Frame"), Is.True, "her face sits beside the line");
            Assert.That(Shown(go, "Pose"), Is.False, "no full figure over the table");
            var face = go.GetComponentsInChildren<Image>(true).First(i => i.name == "Face");
            Assert.That(face.sprite, Is.Not.Null.And.EqualTo(Art.Pose("rosa_neutral")));
            Tap(go); // finish typing
            Tap(go); // on to the narration
            Assert.That(Shown(go, "Dialogue Box"), Is.True);
            Assert.That(Shown(go, "Face Frame"), Is.False, "narration has no face");

            var alone = Make("Scene");
            var scene = alone.AddComponent<StoryPlayer>();
            scene.Init(Compile("=== talk ===\n# pose: rosa_neutral\nROSA: Sit.\n-> END\n"));
            scene.Play("talk", () => { });
            Assert.That(Shown(alone, "Face Frame"), Is.False);
            Assert.That(Shown(alone, "Pose"), Is.True);
        }

        static bool Shown(GameObject root, string name) =>
            root.GetComponentsInChildren<Transform>(true).First(t => t.name == name).gameObject.activeInHierarchy;

        BlackjackTable MakeTable(PlayerState state)
        {
            var table = Make("Table").AddComponent<BlackjackTable>();
            table.Init(new BlackjackRules { MinBet = 1, MaxBet = 50 }, new[] { 1, 5, 25 });
            table.SitDown(state, new ClockPace(), new PracticeHand(10,
                new Card(Rank.Ten, Suit.Spades), new Card(Rank.Ace, Suit.Hearts),
                new Card(Rank.Eight, Suit.Diamonds), new Card(Rank.Six, Suit.Clubs)));
            return table;
        }

        GameObject Make(string name)
        {
            var go = new GameObject(name);
            _made.Add(go);
            return go;
        }

        /// <summary>A tap: it only lands on a button that's on screen and usable, as on a phone.</summary>
        static void Click(Component on, string name)
        {
            var button = on.GetComponentsInChildren<Button>(true).First(b => b.name == name);
            if (button.IsInteractable() && button.gameObject.activeInHierarchy) button.onClick.Invoke();
        }

        static void Tap(GameObject story) =>
            story.GetComponentsInChildren<Button>(true).First(b => b.name == "Backdrop").onClick.Invoke();

        static Ink.Runtime.Story Compile(string source)
        {
            var story = new Compiler(source, new Compiler.Options { sourceFilename = "inline.ink" }).Compile();
            return new Ink.Runtime.Story(story.ToJson());
        }
    }
}
