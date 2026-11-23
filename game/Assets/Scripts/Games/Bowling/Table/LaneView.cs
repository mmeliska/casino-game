using System;
using UnityEngine;
using UnityEngine.UI;

namespace Starflare.Bowling
{
    /// <summary>
    /// The lane from behind and above the foul line, like a 90s console game: the approach at the
    /// bottom, the arrows a third of the way up, the pins small at the top. Flat shapes until the
    /// plate lands. It projects the sim's lane coordinates onto the screen and draws the ball and
    /// pins where the sim puts them; it decides nothing.
    /// </summary>
    public sealed class LaneView : MaskableGraphic
    {
        /// <summary>How far down the screen sits from the foul line, for height: larger is flatter.</summary>
        const double DepthEye = 100;

        /// <summary>How fast the lane narrows toward the pins: smaller narrows more.</summary>
        const double WidthEye = 15;

        const double NearFt = -2.5, FarFt = LaneGeometry.PitFt + 0.6;

        /// <summary>Half the lane's width at the foul line, as a share of the view's width.</summary>
        const float FoulHalf = 0.3f;

        static readonly int[] DotBoards = { 3, 5, 8, 11, 14, 20, 26, 29, 32, 35, 37 };

        /// <summary>The ball while it rolls, in the lane's frame; null when it's on the approach or gone.</summary>
        public Vec2? Ball;
        public Color BallColour = Color.red;

        /// <summary>The pins standing at rest.</summary>
        public PinDeck Deck;

        /// <summary>The board the aim sweep is on, the bowler's count; null when not aiming.</summary>
        public double? AimBoard;

        public Hand Hand;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            var r = rectTransform.rect;
            if (r.width <= 0 || r.height <= 0) return;

            double gutterEdge = LaneGeometry.KickbackX - 0.03;
            Strip(vh, NearFt, 0, LaneGeometry.KickbackX, BallLook.Approach);
            Strip(vh, 0, FarFt, gutterEdge, BallLook.Gutter);
            Strip(vh, 0, LaneGeometry.PitFt, LaneGeometry.HalfWidth, BallLook.Wood);
            Strip(vh, LaneGeometry.PitFt, FarFt, gutterEdge, BallLook.Pit);
            Strip(vh, -0.08, 0.08, LaneGeometry.HalfWidth, BallLook.Mark);

            foreach (int board in DotBoards)
                Shapes.Circle(vh, Project(Lane(board, LaneGeometry.DotsFt)), Px(0.012, LaneGeometry.DotsFt), BallLook.Mark, 0.5f, 10);
            foreach (int board in LaneGeometry.Arrows)
            {
                var tip = Project(Lane(board, LaneGeometry.ArrowsFt + 0.9));
                var left = Project(Lane(board + 0.45, LaneGeometry.ArrowsFt - 0.6));
                var right = Project(Lane(board - 0.45, LaneGeometry.ArrowsFt - 0.6));
                Shapes.Triangle(vh, left, tip, right, BallLook.Mark);
            }

            if (AimBoard.HasValue)
            {
                var at = Lane(AimBoard.Value, LaneGeometry.ArrowsFt);
                var tip = Project(at + new Vec2(0, LaneGeometry.Y(1.6)));
                Shapes.Triangle(vh, Project(Lane(AimBoard.Value + 0.9, LaneGeometry.ArrowsFt - 1.4)), tip, Project(Lane(AimBoard.Value - 0.9, LaneGeometry.ArrowsFt - 1.4)), UiGold);
            }

            // Pins from the back row forward, so nearer ones draw over farther.
            if (Deck != null)
                for (int i = PinDeck.Pins - 1; i >= 0; i--)
                    if (Deck.IsStanding(i)) Pin(vh, Deck.PositionOf(i), true);

            if (Ball.HasValue)
            {
                var at = Ball.Value;
                double ft = LaneGeometry.Feet(at.Y);
                Shapes.Circle(vh, Project(at) + new Vector2(0, Px(0.108, ft) * 0.6f), Px(0.108, ft), BallColour, 0.9f);
            }
        }

        static readonly Color UiGold = new Color(0.95f, 0.76f, 0.25f);

        void Pin(VertexHelper vh, Vec2 at, bool standing)
        {
            double ft = LaneGeometry.Feet(at.Y);
            var foot = Project(at);
            float w = Px(PinDeck.PinRadius, ft);
            if (!standing)
            {
                Shapes.Circle(vh, foot, w * 1.2f, BallLook.PinDown, 0.4f, 12);
                return;
            }
            float h = w * 3.2f;
            Shapes.Circle(vh, foot + new Vector2(0, h * 0.35f), w, BallLook.Pin, 1.4f, 14);
            Shapes.Box(vh, new Rect(foot.x - w * 0.45f, foot.y + h * 0.55f, w * 0.9f, h * 0.18f), BallLook.PinNeck);
            Shapes.Circle(vh, foot + new Vector2(0, h * 0.85f), w * 0.55f, BallLook.Pin, 1.1f, 10);
        }

        /// <summary>A point on the lane by the bowler's board count.</summary>
        Vec2 Lane(double board, double feet)
        {
            double laneBoard = Hand == Hand.Right ? board : LaneGeometry.Mirror(board);
            return LaneGeometry.Point(laneBoard, feet);
        }

        /// <summary>A band across the lane between two feet, this far out each side of the middle.</summary>
        void Strip(VertexHelper vh, double fromFt, double toFt, double halfWidth, Color colour)
        {
            Shapes.Quad(vh,
                Project(new Vec2(-halfWidth, LaneGeometry.Y(fromFt))),
                Project(new Vec2(-halfWidth, LaneGeometry.Y(toFt))),
                Project(new Vec2(halfWidth, LaneGeometry.Y(toFt))),
                Project(new Vec2(halfWidth, LaneGeometry.Y(fromFt))), colour);
        }

        static double DepthScale(double ft) => DepthEye / (DepthEye + ft);
        static double WidthScale(double ft) => WidthEye / (WidthEye + Math.Max(ft, -WidthEye * 0.5));

        /// <summary>Where a lane point (metres, the lane's frame) falls in this view's rect.</summary>
        public Vector2 Project(Vec2 at)
        {
            var r = rectTransform.rect;
            double ft = LaneGeometry.Feet(at.Y);
            double v = (DepthScale(NearFt) - DepthScale(ft)) / (DepthScale(NearFt) - DepthScale(FarFt));
            float x = r.center.x + (float)(at.X * PxPerMetre(ft));
            return new Vector2(x, r.yMin + (float)v * r.height);
        }

        /// <summary>Screen units per metre across, at a depth.</summary>
        public double PxPerMetre(double ft)
        {
            var r = rectTransform.rect;
            double foulHalfPx = Math.Min(r.width * FoulHalf, r.height * 0.22);
            return WidthScale(ft) / WidthScale(0) * foulHalfPx / LaneGeometry.HalfWidth;
        }

        float Px(double metres, double ft) => (float)(metres * PxPerMetre(ft));

        /// <summary>The lane's x (metres) under a point of this rect at a depth: the approach ball's drag.</summary>
        public double LaneXAt(float localX, double ft) => (localX - rectTransform.rect.center.x) / PxPerMetre(ft);

        /// <summary>The ball's size on the approach, for the ball you drag.</summary>
        public float BallPx(double ft) => Px(0.108, ft);

        /// <summary>Redraws: call when anything it shows has moved.</summary>
        public void Redraw() => SetVerticesDirty();

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            SetVerticesDirty();
        }
    }
}
