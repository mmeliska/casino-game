using UnityEngine;
using UnityEngine.UI;

namespace Starflare.Bowling
{
    /// <summary>
    /// The hook as a dial: a half circle from a big hook to the left at its left end, through
    /// straight at the top, to a big hook to the right at its right end (Mike, 2026-11-24). The
    /// middle is grey: a hook that small rolls straight (<see cref="BallSim.StraightBelow"/>).
    /// A needle swings across it while the hook runs.
    /// </summary>
    public sealed class HookGauge : MaskableGraphic
    {
        static readonly Color Ink = new Color(0.04f, 0.03f, 0.03f, 0.95f);
        static readonly Color Straight = new Color(0.55f, 0.55f, 0.58f);
        static readonly Color Mild = new Color(0.36f, 0.79f, 0.82f);
        static readonly Color Big = new Color(0.95f, 0.76f, 0.25f);
        static readonly Color Needle = new Color(0.97f, 0.96f, 0.92f);

        /// <summary>Where the needle points: 0 the biggest hook left, a half straight, 1 the biggest hook right; null hides it.</summary>
        public double? Value;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            var r = rectTransform.rect;
            if (r.width <= 0 || r.height <= 0) return;
            float radius = Mathf.Min(r.width / 2, r.height) * 0.92f;
            var centre = new Vector2(r.center.x, r.yMin + (r.height - radius) / 2 + 2);
            float thick = radius * 0.28f;

            Arc(vh, centre, radius + 3, radius - thick - 3, 0, 1, Ink, Ink);
            float half = (float)BallSim.StraightBelow / 2;
            Arc(vh, centre, radius, radius - thick, 0, 0.5f - half, Big, Mild);
            Arc(vh, centre, radius, radius - thick, 0.5f - half, 0.5f + half, Straight, Straight);
            Arc(vh, centre, radius, radius - thick, 0.5f + half, 1, Mild, Big);
            Shapes.Circle(vh, centre, thick * 0.45f, Ink, 1, 12);

            if (!Value.HasValue) return;
            float angle = Mathf.PI * (1 - Mathf.Clamp01((float)Value.Value));
            var tip = centre + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * (radius + 2);
            Shapes.Line(vh, centre, tip, 8, Ink);
            Shapes.Line(vh, centre, tip, 4, Needle);
            Shapes.Circle(vh, centre, thick * 0.3f, Needle, 1, 12);
        }

        /// <summary>A band of the half circle from one share of the way round to another (0 at the left), shaded from one colour to another.</summary>
        static void Arc(VertexHelper vh, Vector2 centre, float outer, float inner, float from, float to, Color fromColour, Color toColour)
        {
            const int Steps = 32;
            int count = Mathf.Max(1, Mathf.CeilToInt(Steps * (to - from)));
            for (int i = 0; i < count; i++)
            {
                float a = Mathf.Lerp(from, to, i / (float)count), b = Mathf.Lerp(from, to, (i + 1) / (float)count);
                Vector2 da = Dir(a), db = Dir(b);
                var c = Color.Lerp(fromColour, toColour, (a - from) / Mathf.Max(0.0001f, to - from));
                Shapes.Quad(vh, centre + da * inner, centre + da * outer, centre + db * outer, centre + db * inner, c);
            }
        }

        static Vector2 Dir(float share)
        {
            float angle = Mathf.PI * (1 - share);
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }

        public void Redraw() => SetVerticesDirty();
    }
}
