using UnityEngine;

namespace Starflare.Bowling
{
    /// <summary>The colour each ball is drawn in until there are sprites: a red-swirl house ball, a black urethane, a pearl reactive.</summary>
    static class BallLook
    {
        public static Color ColourOf(BowlingBall ball) =>
            ball == BowlingBall.Urethane ? new Color(0.12f, 0.12f, 0.14f)
            : ball == BowlingBall.Reactive ? new Color(0.72f, 0.62f, 0.86f)
            : new Color(0.72f, 0.12f, 0.12f);

        public static readonly Color Pin = new Color(0.97f, 0.96f, 0.92f);
        public static readonly Color PinNeck = new Color(0.8f, 0.1f, 0.1f);
        public static readonly Color PinDown = new Color(0.75f, 0.74f, 0.7f, 0.6f);
        public static readonly Color Wood = new Color(0.78f, 0.62f, 0.4f);
        public static readonly Color Approach = new Color(0.86f, 0.72f, 0.5f);
        public static readonly Color Gutter = new Color(0.32f, 0.32f, 0.35f);
        public static readonly Color Mark = new Color(0.28f, 0.17f, 0.08f, 0.85f);
        public static readonly Color Pit = new Color(0.08f, 0.07f, 0.07f);
    }
}
