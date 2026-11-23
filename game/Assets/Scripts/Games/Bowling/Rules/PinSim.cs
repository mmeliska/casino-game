using System;
using System.Collections.Generic;

namespace Starflare.Bowling
{
    /// <summary>
    /// The hit, seen from above: the ball and the pins as circles trading impulses, one
    /// millisecond at a time, the same arithmetic everywhere. A flat sim can't tip a pin, so a
    /// rule does: a standing pin that takes more than <see cref="FallImpulse"/> is down. Less, and
    /// it wobbles and stands, which is the ringing 10 pin and the stone 8. Down pins slide on with
    /// heavy drag and can fell others by the same rule.
    /// </summary>
    public static class PinSim
    {
        public const double BallPinRestitution = 0.4;
        public const double PinPinRestitution = 0.6;
        public const double KickbackRestitution = 0.3;

        /// <summary>The impulse that puts a standing pin down, kg·m/s.</summary>
        public const double FallImpulse = 3;

        /// <summary>
        /// The pin action's luck: each pin's threshold varies by up to this share either way (a pin
        /// caught high or low), and each impulse's direction by about this many degrees (a pin that
        /// tilts and turns as it's hit). Seeded by the rack.
        /// </summary>
        public const double FallSpread = 0.3, ScatterDegrees = 10;

        /// <summary>A pin on its side sweeps more than its belly: its circle grows by this much.</summary>
        public const double DownReach = 1.3;

        /// <summary>A standing pin's base on the deck: how fast a nudge dies, m/s².</summary>
        public const double StandingFriction = 4;

        /// <summary>A pin on its side: drag in proportion to speed (1/s) and a floor (m/s²).</summary>
        public const double DownDrag = 1.5, DownFriction = 1.0;

        public const double Step = 0.001;
        public const int FrameEvery = 10;
        public const double MaxSeconds = 3;
        const double Still = 0.02;

        /// <summary>The ball from the end of its path into the deck. A gutter ball touches nothing.</summary>
        public static PinResult Hit(BallPath path, PinDeck deck)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (path.Gutter) return PinResult.Nothing(deck);
            return Hit(path.End.Position, path.End.Velocity, path.Ball, deck);
        }

        /// <summary>A ball at a point on the lane, moving so, into the deck. Lane frame throughout.</summary>
        public static PinResult Hit(Vec2 ballAt, Vec2 ballVelocity, BowlingBall ball, PinDeck deck)
        {
            if (ball == null) throw new ArgumentNullException(nameof(ball));
            if (deck == null) throw new ArgumentNullException(nameof(deck));

            const int n = PinDeck.Pins;
            var pos = new Vec2[n];
            var vel = new Vec2[n];
            var live = new bool[n];   // on the deck and simulated
            var down = new bool[n];   // knocked over by this ball
            for (int i = 0; i < n; i++)
            {
                pos[i] = deck.PositionOf(i);
                live[i] = deck.IsStanding(i);
            }

            var rng = new Random(deck.ActionSeed);
            var threshold = new double[n];
            for (int i = 0; i < n; i++) threshold[i] = FallImpulse * (1 + FallSpread * (2 * rng.NextDouble() - 1));

            var bp = ballAt;
            var bv = ballVelocity;
            bool ballLive = true;
            double pit = LaneGeometry.Y(LaneGeometry.PitFt);
            var frames = new List<PinFrame>(320);
            double time = 0;

            for (int step = 0; time < MaxSeconds; step++, time += Step)
            {
                if (step % FrameEvery == 0) frames.Add(Frame(time, bp, !ballLive, pos, live, down, deck));

                // Move.
                if (ballLive)
                {
                    bp += bv * Step;
                    if (bp.Y > pit + ball.Radius || Math.Abs(bp.X) > LaneGeometry.KickbackX + ball.Radius) ballLive = false;
                }
                bool moving = ballLive;
                for (int i = 0; i < n; i++)
                {
                    if (!live[i]) continue;
                    double speed = vel[i].Length;
                    if (speed > 0)
                    {
                        double slow = down[i] ? (DownDrag * speed + DownFriction) * Step : StandingFriction * Step;
                        vel[i] = slow >= speed ? Vec2.Zero : vel[i] * (1 - slow / speed);
                    }
                    pos[i] += vel[i] * Step;
                    if (vel[i].Length > Still) moving = true;

                    // Off the back into the pit, or off the deck's edge into a gutter: down.
                    if (pos[i].Y > pit) { down[i] = true; live[i] = false; continue; }
                    if (!down[i] && Math.Abs(pos[i].X) > LaneGeometry.HalfWidth) down[i] = true;

                    // The kickbacks.
                    double r = Radius(down[i]);
                    double wall = LaneGeometry.KickbackX - r;
                    if (Math.Abs(pos[i].X) > wall && Math.Sign(vel[i].X) == Math.Sign(pos[i].X))
                    {
                        double vn = Math.Abs(vel[i].X);
                        Knock(i, PinDeck.PinMass * (1 + KickbackRestitution) * vn, down, threshold);
                        vel[i] = new Vec2(-vel[i].X * KickbackRestitution, vel[i].Y);
                        pos[i] = new Vec2(Math.Sign(pos[i].X) * wall, pos[i].Y);
                    }
                }

                // Collide: ball with pins, then pins with pins.
                if (ballLive)
                    for (int i = 0; i < n; i++)
                        if (live[i]) Collide(ref bp, ref bv, ball.Radius, ball.Mass, ref pos[i], ref vel[i], Radius(down[i]), PinDeck.PinMass, BallPinRestitution, -1, i, down, threshold, rng);
                for (int i = 0; i < n; i++)
                {
                    if (!live[i]) continue;
                    for (int j = i + 1; j < n; j++)
                        if (live[j]) Collide(ref pos[i], ref vel[i], Radius(down[i]), PinDeck.PinMass, ref pos[j], ref vel[j], Radius(down[j]), PinDeck.PinMass, PinPinRestitution, i, j, down, threshold, rng);
                }

                if (!moving && !AnyMoving(vel, live)) break;
            }
            frames.Add(Frame(time, bp, !ballLive, pos, live, down, deck));

            var fell = new bool[n];
            for (int i = 0; i < n; i++) fell[i] = deck.IsStanding(i) && down[i];
            return new PinResult(fell, pos, frames);
        }

