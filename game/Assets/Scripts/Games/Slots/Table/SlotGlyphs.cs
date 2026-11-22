using UnityEngine;

namespace Starflare.Slots
{
    /// <summary>
    /// Pictures the machine draws for itself until a venue has art: the cherries and the
    /// marquee bulb's glow. Drawn once and shared, like UiKit's sprites; they're textures, not
    /// state.
    /// </summary>
    public static class SlotGlyphs
    {
        static Sprite _cherries, _glow;

        /// <summary>Two red cherries on green stems joined at the top, in colour.</summary>
        public static Sprite Cherries => _cherries ??= Draw(128, (x, y) =>
        {
            var red = new Color(0.78f, 0.06f, 0.1f);
            var shine = new Color(1f, 0.75f, 0.75f);
            var stem = new Color(0.2f, 0.45f, 0.12f);
            var outline = new Color(0.08f, 0.04f, 0.04f);

            float left = Disc(x, y, 40, 36, 24), right = Disc(x, y, 88, 32, 24);
            float fruit = Mathf.Min(left, right);
            if (fruit < -3)
            {
                bool glint = Disc(x, y, 32, 46, 6) < 0 || Disc(x, y, 80, 42, 6) < 0;
                return glint ? shine : red;
            }
            if (fruit < 0) return outline;

            // Each stem curves from its cherry up to a knot near the top right.
            float stems = Mathf.Min(Segment(x, y, 42, 58, 70, 112), Segment(x, y, 86, 54, 70, 112));
            float leaf = Ellipse(x, y, 88, 108, 18, 8);
            if (stems < 3.5f || leaf < 0) return stem;
            if (stems < 5.5f || leaf < 0.25f) return outline;
            return Color.clear;
        });

        /// <summary>A soft white disc fading out to its edge, for a lit bulb's glow.</summary>
        public static Sprite Glow => _glow ??= Draw(64, (x, y) =>
        {
            float d = Mathf.Sqrt((x - 32) * (x - 32) + (y - 32) * (y - 32)) / 32;
            float a = Mathf.Clamp01(1 - d);
            return new Color(1, 1, 1, a * a);
        });

        static float Disc(float x, float y, float cx, float cy, float r) =>
            Mathf.Sqrt((x - cx) * (x - cx) + (y - cy) * (y - cy)) - r;

        static float Ellipse(float x, float y, float cx, float cy, float rx, float ry)
        {
            float dx = (x - cx) / rx, dy = (y - cy) / ry;
            return dx * dx + dy * dy - 1;
        }

        static float Segment(float x, float y, float ax, float ay, float bx, float by)
        {
            var p = new Vector2(x - ax, y - ay);
            var d = new Vector2(bx - ax, by - ay);
            float t = Mathf.Clamp01(Vector2.Dot(p, d) / d.sqrMagnitude);
            return (p - d * t).magnitude;
        }

        static Sprite Draw(int size, System.Func<float, float, Color> colour)
        {
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp };
            var pixels = new Color[size * size];
            for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
                pixels[y * size + x] = colour(x + 0.5f, y + 0.5f);
            texture.SetPixels(pixels);
            texture.Apply();
            return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100);
        }
    }
}
