using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Starflare.Bowling;
using Starflare.Game;
using Starflare.State;
using Starflare.Ui;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Starflare.Tests
{
    /// <summary>
    /// Millstone Lanes' bowling with a stand-in city: your first time on a lane is Juan's frame,
    /// coached over it; league night is watch-only unless a short team's asked you; Juan's
    /// counter sells the balls and shoes once each, and the lane gets what you bought; and a sub
    /// night bowled through is three settled rounds and a night Juan counts.
    /// </summary>
    public class LanesVisitTests
    {
        const string Bowl = "Bowl";
        const string Sub = "Bowl for Chuy's team";

        readonly List<GameObject> _made = new List<GameObject>();

        [SetUp]
        public void RightHanded() => BowlingHand.Current = Hand.Right;

        [TearDown]
        public void CleanUp()
        {
            foreach (var go in _made) Object.Destroy(go);
            _made.Clear();
        }

        (MillstoneLanes lanes, BowlingLane lane) Make(FakeHost host)
        {
            host.Story.Set(StoryContract.MetJuan, true);
            var go = new GameObject("Lanes");
            _made.Add(go);
            var lanes = go.AddComponent<MillstoneLanes>();
            var lane = go.AddComponent<BowlingLane>();
            lanes.Init(host, go.AddComponent<MillstoneBackLot>(), lane);
            lanes.Arrive();
            host.Press("Go inside");
            return (lanes, lane);
        }

        static void At(FakeHost host, DayOfWeek weekday, int minute, Func<int, bool> dayOk = null)
        {
            var clock = host.State.Clock;
            int day = clock.Day;
            while ((DayOfWeek)(day % 7) != weekday || (day == clock.Day && clock.MinuteOfDay >= minute) || (dayOk != null && !dayOk(day))) day++;
            host.State.PassTime((day - clock.Day) * GameClock.MinutesPerDay + minute - clock.MinuteOfDay);
        }

        [Test]
        public void YourFirstGame_IsJuansFrame_CoachedOverTheLane()
        {
            var host = new FakeHost();
            At(host, DayOfWeek.Wednesday, 20 * 60);
            var (_, lane) = Make(host);
            host.Press(Bowl);
            Assert.That(host.Scenes.Last(), Is.EqualTo(StoryContract.Knots.BowlFirst));
            Assert.That(host.Coached, Is.SameAs(lane));
            Assert.That(lane.Practicing, Is.True);
            Assert.That(lane.Ball, Is.SameAs(BowlingBall.House));

            // The scene sets the flag as it ends; the next sitting is a plain open game.
            host.Story.Set(StoryContract.BowledAtLanes, true);
            Click(lane, "Leave game Button");
            Assert.That(lane.Practicing, Is.True, "No getting up out of Juan's frame.");
        }

        [Test]
        public void LeagueNight_IsWatchOnly_UnlessATeamAskedYou()
        {
            var host = new FakeHost();
            At(host, DayOfWeek.Tuesday, 20 * 60, d => !MillstoneLanes.ShortTuesday(d));
            Make(host);
            Assert.That(host.Labels, Has.No.Member(Bowl));
            Assert.That(host.Labels, Has.No.Member(Sub));
            Assert.That(host.Labels, Has.Member("Watch the lanes"));

            host.State.PassTime(2 * 60 + 5);
            host.Press("Step back outside");
            host.Press("Go inside");
            Assert.That(host.Labels, Has.Member(Bowl), "After the last frame the lanes open again.");
        }

        [Test]
        public void TheCounter_SellsEachThingOnce_AndTheLaneGetsWhatYouBought()
        {
            var host = new FakeHost();
            At(host, DayOfWeek.Wednesday, 20 * 60);
            host.Story.Set(StoryContract.BowledAtLanes, true);
            var (_, lane) = Make(host);
            int cash = host.State.BankrollNow;

            host.Press("Go to the counter");
            Assert.That(host.Labels, Has.Member("Buy the urethane ball, $40"));
            Assert.That(host.Labels, Has.Member("Buy a pair of shoes, $30"));
            host.Press("Buy the urethane ball, $40");
            Assert.That(host.State.BankrollNow, Is.EqualTo(cash - 40));
            Assert.That(host.Labels, Has.No.Member("Buy the urethane ball, $40"), "Gone once bought.");
            Assert.That(host.Labels, Has.Member("Buy the reactive ball, $120"));
            host.Press("Buy a pair of shoes, $30");
            Assert.That(host.State.BankrollNow, Is.EqualTo(cash - 70));
            Assert.That(host.Labels.Any(l => l.Contains("shoes")), Is.False);

            host.Press("Back to the lanes");
            host.Press(Bowl);
            Assert.That(lane.Ball, Is.SameAs(BowlingBall.Urethane), "The ball you own.");
            Assert.That(lane.Game.Fee, Is.EqualTo(2), "Your own shoes: no rental.");
        }

        [Test]
        public void ShortTuesday_TheAskComesOnce_AndASubNightBowledThroughCountsForJuan()
        {
            var host = new FakeHost();
            At(host, DayOfWeek.Tuesday, 18 * 60 + 45, MillstoneLanes.ShortTuesday);
            host.Story.Set(StoryContract.BowledAtLanes, true);
            int day = host.State.Clock.Day;
            host.OnScene = knot =>
            {
                // What the ink does: the ask marks the day, and saying yes marks tonight.
                if (knot == StoryContract.Knots.LeagueSubAsk)
                {
                    host.Story.Set(StoryContract.SubAskedDay, day);
                    host.Story.Set(StoryContract.SubTonightDay, day);
                }
                if (knot == StoryContract.Knots.BowlShirts) host.Story.Set(StoryContract.ShirtsAsked, true);
            };
            var (_, lane) = Make(host);
            Assert.That(host.Scenes, Is.EqualTo(new[] { StoryContract.Knots.LeagueSubAsk }));
            Assert.That(host.Labels, Has.Member(Sub));
            host.Press("Step back outside");
            host.Press("Go inside");
            Assert.That(host.Scenes.Count(s => s == StoryContract.Knots.LeagueSubAsk), Is.EqualTo(1), "Asked once a night.");

            int cash = host.State.BankrollNow;
            host.Press(Sub);
            Assert.That(lane.Game.Kind, Is.EqualTo(GameKind.League));
            Assert.That(lane.Practicing, Is.False);
            int guard = 0;
            while (lane.Now != BowlingLane.Step.Over && guard++ < 2000)
            {
                if (lane.Now == BowlingLane.Step.Between) Click(lane, "Bowl Button");
                else if (lane.Now == BowlingLane.Step.Others) Click(lane, "Next Button");
                else if (lane.Now == BowlingLane.Step.Stand) BowlOne(lane);
                Skip(lane);
            }
            Assert.That(lane.FinishedNight, Is.True);
            Assert.That(host.State.RoundsPlayed, Is.EqualTo(3), "Each game is a settled round.");
            Assert.That(Math.Abs(host.State.BankrollNow - cash) % 5, Is.EqualTo(0), "The pot pays in fives.");

            Click(lane, "Leave game Button");
            Assert.That(host.Scenes.Skip(host.Scenes.Count - 2), Is.EqualTo(new[] { StoryContract.Knots.LeagueSubDone, StoryContract.Knots.BowlShirts }));
            Assert.That(host.Story.Int(StoryContract.SubNights), Is.EqualTo(1));
            Assert.That(host.Story.Flag(StoryContract.SubFinished), Is.True);
            Assert.That(host.Labels, Has.No.Member(Sub), "That's the night.");
        }

        static void BowlOne(BowlingLane lane)
        {
            Click(lane, "Aim Button");
            Click(lane, "Aim Button");
            Click(lane, "Hook Button");
            Click(lane, "Speed Button");
        }

        static void Skip(BowlingLane lane)
        {
            for (int i = 0; i < 2 && (lane.Now == BowlingLane.Step.Rolling || lane.Now == BowlingLane.Step.Pins); i++)
            {
                Click(lane, "Lane");
                Click(lane, "Lane");
            }
        }

        /// <summary>A tap on the lane's own screen (the back lot's table shares its object).</summary>
        static void Click(BowlingLane lane, string name)
        {
            var screen = lane.transform.Find("Bowling Canvas");
            var button = screen.GetComponentsInChildren<Button>(true).First(b => b.name == name);
            if (button.IsInteractable() && button.gameObject.activeInHierarchy) button.onClick.Invoke();
        }

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
            public Action<string> OnScene = _ => { };
            public ICoachable Coached;
            (string label, UnityAction action)[] _actions = new (string, UnityAction)[0];

            public IEnumerable<string> Labels => _actions.Select(a => a.label);

            public void Press(string label) => _actions.First(a => a.label == label).action();

            public void PlayScene(string knot, Action then)
            {
                Scenes.Add(knot);
                OnScene(knot);
                then();
            }

            public void PointAtPhone(bool on) { }

            public void PlaySceneOver(string knot, ICoachable game, Action then)
            {
                Scenes.Add(knot);
                Coached = game;
            }

            public void ShowPlace(string view, params (string label, UnityAction action)[] actions) => _actions = actions;

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
