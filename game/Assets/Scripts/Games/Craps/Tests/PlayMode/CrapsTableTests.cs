using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Starflare.Craps;
using Starflare.State;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Starflare.Tests
{
    /// <summary>
    /// The craps circle in Millstone Lanes' back lot: Juan's practice roll coached through the
    /// real buttons and counting for nothing, then real throws that settle rounds, and getting up
    /// only when nothing of yours is riding.
    /// </summary>
    public class CrapsTableTests
    {
        readonly List<GameObject> _made = new List<GameObject>();

        [TearDown]
        public void CleanUp()
        {
            foreach (var go in _made) Object.Destroy(go);
            _made.Clear();
        }

        [Test]
        public void JuansPracticeRoll_CoachedThroughTheRealButtons_CountsForNothing()
        {
            var state = new PlayerState();
            var table = MakeTable(state, new PracticeTurn(20,
                new Roll(3, 3), new Roll(5, 1), new Roll(5, 5), new Roll(4, 3)));
            var taken = new List<string>();
            table.ActionTaken += taken.Add;
            Assert.That(table.Practicing, Is.True);

            string rightUp = CrapsControls.Up(BetKind.Right);
            Coach(table, rightUp);
            Coach(table, CrapsControls.Throw); // 6: the point
            Assert.That(Message(table), Does.Contain("The point is 6"));
            Coach(table, CrapsControls.Throw); // 6: made
            Assert.That(Message(table), Does.Contain("Point made"));
            Assert.That(Text(table, "Payout"), Does.Contain("Right +$1"), "what your bets did is the popup's");
            Assert.That(Text(table, "Payout Title"), Is.EqualTo("You won $1"));
            Coach(table, rightUp);
            Coach(table, CrapsControls.Throw); // 10: a new point
            Coach(table, CrapsControls.Throw); // 7: seven out

            Assert.That(taken, Is.EqualTo(new[] { rightUp, CrapsControls.Throw, CrapsControls.Throw, rightUp, CrapsControls.Throw, CrapsControls.Throw }));
            Assert.That(table.Practicing, Is.False, "the set dice ran out, so the practice is over");
            Assert.That(table.Shooter, Is.EqualTo(1), "the dice passed to the next in the circle");
            Assert.That(Message(table), Does.Contain("Real money from here"));
            Assert.That(state.BankrollNow, Is.EqualTo(PlayerState.StartingBankroll), "no money changes hands");
            Assert.That(state.RoundsPlayed, Is.EqualTo(0));
            Assert.That(state.Clock.TimeLabel, Is.EqualTo("18:00"), "the clock doesn't move through a practice turn");

            table.Lock(null);
            Assert.That(Find(table, "Call it Button").gameObject.activeInHierarchy, Is.True, "a regular's throw is yours to call");
            Assert.That(Find(table, "Leave game Button").IsInteractable(), Is.True);
        }

        [Test]
        public void ARealThrow_SettlesARound_AndYouCantGetUpWhileABetRides()
        {
            var state = new PlayerState();
            var table = MakeTable(state);
            Assert.That(table.Shooter, Is.EqualTo(0));
            Assert.That(Find(table, "Throw Button").IsInteractable(), Is.False, "you bet right to shoot");
            Assert.That(Find(table, "Pass the dice Button").gameObject.activeInHierarchy, Is.True);

            Click(table, CrapsControls.Up(BetKind.Right) + " Button");
            for (int i = 0; i < 200 && state.RoundsPlayed == 0; i++)
            {
                Click(table, "Throw Button");
                if (state.RoundsPlayed == 0)
                {
                    Assert.That(Find(table, "Leave game Button").IsInteractable(), Is.False, "your right bet is riding the point");
                }
            }

            Assert.That(state.RoundsPlayed, Is.EqualTo(1), "your right bet settled within the hand");
            Assert.That(table.SittingNet, Is.EqualTo(1).Or.EqualTo(-1), "a $1 right bet wins or loses a dollar, too small for Juan's cut");
            Assert.That(Text(table, "Payout"), Does.Contain(table.SittingNet > 0 ? "Right +$1" : "Right -$1"));
            Assert.That(Text(table, "Payout Title"), Is.EqualTo(table.SittingNet > 0 ? "You won $1" : "You lost $1"));
        }

        [Test]
        public void GettingUp_BetweenThrows_RaisesTheBreakPoint_AndGivesBackWhatWasntThrown()
        {
            var state = new PlayerState();
            var table = MakeTable(state);
            BreakPoint? reached = null;
            table.BreakPointReached += point => reached = point;

            Click(table, CrapsControls.Up(BetKind.Right) + " Button");
            Click(table, "Leave game Button");

            Assert.That(reached, Is.EqualTo(BreakPoint.GotUpFromTable));
            Assert.That(state.BankrollNow, Is.EqualTo(PlayerState.StartingBankroll), "a bet not thrown on goes back in your pocket");
            Assert.That(table.SittingNet, Is.EqualTo(0));
        }

        [Test]
        public void AWin_ShowsJuansCutOnItsOwnLine()
        {
            Assert.That(CrapsTable.PayoutLine(new SettledBet(BetKind.PointFor, 6, BetOutcome.Won, 1, 11), 4),
                Is.EqualTo("For the 4 +$12 · Juan's dollar -$1"));
            Assert.That(CrapsTable.PayoutLine(new SettledBet(BetKind.Right, 20, BetOutcome.Won, 2, 18), 0),
                Is.EqualTo("Right +$20 · Juan's dollars -$2"));
            Assert.That(CrapsTable.PayoutLine(new SettledBet(BetKind.Right, 5, BetOutcome.Won, 0, 5), 0), Is.EqualTo("Right +$5"));
            Assert.That(CrapsTable.PayoutLine(new SettledBet(BetKind.PointAgainst, 6, BetOutcome.Lost, 0, -6), 8), Is.EqualTo("Against the 8 -$6"));
            Assert.That(CrapsTable.PayoutLine(new SettledBet(BetKind.Wrong, 3, BetOutcome.Push, 0, 0), 0), Is.EqualTo("Wrong pushes · $3 back"));
        }

        /// <summary>
        /// On a painted ground, the dice sit in the painted chalk circle, not the drawn ring, and
        /// a throw sends them up off the painted wall and back to rest there.
        /// </summary>
        [Test]
        public void OnPaintedArt_TheDiceSitInThePaintedCircle()
        {
            var state = new PlayerState();
            var table = MakeTable(state);
            var ring = table.GetComponentsInChildren<Image>(true).First(i => i.name == "Chalk");
            Assert.That(ring.enabled, Is.True, "no art: the table draws its own ring");

            var circle = new Rect(0.1f, 0.5f, 0.8f, 0.12f);
            table.SetCircleArt(new CircleArt(PlainSprite(), circle, 0.72f), null);

            Assert.That(ring.enabled, Is.False, "the art has its own chalk");
            var dice = table.GetComponentsInChildren<RectTransform>(true).First(t => t.name == "Dice");
            Assert.That(dice.parent.name, Is.EqualTo("Ground"), "the dice lie on the plate as it's fitted");
            Assert.That(dice.anchorMin, Is.EqualTo(circle.center), "in the middle of the painted circle");

            Click(table, CrapsControls.Up(BetKind.Right) + " Button");
            Click(table, "Throw Button");
            Assert.That(Text(table, "Message"), Is.Not.Empty, "the throw plays as ever on the painted ground");
            Assert.That(dice.parent.name, Is.EqualTo("Ground"));

            table.SetCircleArt(null, null);
            Assert.That(ring.enabled, Is.True, "with the art taken away, the drawn ring is back");
            Assert.That(dice.parent.name, Is.EqualTo("Circle"));
        }

        [Test]
        public void CircleArt_IsMeasuredInFractionsOfItsPlate()
        {
            var plate = PlainSprite();
            Assert.That(() => new CircleArt(null, new Rect(0.1f, 0.5f, 0.8f, 0.1f), 0.7f), Throws.ArgumentNullException);
            Assert.That(() => new CircleArt(plate, new Rect(0.9f, 0.5f, 0.4f, 0.1f), 0.7f), Throws.InstanceOf<System.ArgumentOutOfRangeException>(), "its middle off the plate");
            Assert.That(new CircleArt(plate, new Rect(-0.06f, 0.07f, 0.44f, 0.37f), 0.66f).Circle.xMin, Is.EqualTo(-0.06f), "running off the edge is fine");
            Assert.That(() => new CircleArt(plate, new Rect(0.1f, 0.5f, 0.8f, 0.1f), 0.5f), Throws.InstanceOf<System.ArgumentOutOfRangeException>(), "the wall below the circle");
            Assert.That(new CircleArt(plate, new Rect(0.1f, 0.5f, 0.8f, 0.1f), 0.7f).WallFoot, Is.EqualTo(0.7f));
        }

        static Sprite PlainSprite()
        {
            var texture = new Texture2D(9, 16);
            return Sprite.Create(texture, new Rect(0, 0, 9, 16), new Vector2(0.5f, 0.5f));
        }

        /// <summary>A new night brings a new crowd, but only while you're out of the circle.</summary>
        [Test]
        public void TheRegulars_ChangeOnlyWhileYoureOutOfTheCircle()
        {
            var state = new PlayerState();
            var table = MakeTable(state);
            Assert.That(() => table.SetRegulars(new Regular("Chuy"), new Regular("Nick"), new Regular("Teddy")),
                Throws.InvalidOperationException, "not while you're sitting");

            Click(table, "Leave game Button");
            Assert.That(() => table.SetRegulars(new Regular("Chuy"), new Regular("Nick")), Throws.ArgumentException);
            table.SetRegulars(new Regular("Chuy"), new Regular("Nick"), new Regular("Teddy"));

            Assert.That(SeatTexts(table, 1), Does.Contain("Chuy").And.Contain("C"), "a regular with no face shows their initial");
            Assert.That(SeatTexts(table, 3), Does.Contain("Teddy").And.No.Contain("Cook"));
            table.SitDown(state, new ClockPace());
            Assert.That(SeatTexts(table, 2), Does.Contain("Nick"), "the new crowd is there when you step back in");
        }

        static string[] SeatTexts(CrapsTable table, int seat) =>
            table.GetComponentsInChildren<Transform>(true).First(t => t.name == "Seat " + seat)
                .GetComponentsInChildren<TMP_Text>(true).Select(t => t.text).ToArray();

        CrapsTable MakeTable(PlayerState state, PracticeTurn practice = null)
        {
            var go = new GameObject("Craps");
            _made.Add(go);
            var table = go.AddComponent<CrapsTable>();
            table.Init(new CrapsRules(), null, 0.5f, new Regular("Bowler"), new Regular("Cabbie"), new Regular("Cook"));
            table.SitDown(state, new ClockPace(), practice);
            return table;
        }

        /// <summary>Juan points at one control, you press it, and he locks the table again while he talks.</summary>
        static void Coach(CrapsTable table, string id)
        {
            table.Lock("");
            table.Lock(id);
            foreach (var other in CrapsControls.All.Where(o => o != id))
                Assert.That(table.Control(other).GetComponent<Button>().IsInteractable(), Is.False, $"{other} is locked while Juan points at {id}");
            Click(table, table.Control(id).name);
            table.Lock("");
        }

        static string Message(CrapsTable table) => Text(table, "Message");

        static string Text(CrapsTable table, string name) =>
            table.GetComponentsInChildren<TMP_Text>(true).First(t => t.name == name).text;

        static Button Find(Component on, string name) => on.GetComponentsInChildren<Button>(true).First(b => b.name == name);

        /// <summary>A tap: it only lands on a button that's on screen and usable, as on a phone.</summary>
        static void Click(Component on, string name)
        {
            var button = Find(on, name);
            if (button.IsInteractable() && button.gameObject.activeInHierarchy) button.onClick.Invoke();
        }
    }
}
