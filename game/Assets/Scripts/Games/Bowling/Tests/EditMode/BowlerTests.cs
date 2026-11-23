using System;
using NUnit.Framework;
using Starflare.Bowling;

namespace Starflare.Tests
{
    /// <summary>The regulars as bowlers (bowling spec, "The regulars").</summary>
    public class BowlerTests
    {
        /// <summary>A game of ten frames on a freshly oiled house shot.</summary>
        internal static int Game(Bowler bowler, int seed)
        {
            var lane = new LaneState(new LaneOil(LanePattern.House), Hand.Right, seed);
            var rng = new Random(seed);
            var sheet = new ScoreSheet();
            while (!sheet.IsComplete)
            {
                if (sheet.FreshRack) lane.Rack();
                sheet.Record(bowler.BowlAt(lane, rng).Count);
            }
            return sheet.Score;
        }

        static double Average(int average, int games)
        {
            double total = 0;
            for (int seed = 0; seed < games; seed++)
                total += Game(Bowler.WithAverage("Regular", "", average), seed);
            return total / games;
        }

        [Test]
        public void A185Bowler_Bowls185()
        {
            Assert.That(Average(185, 200), Is.InRange(175.0, 195.0));
        }

        [Test]
        public void A135Bowler_Bowls135()
        {
            Assert.That(Average(135, 200), Is.InRange(125.0, 145.0));
        }

        [Test]
        public void ABetterBowler_StopsTheMetersCloser()
        {
            var good = Bowler.WithAverage("Chuy", "", 185).Spread;
            var fair = Bowler.WithAverage("Regular", "", 135).Spread;
            Assert.That(good.aimBoards, Is.LessThan(fair.aimBoards));
            Assert.That(good.percent, Is.LessThan(fair.percent));
        }

        [Test]
        public void AMissHigh_MovesHisStandLeft_AndALightMissRight()
        {
            var bowler = Bowler.WithAverage("Chuy", "", 185);
            int stand = bowler.Stand;
            var oil = new LaneOil(LanePattern.House);

            var high = BallSim.Roll(new Throw(bowler.Stand, bowler.Line.Aim + 2, bowler.Line.Hook, bowler.Line.Speed), oil, bowler.Ball);
            Assume.That(high.EntryBoard, Is.GreaterThanOrEqualTo(Bowler.Pocket + Bowler.MissBoards));
            bowler.Adjust(high);
            Assert.That(bowler.Stand, Is.EqualTo(stand + 1));

            var light = BallSim.Roll(new Throw(bowler.Stand, bowler.Line.Aim - 2, bowler.Line.Hook, bowler.Line.Speed), oil, bowler.Ball);
            Assume.That(light.EntryBoard, Is.LessThanOrEqualTo(Bowler.Pocket - Bowler.MissBoards));
            bowler.Adjust(light);
            bowler.Adjust(light);
            Assert.That(bowler.Stand, Is.EqualTo(stand - 1));

            var pocket = BallSim.Roll(bowler.Line, oil, bowler.Ball);
            int before = bowler.Stand;
            bowler.Adjust(pocket);
            Assert.That(bowler.Stand, Is.EqualTo(before), "In the pocket: stay.");
        }

        [Test]
        public void ASingleSpare_IsShotStraight_AtThePin()
        {
            var ten = BallSim.Roll(Bowler.SpareLine(PinDeck.Leave(10)), new LaneOil(LanePattern.House), BowlingBall.House);
            Assert.That(ten.Throw.Hook, Is.EqualTo(0));
            Assert.That(ten.Throw.Stand, Is.GreaterThan(20), "From across the lane.");
            Assert.That(ten.EntryBoard, Is.EqualTo(LaneGeometry.Board(PinDeck.Spots[9].X)).Within(1.5));
            Assert.That(PinSim.Hit(ten, PinDeck.Leave(10)).Fell[9], Is.True);
        }

        [Test]
        public void ARegularsSpareLine_IsTheMirror_ForALeftHander()
        {
            var right = Bowler.SpareLine(PinDeck.Leave(10), Hand.Right);
            var left = Bowler.SpareLine(PinDeck.Leave(7), Hand.Left);
            Assert.That(left.Stand, Is.EqualTo(right.Stand));
            Assert.That(left.Aim, Is.EqualTo(right.Aim).Within(1e-9));
        }
    }
}
