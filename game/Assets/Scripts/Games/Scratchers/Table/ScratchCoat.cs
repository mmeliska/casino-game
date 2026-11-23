using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Starflare.Scratchers
{
    /// <summary>
    /// The latex over one spot: a picture of its <see cref="ScratchMask"/>, silver with a
    /// grain, clear wherever the coin's been. Once you can read the spot the rest flakes away.
    /// It owns its texture, so <see cref="Dispose"/> it with the ticket.
    /// </summary>
    public sealed class ScratchCoat
    {
        /// <summary>The coin's half-width, in the ticket's own units (it's 800 across).</summary>
        public const float CoinRadius = 26f;

        /// <summary>How long the last of the latex takes to fall away once the spot's readable.</summary>
        const float FallAway = 0.2f;

        readonly RawImage _image;
        readonly Texture2D _texture;
        readonly Color32[] _pixels;
        readonly List<int> _cleared = new List<int>();
        bool _dirty;
        float _fade = 1;

        /// <param name="width">The spot's size in ticket units; the latex is one pixel to a unit.</param>
        public ScratchCoat(Transform parent, string name, float width, float height, Color tint, int seed)
        {
            int w = Mathf.Max(8, Mathf.RoundToInt(width)), h = Mathf.Max(8, Mathf.RoundToInt(height));
            Mask = new ScratchMask(w, h);
            _texture = new Texture2D(w, h, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp, name = name };
            _pixels = Paint(w, h, tint, seed);
            _texture.SetPixels32(_pixels);
            _texture.Apply();

            var go = new GameObject(name, typeof(RectTransform), typeof(RawImage));
            go.transform.SetParent(parent, false);
            _image = go.GetComponent<RawImage>();
            _image.texture = _texture;
            _image.raycastTarget = false;
        }

        public ScratchMask Mask { get; }
        public RectTransform Rect => _image.rectTransform;

        /// <summary>Enough is off to read the spot; the rest is falling or has fallen away.</summary>
        public bool Revealed => Mask.Readable;

        /// <summary>
        /// A stroke of the coin between two screen points. Only the part over this spot takes
        /// latex off; once enough is off the whole spot shows.
        /// </summary>
        public void Scratch(Vector2 screenFrom, Vector2 screenTo, Camera camera)
        {
            if (Revealed) return;
            var rect = Rect.rect;
            if (rect.width <= 0 || rect.height <= 0) return;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(Rect, screenFrom, camera, out var from)) return;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(Rect, screenTo, camera, out var to)) return;

            var reach = rect;
            reach.min -= Vector2.one * CoinRadius;
            reach.max += Vector2.one * CoinRadius;
            if (Mathf.Max(from.x, to.x) < reach.xMin || Mathf.Min(from.x, to.x) > reach.xMax
                || Mathf.Max(from.y, to.y) < reach.yMin || Mathf.Min(from.y, to.y) > reach.yMax) return;

            float sx = Mask.Width / rect.width, sy = Mask.Height / rect.height;
            _cleared.Clear();
            Mask.Scratch((from.x - rect.xMin) * sx, (from.y - rect.yMin) * sy, (to.x - rect.xMin) * sx, (to.y - rect.yMin) * sy,
                CoinRadius * sx, CoinRadius * sy, _cleared);
            foreach (int index in _cleared) _pixels[index].a = 0;
            _dirty |= _cleared.Count > 0;
            if (Revealed) Mask.ClearAll();
        }

        /// <summary>All of it off at once.</summary>
        public void Reveal()
        {
            Mask.ClearAll();
            _fade = 0;
            _image.enabled = false;
        }

        /// <summary>Draws this frame's scratching, and lets a revealed spot's last latex fall away.</summary>
        public void Tick(float seconds)
        {
            if (_dirty)
            {
                _texture.SetPixels32(_pixels);
                _texture.Apply();
                _dirty = false;
            }
            if (!Revealed || !_image.enabled) return;
            _fade = Mathf.Max(0, _fade - seconds / FallAway);
            _image.color = new Color(1, 1, 1, _fade);
            if (_fade <= 0) _image.enabled = false;
        }

        public void Dispose()
        {
            if (_texture != null) Object.Destroy(_texture);
        }

        /// <summary>
        /// Silver latex: a sheen down the spot, fine diagonal ribs and a speckle, in the
        /// ticket's tint, with a darker rim.
        /// </summary>
        static Color32[] Paint(int w, int h, Color tint, int seed)
        {
            var pixels = new Color32[w * h];
            var random = new System.Random(seed);
            for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
            {
                float sheen = 0.86f + 0.14f * (y / (float)h);
                float ribs = 0.04f * Mathf.Sin((x + y) * 0.45f);
                float speckle = (float)(random.NextDouble() - 0.5) * 0.1f;
                float rim = x < 2 || y < 2 || x >= w - 2 || y >= h - 2 ? 0.7f : 1f;
                float shade = (sheen + ribs + speckle) * rim;
                pixels[y * w + x] = new Color(tint.r * shade, tint.g * shade, tint.b * shade, 1);
            }
            return pixels;
        }
    }
}
