using System;

namespace Starflare.Bowling
{
    /// <summary>
    /// The lane in real dimensions. Bowlers speak in boards across (39, board 1 at a right-hander's
    /// right gutter, the 20 board the middle) and feet down (0 at the foul line, 60 at the head
    /// pin); the sim runs in metres with x across, positive toward board 1, and y down the lane.
    /// </summary>
    public static class LaneGeometry
    {
        public const int Boards = 39;
        public const int MiddleBoard = 20;
        public const double MetresPerFoot = 0.3048;
        public const double MetresPerInch = 0.0254;

        /// <summary>41.5 in over 39 boards: 1.064 in each.</summary>
        public const double BoardWidth = 41.5 * MetresPerInch / Boards;

        /// <summary>Half the lane's width: past this the ball's in the gutter.</summary>
        public const double HalfWidth = 41.5 * MetresPerInch / 2;

        /// <summary>The gutters are 9.25 in; the kickbacks stand beyond them.</summary>
        public const double KickbackX = HalfWidth + 9.25 * MetresPerInch;

        public const double DotsFt = 7;
        public const double ArrowsFt = 15;
        public const double HeadPinFt = 60;

        /// <summary>The pit, 2.9 ft past the head pin: a pin or ball past this is gone.</summary>
        public const double PitFt = HeadPinFt + 2.9;

        /// <summary>The seven arrows, second from the right on board 10.</summary>
        public static readonly int[] Arrows = { 5, 10, 15, 20, 25, 30, 35 };

        /// <summary>The middle of a board, in metres across.</summary>
        public static double X(double board) => (MiddleBoard - board) * BoardWidth;

        /// <summary>The board (fractional) at a distance across; 0.5 to 39.5 is on the lane.</summary>
        public static double Board(double x) => MiddleBoard - x / BoardWidth;

        public static double Y(double feet) => feet * MetresPerFoot;
        public static double Feet(double y) => y / MetresPerFoot;

        /// <summary>A board as the other hand counts it: your 10 is a left-hander's 30.</summary>
        public static double Mirror(double board) => Boards + 1 - board;

        public static bool OnLane(double x) => Math.Abs(x) <= HalfWidth;

        public static Vec2 Point(double board, double feet) => new Vec2(X(board), Y(feet));
    }
}
