using System;
using System.Collections.Generic;
using System.Linq;

namespace Starflare.Bowling
{
    /// <summary>
    /// A regular on the next lane: a name, a face, an average and the line he bowls. His average
    /// sets how close he stops the meters to his line. He bowls through the same lane with the
    /// same sims, so he feels it change too, and one rule moves him: a miss of two boards or more
    /// moves his stand a board toward the miss. That's all the AI there is.
    /// </summary>
    public sealed class Bowler
    {
        public readonly string Name;

        /// <summary>The face's id, for the venue's art; the rules never load it.</summary>
        public readonly string Face;

        public readonly int Average;
        public readonly BowlingBall Ball;

        /// <summary>The line he'd throw on a fresh house shot.</summary>
        public readonly Throw Line;

        /// <summary>Where he stands tonight, after the lane's moved him.</summary>
        public int Stand { get; private set; }

        /// <summary>The pocket, as the bowler's own board count at the head pin's row.</summary>
        public const double Pocket = 17.5;

        /// <summary>A miss this far off the pocket moves him.</summary>
        public const double MissBoards = 2;

        public Bowler(string name, string face, int average, BowlingBall ball, Throw line)
        {
            Name = name;
            Face = face;
            Average = average;
            Ball = ball ?? throw new ArgumentNullException(nameof(ball));
            Line = line;
            Stand = line.Stand;
        }

        /// <summary>
        /// How close he stops the meters: the spread (a standard deviation) of his aim in boards and
        /// of his hook and speed in per cent of the meter. Tuned so simulated bowlers bowl their
        /// averages: a 185 bowler is within 0.3 of a board and 1%, a 135 bowler 0.6 and 3%. A board
        /// at the arrows is two to four at the pins, and the pocket is about two wide.
        /// </summary>
        public (double aimBoards, double percent) Spread
        {
            get
            {
                double t = Math.Max(0, Math.Min(1, (Average - SpreadLow) / (SpreadHigh - SpreadLow)));
                return (AimAtLow + (AimAtHigh - AimAtLow) * t, PercentAtLow + (PercentAtHigh - PercentAtLow) * t);
            }
        }

        public const double SpreadLow = 135, SpreadHigh = 185;
        public const double AimAtLow = 0.6, AimAtHigh = 0.3, PercentAtLow = 3, PercentAtHigh = 1;

        /// <summary>
        /// His next throw at what's standing: his line on a full rack, and on a leave a straight
        /// ball across the lane at the pins, each with his spread.
        /// </summary>
        public Throw Next(PinDeck deck, Random rng, Hand hand = Hand.Right)
        {
            if (deck == null) throw new ArgumentNullException(nameof(deck));
            if (rng == null) throw new ArgumentNullException(nameof(rng));
            var (aimSpread, percent) = Spread;
            var aimed = deck.StandingCount == PinDeck.Pins ? new Throw(Stand, Line.Aim, Line.Hook, Line.Speed) : SpareFor(deck, hand);
            return new Throw(aimed.Stand,
                aimed.Aim + Gauss(rng) * aimSpread,
                aimed.Hook + Gauss(rng) * percent,
                aimed.Speed + Gauss(rng) * percent / 100 * (Throw.MaxSpeed - Throw.MinSpeed)).Clamp();
        }

        /// <summary>
        /// His ball on a lane: his throw at what's standing, rolled, and on a first ball the miss
        /// rule. The table plays it; the result is the lane's.
        /// </summary>
        public Delivery BowlAt(LaneState lane, Random rng)
        {
            if (lane == null) throw new ArgumentNullException(nameof(lane));
            bool fresh = lane.Deck.StandingCount == PinDeck.Pins;
            var delivery = lane.Bowl(Next(lane.Deck, rng, lane.Hand), fresh ? Ball : BowlingBall.House);
            if (fresh) Adjust(delivery.Path);
            return delivery;
        }

