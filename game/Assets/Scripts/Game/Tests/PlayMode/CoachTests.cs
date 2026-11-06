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
