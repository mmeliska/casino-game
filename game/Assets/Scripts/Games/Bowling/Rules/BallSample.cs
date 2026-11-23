namespace Starflare.Bowling
{
    /// <summary>
    /// The ball at one moment of a throw, for the trace and the tests. Position and velocity are in
    /// the lane's frame; curvature is positive when the ball turns toward the bowler's off side,
    /// the way a hook turns.
    /// </summary>
    public readonly struct BallSample
    {
        public readonly double Time;
        public readonly Vec2 Position;
        public readonly Vec2 Velocity;

        /// <summary>How fast the ball's contact point slides on the lane; zero once it's rolling.</summary>
        public readonly double Slip;

        /// <summary>1/metres: how sharply the path bends here.</summary>
        public readonly double Curvature;

        public BallSample(double time, Vec2 position, Vec2 velocity, double slip, double curvature)
        {
            Time = time;
            Position = position;
            Velocity = velocity;
            Slip = slip;
            Curvature = curvature;
        }

        public double Feet => LaneGeometry.Feet(Position.Y);
    }
}
