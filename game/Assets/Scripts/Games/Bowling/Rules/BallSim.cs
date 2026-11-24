using System;
using System.Collections.Generic;

namespace Starflare.Bowling
{
    /// <summary>
    /// The ball down the lane: a rigid sphere sliding and rolling on the oil, one millisecond at a
    /// time. One rule, friction against the contact point's slip, makes all three shapes bowlers
    /// name (straight; straight then a snap where the oil ends; the long arc) with no special
    /// cases. Same arithmetic everywhere, so the same throw on the same lane is the same path.
    /// </summary>
    public static class BallSim
    {
        public const double Step = 0.001;
        public const int SampleEvery = 10;
        public const double Gravity = 9.81;
        public const double MetresPerSecondPerMph = 0.44704;

        /// <summary>The hook dial at 100: the rolling axis turned this far toward the lane's length.</summary>
        public const double MaxAxisDegrees = 45;

        /// <summary>
        /// The bottom of the hook dial throws a straight ball: a sliver of axis near nothing moves a
        /// ball boards by the pins, so the first tenth of the meter is "nothing on it".
        /// </summary>
        public const double StraightBelow = 0.1;

        /// <summary>What slows a rolling ball, m/s².</summary>
        public const double RollingResistance = 0.25;

        /// <summary>Where the lane hands the ball to the pins: just short of the head pin.</summary>
        public const double HandoffFt = 59;

        /// <summary>No throw runs longer than this; a ball that's stopped is done.</summary>
        public const double MaxSeconds = 10;

        /// <summary>
        /// Rolls a throw down a lane. Boards are counted from the bowler's own gutter; the oil and
        /// the path are in the lane's frame, so a left-hander's ball crosses the lane mirrored.
        /// </summary>
        public static BallPath Roll(Throw t, LaneOil oil, BowlingBall ball, Hand hand = Hand.Right)
        {
            if (oil == null) throw new ArgumentNullException(nameof(oil));
            if (ball == null) throw new ArgumentNullException(nameof(ball));
            t = t.Clamp();
            double mirror = hand == Hand.Right ? 1 : -1;

            // The bowler's frame: x positive toward their own gutter side, board 1.
            var p = LaneGeometry.Point(t.Stand, 0);
            var aim = LaneGeometry.Point(t.Aim, LaneGeometry.ArrowFt(t.Aim));
            var dir = (aim - p).Normalized;
            var v = dir * (t.Speed * MetresPerSecondPerMph);
            var w = Spin(dir, t.Speed * MetresPerSecondPerMph, ball.Radius, t.Hook / 100);

            double r = ball.Radius;
            var lever = new Vec3(0, 0, -r);
            double reach = 1 / ball.Mass + r * r / ball.Inertia; // slip change per unit impulse
            double handoff = LaneGeometry.Y(HandoffFt);

            var samples = new List<BallSample>(300);
            bool rolling = false, gutter = false;
            double rolledFt = double.PositiveInfinity;
            double time = 0;
            int step = 0;
            double curvature = 0;

            while (true)
            {
                var slip = v + Vec3.Cross(w, lever).Flat;
                double slipSize = slip.Length;
                var lanePoint = new Vec2(p.X * mirror, p.Y);

                if (step % SampleEvery == 0)
                    samples.Add(new BallSample(time, lanePoint, new Vec2(v.X * mirror, v.Y), rolling ? 0 : slipSize, curvature));

                if (p.Y >= handoff || !LaneGeometry.OnLane(p.X) || time >= MaxSeconds || v.Y <= 0)
                {
                    gutter = !LaneGeometry.OnLane(p.X);
                    if (step % SampleEvery != 0)
                        samples.Add(new BallSample(time, lanePoint, new Vec2(v.X * mirror, v.Y), rolling ? 0 : slipSize, curvature));
                    break;
                }

                if (rolling)
                {
                    double speed = v.Length;
                    v = speed > RollingResistance * Step ? v * (1 - RollingResistance * Step / speed) : Vec2.Zero;
                    w = RollingSpin(v, r, w.Z);
                    curvature = 0;
                }
                else
                {
                    double mu = oil.FrictionAt(lanePoint, ball);
                    double impulse = mu * ball.Mass * Gravity * Step;
                    if (slipSize <= impulse * reach)
                    {
                        // The slip runs out inside this step: from here it rolls.
                        v -= slip * (1 / (ball.Mass * reach));
                        w = RollingSpin(v, r, w.Z);
                        rolling = true;
                        rolledFt = LaneGeometry.Feet(p.Y);
                        curvature = 0;
                    }
                    else
                    {
                        var force = slip * (-mu * ball.Mass * Gravity / slipSize);
                        var accel = force / ball.Mass;
                        var torque = Vec3.Cross(lever, new Vec3(force.X, force.Y, 0));
                        v += accel * Step;
                        w += torque * (Step / ball.Inertia);
                        double speed = v.Length;
                        // Toward the bowler's off side (negative x) is a hook: positive curvature.
                        curvature = speed > 0 ? -Vec2.Cross(accel, v) / (speed * speed * speed) : 0;
                    }
                }

                p += v * Step;
                time += Step;
                step++;
            }

            return new BallPath(t, hand, ball, samples, gutter, rolledFt);
        }

        /// <summary>
        /// The spin off the hand for a hook dial h in 0..1. At 0 the ball leaves the hand rolling
        /// end over end, its spin matching its speed, so it never slides sideways and never turns.
        /// Past the first tenth, turning the dial turns that axis toward the lane's length, up to <see cref="MaxAxisDegrees"/>,
        /// so the contact point slides toward the bowler's gutter side and friction pulls the ball
        /// back across: the revs and the axis are one dial.
        /// </summary>
        public static Vec3 Spin(Vec2 dir, double speed, double radius, double h)
        {
            double revs = speed / radius;
            double turned = Math.Max(0, (h - StraightBelow) / (1 - StraightBelow));
            double axis = MaxAxisDegrees * turned * Math.PI / 180;
            double c = Math.Cos(axis), s = Math.Sin(axis);
            // End over end is (-dir.Y, dir.X); turned toward the bowler, -dir.
            return new Vec3(-dir.Y * c - dir.X * s, dir.X * c - dir.Y * s, 0) * revs;
        }

        /// <summary>The spin that rolls a ball without slip at this velocity.</summary>
        static Vec3 RollingSpin(Vec2 v, double r, double spinZ) => new Vec3(-v.Y / r, v.X / r, spinZ);
    }
}
