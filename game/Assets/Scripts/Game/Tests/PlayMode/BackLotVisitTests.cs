using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Starflare.Craps;
using Starflare.Game;
using Starflare.State;
using Starflare.Ui;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Starflare.Tests
{
    /// <summary>
    /// Millstone Lanes' back lot with a stand-in city: your first time in the circle is Juan's
    /// practice roll, coached over the table, costing nothing and moving no clock, while the lot
    /// (not the table) counts the night's heat once. Outside the late game's hours it's the empty
    /// lot, and hanging out a few minutes at a time brings Juan out at ten. You leave the way you came: around
    /// the front, or back in through the steel door.
    /// </summary>
    public class BackLotVisitTests
    {
        const string Circle = "Step into the circle";
        const string HangOut = "Hang out";

        readonly List<GameObject> _made = new List<GameObject>();

        [TearDown]
        public void CleanUp()
        {
            foreach (var go in _made) Object.Destroy(go);
            _made.Clear();
        }

        [Test]
        public void TheFirstSitting_IsJuansPracticeRoll_AndTheLotCountsHeatOnce()
        {
            var host = new FakeHost();
            host.State.PassTime(MillstoneBackLot.OpensAt + 30 - host.State.Clock.MinuteOfDay);
            var (lot, table) = Make(host);

            lot.StepOut();
            Assert.That(host.Scenes, Is.EqualTo(new[] { StoryContract.Knots.BackLot }));
            host.Press(Circle);

            Assert.That(host.Scenes.Last(), Is.EqualTo(StoryContract.Knots.BackLotPractice));
            Assert.That(host.Coached, Is.SameAs(table), "Juan coaches over the table");
            Assert.That(table.Practicing, Is.True);
            Assert.That(host.State.Heat.Points, Is.EqualTo(1), "a heat point for the night in the circle");
            var names = SeatNames(table);
            Assert.That(names.Intersect(new[] { "Chuy", "Nick", "Teddy" }), Is.Not.Empty, "one of the regulars is always there");
            Assert.That(names, Has.No.Member("Regular"), "tonight's crowd stands in before you step in");

            // The scene's "# point:" lines, in order (millstone_lanes.ink, back_lot_practice).
            foreach (string id in new[] { "right_up", "throw", "for_up", "throw", "right_up", "throw", "throw" })
                Click(table, table.Control(id).name);

            Assert.That(table.Practicing, Is.False, "the set dice ran out");
            Assert.That(host.State.BankrollNow, Is.EqualTo(PlayerState.StartingBankroll), "Juan's money, pretend");
            Assert.That(host.State.RoundsPlayed, Is.EqualTo(0));
            Assert.That(host.State.Clock.TimeLabel, Is.EqualTo("22:30"), "no time passes through the practice");

            Click(table, "Leave game Button");
            Assert.That(host.Scenes.Last(), Is.EqualTo(StoryContract.Knots.BackLotGetUp));
            Assert.That(host.View, Is.EqualTo("lanes_backlot"));

            // The scene sets the flag as it ends; the next sitting is real money and no more heat tonight.
            host.Story.Set(StoryContract.RolledWithJuan, true);
            int scenes = host.Scenes.Count;
            host.Press(Circle);
            Assert.That(table.Practicing, Is.False);
            Assert.That(host.Scenes.Count, Is.EqualTo(scenes), "no coaching the second time");
            Assert.That(host.State.Heat.Points, Is.EqualTo(1), "heat counts once a night");
        }

        [Test]
        public void BeforeTen_ItsTheEmptyLot_AndHangingOutBringsTheGameOut()
        {
            var host = new FakeHost(); // 6:00 p.m.
            bool backInside = false, backOutFront = false;
            var (lot, _) = Make(host, () => backInside = true, () => backOutFront = true);

            lot.WalkAround();
            Assert.That(host.Scenes, Is.EqualTo(new[] { StoryContract.Knots.BackLotEmpty }));
            Assert.That(host.Labels, Has.No.Member(Circle));

            Assert.That(host.Labels, Has.Member(HangOut));
            host.State.PassTime(MillstoneBackLot.OpensAt - 10 - host.State.Clock.MinuteOfDay); // 9:50 p.m.

            host.Press(HangOut);
            Assert.That(host.Labels, Is.EqualTo(new[] { "Hang out 5 minutes", "Hang out 15 minutes", "Hang out 30 minutes", "Never mind" }));
            host.Press("Hang out 5 minutes");
            Assert.That(host.State.Clock.TimeLabel, Is.EqualTo("21:55"));
            Assert.That(host.Scenes, Is.EqualTo(new[] { StoryContract.Knots.BackLotEmpty }), "still nobody out");
            Assert.That(host.Labels, Has.Member(HangOut));

            host.Press(HangOut);
            host.Press("Never mind");
            Assert.That(host.State.Clock.TimeLabel, Is.EqualTo("21:55"));

            host.Press(HangOut);
            host.Press("Hang out 15 minutes");
            Assert.That(host.State.Clock.TimeLabel, Is.EqualTo("22:10"));
            Assert.That(host.Scenes.Last(), Is.EqualTo(StoryContract.Knots.BackLot), "Juan's out with the chalk");
            Assert.That(host.Labels, Has.Member(Circle));
            Assert.That(host.State.Heat.Points, Is.EqualTo(0), "no heat for standing in the lot");

            Assert.That(host.Labels, Has.No.Member("Back inside"), "you came around the side, so you go back that way");
            host.Press("Back around front");
            Assert.That(backOutFront, Is.True);
            Assert.That(backInside, Is.False);

            lot.StepOut();
            host.Press("Back inside");
            Assert.That(backInside, Is.True);
        }

        (MillstoneBackLot lot, CrapsTable table) Make(FakeHost host, Action backInside = null, Action backOutFront = null)
        {
            var go = new GameObject("Back lot");
            _made.Add(go);
            var lot = go.AddComponent<MillstoneBackLot>();
            lot.Init(host, backInside ?? (() => { }), backOutFront ?? (() => { }));
            return (lot, go.GetComponent<CrapsTable>());
        }

        static string[] SeatNames(CrapsTable table) =>
            table.GetComponentsInChildren<Transform>(true).Where(t => t.name.StartsWith("Seat ") && t.name != "Seat 0")
                .SelectMany(t => t.GetComponentsInChildren<TMP_Text>(true)).Select(t => t.text).Where(t => t.Length > 1).ToArray();

        static void Click(Component on, string name)
        {
            var button = on.GetComponentsInChildren<Button>(true).First(b => b.name == name);
            Assert.That(button.IsInteractable() && button.gameObject.activeInHierarchy, Is.True, $"{name} can't be pressed");
            button.onClick.Invoke();
        }

        /// <summary>The city, stood in for: scenes are only noted (and end at once), and the place screen keeps its actions to press.</summary>
        sealed class FakeHost : IPlaceHost
        {
            public FakeHost()
            {
                State = new PlayerState();
                Story = StoryBridge.Load(null, State);
            }

            public PlayerState State { get; }
            public StoryBridge Story { get; }
            public ClockPace Pace { get; } = new ClockPace();

            public readonly List<string> Scenes = new List<string>();
            public ICoachable Coached;
            public string View;
            (string label, UnityAction action)[] _actions = new (string, UnityAction)[0];

            public IEnumerable<string> Labels => _actions.Select(a => a.label);

            public void Press(string label) => _actions.First(a => a.label == label).action();

            public void PlayScene(string knot, Action then)
            {
                Scenes.Add(knot);
                then();
            }

            public void PlaySceneOver(string knot, ICoachable game, Action then)
            {
                Scenes.Add(knot);
                Coached = game;
            }

            public void ShowPlace(string view, params (string label, UnityAction action)[] actions)
            {
                View = view;
                _actions = actions;
            }

            public void LeavePlaceScreen(Action then) => then();

            public void ShowWait(string view, string how, int price, Action then)
            {
                State.PassTime(60);
                then();
            }

            public void OpenMap() { }
            public void StartOver() { }
        }
    }
}
