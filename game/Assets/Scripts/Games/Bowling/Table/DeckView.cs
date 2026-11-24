using UnityEngine;
using UnityEngine.UI;

namespace Starflare.Bowling
{
    /// <summary>
    /// The close-up of the deck for the hit, from above: what the pin sim does, shown as it is.
    /// With the deck's picture (<c>deck_top</c>), the pins stand on its painted spots and the
    /// ones that go down lie on their sides; without it, flat shapes, the down pins grey.
    /// </summary>
    public sealed class DeckView : MaskableGraphic
    {
        const double FromFt = LaneGeometry.HeadPinFt - 1.4;

        /// <summary>
        /// Where the deck picture's spots are, measured off it (tools/import_bowling.py prints
        /// these): the head pin's spot as a share of the picture from its left and its top, and
        /// how much of the picture's width an inch is across the lane and down it. The spots are
        /// 12 in apart across but painted 9 in apart down, not 10.4, so down has its own scale.
        /// </summary>
        const float HeadX = 0.4994f, HeadY = 0.7697f, InchAcross = 0.018296f, InchDown = 0.015851f;

        /// <summary>The moment of the hit to show; null for an empty deck.</summary>
        public PinFrame? Frame;

        /// <summary>The pins that were standing before this ball: the rest were swept and aren't drawn.</summary>
        public PinDeck Before;

        public Color BallColour = Color.red;

        Sprite _picture;
        SpriteQuads _down, _standing, _ball;
        RectMask2D _clip;

        public override Texture mainTexture => _picture != null ? _picture.texture : s_WhiteTexture;

        /// <summary>Sets the deck's picture and the pins' and adds their layers. Call once.</summary>
        public void Init(Sprite picture, Sprite pinTop, Sprite pinDown)
        {
            _picture = picture;
            _down = SpriteQuads.Make(transform, "Down Pins", DrawDown);
            _down.Sprite = pinDown;
            _standing = SpriteQuads.Make(transform, "Standing Pins", DrawStanding);
            _standing.Sprite = pinTop;
            _ball = SpriteQuads.Make(transform, "Ball", DrawBall);
            // Pins knocked into the gutters stay inside the picture, not over the lane round it.
            if (picture != null) _clip = gameObject.AddComponent<RectMask2D>();
            ClipToPicture();
            SetMaterialDirty();
            SetVerticesDirty();
        }

        /// <summary>The ball's picture for this delivery; null draws it in its colour.</summary>
        public Sprite BallSprite
        {
            set { if (_ball != null) _ball.Sprite = value; }
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            var r = rectTransform.rect;
            if (r.width <= 0 || r.height <= 0) return;
            if (_picture != null)
            {
                var box = PictureBox(r);
                int i = vh.currentVertCount;
                vh.AddVert(new Vector2(box.xMin, box.yMin), color, new Vector2(0, 0));
                vh.AddVert(new Vector2(box.xMin, box.yMax), color, new Vector2(0, 1));
                vh.AddVert(new Vector2(box.xMax, box.yMax), color, new Vector2(1, 1));
                vh.AddVert(new Vector2(box.xMax, box.yMin), color, new Vector2(1, 0));
                vh.AddTriangle(i, i + 1, i + 2);
                vh.AddTriangle(i, i + 2, i + 3);
                return;
            }
            Shapes.Box(vh, r, BallLook.Gutter);
            var laneLeft = Map(r, new Vec2(-LaneGeometry.HalfWidth, LaneGeometry.Y(FromFt)));
            var laneRight = Map(r, new Vec2(LaneGeometry.HalfWidth, LaneGeometry.Y(LaneGeometry.PitFt)));
            Shapes.Box(vh, Rect.MinMaxRect(laneLeft.x, laneLeft.y, laneRight.x, laneRight.y), BallLook.Wood);
            Shapes.Box(vh, Rect.MinMaxRect(r.xMin, laneRight.y, r.xMax, r.yMax), BallLook.Pit);
        }

        void DrawStanding(VertexHelper vh, Rect r, SpriteQuads layer)
        {
            if (Frame == null || Before == null) return;
            var frame = Frame.Value;
            for (int i = 0; i < PinDeck.Pins; i++)
            {
                if (!Before.IsStanding(i) || frame.Down[i]) continue;
                var at = Map(r, frame.Pins[i]);
                var (across, down) = Size(r, PinDeck.PinRadius * 2);
                if (layer.Sprite != null)
                {
                    layer.Turned(vh, at, across * 1.08f, down * 1.08f, 0, Color.white);
                    continue;
                }
                Shapes.Circle(vh, at, across / 2, BallLook.Pin, down / across, 16);
                Shapes.Circle(vh, at, across * 0.22f, BallLook.PinNeck, down / across, 12);
            }
        }

