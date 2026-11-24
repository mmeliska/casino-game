using System;
using NUnit.Framework;
using Starflare.Bowling;

namespace Starflare.Tests
{
    /// <summary>
    /// The three shot shapes bowlers name, out of one friction rule with no special cases
    /// (bowling spec, "The ball"; Mike, 2026-11-23).
    /// </summary>
    public class BallSimTests
    {
        static BallPath Roll(Throw t, BowlingBall ball, LanePattern pattern = null, Hand hand = Hand.Right) =>
            BallSim.Roll(t, new LaneOil(pattern ?? LanePattern.House), ball, hand);

        /// <summary>Juan's first ball: seventh board, second arrow, nothing on it.</summary>
        static readonly Throw Straight = new Throw(7, 10, 0, 16);

        /// <summary>A reactive ball on the house shot: deep inside, swung out to the edge of the oil.</summary>
        static readonly Throw Snap = new Throw(32, 19, 75, 17);

        /// <summary>Medium hook, slow, an outside line on the light boards.</summary>
        static readonly Throw Arc = new Throw(15, 7, 50, 13);

        [Test]
        public void ANegativeHook_TurnsTheOtherWay_AndNearZeroRollsStraight()
        {
            var oil = new LaneOil(LanePattern.House);
            var straight = BallSim.Roll(new Throw(20, 20, 0, 15), oil, BowlingBall.Urethane);
            var usual = BallSim.Roll(new Throw(20, 20, 60, 15), oil, BowlingBall.Urethane);
            var other = BallSim.Roll(new Throw(20, 20, -60, 15), oil, BowlingBall.Urethane);
            Assert.That(usual.EntryBoard, Is.GreaterThan(straight.EntryBoard + 3), "The usual hook: toward the bowler's left, up the board count.");
            Assert.That(other.EntryBoard, Is.LessThan(straight.EntryBoard - 3), "The other way.");
            Assert.That(usual.EntryBoard - straight.EntryBoard, Is.EqualTo(straight.EntryBoard - other.EntryBoard).Within(0.5), "As far either way.");
            var nearlyLeft = BallSim.Roll(new Throw(20, 20, -8, 15), oil, BowlingBall.Urethane);
            Assert.That(nearlyLeft.EntryBoard, Is.EqualTo(straight.EntryBoard).Within(0.01), "Inside a tenth of 0 either way it's straight.");
        }

        [Test]
        public void HookZero_HoldsItsLine_ToTheHeadPin()
        {
            var path = Roll(Straight, BowlingBall.House);
            var start = LaneGeometry.Point(Straight.Stand, 0);
            var aim = LaneGeometry.Point(Straight.Aim, LaneGeometry.ArrowFt(Straight.Aim));
            foreach (var s in path.Samples)
            {
                double lineX = start.X + (aim.X - start.X) * (s.Position.Y - start.Y) / (aim.Y - start.Y);
                Assert.That(Math.Abs(s.Position.X - lineX), Is.LessThan(LaneGeometry.BoardWidth / 2), $"At {s.Feet:0.0} ft");
            }
            Assert.That(path.Gutter, Is.False);
            Assert.That(path.EntryBoard, Is.InRange(18.0, 20.0), "Flush on the head pin.");
            Assert.That(path.EntrySpeed, Is.GreaterThan(14), "A rolling ball keeps its speed.");
        }

        [Test]
        public void HookHigh_SkidsThroughTheOil_AndSnapsWhereItEnds()
        {
            var path = Roll(Snap, BowlingBall.Reactive);
            Assert.That(path.TurnShareBy(30), Is.LessThan(0.15), "Barely turns on the oil.");
            Assert.That(path.TurnShareBy(36), Is.LessThan(0.25), "Most of the turn is past the oil.");
            Assert.That(path.TurnDoneBy(0.5), Is.InRange(38.0, 48.0), "The break sits at the end of the oil.");
            Assert.That(path.RolledFt, Is.LessThan(55), "It rolls out of the turn before the pins.");
            Assert.That(path.EntryBoard, Is.GreaterThanOrEqualTo(path.BoardAt(40) + 4), "It comes back at least four boards.");
            Assert.That(path.EntryBoard, Is.InRange(16.5, 19.0), "Into the pocket.");
            Assert.That(path.EntryAngle, Is.InRange(4.0, 8.0));
        }

