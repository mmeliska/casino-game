using System;
using System.Collections.Generic;

namespace Starflare.Bowling
{
    /// <summary>
    /// What a ball did on the lane: the samples the trace draws (where it skidded, where it broke,
    /// where it hit) and how it arrived at the pins, or the foot where it found the gutter.
    /// </summary>
    public sealed class BallPath
    {
        public readonly Throw Throw;
        public readonly Hand Hand;
        public readonly BowlingBall Ball;
        public readonly IReadOnlyList<BallSample> Samples;

        /// <summary>It left the lane before the pins: nothing falls.</summary>
        public readonly bool Gutter;

        /// <summary>The last sample: at the pins, or where it went off the lane.</summary>
        public BallSample End => Samples[Samples.Count - 1];

        /// <summary>The foot where the ball stopped sliding and rolled; past the pins if it never did.</summary>
        public readonly double RolledFt;

        public BallPath(Throw t, Hand hand, BowlingBall ball, IReadOnlyList<BallSample> samples, bool gutter, double rolledFt)
        {
            Throw = t;
            Hand = hand;
            Ball = ball;
            Samples = samples ?? throw new ArgumentNullException(nameof(samples));
            if (samples.Count == 0) throw new ArgumentException("A path has at least one sample.", nameof(samples));
            Gutter = gutter;
            RolledFt = rolledFt;
        }

        /// <summary>A lane position's board as this bowler counts it, from their own gutter.</summary>
        public double BoardOf(Vec2 at)
        {
            double board = LaneGeometry.Board(at.X);
            return Hand == Hand.Right ? board : LaneGeometry.Mirror(board);
        }

        /// <summary>The board (the bowler's count) where the path crosses a foot down the lane.</summary>
        public double BoardAt(double feet)
        {
            double y = LaneGeometry.Y(feet);
            for (int i = 1; i < Samples.Count; i++)
            {
                var a = Samples[i - 1].Position;
                var b = Samples[i].Position;
                if (b.Y < y) continue;
                double t = b.Y > a.Y ? (y - a.Y) / (b.Y - a.Y) : 0;
                return BoardOf(a + (b - a) * t);
            }
            return BoardOf(End.Position);
        }

        /// <summary>
        /// The board its line crosses the head pin's row on, the bowler's count: 17.5 is a
        /// right-hander's pocket, between the 1 and the 3.
        /// </summary>
        public double EntryBoard
        {
            get
            {
                var end = End;
                double toHeadPin = LaneGeometry.Y(LaneGeometry.HeadPinFt) - end.Position.Y;
                if (end.Velocity.Y <= 0) return BoardOf(end.Position);
                return BoardOf(end.Position + end.Velocity * (toHeadPin / end.Velocity.Y));
            }
        }

        /// <summary>
        /// The angle into the pins in degrees, positive when it's moving toward the bowler's off
        /// side (into a right-hander's 1–3 pocket from the right).
        /// </summary>
        public double EntryAngle => Heading(End);

        /// <summary>Miles an hour at the end of the path.</summary>
        public double EntrySpeed => End.Velocity.Length / BallSim.MetresPerSecondPerMph;

        /// <summary>The foot where the path bent hardest: the break point.</summary>
        public double BreakFt
        {
            get
            {
                double best = 0, at = 0;
                foreach (var s in Samples)
                    if (s.Curvature > best) { best = s.Curvature; at = s.Feet; }
                return at;
            }
        }

        /// <summary>How far the ball turned over the whole path, in degrees toward the off side.</summary>
        public double Turn => Heading(End) - Heading(Samples[0]);

        /// <summary>
        /// The foot by which this share of the turn was done: 0.5 is where the hook is half spent,
        /// early for a ball that arcs, late for one that skids and snaps.
        /// </summary>
        public double TurnDoneBy(double share)
        {
            double start = Heading(Samples[0]), total = Turn;
            if (Math.Abs(total) < 1e-9) return 0;
            foreach (var s in Samples)
                if ((Heading(s) - start) / total >= share) return s.Feet;
            return End.Feet;
        }

        /// <summary>The share of the turn done by a foot down the lane.</summary>
        public double TurnShareBy(double feet)
        {
            double start = Heading(Samples[0]), total = Turn, done = 0;
            if (Math.Abs(total) < 1e-9) return 0;
            foreach (var s in Samples)
                if (s.Feet <= feet) done = Heading(s) - start;
            return done / total;
        }

        /// <summary>The direction of travel in degrees, positive toward the bowler's off side.</summary>
        double Heading(BallSample s)
        {
            double across = Hand == Hand.Right ? -s.Velocity.X : s.Velocity.X;
            return Math.Atan2(across, s.Velocity.Y) * 180 / Math.PI;
        }

        /// <summary>The same path seen from the other hand: mirrored across the middle board.</summary>
        public BallPath Mirrored()
        {
            var samples = new List<BallSample>(Samples.Count);
            foreach (var s in Samples)
                samples.Add(new BallSample(s.Time, new Vec2(-s.Position.X, s.Position.Y), new Vec2(-s.Velocity.X, s.Velocity.Y), s.Slip, s.Curvature, new Vec3(s.Spin.X, -s.Spin.Y, -s.Spin.Z)));
            return new BallPath(Throw, Hand == Hand.Right ? Hand.Left : Hand.Right, Ball, samples, Gutter, RolledFt);
        }
    }
}