        /// <summary>
        /// His spare ball at a leave. One pin is easy: <see cref="SpareLine"/>. For more, he knows
        /// from experience which of a few lines around it takes the most (the first time he sees a
        /// leave tonight he tries them out in his head: a straight ball doesn't care about the oil),
        /// and remembers.
        /// </summary>
        public Throw SpareFor(PinDeck deck, Hand hand = Hand.Right)
        {
            var line = SpareLine(deck, hand, out bool coversAll);
            if (coversAll) return line;
            int key = (hand == Hand.Left ? 1 << PinDeck.Pins : 0);
            for (int i = 0; i < PinDeck.Pins; i++) if (deck.IsStanding(i)) key |= 1 << i;
            if (_spares.TryGetValue(key, out var known)) return known;

            var leave = deck.StandingPins().ToArray();
            var best = line;
            int bestCount = -1;
            foreach (double shift in SpareShifts)
            {
                var t = Shifted(line, shift);
                var path = BallSim.Roll(t, _anyLane, BowlingBall.House, hand);
                int count = 0;
                for (int seed = 1; seed <= SpareTries; seed++)
                    count += PinSim.Hit(path, PinDeck.Leave(seed, leave)).Count;
                if (count > bestCount) { bestCount = count; best = t; }
            }
            _spares[key] = best;
            return best;
        }

        static readonly double[] SpareShifts = { 0, -1.5, 1.5, -3, 3, -4.5, 4.5, -6, 6 };
        const int SpareTries = 3;

        /// <summary>The same straight spare ball, moved so it reaches the pins this many boards over.</summary>
        static Throw Shifted(Throw line, double boards) =>
            new Throw(line.Stand, line.Aim + boards * LaneGeometry.ArrowFt(line.Aim) / LaneGeometry.HeadPinFt, 0, line.Speed).Clamp();

        readonly Dictionary<int, Throw> _spares = new Dictionary<int, Throw>();

        /// <summary>A lane to try spare balls on in his head; a straight ball never reads the oil.</summary>
        readonly LaneOil _anyLane = new LaneOil(LanePattern.House);

        /// <summary>
        /// A straight ball at a leave, from across the lane (the further the pins are to one side,
        /// the further he stands to the other), at the board where the ball itself takes the most
        /// pins, the front row counting double since it's what the ball meets first, and in the
        /// middle of that window for the most room. The rest go by deflection or not at all: a
        /// 7–10 is a 7–10.
        /// </summary>
        public static Throw SpareLine(PinDeck deck, Hand hand = Hand.Right) => SpareLine(deck, hand, out _);

        /// <summary>The same, and whether the ball takes every pin standing itself.</summary>
        public static Throw SpareLine(PinDeck deck, Hand hand, out bool coversAll)
        {
            var boards = new List<double>();
            var front = new List<bool>();
            double frontY = double.MaxValue;
            for (int i = 0; i < PinDeck.Pins; i++)
                if (deck.IsStanding(i)) frontY = Math.Min(frontY, deck.PositionOf(i).Y);
            for (int i = 0; i < PinDeck.Pins; i++)
            {
                if (!deck.IsStanding(i)) continue;
                boards.Add(BoardOf(deck.PositionOf(i), hand));
                front.Add(deck.PositionOf(i).Y < frontY + 0.1);
            }
            double target = LaneGeometry.MiddleBoard, best = double.MinValue;
            int bestTaken = 0;
            for (double t = 1; t <= LaneGeometry.Boards; t += 0.25)
            {
                double weight = 0, room = SpareReach;
                int taken = 0;
                for (int i = 0; i < boards.Count; i++)
                {
                    double off = Math.Abs(boards[i] - t);
                    if (off >= SpareReach) continue;
                    weight += front[i] ? 2 : 1;
                    taken++;
                    room = Math.Min(room, SpareReach - off);
                }
                double score = weight * 100 + room;
                if (score > best) { best = score; target = t; bestTaken = taken; }
            }
            coversAll = bestTaken == boards.Count;
            int stand = (int)Math.Round(Math.Max(Throw.MinStand + 4, Math.Min(Throw.MaxStand - 4, LaneGeometry.MiddleBoard + 0.6 * (LaneGeometry.MiddleBoard - target))));
            // The aim is read at its arrow's distance, which depends on the aim: a few rounds settle it.
            double aim = stand;
            for (int i = 0; i < 4; i++) aim = stand + (target - stand) * LaneGeometry.ArrowFt(aim) / LaneGeometry.HeadPinFt;
            return new Throw(stand, aim, 0, 16).Clamp();
        }

