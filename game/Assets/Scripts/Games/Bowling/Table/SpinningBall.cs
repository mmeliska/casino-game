using System;
using UnityEngine;
using UnityEngine.UI;

namespace Starflare.Bowling
{
    /// <summary>
    /// The rolling ball as a sphere with its surface wrapped round it, the way a globe's is (a
    /// picture twice as wide as it's tall), turned by the sim's spin (<see cref="BallSpin"/>):
    /// the swirl and the finger holes come up over the top as it rolls away and sweep round as it
    /// hooks. Seen from a little above and behind on the lane, straight down on the deck. Shaded
    /// in flat bands, lit from above, to sit with the ink-and-cel art; its owner draws the ink
    /// ring under it.
    /// </summary>
    public sealed class SpinningBall : MaskableGraphic
    {
        const int Rings = 9, Segments = 28;

        /// <summary>Where the light comes from, in the lane's frame: above, a little left and toward you.</summary>
        static readonly Vec3 Light = Unit(new Vec3(-0.35, -0.45, 0.82));

        Texture _surface;
        Func<Rect, (bool show, Vector2 centre, float radius, Turn turn)> _place;
        Vec3 _right, _up, _toward;

        public override Texture mainTexture => _surface != null ? _surface : s_WhiteTexture;

        /// <summary>
        /// A ball stretched over its parent, seen from this far above the lane (degrees: 0 level,
        /// 90 straight down), placed and turned each draw by <paramref name="place"/>.
        /// </summary>
        public static SpinningBall Make(Transform parent, string name, double elevationDegrees,
            Func<Rect, (bool show, Vector2 centre, float radius, Turn turn)> place)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(SpinningBall));
            go.transform.SetParent(parent, false);
            var rect = (RectTransform)go.transform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = rect.offsetMax = Vector2.zero;
            var ball = go.GetComponent<SpinningBall>();
            ball.raycastTarget = false;
            ball._place = place;
            double a = elevationDegrees * Math.PI / 180;
            ball._right = new Vec3(1, 0, 0);
            ball._up = new Vec3(0, Math.Sin(a), Math.Cos(a));
            ball._toward = new Vec3(0, -Math.Cos(a), Math.Sin(a));
            return ball;
        }

        /// <summary>The ball's surface for this delivery; null draws it white.</summary>
        public Texture Surface
        {
            set
            {
                if (_surface == value) return;
                _surface = value;
                SetMaterialDirty();
                SetVerticesDirty();
            }
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            var r = rectTransform.rect;
            if (r.width <= 0 || r.height <= 0 || _place == null) return;
            var (show, centre, radius, turn) = _place(r);
            if (!show || radius <= 0.5f) return;
            var back = turn.Inverse;
            for (int ring = 0; ring < Rings; ring++)
            {
                float r0 = ring / (float)Rings, r1 = (ring + 1) / (float)Rings;
                for (int seg = 0; seg < Segments; seg++)
                {
                    float a0 = seg * Mathf.PI * 2 / Segments, a1 = (seg + 1) * Mathf.PI * 2 / Segments;
                    var p00 = Polar(r0, a0);
                    var p01 = Polar(r0, a1);
                    var p10 = Polar(r1, a0);
                    var p11 = Polar(r1, a1);
                    Triangle(vh, centre, radius, back, p00, p10, p11);
                    if (ring > 0) Triangle(vh, centre, radius, back, p00, p11, p01);
                }
            }
        }

        static Vector2 Polar(float r, float angle) => new Vector2(Mathf.Cos(angle) * r, Mathf.Sin(angle) * r);

        /// <summary>
        /// One triangle of the disc, each corner looked up on the sphere's surface. Its own corners,
        /// so a triangle across the picture's join can carry its left edge on past the right.
        /// </summary>
        void Triangle(VertexHelper vh, Vector2 centre, float radius, Turn back, Vector2 a, Vector2 b, Vector2 c)
        {
            var (ua, la) = Look(a, back);
            var (ub, lb) = Look(b, back);
            var (uc, lc) = Look(c, back);
            float lo = Mathf.Min(ua.x, Mathf.Min(ub.x, uc.x)), hi = Mathf.Max(ua.x, Mathf.Max(ub.x, uc.x));
            if (hi - lo > 0.5f)
            {
                if (ua.x < 0.5f) ua.x += 1;
                if (ub.x < 0.5f) ub.x += 1;
                if (uc.x < 0.5f) uc.x += 1;
            }
            int i = vh.currentVertCount;
            vh.AddVert(centre + a * radius, la, ua);
            vh.AddVert(centre + b * radius, lb, ub);
            vh.AddVert(centre + c * radius, lc, uc);
            vh.AddTriangle(i, i + 1, i + 2);
        }

        /// <summary>Where on the ball's own surface a point of the disc is, and how lit it is there.</summary>
        (Vector2 uv, Color light) Look(Vector2 onDisc, Turn back)
        {
            double depth = Math.Sqrt(Math.Max(0, 1 - onDisc.x * onDisc.x - onDisc.y * onDisc.y));
            var normal = _right * onDisc.x + _up * onDisc.y + _toward * depth;
            var own = back.Rotate(normal);
            float u = (float)(Math.Atan2(own.Y, own.X) / (2 * Math.PI) + 0.5);
            float v = (float)(Math.Asin(Math.Max(-1, Math.Min(1, own.Z))) / Math.PI + 0.5);
            double lit = normal.X * Light.X + normal.Y * Light.Y + normal.Z * Light.Z;
            float band = lit > 0.55 ? 1f : lit > 0.1 ? 0.8f : 0.58f;
            var c = color;
            return (new Vector2(u, v), new Color(c.r * band, c.g * band, c.b * band, c.a));
        }

        static Vec3 Unit(Vec3 v) => v * (1 / v.Length);

        public void Redraw() => SetVerticesDirty();

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            SetVerticesDirty();
        }
    }
}
