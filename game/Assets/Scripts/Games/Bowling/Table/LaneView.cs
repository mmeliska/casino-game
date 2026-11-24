using System;
using UnityEngine;
using UnityEngine.UI;

namespace Starflare.Bowling
{
    /// <summary>
    /// What's drawn over the lane's textured surfaces (<see cref="LaneSurface"/>): the ink, like
    /// the rest of the game's art (the lanes' edges, the board joints, the foul line, the dots
    /// and arrows), the capping and the pit, the racks on the lanes either side, and the ball and
    /// pins where the sim puts them. All through the same projection as the surfaces, so a board
    /// on screen is a board in the sim. It decides nothing.
    /// </summary>
    public sealed class LaneView : MaskableGraphic
    {
        static readonly int[] DotBoards = { 3, 5, 8, 11, 14, 20, 26, 29, 32, 35, 37 };

        static readonly Color Ink = new Color(0.04f, 0.03f, 0.03f, 0.92f);
        static readonly Color UiGold = new Color(0.95f, 0.76f, 0.25f);

        /// <summary>The ball while it rolls, in the lane's frame; null when it's on the approach or gone.</summary>
        public Vec2? Ball;
        public Color BallColour = Color.red;

        /// <summary>The pins standing at rest.</summary>
        public PinDeck Deck;

        /// <summary>The board the aim sweep is on, the bowler's count; null when not aiming.</summary>
        public double? AimBoard;

        public Hand Hand;

        /// <summary>A full rack, for the lanes either side.</summary>
        PinDeck _rack;

        LaneProjection View => new LaneProjection(rectTransform.rect);

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            var r = rectTransform.rect;
            if (r.width <= 0 || r.height <= 0) return;
            var view = View;
            int n = LaneProjection.Neighbours;
            double pitch = LaneProjection.Pitch, room = (n + 0.5) * pitch;

            // The capping between lanes, then the pit across the far end of all of them.
            for (int lane = -n; lane < n; lane++)
            {
                double mid = (lane + 0.5) * pitch;
                Strip(vh, view, 0, LaneGeometry.PitFt, mid - LaneProjection.Capping / 2, mid + LaneProjection.Capping / 2,
                    new Color(0.2f, 0.13f, 0.08f), Math.Max(LaneProjection.Dim(lane), LaneProjection.Dim(lane + 1)));
            }
            Strip(vh, view, LaneGeometry.PitFt, LaneProjection.FarFt, -room, room, BallLook.Pit, 1, false);
            Shapes.Line(vh, new Vector2(r.xMin, view.FarY), new Vector2(r.xMax, view.FarY), 3, Ink);

            // Ink: every lane's edges and its gutters' outer edges; your lane's board joints.
            for (int lane = -n; lane <= n; lane++)
            {
                double centre = lane * pitch;
                foreach (double x in new[] { LaneGeometry.HalfWidth, -LaneGeometry.HalfWidth, LaneGeometry.KickbackX, -LaneGeometry.KickbackX })
                    InkLine(vh, view, centre + x, 0, LaneGeometry.PitFt, 0.012, Ink, false);
            }
            var joint = new Color(0.25f, 0.15f, 0.06f, 0.3f);
            for (int b = 1; b < LaneGeometry.Boards; b++)
                InkLine(vh, view, LaneGeometry.HalfWidth - b * LaneGeometry.BoardWidth, 0, 46, 0.003, joint, true);

            // The foul line across every lane and approach.
            Strip(vh, view, -0.1, 0.1, -room, room, Ink, 1, false);

            foreach (int board in DotBoards)
            {
                var at = view.Project(Lane(board, LaneGeometry.DotsFt));
                float radius = view.Px(0.014, LaneGeometry.DotsFt);
                Shapes.Circle(vh, at, radius + 1.5f, Ink, 0.5f, 10);
                Shapes.Circle(vh, at, radius, BallLook.Mark, 0.5f, 10);
            }
            foreach (int board in LaneGeometry.Arrows)
            {
                Arrow(vh, view, board, 0.55, 1.1, Ink);
                Arrow(vh, view, board, 0.4, 0.85, BallLook.Mark);
            }

            if (AimBoard.HasValue)
            {
                double aim = AimBoard.Value;
                var tip = view.Project(Lane(aim, LaneGeometry.ArrowsFt) + new Vec2(0, LaneGeometry.Y(1.6)));
                Shapes.Triangle(vh, view.Project(Lane(aim + 0.9, LaneGeometry.ArrowsFt - 1.4)), tip, view.Project(Lane(aim - 0.9, LaneGeometry.ArrowsFt - 1.4)), UiGold);
            }

            // The racks on the lanes either side, farthest first, then yours.
            var rack = _rack ??= PinDeck.Exact(0);
            for (int lane = n; lane >= 1; lane--)
                foreach (int side in new[] { -1, 1 })
                {
                    float dim = LaneProjection.Dim(lane) * 0.8f;
                    for (int i = PinDeck.Pins - 1; i >= 0; i--)
                        Pin(vh, view, rack.PositionOf(i) + new Vec2(side * lane * pitch, 0), dim);
                }
            if (Deck != null)
                for (int i = PinDeck.Pins - 1; i >= 0; i--)
                    if (Deck.IsStanding(i)) Pin(vh, view, Deck.PositionOf(i), 1);

