using UnityEngine;
using UnityEngine.UI;

namespace Starflare.Bowling
{
    /// <summary>
    /// After each throw, a narrow upright strip of the lane from above down the side of the
    /// screen, the foul line at the foot and the pins at the top, with the path the ball took:
    /// teal while it slid, gold once it rolled. This is how the player reads the oil: nobody sees
    /// oil on a lane, they see what the ball did on it.
    /// </summary>
    public sealed class PathTrace : MaskableGraphic
    {
        static readonly Color Slid = new Color(0.36f, 0.79f, 0.82f);
        static readonly Color Rolled = new Color(0.95f, 0.76f, 0.25f);
        static readonly Color Ink = new Color(0.04f, 0.03f, 0.03f, 0.9f);

        /// <summary>The last throw's path; null for none yet.</summary>
        public BallPath Path;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            var r = rectTransform.rect;
            if (r.width <= 0 || r.height <= 0) return;
            Shapes.Box(vh, new Rect(r.xMin - 2, r.yMin - 2, r.width + 4, r.height + 4), Ink);
            Shapes.Box(vh, r, BallLook.Gutter);
            float inset = r.width * 0.14f;
            Shapes.Box(vh, Rect.MinMaxRect(r.xMin + inset, r.yMin, r.xMax - inset, r.yMax), BallLook.Wood);
            foreach (int board in LaneGeometry.Arrows)
                Shapes.Circle(vh, Map(LaneGeometry.Point(board, LaneGeometry.ArrowFt(board))), 2.5f, BallLook.Mark, 1, 8);
            Shapes.Circle(vh, Map(LaneGeometry.Point(LaneGeometry.MiddleBoard, LaneGeometry.HeadPinFt)), 4, BallLook.Pin, 1, 8);
            if (Path == null) return;
            var samples = Path.Samples;
            for (int i = 1; i < samples.Count; i++)
                Shapes.Line(vh, Map(samples[i - 1].Position), Map(samples[i].Position), 3, samples[i].Slip > 0 ? Slid : Rolled);
            Shapes.Circle(vh, Map(Path.End.Position), 5, Path.Gutter ? BallLook.Pit : Rolled, 1, 12);
        }

        /// <summary>A lane point (the lane's frame, x positive toward board 1) in the strip, as the bowler sees it.</summary>
        Vector2 Map(Vec2 at)
        {
            var r = rectTransform.rect;
            float inset = r.width * 0.14f;
            float across = (float)(at.X / LaneGeometry.HalfWidth);
            return new Vector2(r.center.x + across * (r.width / 2 - inset),
                r.yMin + (float)(at.Y / LaneGeometry.Y(LaneGeometry.HeadPinFt + 1)) * r.height);
        }

        public void Redraw() => SetVerticesDirty();
    }
}
