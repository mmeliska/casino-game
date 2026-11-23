using UnityEngine;
using UnityEngine.UI;

namespace Starflare.Bowling
{
    /// <summary>
    /// After each throw, a small strip of the lane from above, the foul line at the left and the
    /// pins at the right, with the path the ball took: teal while it slid, gold once it rolled.
    /// This is how the player reads the oil: nobody sees oil on a lane, they see what the ball did on it.
    /// </summary>
    public sealed class PathTrace : MaskableGraphic
    {
        static readonly Color Slid = new Color(0.36f, 0.79f, 0.82f);
        static readonly Color Rolled = new Color(0.95f, 0.76f, 0.25f);

        /// <summary>The last throw's path; null for none yet.</summary>
        public BallPath Path;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            var r = rectTransform.rect;
            if (r.width <= 0 || r.height <= 0) return;
            Shapes.Box(vh, r, BallLook.Gutter);
            float inset = r.height * 0.12f;
            Shapes.Box(vh, Rect.MinMaxRect(r.xMin, r.yMin + inset, r.xMax, r.yMax - inset), BallLook.Wood);
            foreach (int board in LaneGeometry.Arrows)
                Shapes.Circle(vh, Map(LaneGeometry.Point(board, LaneGeometry.ArrowsFt)), 3, BallLook.Mark, 1, 8);
            Shapes.Circle(vh, Map(LaneGeometry.Point(LaneGeometry.MiddleBoard, LaneGeometry.HeadPinFt)), 4, BallLook.Pin, 1, 8);
            if (Path == null) return;
            var samples = Path.Samples;
            for (int i = 1; i < samples.Count; i++)
                Shapes.Line(vh, Map(samples[i - 1].Position), Map(samples[i].Position), 4, samples[i].Slip > 0 ? Slid : Rolled);
            Shapes.Circle(vh, Map(Path.End.Position), 6, Path.Gutter ? BallLook.Pit : Rolled, 1, 12);
        }

        Vector2 Map(Vec2 at)
        {
            var r = rectTransform.rect;
            float inset = r.height * 0.12f;
            float across = (float)(at.X / LaneGeometry.HalfWidth);
            return new Vector2(r.xMin + (float)(at.Y / LaneGeometry.Y(LaneGeometry.HeadPinFt + 1)) * r.width,
                r.center.y - across * (r.height / 2 - inset));
        }

        public void Redraw() => SetVerticesDirty();
    }
}
