using UnityEngine;
using UnityEngine.UI;

namespace Starflare.Bowling
{
    /// <summary>
    /// The close-up of the deck for the hit, from above: what the pin sim does, shown as it is.
    /// The pins that go down turn grey; the ones that stand stay white.
    /// </summary>
    public sealed class DeckView : MaskableGraphic
    {
        const double FromFt = LaneGeometry.HeadPinFt - 1.4;

        /// <summary>The moment of the hit to show; null for an empty deck.</summary>
        public PinFrame? Frame;

        /// <summary>The pins that were standing before this ball: the rest were swept and aren't drawn.</summary>
        public PinDeck Before;

        public Color BallColour = Color.red;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            var r = rectTransform.rect;
            if (r.width <= 0 || r.height <= 0) return;
            Shapes.Box(vh, r, BallLook.Gutter);
            var laneLeft = Map(new Vec2(-LaneGeometry.HalfWidth, LaneGeometry.Y(FromFt)));
            var laneRight = Map(new Vec2(LaneGeometry.HalfWidth, LaneGeometry.Y(LaneGeometry.PitFt)));
            Shapes.Box(vh, Rect.MinMaxRect(laneLeft.x, laneLeft.y, laneRight.x, laneRight.y), BallLook.Wood);
            Shapes.Box(vh, Rect.MinMaxRect(r.xMin, laneRight.y, r.xMax, r.yMax), BallLook.Pit);

            if (Frame == null || Before == null) return;
            var frame = Frame.Value;
            float pinR = Scale() * (float)PinDeck.PinRadius;
            for (int i = 0; i < PinDeck.Pins; i++)
            {
                if (!Before.IsStanding(i)) continue;
                var at = Map(frame.Pins[i]);
                if (frame.Down[i]) Shapes.Circle(vh, at, pinR * 1.3f, BallLook.PinDown, 1, 16);
                else
                {
                    Shapes.Circle(vh, at, pinR, BallLook.Pin, 1, 16);
                    Shapes.Circle(vh, at, pinR * 0.45f, BallLook.PinNeck, 1, 12);
                }
            }
            if (!frame.BallGone) Shapes.Circle(vh, Map(frame.Ball), Scale() * 0.108f, BallColour, 1, 24);
        }

        float Scale()
        {
            var r = rectTransform.rect;
            double depth = LaneGeometry.Y(LaneGeometry.PitFt + 0.4 - FromFt);
            return (float)System.Math.Min(r.width / (2 * LaneGeometry.KickbackX), r.height / depth);
        }

        Vector2 Map(Vec2 at)
        {
            var r = rectTransform.rect;
            float s = Scale();
            return new Vector2(r.center.x + (float)at.X * s, r.yMin + (float)(at.Y - LaneGeometry.Y(FromFt)) * s);
        }

        public void Redraw() => SetVerticesDirty();
    }
}
