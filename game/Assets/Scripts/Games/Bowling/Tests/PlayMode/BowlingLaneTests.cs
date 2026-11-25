using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Starflare.Bowling;
using Starflare.State;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Starflare.Tests
{
    /// <summary>
    /// A lane at Millstone Lanes: built both ways up, a whole open game tapped through to one
    /// settled round, the ball dragged along the foul line, Juan's coached frame counting for
    /// nothing, a regular's challenge taken and settled, his face and line under the lane, and getting up between games.
    /// </summary>
    public class BowlingLaneTests
    {
        readonly List<GameObject> _made = new List<GameObject>();

        [TearDown]
        public void CleanUp()
        {
            foreach (var go in _made) Object.Destroy(go);
            _made.Clear();
        }

        BowlingLane MakeLane(PlayerState state, LaneSetup setup = null, BowlingRules rules = null)
        {
            var go = new GameObject("Bowling");
            _made.Add(go);
            var lane = go.AddComponent<BowlingLane>();
            lane.Init(rules ?? new BowlingRules(), null);
            lane.SitDown(state, new ClockPace(), setup ?? new LaneSetup { Seed = 3 });
            Canvas.ForceUpdateCanvases();
            return lane;
        }

        [Test]
        public void TheLane_BuildsBothWaysUp()
        {
            var lane = MakeLane(new PlayerState());
            foreach (bool landscape in new[] { false, true, false })
            {
                lane.Layout(landscape);
                var view = lane.GetComponentsInChildren<LaneView>(true).Single();
                Assert.That(view.gameObject.activeInHierarchy, Is.True);
                Assert.That(Find(lane, "Aim Button").gameObject.activeInHierarchy, Is.True);
                Assert.That(view.rectTransform.rect.height, Is.GreaterThan(200), "The lane gets room.");
                var lanePos = view.transform.position;
                var aimPos = Find(lane, "Aim Button").transform.position;
                if (landscape) Assert.That(lanePos.x, Is.LessThan(aimPos.x), "The lane on the left, the meters on the right.");
                else Assert.That(lanePos.y, Is.GreaterThan(aimPos.y), "The lane above the meters.");
            }
        }

        [Test]
        public void AFullOpenGame_ReportsOneRound_AndChargesTheFeeAndShoes()
        {
            var state = new PlayerState();
            var lane = MakeLane(state);
            int start = state.BankrollNow;
            Assert.That(lane.Now, Is.EqualTo(BowlingLane.Step.Stand));

            int balls = 0;
            while (state.RoundsPlayed == 0 && balls < 30)
            {
                Assert.That(Find(lane, "Leave game Button").IsInteractable(), Is.EqualTo(balls == 0), "No getting up mid-game.");
                BowlOne(lane);
                balls++;
            }
            Assert.That(state.RoundsPlayed, Is.EqualTo(1));
            Assert.That(balls, Is.InRange(11, 21));
            Assert.That(state.BankrollNow, Is.EqualTo(start - 4), "$2 a game and $2 for the shoes.");
            Assert.That(lane.GamesBowled, Is.EqualTo(1));
            Assert.That(lane.Game.IsComplete, Is.True);
            Assert.That(lane.Now, Is.EqualTo(BowlingLane.Step.Between));
            Assert.That(Find(lane, "Bowl Button").GetComponentInChildren<TMP_Text>().text, Does.Contain("$2"), "The shoes are paid for the visit.");
            Assert.That(lane.GetComponentsInChildren<PathTrace>(true).Single().Path, Is.Not.Null, "The trace shows the last ball.");
        }

        [Test]
        public void EachFrame_IsCalledBig_ThenTheScoreAtTheEnd()
        {
            var state = new PlayerState();
            var lane = MakeLane(state);
            var callout = lane.GetComponentsInChildren<Callout>(true).Single();
            Assert.That(callout.Showing, Is.False);
            var sheet = lane.Game.You;
            while (sheet.CurrentFrame == 1) BowlOne(lane);
            Assert.That(callout.Showing, Is.True, "The first frame's marks go up.");
            Assert.That(callout.Big, Is.EqualTo(sheet.FrameMarks(1)));
            var rules = callout.GetComponentsInChildren<Transform>(true).Where(t => t.name == "Rules").ToList();
            Assert.That(rules[0].gameObject.activeSelf, Is.False, "The first mark stands plain.");
            Assert.That(rules[1].gameObject.activeSelf, Is.True, "The second is boxed.");
            Assert.That(lane.Now, Is.EqualTo(BowlingLane.Step.Stand), "It doesn't hold up the next throw.");

            while (state.RoundsPlayed == 0) BowlOne(lane);
            Assert.That(callout.Big, Is.EqualTo(sheet.FrameMarks(10)), "The tenth's marks first.");
            Click(lane, "Lane");
            Assert.That(callout.Big, Is.EqualTo(sheet.Score.ToString()), "Then the score, big.");
            Click(lane, "Lane");
            Assert.That(callout.Showing, Is.False, "A tap clears it.");
        }

        [Test]
        public void GettingUp_BetweenGames_RaisesTheBreakPointOnce()
        {
            var state = new PlayerState();
            var lane = MakeLane(state);
            int points = 0;
            lane.BreakPointReached += p => { Assert.That(p, Is.EqualTo(BreakPoint.GotUpFromTable)); points++; };
            BowlOne(lane);
            Click(lane, "Leave game Button");
            Assert.That(points, Is.EqualTo(0), "Not mid-game.");
            while (state.RoundsPlayed == 0) BowlOne(lane);
            Click(lane, "Leave game Button");
            Click(lane, "Leave game Button");
            Assert.That(points, Is.EqualTo(1));
            Assert.That(lane.GetComponentInChildren<Canvas>(true).gameObject.activeSelf, Is.False, "The lane hides.");
            Assert.That(lane.SittingNet, Is.EqualTo(-4), "Still readable after you get up.");
        }

        [Test]
        public void DraggingTheBall_SnapsToABoard_AndStopsAtTheGutters()
        {
            var lane = MakeLane(new PlayerState());
            var view = lane.GetComponentsInChildren<LaneView>(true).Single();
            var ball = lane.GetComponentsInChildren<ApproachBall>(true).Single();
            var rect = view.rectTransform.rect;
            var low = view.rectTransform.TransformPoint(rect.min);
            var high = view.rectTransform.TransformPoint(rect.max);
            float y = low.y + (high.y - low.y) * 0.05f;
            float mid = (low.x + high.x) / 2;
            int seen = -1;

            ball.DragTo(new Vector2(mid + 3, y));
            seen = lane.StandBoard;
            Assert.That(seen, Is.InRange(18, 22), "About the middle dot.");

            ball.DragTo(new Vector2(low.x - 500, y));
            Assert.That(lane.StandBoard, Is.EqualTo(39), "Past the left gutter: board 39 for a right-hander.");
            ball.DragTo(new Vector2(high.x + 500, y));
            Assert.That(lane.StandBoard, Is.EqualTo(1));

            Click(lane, "Stand Left Button");
            Assert.That(lane.StandBoard, Is.EqualTo(2), "The left arrow nudges a board toward 39.");
        }

        [Test]
        public void JuansFrame_LocksAndUnlocksTheRightControls_AndCountsForNothing()
        {
            var state = new PlayerState();
            var lane = MakeLane(state, new LaneSetup { Seed = 3, Practice = PracticeFrame.Juans() });
            var taken = new List<string>();
            lane.ActionTaken += taken.Add;
            Assert.That(lane.Practicing, Is.True);
            Assert.That(lane.StandBoard, Is.EqualTo(7), "Juan's board.");

            for (int ball = 0; ball < 2; ball++)
            {
                lane.Lock("");
                lane.Lock(BowlingControls.Aim);
                foreach (var other in new[] { BowlingControls.StandLeft, BowlingControls.Hook, BowlingControls.Speed, BowlingControls.GetUp })
                    Assert.That(lane.Control(other).GetComponent<Button>().IsInteractable(), Is.False, $"{other} is locked while Juan points at the aim");
                Click(lane, "Aim Button");
                Click(lane, "Aim Button");
                lane.Lock(BowlingControls.Hook);
                Assert.That(Find(lane, "Hook Button").IsInteractable(), Is.True);
                Click(lane, "Hook Button");
                lane.Lock(BowlingControls.Speed);
                Click(lane, "Speed Button");
                lane.Lock("");
                Assert.That(lane.LastDelivery.Throw, Is.EqualTo(PracticeFrame.Juans().Balls[ball]), "The meters snap to his numbers.");
                Skip(lane);
            }
            lane.Lock(null);

            Assert.That(taken, Is.EqualTo(new[] { BowlingControls.Aim, BowlingControls.Hook, BowlingControls.Speed, BowlingControls.Aim, BowlingControls.Hook, BowlingControls.Speed }));
            Assert.That(lane.Practicing, Is.False);
            Assert.That(Message(lane), Does.Contain("Your game now"));
            Assert.That(state.BankrollNow, Is.EqualTo(PlayerState.StartingBankroll), "No money changes hands.");
            Assert.That(state.RoundsPlayed, Is.EqualTo(0));
            Assert.That(state.Clock.TimeLabel, Is.EqualTo("6:00 PM"), "The clock doesn't move through a coached frame.");
            Assert.That(lane.Now, Is.EqualTo(BowlingLane.Step.Stand), "Then it's your game.");
        }

        [Test]
        public void ARegularsChallenge_IsTakenAndSettled_HisFaceUpForHisTurn()
        {
            var state = new PlayerState();
            var chuy = Bowler.WithAverage("Chuy", "", 185);
            var rules = new BowlingRules { ChallengeOdds = 2 }; // an offer after every game that didn't follow one
            var lane = MakeLane(state, new LaneSetup { Seed = 3, Regulars = new[] { chuy }, OwnShoes = true }, rules);
            while (state.RoundsPlayed == 0) BowlOne(lane);
            Assert.That(lane.Offer, Is.Not.Null);
            Assert.That(lane.Offer.Bowler, Is.SameAs(chuy));
            int stake = lane.Offer.Stake;
            Assert.That(Text(lane, "Talk Title"), Is.EqualTo("Chuy"), "His face and name with the ask.");
            Assert.That(Text(lane, "Talk Line"), Does.Contain(stake == 5 ? "five" : stake == 10 ? "ten" : "twenty").IgnoreCase);
            int before = state.BankrollNow;

            Click(lane, "Take the game Button");
            Assert.That(lane.Game.Kind, Is.EqualTo(GameKind.Challenge));
            Assert.That(Find(lane, "Next Button"), Is.Null, "No button to start his ball.");
            int guard = 0;
            bool sawHisTurn = false;
            while (state.RoundsPlayed == 1 && guard++ < 80)
            {
                if (lane.Now == BowlingLane.Step.Others)
                {
                    Assert.That(Text(lane, "Talk Title"), Is.EqualTo("Chuy's Turn"));
                    Assert.That(Find(lane, "Aim Button").gameObject.activeInHierarchy, Is.False, "His turn takes the meters' place.");
                    if (!sawHisTurn) Assert.That(Text(lane, "Talk Line"), Is.Not.Empty, "He answers on his first turn.");
                    sawHisTurn = true;
                    Click(lane, "Lane");
                }
                while (lane.Now == BowlingLane.Step.Rolling || lane.Now == BowlingLane.Step.Pins) Skip(lane);
                if (lane.Now == BowlingLane.Step.Stand) BowlOne(lane);
            }
            Assert.That(state.RoundsPlayed, Is.EqualTo(2), "The challenge game is one settled round.");
            Assert.That(lane.Game.Seats[1].Sheet.IsComplete, Is.True, "Chuy bowled his ten.");
            Assert.That(state.BankrollNow - before, Is.EqualTo(lane.Game.Settle()));
            Assert.That(System.Math.Abs(state.BankrollNow - before), Is.EqualTo(stake).Or.EqualTo(0));
            Assert.That(lane.ChallengesWon + lane.ChallengesLost, Is.LessThanOrEqualTo(1));
            Assert.That(lane.Offer, Is.Null, "Never two in a row.");
            Assert.That(sawHisTurn, Is.True);
            Assert.That(Text(lane, "Talk Title"), Is.EqualTo("Chuy"), "His word on the game.");
            Assert.That(Text(lane, "Talk Line"), Is.Not.Empty);
            Assert.That(Find(lane, "Bowl Button").gameObject.activeInHierarchy, Is.False, "Continue first, then your lane.");

            Click(lane, "Continue Button");
            Assert.That(Find(lane, "Continue Button").gameObject.activeInHierarchy, Is.False);
            Assert.That(lane.GetComponentInChildren<TalkPanel>(true).gameObject.activeInHierarchy, Is.False, "He's gone back to his lane.");
            Assert.That(Find(lane, "Bowl Button").IsInteractable() && Find(lane, "Bowl Button").gameObject.activeInHierarchy, Is.True);
        }

        /// <summary>One ball: start and stop the aim, stop the hook and speed, then skip to what's left.</summary>
        static void BowlOne(BowlingLane lane)
        {
            Click(lane, "Aim Button");
            Click(lane, "Aim Button");
            Click(lane, "Hook Button");
            Click(lane, "Speed Button");
            Skip(lane);
        }

        /// <summary>Two taps on the lane skip the ball's run, two more the pins.</summary>
        static void Skip(BowlingLane lane)
        {
            for (int i = 0; i < 2 && (lane.Now == BowlingLane.Step.Rolling || lane.Now == BowlingLane.Step.Pins); i++)
            {
                Click(lane, "Lane");
                Click(lane, "Lane");
            }
        }

        static string Message(BowlingLane lane) => Text(lane, "Message");

        static string Text(Component on, string name) =>
            on.GetComponentsInChildren<TMP_Text>(true).First(t => t.name == name).text;

        static Button Find(Component on, string name) => on.GetComponentsInChildren<Button>(true).FirstOrDefault(b => b.name == name);

        /// <summary>A tap: it only lands on a button that's on screen and usable, as on a phone.</summary>
        static void Click(Component on, string name)
        {
            var button = Find(on, name);
            if (button.IsInteractable() && button.gameObject.activeInHierarchy) button.onClick.Invoke();
        }
    }
}