        /// <summary>
        /// The pins that went down, on their sides, lying the way they were knocked: from their
        /// spot toward where they are now.
        /// </summary>
        void DrawDown(VertexHelper vh, Rect r, SpriteQuads layer)
        {
            if (Frame == null || Before == null) return;
            var frame = Frame.Value;
            for (int i = 0; i < PinDeck.Pins; i++)
            {
                if (!Before.IsStanding(i) || !frame.Down[i]) continue;
                var at = Map(r, frame.Pins[i]);
                var (across, _) = Size(r, PinDeck.PinRadius * 2);
                if (layer.Sprite != null)
                {
                    var moved = at - Map(r, Before.PositionOf(i));
                    float degrees = moved.sqrMagnitude > 1 ? Mathf.Atan2(moved.y, moved.x) * Mathf.Rad2Deg : 90 + i * 37;
                    // A pin is 15 in long; the picture's pin runs left to right, its head on the right.
                    var (length, _) = Size(r, 15 * LaneGeometry.MetresPerInch);
                    // The picture's pin has its outline and a little margin round it.
                    layer.Turned(vh, at, length * 0.88f, length * 0.88f * layer.Aspect, degrees, Color.white);
                    continue;
                }
                Shapes.Circle(vh, at, across * 0.65f, BallLook.PinDown, 1, 16);
            }
        }

        void DrawBall(VertexHelper vh, Rect r, SpriteQuads layer)
        {
            if (Frame == null || Before == null || Frame.Value.BallGone) return;
            var at = Map(r, Frame.Value.Ball);
            var (across, down) = Size(r, 0.216);
            if (layer.Sprite != null)
            {
                layer.Turned(vh, at, across * 1.05f, across * 1.05f * layer.Aspect, 0, Color.white);
                return;
            }
            Shapes.Circle(vh, at, across / 2, BallColour, down / across, 24);
        }

        /// <summary>The picture, as large as fits the rect without stretching, centred.</summary>
        Rect PictureBox(Rect r)
        {
            float aspect = _picture.rect.width / _picture.rect.height;
            float width = Mathf.Min(r.width, r.height * aspect), height = width / aspect;
            return new Rect(r.center.x - width / 2, r.center.y - height / 2, width, height);
        }

        /// <summary>How big something this many metres wide is on screen, across the lane and down it.</summary>
        (float across, float down) Size(Rect r, double metres)
        {
            if (_picture == null)
            {
                float s = Scale(r) * (float)metres;
                return (s, s);
            }
            var box = PictureBox(r);
            float inches = (float)(metres / LaneGeometry.MetresPerInch);
            return (box.width * InchAcross * inches, box.width * InchDown * inches);
        }

        float Scale(Rect r)
        {
            double depth = LaneGeometry.Y(LaneGeometry.PitFt + 0.4 - FromFt);
            return (float)System.Math.Min(r.width / (2 * LaneGeometry.KickbackX), r.height / depth);
        }

        /// <summary>Where a point on the deck (metres, the lane's frame) falls in the rect.</summary>
        Vector2 Map(Rect r, Vec2 at)
        {
            if (_picture != null)
            {
                var box = PictureBox(r);
                float inchesAcross = (float)(at.X / LaneGeometry.MetresPerInch);
                float inchesPast = (float)((at.Y - LaneGeometry.Y(LaneGeometry.HeadPinFt)) / LaneGeometry.MetresPerInch);
                return new Vector2(
                    box.xMin + box.width * (HeadX + InchAcross * inchesAcross),
                    box.yMax - box.height * HeadY + box.width * InchDown * inchesPast);
            }
            float s = Scale(r);
            return new Vector2(r.center.x + (float)at.X * s, r.yMin + (float)(at.Y - LaneGeometry.Y(FromFt)) * s);
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            ClipToPicture();
        }

        /// <summary>Insets the clip from the rect's edges to the picture's.</summary>
        void ClipToPicture()
        {
            if (_clip == null || _picture == null) return;
            var r = rectTransform.rect;
            if (r.width <= 0 || r.height <= 0) return;
            var box = PictureBox(r);
            _clip.padding = new Vector4(box.xMin - r.xMin, box.yMin - r.yMin, r.xMax - box.xMax, r.yMax - box.yMax);
        }

        public void Redraw()
        {
            SetVerticesDirty();
            if (_down != null) _down.Redraw();
            if (_standing != null) _standing.Redraw();
            if (_ball != null) _ball.Redraw();
        }
    }
}
