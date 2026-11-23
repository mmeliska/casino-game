using UnityEngine;
using UnityEngine.UI;

namespace Starflare.Bowling
{
    /// <summary>Flat shapes for the drawn views: quads, circles and lines into a graphic's mesh.</summary>
    static class Shapes
    {
        /// <summary>Four corners in order around the edge.</summary>
        public static void Quad(VertexHelper vh, Vector2 a, Vector2 b, Vector2 c, Vector2 d, Color32 color)
        {
            int i = vh.currentVertCount;
            vh.AddVert(a, color, Vector2.zero);
            vh.AddVert(b, color, Vector2.zero);
            vh.AddVert(c, color, Vector2.zero);
            vh.AddVert(d, color, Vector2.zero);
            vh.AddTriangle(i, i + 1, i + 2);
            vh.AddTriangle(i, i + 2, i + 3);
        }

        public static void Box(VertexHelper vh, Rect r, Color32 color) =>
            Quad(vh, new Vector2(r.xMin, r.yMin), new Vector2(r.xMin, r.yMax), new Vector2(r.xMax, r.yMax), new Vector2(r.xMax, r.yMin), color);

        public static void Triangle(VertexHelper vh, Vector2 a, Vector2 b, Vector2 c, Color32 color)
        {
            int i = vh.currentVertCount;
            vh.AddVert(a, color, Vector2.zero);
            vh.AddVert(b, color, Vector2.zero);
            vh.AddVert(c, color, Vector2.zero);
            vh.AddTriangle(i, i + 1, i + 2);
        }

        /// <summary>An ellipse: a circle squashed to <paramref name="squash"/> of its height.</summary>
        public static void Circle(VertexHelper vh, Vector2 centre, float radius, Color32 color, float squash = 1, int segments = 24)
        {
            int i = vh.currentVertCount;
            vh.AddVert(centre, color, Vector2.zero);
            for (int s = 0; s <= segments; s++)
            {
                float a = s * Mathf.PI * 2 / segments;
                vh.AddVert(centre + new Vector2(Mathf.Cos(a) * radius, Mathf.Sin(a) * radius * squash), color, Vector2.zero);
                if (s > 0) vh.AddTriangle(i, i + s, i + s + 1);
            }
        }

        public static void Line(VertexHelper vh, Vector2 a, Vector2 b, float width, Color32 color)
        {
            var d = b - a;
            if (d.sqrMagnitude < 1e-6f) return;
            var n = new Vector2(-d.y, d.x).normalized * (width / 2);
            Quad(vh, a - n, a + n, b + n, b - n, color);
        }
    }
}
