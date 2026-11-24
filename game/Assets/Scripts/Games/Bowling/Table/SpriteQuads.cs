using System;
using UnityEngine;
using UnityEngine.UI;

namespace Starflare.Bowling
{
    /// <summary>
    /// A layer of the lane or the deck drawn with one sprite as many times as it's needed (every
    /// pin standing, the ball), or in flat shapes while the sprite isn't in. Its owner says what
    /// goes where, given the layer's rect and its sprite; it's redrawn when the owner redraws.
    /// </summary>
    public sealed class SpriteQuads : MaskableGraphic
    {
        Sprite _sprite;
        Action<VertexHelper, Rect, SpriteQuads> _draw;

        public Sprite Sprite
        {
            get => _sprite;
            set
            {
                if (_sprite == value) return;
                _sprite = value;
                SetMaterialDirty();
                SetVerticesDirty();
            }
        }

        public override Texture mainTexture => _sprite != null ? _sprite.texture : s_WhiteTexture;

        /// <summary>A layer stretched over its parent, drawn by <paramref name="draw"/>.</summary>
        public static SpriteQuads Make(Transform parent, string name, Action<VertexHelper, Rect, SpriteQuads> draw)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(SpriteQuads));
            go.transform.SetParent(parent, false);
            var rect = (RectTransform)go.transform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = rect.offsetMax = Vector2.zero;
            var layer = go.GetComponent<SpriteQuads>();
            layer.raycastTarget = false;
            layer._draw = draw;
            return layer;
        }

        /// <summary>
        /// The sprite as a quad from its bottom-left corner round to its bottom-right, tinted.
        /// </summary>
        public void Quad(VertexHelper vh, Vector2 bottomLeft, Vector2 topLeft, Vector2 topRight, Vector2 bottomRight, Color tint)
        {
            var uv = UvRect();
            int i = vh.currentVertCount;
            vh.AddVert(bottomLeft, tint, new Vector2(uv.xMin, uv.yMin));
            vh.AddVert(topLeft, tint, new Vector2(uv.xMin, uv.yMax));
            vh.AddVert(topRight, tint, new Vector2(uv.xMax, uv.yMax));
            vh.AddVert(bottomRight, tint, new Vector2(uv.xMax, uv.yMin));
            vh.AddTriangle(i, i + 1, i + 2);
            vh.AddTriangle(i, i + 2, i + 3);
        }

        /// <summary>The sprite upright in a box, tinted.</summary>
        public void Box(VertexHelper vh, Rect box, Color tint) =>
            Quad(vh, new Vector2(box.xMin, box.yMin), new Vector2(box.xMin, box.yMax), new Vector2(box.xMax, box.yMax), new Vector2(box.xMax, box.yMin), tint);

        /// <summary>The sprite centred on a point, this wide and tall, turned by an angle (degrees, anticlockwise).</summary>
        public void Turned(VertexHelper vh, Vector2 centre, float width, float height, float degrees, Color tint)
        {
            var across = (Vector2)(Quaternion.Euler(0, 0, degrees) * Vector3.right) * (width / 2);
            var up = new Vector2(-across.y, across.x) * (height / width);
            Quad(vh, centre - across - up, centre - across + up, centre + across + up, centre + across - up, tint);
        }

        /// <summary>Height over width of the sprite's picture; 1 without one.</summary>
        public float Aspect => _sprite == null ? 1 : _sprite.rect.height / _sprite.rect.width;

        Rect UvRect()
        {
            if (_sprite == null) return new Rect(0, 0, 1, 1);
            var tex = _sprite.texture;
            var r = _sprite.textureRect;
            return new Rect(r.x / tex.width, r.y / tex.height, r.width / tex.width, r.height / tex.height);
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            var r = rectTransform.rect;
            if (r.width <= 0 || r.height <= 0 || _draw == null) return;
            _draw(vh, r, this);
        }

        public void Redraw() => SetVerticesDirty();

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            SetVerticesDirty();
        }
    }
}
