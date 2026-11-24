using System;
using NUnit.Framework;
using Starflare.Bowling;

namespace Starflare.Tests
{
    /// <summary>The drawn ball turns the way the sim's ball spins: forward over the top, at its speed once rolling.</summary>
    public class BallSpinTests
    {
        static readonly LaneOil Oil = new LaneOil(LanePattern.House);

        static double Angle(Turn from, Turn to)
        {
            var d = to * from.Inverse;
            return 2 * Math.Acos(Math.Min(1, Math.Abs(d.W)));
        }

        [Test]
        public void TheTopOfARollingBall_GoesForward()
        {
            var path = BallSim.Roll(new Throw(20, 20, 0, 16), Oil, BowlingBall.House);
            var spin = new BallSpin(path);
            double t = path.End.Time * 0.8;
            var onTop = spin.At(t).Inverse.Rotate(new Vec3(0, 0, 1));
            var later = spin.At(t + 0.02).Rotate(onTop);
            Assert.That(later.Y, Is.GreaterThan(0.05), "Over the top and away down the lane.");
        }

        [Test]
        public void ARollingBall_TurnsOnceEveryTimeItRollsItsOwnRoundness()
        {
            var path = BallSim.Roll(new Throw(20, 20, 0, 16), Oil, BowlingBall.House);
            var spin = new BallSpin(path);
            double t = path.End.Time * 0.85, dt = 0.04;
            Assert.That(path.RolledFt, Is.LessThan(LaneGeometry.Feet(path.End.Position.Y) * 0.85), "Rolling by now.");
            var a = Sample(path, t);
            double travelled = a.Velocity.Length * dt;
            Assert.That(Angle(spin.At(t), spin.At(t + dt)), Is.EqualTo(travelled / BowlingBall.House.Radius).Within(5).Percent);
        }

        [Test]
        public void ASkiddingBall_TurnsSlowerThanItTravels()
        {
            var path = BallSim.Roll(new Throw(32, 19, 75, 17), Oil, BowlingBall.Reactive);
            var first = path.Samples[2];
            Assert.That(first.Slip, Is.GreaterThan(0), "Still skidding.");
            Assert.That(first.Spin.Length, Is.LessThan(first.Velocity.Length / BowlingBall.Reactive.Radius));
        }

        [Test]
        public void AHook_TurnsTheAxis_AStraightBallDoesnt()
        {
            var straight = BallSim.Roll(new Throw(20, 20, 0, 16), Oil, BowlingBall.Urethane).Samples[1];
            var hook = BallSim.Roll(new Throw(20, 20, 60, 16), Oil, BowlingBall.Urethane).Samples[1];
            Assert.That(Math.Abs(straight.Spin.Y), Is.LessThan(0.01 * straight.Spin.Length));
            Assert.That(Math.Abs(hook.Spin.Y), Is.GreaterThan(0.2 * hook.Spin.Length));
        }

        [Test]
        public void ALeftHandersBall_SpinsAsTheMirror()
        {
            var right = BallSim.Roll(new Throw(20, 20, 60, 16), Oil, BowlingBall.Urethane, Hand.Right).Samples[1].Spin;
            var left = BallSim.Roll(new Throw(20, 20, 60, 16), Oil.Copy(), BowlingBall.Urethane, Hand.Left).Samples[1].Spin;
            Assert.That(left.X, Is.EqualTo(right.X).Within(1e-6));
            Assert.That(left.Y, Is.EqualTo(-right.Y).Within(1e-6));
            Assert.That(left.Z, Is.EqualTo(-right.Z).Within(1e-6));
        }

        static BallSample Sample(BallPath path, double t)
        {
            foreach (var s in path.Samples)
                if (s.Time >= t) return s;
            return path.End;
        }
    }
}