        static double Radius(bool isDown) => isDown ? PinDeck.PinRadius * DownReach : PinDeck.PinRadius;

        /// <summary>
        /// Two circles that overlap and close trade an impulse along the line between them and are
        /// pushed apart. A standing pin on either side (index 0 to 9; -1 is the ball) falls if the
        /// impulse is over the threshold, whatever hit it.
        /// </summary>
        static void Collide(ref Vec2 pa, ref Vec2 va, double ra, double ma, ref Vec2 pb, ref Vec2 vb, double rb, double mb, double e, int a, int b, bool[] down, double[] threshold, Random rng)
        {
            var d = pb - pa;
            double dist2 = d.LengthSquared, reach = ra + rb;
            if (dist2 >= reach * reach || dist2 == 0) return;
            double dist = Math.Sqrt(dist2);
            var normal = d / dist;
            double closing = Vec2.Dot(vb - va, normal);
            double inv = 1 / ma + 1 / mb;
            if (closing < 0)
            {
                double impulse = -(1 + e) * closing / inv;
                // The pins tilt and turn as they're hit: the push goes off true by a few degrees.
                double turn = (rng.NextDouble() + rng.NextDouble() + rng.NextDouble() - 1.5) * 2 * ScatterDegrees * Math.PI / 180;
                double c = Math.Cos(turn), sn = Math.Sin(turn);
                var push = new Vec2(normal.X * c - normal.Y * sn, normal.X * sn + normal.Y * c);
                va -= push * (impulse / ma);
                vb += push * (impulse / mb);
                if (a >= 0) Knock(a, impulse, down, threshold);
                if (b >= 0) Knock(b, impulse, down, threshold);
            }
            double overlap = reach - dist;
            pa -= normal * (overlap * (1 / ma) / inv);
            pb += normal * (overlap * (1 / mb) / inv);
        }

        static void Knock(int pin, double impulse, bool[] down, double[] threshold)
        {
            if (impulse > threshold[pin]) down[pin] = true;
        }

        static bool AnyMoving(Vec2[] vel, bool[] live)
        {
            for (int i = 0; i < vel.Length; i++)
                if (live[i] && vel[i].Length > Still) return true;
            return false;
        }

        static PinFrame Frame(double time, Vec2 ball, bool ballGone, Vec2[] pos, bool[] live, bool[] down, PinDeck deck)
        {
            var pins = (Vec2[])pos.Clone();
            var flat = new bool[pos.Length];
            for (int i = 0; i < pos.Length; i++) flat[i] = down[i] || !deck.IsStanding(i);
            return new PinFrame(time, ball, ballGone, pins, flat);
        }
    }
}
