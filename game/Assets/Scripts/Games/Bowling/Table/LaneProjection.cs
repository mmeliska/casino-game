using System;
using UnityEngine;

namespace Starflare.Bowling
{
    /// <summary>
    /// The view from behind and above the foul line, like a 90s console game, for a rect: the
    /// approach at the bottom, the pins small near the top, and above the pit a band for the
    /// masking panel. The lane, the room either side and everything on them are drawn through it,
    /// so the boards, the arrows and the ball always agree with the sim.
    /// </summary>
    readonly struct LaneProjection
    {
        /// <summary>
        /// How far behind the foul line the eye is, in feet, for both how the lane narrows and
        /// how its length is spread up the screen. One distance for both is a true perspective,
        /// so the lane's lines are straight (two, as this first had, bent every line). Far back
        /// is a long lens, like TV bowling: the lane narrows gently and the far end, where the
        /// hook happens, keeps room on the screen.
        /// </summary>
        const double Eye = 60;

        public const double NearFt = -2.5, FarFt = LaneGeometry.PitFt + 0.6;

        /// <summary>Half the lane's width at the foul line, as a share of the view's width.</summary>
        const float FoulHalf = 0.3f;

        /// <summary>The top share of the view, above the pit, where the masking panel hangs.</summary>
        public const float Headroom = 0.12f;

        /// <summary>
        /// The capping between one lane's gutter and the next one's: 4 in, so lanes sit about
        /// 63 in apart, centre to centre.
        /// </summary>
        public const double Capping = 4 * LaneGeometry.MetresPerInch;

        public const double Pitch = 2 * LaneGeometry.KickbackX + Capping;

        /// <summary>The lanes drawn either side of yours, dimmer the further out.</summary>
        public const int Neighbours = 3;

        readonly Rect _r;

        public LaneProjection(Rect rect) => _r = rect;

        public Rect Rect => _r;

        /// <summary>The far end of the lanes, where the masking panel's band starts.</summary>
        public float FarY => _r.yMin + (1 - Headroom) * _r.height;

        static double Scale(double ft) => Eye / (Eye + ft);

        /// <summary>Where a lane point (metres, the lane's frame) falls in the rect.</summary>
        public Vector2 Project(Vec2 at) => Project(at.X, LaneGeometry.Feet(at.Y));

        /// <summary>Where a point this many metres across and feet down falls in the rect.</summary>
        public Vector2 Project(double x, double ft)
        {
            double v = (Scale(NearFt) - Scale(ft)) / (Scale(NearFt) - Scale(FarFt));
            return new Vector2(_r.center.x + (float)(x * PxPerMetre(ft)), _r.yMin + (float)v * (1 - Headroom) * _r.height);
        }

        /// <summary>Screen units per metre across, at a depth.</summary>
        public double PxPerMetre(double ft)
        {
            double foulHalfPx = Math.Min(_r.width * FoulHalf, _r.height * 0.22);
            return Scale(ft) / Scale(0) * foulHalfPx / LaneGeometry.HalfWidth;
        }

        public float Px(double metres, double ft) => (float)(metres * PxPerMetre(ft));

        /// <summary>The lane's x (metres) under a point of the rect at a depth.</summary>
        public double LaneXAt(float localX, double ft) => (localX - _r.center.x) / PxPerMetre(ft);

        /// <summary>
        /// The light down the lane at night: the approach under the house lights, the middle of
        /// the lane dimmer, the deck bright under its hood.
        /// </summary>
        public static float Light(double ft)
        {
            if (ft < 0) return 0.82f;
            if (ft < 30) return Mathf.Lerp(0.86f, 0.62f, (float)(ft / 30));
            if (ft < 52) return 0.62f;
            return Mathf.Lerp(0.62f, 1f, Mathf.Clamp01((float)((ft - 52) / (LaneGeometry.HeadPinFt - 52))));
        }

        /// <summary>The lanes either side are dimmer: nobody's bowling on them, their lights are low.</summary>
        public static float Dim(int lane) => lane == 0 ? 1f : lane == 1 || lane == -1 ? 0.5f : lane == 2 || lane == -2 ? 0.34f : 0.24f;

        /// <summary>Depths to cut a strip into, so a texture keeps its perspective down the lane.</summary>
        public static double[] Rows(double fromFt, double toFt, double stepFt = 1.5)
        {
            int n = Math.Max(1, (int)Math.Ceiling((toFt - fromFt) / stepFt));
            var rows = new double[n + 1];
            for (int i = 0; i <= n; i++) rows[i] = fromFt + (toFt - fromFt) * i / n;
            return rows;
        }
    }
}
