using NUnit.Framework;
using Starflare.Bowling;

namespace Starflare.Tests
{
    /// <summary>Millstone's lanes as the bowling spec measures them ("The lane").</summary>
    public class LaneTests
    {
        [Test]
        public void Board20_IsTheMiddle_AndBoard1_IsARightHandersRight()
        {
            Assert.That(LaneGeometry.X(20), Is.EqualTo(0).Within(1e-12));
            Assert.That(LaneGeometry.X(1), Is.GreaterThan(0));
            Assert.That(LaneGeometry.X(39), Is.EqualTo(-LaneGeometry.X(1)).Within(1e-12));
        }

        [Test]
        public void TheArrows_SitInAV_TheSecondOnBoard10At14Feet()
        {
            Assert.That(LaneGeometry.Arrows[1], Is.EqualTo(10));
            Assert.That(LaneGeometry.ArrowFt(10), Is.EqualTo(14));
            Assert.That(LaneGeometry.ArrowFt(20), Is.EqualTo(16));
            Assert.That(LaneGeometry.ArrowFt(5), Is.EqualTo(LaneGeometry.ArrowFt(35)));
            foreach (int board in LaneGeometry.Arrows)
                Assert.That(LaneGeometry.ArrowFt(board), Is.InRange(12.0, 16.0), "USBC: 12 to 16 ft");
        }

        [TestCase(1.0)]
        [TestCase(10.5)]
        [TestCase(39.0)]
        public void BoardsAndMetres_RoundTrip(double board)
        {
            Assert.That(LaneGeometry.Board(LaneGeometry.X(board)), Is.EqualTo(board).Within(1e-9));
            Assert.That(LaneGeometry.Feet(LaneGeometry.Y(42)), Is.EqualTo(42).Within(1e-9));
        }

        [Test]
        public void TheHouseShot_IsHeavierInTheMiddle_AndEndsAt40Feet()
        {
            var oil = new LaneOil(LanePattern.House);
            Assert.That(oil.UnitsAt(20, 20), Is.GreaterThan(oil.UnitsAt(5, 20)));
            Assert.That(oil.UnitsAt(20, 20) / oil.UnitsAt(5, 20), Is.GreaterThan(6), "About 8 to 1, middle to outside.");
            for (int board = 1; board <= LaneOil.Boards; board++)
                for (int foot = 40; foot < LaneOil.Feet; foot++)
                    Assert.That(oil.UnitsAt(board, foot), Is.EqualTo(0), $"Board {board} at {foot} ft");
        }

        [Test]
        public void Oil_IsSlick_AndTheLanePastIt_Grips()
        {
            var ball = BowlingBall.Urethane;
            var oil = new LaneOil(LanePattern.House);
            double onOil = oil.FrictionAt(LaneGeometry.Point(20, 20), ball);
            double outside = oil.FrictionAt(LaneGeometry.Point(3, 20), ball);
            double past = oil.FrictionAt(LaneGeometry.Point(20, 45), ball);
            Assert.That(onOil, Is.LessThan(outside));
            Assert.That(outside, Is.LessThan(past));
            Assert.That(past, Is.EqualTo(LaneOil.BareFriction).Within(1e-12));
        }

        [Test]
        public void ABall_PicksUpOilWhereItRolls_AndCarriesSomePastThePattern()
        {
            var fresh = new LaneOil(LanePattern.House);
            var oil = fresh.Copy();
            var path = BallSim.Roll(new Throw(10, 10, 0, 17), oil, BowlingBall.Urethane);
            oil.Pass(path);

            Assert.That(oil.UnitsAt(10, 20), Is.LessThan(fresh.UnitsAt(10, 20)), "Picked up on its line.");
            Assert.That(oil.UnitsAt(30, 20), Is.EqualTo(fresh.UnitsAt(30, 20)), "Untouched off its line.");
            Assert.That(oil.UnitsAt(10, 41), Is.GreaterThan(0), "Carried past 40 ft on its line.");
            Assert.That(oil.UnitsAt(30, 41), Is.EqualTo(0));
        }

        [Test]
        public void AThousandBalls_WearTheLeaguesTrack_AndLeaveTheOtherSideAlone()
        {
            var fresh = new LaneOil(LanePattern.House);
            var worn = LaneOil.Worn(LanePattern.House, 1000);
            for (int board = 8; board <= 12; board++)
                Assert.That(worn.UnitsAt(board, 20), Is.LessThan(fresh.UnitsAt(board, 20) * 0.8), $"Board {board} thin");
            for (int foot = 0; foot < LaneOil.Feet; foot++)
                Assert.That(worn.UnitsAt(35, foot), Is.EqualTo(fresh.UnitsAt(35, foot)), $"Board 35 at {foot} ft");
            Assert.That(worn.UnitsAt(10, 42), Is.GreaterThan(0), "A faint stripe past the pattern.");
        }

        [Test]
        public void ALeftHandersWear_MirrorsARightHanders()
        {
            var right = LaneOil.Worn(LanePattern.House, 1000, Hand.Right);
            var left = LaneOil.Worn(LanePattern.House, 1000, Hand.Left);
            for (int board = 1; board <= LaneOil.Boards; board++)
                for (int foot = 0; foot < LaneOil.Feet; foot += 3)
                    Assert.That(left.UnitsAt(LaneOil.Boards + 1 - board, foot), Is.EqualTo(right.UnitsAt(board, foot)).Within(1e-12));
        }

        [Test]
        public void LaneSix_IsShorterAndLighter()
        {
            var house = new LaneOil(LanePattern.House);
            var six = new LaneOil(LanePattern.LaneSix);
            Assert.That(six.PatternEndFt, Is.LessThan(house.PatternEndFt));
            Assert.That(six.UnitsAt(20, 10), Is.LessThan(house.UnitsAt(20, 10)));
            Assert.That(six.UnitsAt(10, 36), Is.EqualTo(0));
        }
    }
}
