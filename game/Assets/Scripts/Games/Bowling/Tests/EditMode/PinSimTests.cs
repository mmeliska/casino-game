using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Starflare.Bowling;

namespace Starflare.Tests
{
    /// <summary>The pin deck and the fall rule (bowling spec, "The pins").</summary>
    public class PinSimTests
    {
        /// <summary>A ball 1 ft short of the head pin, on a line through a board at 60 ft.</summary>
        internal static (Vec2 at, Vec2 velocity) Entry(double board, double angleDegrees, double mph)
        {
            double a = angleDegrees * Math.PI / 180, v = mph * BallSim.MetresPerSecondPerMph;
            var target = LaneGeometry.Point(board, LaneGeometry.HeadPinFt);
            double back = LaneGeometry.Y(1);
            return (new Vec2(target.X + Math.Tan(a) * back, target.Y - back), new Vec2(-Math.Sin(a) * v, Math.Cos(a) * v));
        }

        static List<PinDeck> Leaves(double board, double angle, double mph, int racks)
        {
            var (at, velocity) = Entry(board, angle, mph);
            var leaves = new List<PinDeck>(racks);
            for (int seed = 0; seed < racks; seed++)
            {
                var deck = PinDeck.Rack(new Random(seed));
                leaves.Add(deck.After(PinSim.Hit(at, velocity, BowlingBall.Urethane, deck)));
            }
            return leaves;
        }

        [Test]
        public void APocketHit_At5Degrees_StrikesAboutHalfTheTime()
        {
            var leaves = Leaves(17.5, 5, 16, 500);
            double strikes = leaves.Count(d => d.StandingCount == 0) / 500.0;
            Assert.That(strikes, Is.InRange(0.45, 0.75));
        }

        [Test]
        public void FlushOnTheHeadPin_LeavesSplits()
        {
            var leaves = Leaves(20, 2, 16, 500);
            Assert.That(leaves.Count(d => d.StandingCount >= 2) / 500.0, Is.GreaterThanOrEqualTo(0.6));
        }

        [Test]
        public void LightInThePocket_LeavesThe10()
        {
            var leaves = Leaves(16, 5, 16, 300).Where(d => d.StandingCount > 0).ToList();
            var commonest = leaves.GroupBy(d => string.Join("-", d.StandingPins())).OrderByDescending(g => g.Count()).First();
            Assert.That(commonest.Key, Is.EqualTo("10"));
        }

        [Test]
        public void HighOnTheHeadPin_LeavesThe4OrThe7()
        {
            var leaves = Leaves(19, 4, 16, 300).Where(d => d.StandingCount > 0).ToList();
            int leftSide = leaves.Count(d => d.StandingPins().Contains(4) || d.StandingPins().Contains(7));
            Assert.That(leftSide, Is.GreaterThan(leaves.Count / 2));
        }

        [Test]
        public void A10PinBall_TakesJustThe10()
        {
            var leaves = Leaves(2.5, -2, 16, 200);
            int justThe10 = leaves.Count(d => d.StandingCount == 9 && !d.StandingPins().Contains(10));
            Assert.That(justThe10, Is.GreaterThan(150));
        }

        [Test]
        public void ATapUnderTheThreshold_LeavesThePinStanding()
        {
            // A ball clipping the 7 pin as it rolls past: the impulse is (1 + e) times the closing
            // speed along the line of centres times the reduced mass.
            var ball = BowlingBall.Urethane;
            double reduced = ball.Mass * PinDeck.PinMass / (ball.Mass + PinDeck.PinMass);
            const double speed = 3;
            double closing = 1.0 / ((1 + PinSim.BallPinRestitution) * reduced);
            double reach = ball.Radius + PinDeck.PinRadius;
            double offset = reach * Math.Sqrt(1 - closing / speed * (closing / speed));
            var seven = PinDeck.Spots[6];

            var tap = PinSim.Hit(new Vec2(seven.X + offset, seven.Y - 0.3), new Vec2(0, speed), ball, PinDeck.Leave(7));
            Assert.That(tap.Fell[6], Is.False, "A 1.0 kg·m/s tap wobbles it.");

            var flush = PinSim.Hit(new Vec2(seven.X, seven.Y - 0.3), new Vec2(0, speed), ball, PinDeck.Leave(7));
            Assert.That(flush.Fell[6], Is.True);
        }

