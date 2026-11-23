using System;
using System.Text;
using NUnit.Framework;
using Starflare.Bowling;

namespace Starflare.Tests
{
    /// <summary>
    /// The tuning harness (bowling spec, "Tests"): not part of the suite. Run it by name to print
    /// paths as ASCII lane strips and the rates the thresholds are set from, so the hook can be
    /// read in a test log before any art exists.
    /// </summary>
    [Explicit("Tuning harness: prints, asserts nothing.")]
    public class BowlingTuning
    {
        [Test]
        public void Shapes()
        {
            var lines = new[]
            {
                ("Straight, house ball", new Throw(7, 10, 0, 16), BowlingBall.House, LanePattern.House),
                ("Snap, reactive", new Throw(32, 19, 75, 17), BowlingBall.Reactive, LanePattern.House),
                ("Snap on lane six", new Throw(32, 19, 75, 17), BowlingBall.Reactive, LanePattern.LaneSix),
                ("Arc, urethane", new Throw(15, 7, 50, 13), BowlingBall.Urethane, LanePattern.House),
                ("A league line, urethane", new Throw(29, 16.5, 70, 17), BowlingBall.Urethane, LanePattern.House),
            };
            foreach (var (label, t, ball, pattern) in lines)
            {
                var oil = new LaneOil(pattern);
                var path = BallSim.Roll(t, oil, ball);
                TestContext.Out.WriteLine($"{label}: {t}, {ball.Name}, {pattern.Name}");
                TestContext.Out.WriteLine(Summary(path));
                TestContext.Out.WriteLine(Strip(path, oil));
            }
        }

        [Test]
        public void Carry()
        {
            TestContext.Out.WriteLine("Strikes from a pocket entry, by board and angle, 16 mph, 200 racks each:");
            foreach (double angle in new[] { 2.0, 4, 6 })
            {
                var line = new StringBuilder($"  {angle}°:");
                foreach (double board in new[] { 20.0, 19, 18.5, 18, 17.5, 17, 16.5, 16, 15 })
                {
                    var (at, v) = PinSimTests.Entry(board, angle, 16);
                    int strikes = 0;
                    for (int seed = 0; seed < 200; seed++)
                        if (PinSim.Hit(at, v, BowlingBall.Urethane, PinDeck.Rack(new Random(seed))).Count == 10) strikes++;
                    line.Append($" {board}:{strikes / 2,3}%");
                }
                TestContext.Out.WriteLine(line.ToString());
            }

            TestContext.Out.WriteLine("Real throws, 200 racks each (strike, then the commonest leaves):");
            var throws = new[]
            {
                (new Throw(32, 19, 75, 17), BowlingBall.Reactive),
                (new Throw(29, 16.5, 70, 17), BowlingBall.Urethane),
                (new Throw(13, 10, 70, 16), BowlingBall.House),
                (new Throw(7, 10, 0, 16), BowlingBall.House),
            };
            foreach (var (t, ball) in throws)
            {
                var path = BallSim.Roll(t, new LaneOil(LanePattern.House), ball);
                int strikes = 0;
                var leaves = new System.Collections.Generic.Dictionary<string, int>();
                for (int seed = 0; seed < 200; seed++)
                {
                    var deck = PinDeck.Rack(new Random(seed));
                    var after = deck.After(PinSim.Hit(path, deck));
                    if (after.StandingCount == 0) { strikes++; continue; }
                    string key = string.Join("-", after.StandingPins());
                    leaves[key] = leaves.TryGetValue(key, out int c) ? c + 1 : 1;
                }
                var top = new StringBuilder();
                foreach (var kv in System.Linq.Enumerable.Take(System.Linq.Enumerable.OrderByDescending(leaves, x => x.Value), 4))
                    top.Append($" {kv.Key} ({kv.Value})");
                TestContext.Out.WriteLine($"  {t}, {ball.Name}: entry {path.EntryBoard:0.0} at {path.EntryAngle:0.0}°, strikes {strikes / 2}%;{top}");
            }
        }

        /// <summary>The numbers a tuner reads off a path.</summary>
        internal static string Summary(BallPath path) =>
            $"  gutter {path.Gutter}, entry board {path.EntryBoard:0.0} at {path.EntryAngle:0.0}°, {path.EntrySpeed:0.0} mph; " +
            $"turn {path.Turn:0.0}°, half done by {path.TurnDoneBy(0.5):0.0} ft, rolling from {path.RolledFt:0.0} ft";

        /// <summary>
        /// The lane from above, pins at the top, every two feet: the oil as # + . (heavy to light),
        /// the ball as o while it slides and O once it rolls.
        /// </summary>
        internal static string Strip(BallPath path, LaneOil oil)
        {
            var sb = new StringBuilder();
            for (int foot = 58; foot >= 0; foot -= 2)
            {
                var row = new char[LaneOil.Boards];
                for (int b = 1; b <= LaneOil.Boards; b++)
                {
                    double u = oil.UnitsAt(LaneOil.Boards + 1 - b, foot);
                    row[b - 1] = u > 30 ? '#' : u > 15 ? '+' : u > 0.5 ? '.' : ' ';
                }
                if (path.End.Feet >= foot)
                {
                    double board = path.BoardAt(foot);
                    int col = LaneOil.Boards - (int)Math.Round(board);
                    bool rolling = foot >= path.RolledFt;
                    if (col >= 0 && col < LaneOil.Boards) row[col] = rolling ? 'O' : 'o';
                }
                sb.Append($"  {foot,2} |").Append(row).Append("|\n");
            }
            sb.Append("      39        30        20        10        1");
            return sb.ToString();
        }
    }
}