            if (Ball.HasValue)
            {
                var at = Ball.Value;
                double ft = LaneGeometry.Feet(at.Y);
                float radius = view.Px(0.108, ft);
                var centre = view.Project(at) + new Vector2(0, radius * 0.6f);
                Shapes.Circle(vh, centre, radius + Mathf.Max(1.5f, radius * 0.12f), Ink, 0.9f);
                Shapes.Circle(vh, centre, radius, BallColour, 0.9f);
            }
        }

        /// <summary>A pin, outlined in ink, at this brightness.</summary>
        void Pin(VertexHelper vh, LaneProjection view, Vec2 at, float light)
        {
            double ft = LaneGeometry.Feet(at.Y);
            var foot = view.Project(at);
            float w = view.Px(PinDeck.PinRadius, ft);
            float h = w * 3.2f, edge = Mathf.Max(1f, w * 0.18f);
            Color body = BallLook.Pin * light, neck = BallLook.PinNeck * light, ink = Ink;
            body.a = neck.a = 1;
            Shapes.Circle(vh, foot + new Vector2(0, h * 0.35f), w + edge, ink, 1.4f, 14);
            Shapes.Circle(vh, foot + new Vector2(0, h * 0.85f), w * 0.55f + edge, ink, 1.1f, 10);
            Shapes.Circle(vh, foot + new Vector2(0, h * 0.35f), w, body, 1.4f, 14);
            Shapes.Box(vh, new Rect(foot.x - w * 0.45f, foot.y + h * 0.55f, w * 0.9f, h * 0.18f), neck);
            Shapes.Circle(vh, foot + new Vector2(0, h * 0.85f), w * 0.55f, body, 1.1f, 10);
        }

        /// <summary>An arrow on a board at 15 ft, pointing down the lane, this many boards either side of its tip.</summary>
        void Arrow(VertexHelper vh, LaneProjection view, int board, double halfBoards, double lengthFt, Color colour)
        {
            var tip = view.Project(Lane(board, LaneGeometry.ArrowsFt + lengthFt));
            var left = view.Project(Lane(board + halfBoards, LaneGeometry.ArrowsFt - lengthFt * 0.6));
            var right = view.Project(Lane(board - halfBoards, LaneGeometry.ArrowsFt - lengthFt * 0.6));
            Shapes.Triangle(vh, left, tip, right, colour);
        }

        /// <summary>
        /// A line down the lanes at one x, in short pieces because the projection bends it; this
        /// thick in metres, never under a pixel, and fading out down the lane if asked, so the
        /// board joints don't shimmer where they crowd together.
        /// </summary>
        static void InkLine(VertexHelper vh, LaneProjection view, double x, double fromFt, double toFt, double metres, Color colour, bool fade)
        {
            var rows = LaneProjection.Rows(fromFt, toFt, 2);
            for (int k = 0; k + 1 < rows.Length; k++)
            {
                double ft = (rows[k] + rows[k + 1]) / 2;
                var c = colour;
                if (fade) c.a *= Mathf.Clamp01(1 - (float)((ft - fromFt) / (toFt - fromFt)));
                if (c.a <= 0.01f) continue;
                Shapes.Line(vh, view.Project(x, rows[k]), view.Project(x, rows[k + 1]), Mathf.Max(1f, view.Px(metres, ft)), c);
            }
        }

        /// <summary>A band across the lanes between two feet and two x's (metres), lit like the lane.</summary>
        static void Strip(VertexHelper vh, LaneProjection view, double fromFt, double toFt, double fromX, double toX, Color colour, float dim, bool lit = true)
        {
            var rows = LaneProjection.Rows(fromFt, toFt, 3);
            for (int k = 0; k + 1 < rows.Length; k++)
            {
                float light = lit ? LaneProjection.Light(rows[k]) * dim : 1;
                var c = new Color(colour.r * light, colour.g * light, colour.b * light, colour.a);
                Shapes.Quad(vh, view.Project(fromX, rows[k]), view.Project(fromX, rows[k + 1]), view.Project(toX, rows[k + 1]), view.Project(toX, rows[k]), c);
            }
        }

        /// <summary>A point on the lane by the bowler's board count.</summary>
        Vec2 Lane(double board, double feet)
        {
            double laneBoard = Hand == Hand.Right ? board : LaneGeometry.Mirror(board);
            return LaneGeometry.Point(laneBoard, feet);
        }

        /// <summary>Where a lane point (metres, the lane's frame) falls in this view's rect.</summary>
        public Vector2 Project(Vec2 at) => View.Project(at);

        /// <summary>Screen units per metre across, at a depth.</summary>
        public double PxPerMetre(double ft) => View.PxPerMetre(ft);

        /// <summary>The lane's x (metres) under a point of this rect at a depth: the approach ball's drag.</summary>
        public double LaneXAt(float localX, double ft) => View.LaneXAt(localX, ft);

        /// <summary>The ball's size on the approach, for the ball you drag.</summary>
        public float BallPx(double ft) => View.Px(0.108, ft);

        /// <summary>Redraws: call when anything it shows has moved.</summary>
        public void Redraw() => SetVerticesDirty();

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            SetVerticesDirty();
        }
    }
}
