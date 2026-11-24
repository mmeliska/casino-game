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
        static readonly Color Ink = new Color(0.04f, 0.03f, 0.03f, 0.92f);
        static readonly Color UiGold = new Color(0.95f, 0.76f, 0.25f);
        static readonly Color PitLit = new Color(0.16f, 0.12f, 0.08f);
        static readonly Color Ceiling = new Color(0.09f, 0.07f, 0.06f);

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

        SpriteQuads _pins, _ball;

        /// <summary>The board your ball's on, the bowler's count: where the aim arrow swings from.</summary>
        public int StandBoard = LaneGeometry.MiddleBoard;

        /// <summary>
        /// The space the layout leaves the lane, between the scores and the meters: it places the
        /// lane, which is drawn over the whole of this view (the screen).
        /// </summary>
        public RectTransform Frame;

        readonly System.Collections.Generic.List<Graphic> _under = new System.Collections.Generic.List<Graphic>();
        Rect _lastFrame;

        LaneProjection View => ProjectionOver(rectTransform.rect);

        /// <summary>The projection for a rect the size of this view (the surfaces under it share it).</summary>
        internal LaneProjection ProjectionOver(Rect bounds) => new LaneProjection(bounds, FrameRect());

        /// <summary>The frame in this view's space; the whole view if there's no frame.</summary>
        Rect FrameRect()
        {
            if (Frame == null) return rectTransform.rect;
            var r = Frame.rect;
            Vector2 min = rectTransform.InverseTransformPoint(Frame.TransformPoint(r.min));
            Vector2 max = rectTransform.InverseTransformPoint(Frame.TransformPoint(r.max));
            var frame = Rect.MinMaxRect(min.x, min.y, max.x, max.y);
            return frame.width > 1 && frame.height > 1 ? frame : rectTransform.rect;
        }

        /// <summary>A surface drawn under this view, through its projection: redrawn with it.</summary>
        public void AddUnder(Graphic surface) => _under.Add(surface);

        /// <summary>Redraws if the layout has moved the frame since the last look; call every frame.</summary>
        public void FollowFrame()
        {
            var frame = FrameRect();
            if (frame == _lastFrame) return;
            _lastFrame = frame;
            Redraw();
        }

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
            // The pit behind the pins, dark up to the masking panel's foot; the deck's lights catch
            // its near edge.
            int i0 = vh.currentVertCount;
            vh.AddVert(new Vector2(r.xMin, view.FarY), PitLit, Vector2.zero);
            vh.AddVert(new Vector2(r.xMin, view.MaskingY), BallLook.Pit, Vector2.zero);
            vh.AddVert(new Vector2(r.xMax, view.MaskingY), BallLook.Pit, Vector2.zero);
            vh.AddVert(new Vector2(r.xMax, view.FarY), PitLit, Vector2.zero);
            vh.AddTriangle(i0, i0 + 1, i0 + 2);
            vh.AddTriangle(i0, i0 + 2, i0 + 3);
            Shapes.Line(vh, new Vector2(r.xMin, view.MaskingY), new Vector2(r.xMax, view.MaskingY), 4, Ink);
            // The dark ceiling above the masking panel, behind the cash and the scores.
            if (view.MaskingTopY < r.yMax)
            {
                Shapes.Quad(vh, new Vector2(r.xMin, view.MaskingTopY), new Vector2(r.xMin, r.yMax),
                    new Vector2(r.xMax, r.yMax), new Vector2(r.xMax, view.MaskingTopY), Ceiling);
                Shapes.Line(vh, new Vector2(r.xMin, view.MaskingTopY), new Vector2(r.xMax, view.MaskingTopY), 4, Ink);
            }

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

            foreach (int board in LaneGeometry.DotBoards) Dot(vh, view, board, LaneGeometry.DotsFt);
            foreach (int board in LaneGeometry.ApproachDotBoards) Dot(vh, view, board, LaneGeometry.ApproachDotsFt);
            foreach (int board in LaneGeometry.Arrows)
            {
                Arrow(vh, view, board, LaneGeometry.ArrowWidthBoards / 2 + 0.35, LaneGeometry.ArrowLengthFt + 0.25, Ink);
                Arrow(vh, view, board, LaneGeometry.ArrowWidthBoards / 2, LaneGeometry.ArrowLengthFt, BallLook.Mark);
            }
            foreach (int board in LaneGeometry.RangeFinderBoards)
                Strip(vh, view, LaneGeometry.RangeFindersFromFt, LaneGeometry.RangeFindersToFt,
                    Lane(board + 0.3, 0).X, Lane(board - 0.3, 0).X, BallLook.Mark, 1, false);

            if (AimBoard.HasValue) AimArrow(vh, view, AimBoard.Value);
        }

        /// <summary>
        /// Adds the pins and the rolling ball as layers over the lane, drawn from their sprites
        /// (or in shapes while they aren't in). Call once, before anything else goes on the lane.
        /// </summary>
        public void AddLayers(Sprite pinUp)
        {
            _pins = SpriteQuads.Make(transform, "Pins", DrawPins);
            _pins.Sprite = pinUp;
            _ball = SpriteQuads.Make(transform, "Rolling Ball", DrawBall);
        }

        /// <summary>The ball's picture for this delivery; null draws it in its colour.</summary>
        public Sprite BallSprite
        {
            set { if (_ball != null) _ball.Sprite = value; }
        }

        /// <summary>The racks on the lanes either side, farthest first, then yours.</summary>
        void DrawPins(VertexHelper vh, Rect r, SpriteQuads layer)
        {
            var view = ProjectionOver(r);
            var rack = _rack ??= PinDeck.Exact(0);
            double pitch = LaneProjection.Pitch;
            for (int lane = LaneProjection.Neighbours; lane >= 1; lane--)
                foreach (int side in new[] { -1, 1 })
                {
                    float dim = LaneProjection.Dim(lane) * 0.8f;
                    for (int i = PinDeck.Pins - 1; i >= 0; i--)
                        Pin(vh, view, layer, rack.PositionOf(i) + new Vec2(side * lane * pitch, 0), dim);
                }
            if (Deck != null)
                for (int i = PinDeck.Pins - 1; i >= 0; i--)
                    if (Deck.IsStanding(i)) Pin(vh, view, layer, Deck.PositionOf(i), 1);
        }

        void DrawBall(VertexHelper vh, Rect r, SpriteQuads layer)
        {
            if (!Ball.HasValue) return;
            var view = ProjectionOver(r);
            var at = Ball.Value;
            double ft = LaneGeometry.Feet(at.Y);
            float radius = view.Px(0.108, ft);
            var foot = view.Project(at);
            if (layer.Sprite != null)
            {
                float width = radius * 2.1f, height = width * layer.Aspect * 0.95f;
                layer.Box(vh, new Rect(foot.x - width / 2, foot.y - radius * 0.35f, width, height), Color.white);
                return;
            }
            var centre = foot + new Vector2(0, radius * 0.6f);
            Shapes.Circle(vh, centre, radius + Mathf.Max(1.5f, radius * 0.12f), Ink, 0.9f);
            Shapes.Circle(vh, centre, radius, BallColour, 0.9f);
        }

        /// <summary>A pin standing on its spot at this brightness: its picture, or a shape outlined in ink.</summary>
        static void Pin(VertexHelper vh, LaneProjection view, SpriteQuads layer, Vec2 at, float light)
        {
            double ft = LaneGeometry.Feet(at.Y);
            var foot = view.Project(at);
            float w = view.Px(PinDeck.PinRadius, ft);
            if (layer.Sprite != null)
            {
                // The picture's pin, outline and all, a little wider than the pin's belly.
                float width = w * 2.3f, height = width * layer.Aspect;
                layer.Box(vh, new Rect(foot.x - width / 2, foot.y - w * 0.3f, width, height), new Color(light, light, light, 1));
                return;
            }
            float h = w * 3.2f, edge = Mathf.Max(1f, w * 0.18f);
            Color body = BallLook.Pin * light, neck = BallLook.PinNeck * light, ink = Ink;
            body.a = neck.a = 1;
            Shapes.Circle(vh, foot + new Vector2(0, h * 0.35f), w + edge, ink, 1.4f, 14);
            Shapes.Circle(vh, foot + new Vector2(0, h * 0.85f), w * 0.55f + edge, ink, 1.1f, 10);
            Shapes.Circle(vh, foot + new Vector2(0, h * 0.35f), w, body, 1.4f, 14);
            Shapes.Box(vh, new Rect(foot.x - w * 0.45f, foot.y + h * 0.55f, w * 0.9f, h * 0.18f), neck);
            Shapes.Circle(vh, foot + new Vector2(0, h * 0.85f), w * 0.55f, body, 1.1f, 10);
        }

        /// <summary>
        /// The aim: a gold arrow from your ball over the board you're aiming at, where its lane
        /// arrow is, and on a little; it swings across the lane while the aim runs.
        /// </summary>
        void AimArrow(VertexHelper vh, LaneProjection view, double aim)
        {
            var from = Lane(StandBoard, 0);
            var over = Lane(aim, LaneGeometry.ArrowFt(aim));
            var tip = from + (over - from) * 1.35;
            var back = from + (over - from) * 1.15;
            var a = view.Project(from);
            var b = view.Project(back);
            float width = Mathf.Max(4f, view.Px(0.035, LaneGeometry.ArrowFt(aim)));
            var side = (Vector2)(view.Project(back + new Vec2(LaneGeometry.BoardWidth * 2.2, 0)) - b);
            Shapes.Line(vh, a, b, width + 4, Ink);
            Shapes.Triangle(vh, b - side * 1.2f, view.Project(tip) + (view.Project(tip) - b).normalized * 3, b + side * 1.2f, Ink);
            Shapes.Line(vh, a, b, width, UiGold);
            Shapes.Triangle(vh, b - side, view.Project(tip), b + side, UiGold);
            Shapes.Circle(vh, view.Project(over), width * 0.9f, UiGold, 0.5f, 12);
        }

        /// <summary>An arrow on its board in the V, pointing down the lane: this long, this many boards either side of its middle at the base.</summary>
        void Arrow(VertexHelper vh, LaneProjection view, int board, double halfBoards, double lengthFt, Color colour)
        {
            double ft = LaneGeometry.ArrowFt(board);
            var tip = view.Project(Lane(board, ft + lengthFt / 2));
            var left = view.Project(Lane(board + halfBoards, ft - lengthFt / 2));
            var right = view.Project(Lane(board - halfBoards, ft - lengthFt / 2));
            Shapes.Triangle(vh, left, tip, right, colour);
        }

        /// <summary>A guide dot, ¾ in across, outlined in ink.</summary>
        void Dot(VertexHelper vh, LaneProjection view, int board, double ft)
        {
            var at = view.Project(Lane(board, ft));
            float radius = Mathf.Max(1.5f, view.Px(0.75 / 2 * LaneGeometry.MetresPerInch, ft) * 1.4f);
            Shapes.Circle(vh, at, radius + 1.5f, Ink, 0.5f, 10);
            Shapes.Circle(vh, at, radius, BallLook.Mark, 0.5f, 10);
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
        public void Redraw()
        {
            SetVerticesDirty();
            foreach (var surface in _under) surface.SetVerticesDirty();
            if (_pins != null) _pins.Redraw();
            if (_ball != null) _ball.Redraw();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            SetVerticesDirty();
        }
    }
}