        [Test]
        public void HookMedium_SlowOnTheLightBoards_ArcsTheWholeWay()
        {
            var path = Roll(Arc, BowlingBall.Urethane);
            foreach (var s in path.Samples)
                if (s.Feet >= 5 && s.Slip > 0)
                    Assert.That(s.Curvature, Is.GreaterThan(0), $"Still turning at {s.Feet:0.0} ft");
            Assert.That(path.TurnShareBy(30), Is.GreaterThan(0.4), "Friction gets hold of it early.");
            Assert.That(BusiestFiveFeet(path), Is.LessThan(0.4), "No straight-then-snap.");
            Assert.That(path.RolledFt, Is.LessThan(45), "It's spent before the pins.");
        }

        [Test]
        public void LaneSix_BreaksEarlier_ThanTheHouseShot()
        {
            var house = Roll(Snap, BowlingBall.Reactive);
            var six = Roll(Snap, BowlingBall.Reactive, LanePattern.LaneSix);
            Assert.That(six.TurnDoneBy(0.5), Is.LessThanOrEqualTo(house.TurnDoneBy(0.5) - 4));
            Assert.That(six.RolledFt, Is.LessThanOrEqualTo(house.RolledFt - 4));
            Assert.That(six.EntryBoard, Is.GreaterThan(house.EntryBoard + 3), "High: move left.");
        }

        [Test]
        public void TheHouseBall_HooksLeast_AndTheReactiveMost_AtTheBreak()
        {
            var house = Roll(Snap, BowlingBall.House);
            var urethane = Roll(Snap, BowlingBall.Urethane);
            var reactive = Roll(Snap, BowlingBall.Reactive);
            Assert.That(house.Turn, Is.LessThan(urethane.Turn / 2), "Plastic barely grips.");
            Assert.That(reactive.TurnShareBy(36), Is.LessThan(urethane.TurnShareBy(36)), "The pearl skids longer.");
        }

        [Test]
        public void TheSameThrow_OnTheSameLane_IsTheSamePath()
        {
            var a = Roll(Snap, BowlingBall.Reactive);
            var b = Roll(Snap, BowlingBall.Reactive);
            Assert.That(b.Samples.Count, Is.EqualTo(a.Samples.Count));
            for (int i = 0; i < a.Samples.Count; i++)
            {
                Assert.That(b.Samples[i].Position, Is.EqualTo(a.Samples[i].Position));
                Assert.That(b.Samples[i].Velocity, Is.EqualTo(a.Samples[i].Velocity));
            }
        }

        [Test]
        public void ALeftHandedThrow_MirrorsTheRightHanded_BoardForBoard()
        {
            var right = BallSim.Roll(Snap, LaneOil.Worn(LanePattern.House, 600, Hand.Right), BowlingBall.Reactive, Hand.Right);
            var left = BallSim.Roll(Snap, LaneOil.Worn(LanePattern.House, 600, Hand.Left), BowlingBall.Reactive, Hand.Left);
            Assert.That(left.Samples.Count, Is.EqualTo(right.Samples.Count));
            for (int i = 0; i < right.Samples.Count; i++)
            {
                Assert.That(left.Samples[i].Position.X, Is.EqualTo(-right.Samples[i].Position.X).Within(1e-9));
                Assert.That(left.Samples[i].Position.Y, Is.EqualTo(right.Samples[i].Position.Y).Within(1e-9));
            }
            Assert.That(left.EntryBoard, Is.EqualTo(right.EntryBoard).Within(1e-6), "Counted from their own gutter.");
        }

        [Test]
        public void AWideLine_FindsTheGutter()
        {
            var path = Roll(new Throw(9, 5, 0, 16), BowlingBall.House);
            Assert.That(path.Gutter, Is.True);
            Assert.That(path.End.Feet, Is.LessThan(LaneGeometry.HeadPinFt));
        }

        [Test]
        public void Throws_AreClampedToTheMeters()
        {
            var t = new Throw(0, 50, 140, 30).Clamp();
            Assert.That(t, Is.EqualTo(new Throw(1, 35, 100, 20)));
        }

        /// <summary>The largest share of the turn inside any five feet.</summary>
        internal static double BusiestFiveFeet(BallPath path)
        {
            double best = 0, total = path.Turn;
            for (int i = 0; i < path.Samples.Count; i++)
                for (int j = i; j < path.Samples.Count && path.Samples[j].Feet - path.Samples[i].Feet <= 5; j++)
                    best = Math.Max(best, (path.TurnShareBy(path.Samples[j].Feet) - path.TurnShareBy(path.Samples[i].Feet)));
            return best;
        }
    }
}
