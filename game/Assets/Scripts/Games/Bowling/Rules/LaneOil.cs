using System;

namespace Starflare.Bowling
{
    /// <summary>
    /// The oil on one lane tonight: a grid of units, a board wide and a foot long, 39 by 60, with
    /// board 1 at a right-hander's right gutter. Live within a sitting (every ball that passes picks
    /// some up and carries a little past the end of the pattern); never saved, because the venue
    /// can compute it from the clock (<see cref="Worn"/>).
    /// </summary>
    public sealed class LaneOil
    {
        public const int Boards = LaneGeometry.Boards;
        public const int Feet = 60;

        /// <summary>Friction with no oil and with a full coat, for a urethane cover.</summary>
        public const double BareFriction = 0.20, OiledFriction = 0.015;

        /// <summary>How fast oil takes the grip away: each this many units leaves 1/e of the difference.</summary>
        public const double CoatUnits = 4;

        /// <summary>The share of a cell's oil a ball picks up crossing it.</summary>
        public const double PickUp = 0.003;

        /// <summary>The share of what it picked up that a ball lays down past the pattern.</summary>
        public const double CarryShare = 0.1;

        /// <summary>How fast the carried oil fades past the pattern's end.</summary>
        public const double CarryFadeFt = 5;

        /// <summary>The league's track, boards counted from the bowling hand's gutter.</summary>
        public const double TrackBoard = 10, TrackSpread = 1.5;

        readonly double[,] _units;

        /// <summary>Where the fresh pattern stopped: carrydown lands past here.</summary>
        public readonly double PatternEndFt;

        public LaneOil(LanePattern pattern)
        {
            if (pattern == null) throw new ArgumentNullException(nameof(pattern));
            _units = pattern.ToGrid();
            PatternEndFt = pattern.LengthFt;
        }

        LaneOil(double[,] units, double patternEndFt)
        {
            _units = units;
            PatternEndFt = patternEndFt;
        }

        public LaneOil Copy() => new LaneOil((double[,])_units.Clone(), PatternEndFt);

        /// <summary>Units on a board (1 to 39) at a foot (0 to 59).</summary>
        public double UnitsAt(int board, int foot) => _units[board - 1, foot];

        /// <summary>Units under a point on the lane; none off it or past 60 ft.</summary>
        public double UnitsAt(Vec2 at)
        {
            int board = (int)Math.Round(LaneGeometry.Board(at.X));
            int foot = (int)Math.Floor(LaneGeometry.Feet(at.Y));
            if (board < 1 || board > Boards || foot < 0 || foot >= Feet) return 0;
            return _units[board - 1, foot];
        }

        /// <summary>
        /// The friction under a point for a ball: from bare toward oiled as the units rise, quickly
        /// at first (a thin film takes most of the grip away) and then levelling off; the ball's
        /// cover scales the two ends. The body of the pattern is slick, the outside boards grip
        /// more, and past the oil everything grabs.
        /// </summary>
        public double FrictionAt(Vec2 at, BowlingBall ball) => Friction(UnitsAt(at), ball);

        public static double Friction(double units, BowlingBall ball)
        {
            double oiled = OiledFriction * ball.OilGrip, bare = BareFriction * ball.BareGrip;
            return oiled + (bare - oiled) * Math.Exp(-units / CoatUnits);
        }

        /// <summary>
        /// A ball has gone down the lane: each cell it crossed gives up a little oil, and a tenth of
        /// what it picked up is laid on the cells it crosses past the pattern, fading with distance.
        /// </summary>
        public void Pass(BallPath path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            double picked = 0;
            int lastBoard = -1, lastFoot = -1;
            foreach (var sample in path.Samples)
            {
                if (!Cell(sample.Position, out int board, out int foot) || (board == lastBoard && foot == lastFoot)) continue;
                lastBoard = board;
                lastFoot = foot;
                if (foot < PatternEndFt)
                {
                    double take = _units[board, foot] * PickUp;
                    _units[board, foot] -= take;
                    picked += take;
                }
                else if (picked > 0)
                {
                    double past = foot + 0.5 - PatternEndFt;
                    _units[board, foot] += picked * CarryShare * CarryWeight(past);
                }
            }
        }

        /// <summary>
        /// The lane after so many balls since it was oiled: each, on average, rolled the league's
        /// track (boards 8 to 12 from the bowling hand's gutter, most on the 10) and carried what it
        /// picked up past the pattern. The venue's sit-down lane, from the clock; nothing saved.
        /// </summary>
        public static LaneOil Worn(LanePattern pattern, int balls, Hand hand = Hand.Right)
        {
            var oil = new LaneOil(pattern);
            if (balls <= 0) return oil;
            double end = oil.PatternEndFt;
            for (int b = 1; b <= Boards; b++)
            {
                double share = TrackShare(b);
                if (share < 1e-4) continue;
                int board = (hand == Hand.Right ? b : Boards + 1 - b) - 1;
                double keep = Math.Pow(1 - PickUp, balls * share);
                double picked = 0;
                for (int foot = 0; foot < Feet && foot < end; foot++)
                {
                    double before = oil._units[board, foot];
                    oil._units[board, foot] = before * keep;
                    picked += before - before * keep;
                }
                for (int foot = (int)Math.Ceiling(end); foot < Feet; foot++)
                    oil._units[board, foot] += picked * CarryShare * CarryWeight(foot + 0.5 - end);
            }
            return oil;
        }

        /// <summary>The share of balls that roll a board, a bell around the 10.</summary>
        public static double TrackShare(int board)
        {
            double d = (board - TrackBoard) / TrackSpread;
            return Math.Exp(-0.5 * d * d) / (TrackSpread * Math.Sqrt(2 * Math.PI));
        }

        /// <summary>The share of the carried oil laid on the foot this far past the pattern.</summary>
        static double CarryWeight(double pastFt) =>
            pastFt < 0 ? 0 : (1 - Math.Exp(-1 / CarryFadeFt)) * Math.Exp(-Math.Floor(pastFt) / CarryFadeFt);

        static bool Cell(Vec2 at, out int board, out int foot)
        {
            board = (int)Math.Round(LaneGeometry.Board(at.X)) - 1;
            foot = (int)Math.Floor(LaneGeometry.Feet(at.Y));
            return board >= 0 && board < Boards && foot >= 0 && foot < Feet;
        }
    }
}
