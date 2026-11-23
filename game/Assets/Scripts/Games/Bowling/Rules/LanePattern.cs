using System;
using System.Collections.Generic;

namespace Starflare.Bowling
{
    /// <summary>
    /// How a lane is oiled: its loads, laid down fresh every afternoon. Nobody in the game ever
    /// sees one; Juan tells you about a lane in words and the trace shows you he's right.
    /// </summary>
    public sealed class LanePattern
    {
        public readonly string Name;
        public readonly IReadOnlyList<OilLoad> Loads;

        public LanePattern(string name, params OilLoad[] loads)
        {
            Name = name;
            Loads = loads ?? throw new ArgumentNullException(nameof(loads));
        }

        /// <summary>Where the oil stops, in feet: the end of the longest load.</summary>
        public double LengthFt
        {
            get
            {
                double end = 0;
                foreach (var load in Loads) end = Math.Max(end, load.ToFt);
                return end;
            }
        }

        /// <summary>
        /// The Millstone house shot: a 40 ft pattern at about 8 to 1, middle to outside, what a
        /// Backwater alley oils for its league. Easy to the pocket from the right line, forgiving
        /// of a miss inside, punishing of one outside.
        /// </summary>
        public static readonly LanePattern House = new LanePattern("Millstone house",
            new OilLoad(0, 40, 1, 39, 6),
            new OilLoad(0, 36, 8, 32, 20),
            new OilLoad(0, 30, 13, 27, 20));

        /// <summary>
        /// Lane six, shorter and lighter: the body ends at 34 ft, the other loads shorten with it,
        /// and every load is two thirds. It breaks early. "Lane six is running dry tonight. Move
        /// left two boards."
        /// </summary>
        public static readonly LanePattern LaneSix = new LanePattern("Lane six",
            new OilLoad(0, 34, 1, 39, 6 * 2.0 / 3),
            new OilLoad(0, 31, 8, 32, 20 * 2.0 / 3),
            new OilLoad(0, 26, 13, 27, 20 * 2.0 / 3));

        /// <summary>The fresh grid: units per cell, a board wide and a foot long.</summary>
        public double[,] ToGrid()
        {
            var grid = new double[LaneOil.Boards, LaneOil.Feet];
            foreach (var load in Loads)
                for (int board = Math.Max(1, load.FromBoard); board <= Math.Min(LaneOil.Boards, load.ToBoard); board++)
                    for (int foot = (int)Math.Max(0, load.FromFt); foot < Math.Min(LaneOil.Feet, load.ToFt); foot++)
                        grid[board - 1, foot] += load.Units;
            return grid;
        }
    }
}
