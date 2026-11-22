using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Starflare.Slots;
using Starflare.State;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Starflare.Tests
{
    /// <summary>
    /// The slot machine at the glass: a pull settles at once and the reels only show it, you
    /// can't get up while they turn, a tap stops them, and a win lights the bulbs and counts in.
    /// </summary>
    public class SlotMachineTests
    {
        readonly List<GameObject> _made = new List<GameObject>();

        [TearDown]
        public void CleanUp()
        {
            foreach (var go in _made) Object.Destroy(go);
            _made.Clear();
        }

        /// <summary>Every stop is an A, and three A's pay 2 for 1, so every pull wins.</summary>
        static SlotRules AlwaysWins() => new SlotRules("always", "ALWAYS", 1,
            new[] { new[] { "A", "A" }, new[] { "A", "A" }, new[] { "A", "A" } },
            new[] { new[] { 0, 0, 0 } },
            new[] { new SlotPay("A A A", 3, 2, "A") });

        [Test]
        public void APull_SettlesTheMomentItsMade_AndYouCantGetUpWhileTheReelsTurn()
        {
            var state = new PlayerState();
            var machine = MakeMachine(state, SlotRules.TripleBar());
            BreakPoint? reached = null;
            machine.BreakPointReached += point => reached = point;

            Click(machine, "Spin Button");

            Assert.That(machine.Spinning, Is.True);
            Assert.That(state.RoundsPlayed, Is.EqualTo(1), "settled at the pull, so quitting mid-spin can't take it back");
            Assert.That(state.BankrollNow, Is.EqualTo(PlayerState.StartingBankroll + machine.LastSpin.Net));
            Assert.That(Text(machine, "Cash"), Is.EqualTo($"Cash ${PlayerState.StartingBankroll - 1}"), "the meter shows the bet gone, the win not yet in");
            Assert.That(machine.Lights, Is.EqualTo(LightPattern.Spinning));
            Assert.That(Find(machine, "Spin Button").GetComponentInChildren<TMP_Text>().text, Is.EqualTo("Stop"));
            Assert.That(Find(machine, "Bet one Button").IsInteractable(), Is.False);

            Click(machine, "Leave game Button");
            Assert.That(reached, Is.Null, "not while the reels turn");
        }

        [UnityTest]
        public IEnumerator TappingStop_LandsTheReels_AndAWinLightsUpAndCountsIn()
        {
            var state = new PlayerState();
            var machine = MakeMachine(state, AlwaysWins());
            Click(machine, "Bet max Button");
            Assert.That(machine.LastSpin.Credits, Is.EqualTo(3));
            Assert.That(state.BankrollNow, Is.EqualTo(PlayerState.StartingBankroll + 3), "three credits back as six");

            Click(machine, "Spin Button");
            float waited = 0;
            while (machine.Spinning && waited < 2) { waited += Time.unscaledDeltaTime; yield return null; }
            Assert.That(machine.Spinning, Is.False);
            Assert.That(waited, Is.LessThan(SlotMachine.FirstStop + 2 * SlotMachine.NextStop), "the tap landed them early");
            Assert.That(machine.Lights, Is.EqualTo(LightPattern.Jackpot), "the machine's only pay is its top award");

            waited = 0;
            while (!Text(machine, "Cash").EndsWith($"${PlayerState.StartingBankroll + 3}") && waited < 3) { waited += Time.unscaledDeltaTime; yield return null; }
            Assert.That(Text(machine, "Cash"), Is.EqualTo($"Cash ${PlayerState.StartingBankroll + 3}"), "the win counted in");
            Assert.That(Text(machine, "Status"), Does.Contain("A A A").And.Contain("Win $6"));
        }

        [Test]
        public void GettingUp_BetweenPulls_RaisesTheBreakPoint()
        {
            var state = new PlayerState();
            var machine = MakeMachine(state, SlotRules.TripleBar());
            BreakPoint? reached = null;
            machine.BreakPointReached += point => reached = point;

            Click(machine, "Leave game Button");

            Assert.That(reached, Is.EqualTo(BreakPoint.GotUpFromTable));
            Assert.That(machine.GetComponentInChildren<Canvas>(true).gameObject.activeSelf, Is.False);
        }

        [Test]
        public void AnyGrid_BuildsItsReelsAndRows()
        {
            var rules = new SlotRules("five", "FIVE", 3,
                Enumerable.Repeat(new[] { "A", "B", "C", "D" }, 5).ToArray(),
                new[] { new[] { 1, 1, 1, 1, 1 }, new[] { 0, 1, 2, 1, 0 } },
                new[] { new SlotPay("A x3", 3, 5, "A") });
            var machine = MakeMachine(new PlayerState(), rules);
            var names = machine.GetComponentsInChildren<RectTransform>(true).Select(t => t.name).ToList();

            Assert.That(names.Count(n => n.StartsWith("Reel ")), Is.EqualTo(5));
            Assert.That(names.Count(n => n.StartsWith("Cell ")), Is.EqualTo(5 * (3 + 2)), "each reel's rows, and one above and below to turn in");
            Assert.That(names.Count(n => n.StartsWith("Payline ")), Is.EqualTo(5), "only the straight line is drawn, behind each reel's symbols");
            Assert.That(Text(machine, "Bet"), Is.EqualTo("Bet $2 on 2 lines"));
        }

        [Test]
        public void Broke_TheMachineWontTakeAPull()
        {
            var state = new PlayerState { BankrollNow = 0 };
            var machine = MakeMachine(state, SlotRules.TripleBar());
            Assert.That(Find(machine, "Spin Button").IsInteractable(), Is.False);
            Assert.That(Text(machine, "Status"), Does.Contain("broke"));
            Assert.That(Find(machine, "Leave game Button").IsInteractable(), Is.True);
        }

        SlotMachine MakeMachine(PlayerState state, SlotRules rules)
        {
            var go = new GameObject("Slots");
            _made.Add(go);
            var machine = go.AddComponent<SlotMachine>();
            machine.Init(rules, null, 0.5f, SlotLook.TripleBar(), new System.Random(3));
            machine.SitDown(state, new ClockPace());
            return machine;
        }

        static string Text(Component on, string name) =>
            on.GetComponentsInChildren<TMP_Text>(true).First(t => t.name == name).text;

        static Button Find(Component on, string name) => on.GetComponentsInChildren<Button>(true).First(b => b.name == name);

        /// <summary>A tap: it only lands on a button that's on screen and usable, as on a phone.</summary>
        static void Click(Component on, string name)
        {
            var button = Find(on, name);
            if (button.IsInteractable() && button.gameObject.activeInHierarchy) button.onClick.Invoke();
        }
    }
}