        [Test]
        public void APinThatStands_RocksInPlace_AndOnlyRarelySkids()
        {
            // Pocket, light, high and Brooklyn hits: whatever's left standing has barely moved. A
            // pin bumped more than once can walk an inch or two; one sent sliding went down, but
            // for the rare one that skids a few inches and stays up.
            double inch = LaneGeometry.MetresPerInch, worst = 0;
            int standing = 0, walked = 0, skidded = 0, racks = 0;
            foreach (double board in new[] { 3.0, 8, 14, 16, 17.5, 19, 20, 21 })
                foreach (double angle in new[] { 2.0, 4, 6 })
                {
                    var (at, velocity) = Entry(board, angle, 17);
                    for (int seed = 0; seed < 400; seed++)
                    {
                        var deck = PinDeck.Rack(new Random(seed));
                        var result = PinSim.Hit(at, velocity, BowlingBall.Urethane, deck);
                        racks++;
                        for (int i = 0; i < PinDeck.Pins; i++)
                        {
                            if (result.Fell[i]) continue;
                            standing++;
                            double moved = (result.FinalPositions[i] - deck.PositionOf(i)).Length;
                            worst = Math.Max(worst, moved);
                            if (moved > inch) walked++;
                            if (moved > 3 * inch) skidded++;
                        }
                    }
                }
            Assert.That(standing, Is.GreaterThan(5000), "Plenty of leaves to look at.");
            Assert.That(worst, Is.LessThan(6 * inch), $"A standing pin went {worst / inch:F1} in.");
            Assert.That(skidded, Is.GreaterThan(0), "It happens.");
            Assert.That(skidded / (double)racks, Is.InRange(0.0005, 0.002), "But about one ball in a thousand.");
            Assert.That(walked / (double)standing, Is.LessThan(0.02), "Almost every one stays within an inch.");
        }

        [Test]
        public void AGutterBall_TouchesNothing()
        {
            var path = BallSim.Roll(new Throw(9, 5, 0, 16), new LaneOil(LanePattern.House), BowlingBall.House);
            Assert.That(path.Gutter, Is.True);
            var result = PinSim.Hit(path, PinDeck.Exact());
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public void TheSameBall_IntoTheSameRack_FellsTheSamePins()
        {
            var (at, velocity) = Entry(17.5, 5, 16);
            var a = PinSim.Hit(at, velocity, BowlingBall.Urethane, PinDeck.Rack(new Random(7)));
            var b = PinSim.Hit(at, velocity, BowlingBall.Urethane, PinDeck.Rack(new Random(7)));
            Assert.That(b.Fell, Is.EqualTo(a.Fell));
            Assert.That(b.FinalPositions, Is.EqualTo(a.FinalPositions));
        }

        [Test]
        public void TheRack_IsWithinAnEighthOfAnInch_OfTheSpots()
        {
            for (int seed = 0; seed < 50; seed++)
            {
                var deck = PinDeck.Rack(new Random(seed));
                for (int i = 0; i < PinDeck.Pins; i++)
                    Assert.That((deck.PositionOf(i) - PinDeck.Spots[i]).Length, Is.LessThanOrEqualTo(PinDeck.Jitter + 1e-12));
            }
        }

        [Test]
        public void After_SweepsTheFallen_AndKeepsTheStanding()
        {
            var deck = PinDeck.Exact();
            var (at, velocity) = Entry(16, 5, 16);
            var result = PinSim.Hit(at, velocity, BowlingBall.Urethane, deck);
            var next = deck.After(result);
            Assert.That(next.StandingCount, Is.EqualTo(10 - result.Count));
            for (int i = 0; i < PinDeck.Pins; i++)
                Assert.That(next.IsStanding(i), Is.EqualTo(!result.Fell[i]));
        }

        [Test]
        public void TheHeadPin_IsOnTheMiddleBoard_AndThe10_OnARightHandersSide()
        {
            Assert.That(LaneGeometry.Board(PinDeck.Spots[0].X), Is.EqualTo(20).Within(1e-9));
            Assert.That(PinDeck.Spots[0].Y, Is.EqualTo(LaneGeometry.Y(60)).Within(1e-9));
            Assert.That(LaneGeometry.Board(PinDeck.Spots[9].X), Is.LessThan(5), "The 10 pin is near board 1.");
            Assert.That(LaneGeometry.Board(PinDeck.Spots[6].X), Is.GreaterThan(35), "The 7 pin is near board 39.");
        }
    }
}