        /// <summary>How far off a pin's board the ball's centre can pass and still take it: the ball and pin together.</summary>
        const double SpareReach = (0.108 + PinDeck.PinRadius) / LaneGeometry.BoardWidth;

        static double BoardOf(Vec2 at, Hand hand)
        {
            double board = LaneGeometry.Board(at.X);
            return hand == Hand.Right ? board : LaneGeometry.Mirror(board);
        }

        /// <summary>
        /// After a first ball: if it reached the head pin's row two boards or more off the pocket,
        /// he moves his stand a board toward the miss (high, left; light, right, for a right-hander).
        /// </summary>
        public void Adjust(BallPath firstBall)
        {
            if (firstBall == null) throw new ArgumentNullException(nameof(firstBall));
            double entry = firstBall.Gutter ? 0 : firstBall.EntryBoard;
            if (entry >= Pocket + MissBoards) Stand = Math.Min(Throw.MaxStand, Stand + 1);
            else if (entry <= Pocket - MissBoards) Stand = Math.Max(Throw.MinStand, Stand - 1);
        }

        /// <summary>
        /// The stake he names for a game: $10 from a bowler of 175 or better and $5 below, a step
        /// up if he's ahead of you tonight and a step down if he's behind, and never more than
        /// you've got. Nothing if you can't cover the smallest.
        /// </summary>
        public int? StakeAgainst(NightRecord record, int bankroll, BowlingRules rules)
        {
            if (rules == null) throw new ArgumentNullException(nameof(rules));
            var stakes = rules.Stakes;
            int step = Average >= 175 ? 1 : 0;
            if (record != null)
            {
                if (record.Won > record.Lost) step++;
                else if (record.Lost > record.Won) step--;
            }
            step = Math.Max(0, Math.Min(stakes.Length - 1, step));
            while (step >= 0 && stakes[step] > bankroll) step--;
            return step < 0 ? (int?)null : stakes[step];
        }

        /// <summary>
        /// A Lanes face with an average: the ball and the line a bowler of that average throws on
        /// the house shot (a 175-plus bowler a urethane ball from the 23 board at 12.8, medium hook;
        /// a 150 the urethane up the second arrow; below that the house ball from deep inside).
        /// </summary>
        public static Bowler WithAverage(string name, string face, int average)
        {
            if (average >= 175) return new Bowler(name, face, average, BowlingBall.Urethane, new Throw(23, 12.8, 60, 16.5));
            if (average >= 150) return new Bowler(name, face, average, BowlingBall.Urethane, new Throw(19, 10.5, 50, 16));
            return new Bowler(name, face, average, BowlingBall.House, new Throw(34, 25, 55, 15));
        }

        /// <summary>A standard normal draw (Box–Muller), from the seeded generator.</summary>
        internal static double Gauss(Random rng)
        {
            double u = 1 - rng.NextDouble(), v = rng.NextDouble();
            return Math.Sqrt(-2 * Math.Log(u)) * Math.Cos(2 * Math.PI * v);
        }
    }

    /// <summary>A regular's games against you tonight, from his side: won, lost, and his losing run.</summary>
    public sealed class NightRecord
    {
        public int Won, Lost, Pushed, LosingRun;
    }
}
