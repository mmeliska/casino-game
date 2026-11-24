using System;
using UnityEngine;

namespace Starflare.Bowling
{
    /// <summary>
    /// The textures the lane is drawn with, handed over by the venue (Millstone Lanes loads them;
    /// a game never loads its own art). Each one tiles: the boards and the approach in both
    /// directions, the gutter down its length, the masking panel across. Any that's missing is
    /// stood in for by a plain one made here, until the pictures are in.
    /// </summary>
    public sealed class LaneArt
    {
        // The textures tile; the sprites (the deck, the balls, the pins) are drawn as they are,
        // and while one isn't in, the lane draws that thing in flat shapes.

        /// <summary>The lane's maple, grain only, seen from above: the code draws the board joints.</summary>
        public Texture2D Boards;

        /// <summary>The worn approach behind the foul line; the code draws the dots and the line.</summary>
        public Texture2D Approach;

        /// <summary>The inside of a gutter, from the lane's edge outward across the picture.</summary>
        public Texture2D Gutter;

        /// <summary>The painted panel that hangs over the far end of the lanes.</summary>
        public Texture2D Masking;

        /// <summary>The deck from straight above, for the hit; its spots measured in <see cref="DeckView"/>.</summary>
        public Sprite Deck;

        /// <summary>The house ball and the two on Juan's counter, from a little above and behind.</summary>
        public Sprite BallHouse, BallUrethane, BallReactive;

        /// <summary>A pin upright (the lane), on its side (knocked down, on the deck) and from above (standing, on the deck).</summary>
        public Sprite PinUp, PinDown, PinTop;

        /// <summary>
        /// Each ball's surface, unwrapped like a globe (twice as wide as tall), for the rolling ball
        /// (<see cref="SpinningBall"/>); the holes are stamped on at <see cref="BallSpin.HolePlaces"/>.
        /// </summary>
        public Texture2D SurfaceHouse, SurfaceUrethane, SurfaceReactive;

        public Texture2D SurfaceFor(BowlingBall ball) =>
            ball == BowlingBall.Urethane ? SurfaceUrethane : ball == BowlingBall.Reactive ? SurfaceReactive : SurfaceHouse;

        /// <summary>The picture of a ball, or null while it isn't in.</summary>
        public Sprite BallSprite(BowlingBall ball) =>
            ball == BowlingBall.Urethane ? BallUrethane : ball == BowlingBall.Reactive ? BallReactive : BallHouse;

        /// <summary>How long a stretch of lane one tile of each texture covers, in feet.</summary>
        public const double BoardsTileFt = 8, ApproachTileFt = 4, GutterTileFt = 3;

        /// <summary>How wide a stretch of approach one tile covers, in metres: about a lane's width.</summary>
        public const double ApproachTileM = 1.1;

        /// <summary>
        /// This art with any missing texture made up, and the made-up ones to destroy when the
        /// lane goes.
        /// </summary>
        public static LaneArt Complete(LaneArt art, out Texture2D[] made)
        {
            var result = new LaneArt
            {
                Boards = art?.Boards, Approach = art?.Approach, Gutter = art?.Gutter, Masking = art?.Masking,
                Deck = art?.Deck, BallHouse = art?.BallHouse, BallUrethane = art?.BallUrethane, BallReactive = art?.BallReactive,
                PinUp = art?.PinUp, PinDown = art?.PinDown, PinTop = art?.PinTop,
                SurfaceHouse = art?.SurfaceHouse, SurfaceUrethane = art?.SurfaceUrethane, SurfaceReactive = art?.SurfaceReactive,
            };
            var list = new System.Collections.Generic.List<Texture2D>();
            if (result.Boards == null) list.Add(result.Boards = Grain("Boards", 64, 256, new Color(0.83f, 0.66f, 0.43f), 11));
            if (result.Approach == null) list.Add(result.Approach = Grain("Approach", 128, 128, new Color(0.62f, 0.44f, 0.26f), 23));
            if (result.Gutter == null) list.Add(result.Gutter = Channel());
            if (result.Masking == null) list.Add(result.Masking = Panel());
            if (result.SurfaceHouse == null)
                list.Add(result.SurfaceHouse = Surface("House", new Color(0.72f, 0.12f, 0.12f), new Color(0.42f, 0.05f, 0.06f), 1f));
            if (result.SurfaceUrethane == null)
                list.Add(result.SurfaceUrethane = Surface("Urethane", new Color(0.13f, 0.13f, 0.15f), new Color(0.3f, 0.3f, 0.33f), 0.15f));
            if (result.SurfaceReactive == null)
                list.Add(result.SurfaceReactive = Surface("Reactive", new Color(0.72f, 0.62f, 0.86f), new Color(0.93f, 0.9f, 0.97f), 1f));
            made = list.ToArray();
            return result;
        }

        /// <summary>Wood with grain running down the picture: streaks across, a slow waver along.</summary>
        static Texture2D Grain(string name, int width, int height, Color wood, int seed)
        {
            var rng = new System.Random(seed);
            var streak = new float[width];
            for (int x = 0; x < width; x++) streak[x] = (float)rng.NextDouble();
            var pixels = new Color[width * height];
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    float wave = Mathf.Sin((y / (float)height) * Mathf.PI * 2 * 2 + x * 0.37f) * 0.5f + 0.5f;
                    float g = 0.9f + 0.07f * streak[x] + 0.05f * wave * streak[(x + 7) % width];
                    pixels[y * width + x] = new Color(wood.r * g, wood.g * g, wood.b * g, 1);
                }
            return Made(name, width, height, pixels);
        }

        /// <summary>A gutter: grey, darker at the bottom of the channel, a highlight up its far wall.</summary>
        static Texture2D Channel()
        {
            const int width = 32, height = 32;
            var pixels = new Color[width * height];
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    float across = x / (float)(width - 1);
                    float g = 0.3f + 0.12f * Mathf.Abs(across - 0.45f) * 2 + (across > 0.82f && across < 0.9f ? 0.12f : 0);
                    pixels[y * width + x] = new Color(g, g, g * 1.08f, 1);
                }
            return Made("Gutter", width, height, pixels);
        }

        /// <summary>
        /// The masking front as it is in the Lanes' picture: two plain cream panels, outlined, a
        /// dark gap between and half a gap at each edge; a navy line; tired green below.
        /// </summary>
        static Texture2D Panel()
        {
            const int width = 128, height = 32;
            var cream = new Color(0.86f, 0.8f, 0.64f);
            var ink = new Color(0.06f, 0.05f, 0.04f);
            var navy = new Color(0.12f, 0.16f, 0.3f);
            var green = new Color(0.36f, 0.42f, 0.3f);
            var pixels = new Color[width * height];
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    // Rows count up from the bottom: green, then the navy line, then the panels.
                    Color c;
                    if (y < 12) c = green * (0.85f + 0.15f * ((x * 7) % 5) / 4f);
                    else if (y < 14) c = navy;
                    else
                    {
                        int along = x % (width / 2);
                        bool gap = along < 2 || along >= width / 2 - 2;
                        bool edge = along < 3 || along >= width / 2 - 3 || y < 16 || y >= height - 2;
                        c = gap ? ink * 2 : edge ? ink : cream;
                    }
                    c.a = 1;
                    pixels[y * width + x] = c;
                }
            return Made("Masking", width, height, pixels);
        }

        /// <summary>
        /// A ball's surface, unwrapped: a swirl of two colours (or, swirl near 0, a plain ball with
        /// scuffs), and the finger holes where the ball's own x is.
        /// </summary>
        static Texture2D Surface(string name, Color body, Color swirl, float swirliness)
        {
            const int width = 256, height = 128;
            var rng = new System.Random(name.Length * 7919);
            var pixels = new Color[width * height];
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    double lon = ((x + 0.5) / width - 0.5) * Math.PI * 2, lat = ((y + 0.5) / height - 0.5) * Math.PI;
                    double wave = Math.Sin(3 * lon + 3.5 * Math.Sin(2 * lat + lon) + 2 * lat);
                    float mix = Mathf.Clamp01((float)(wave * 0.5 + 0.5) * swirliness);
                    var c = Color.Lerp(body, swirl, mix * mix);
                    if (swirliness < 0.5f && rng.NextDouble() < 0.01) c = swirl;
                    var at = Direction(lat, lon);
                    foreach (var (hLat, hLon) in BallSpin.HolePlaces)
                    {
                        var hole = Direction(hLat, hLon);
                        double apart = Math.Acos(Math.Max(-1, Math.Min(1, at.X * hole.X + at.Y * hole.Y + at.Z * hole.Z)));
                        if (apart < BallSpin.HoleRadius) c = apart < BallSpin.HoleRadius * 0.75 ? new Color(0.03f, 0.02f, 0.02f) : Color.Lerp(c, Color.black, 0.6f);
                    }
                    c.a = 1;
                    pixels[y * width + x] = c;
                }
            return Made($"{name} Surface", width, height, pixels);
        }

        static Vec3 Direction(double lat, double lon) => new Vec3(Math.Cos(lat) * Math.Cos(lon), Math.Cos(lat) * Math.Sin(lon), Math.Sin(lat));

        static Texture2D Made(string name, int width, int height, Color[] pixels)
        {
            var texture = new Texture2D(width, height, TextureFormat.RGBA32, true)
            {
                name = $"Placeholder {name}",
                wrapMode = TextureWrapMode.Repeat,
                filterMode = FilterMode.Trilinear,
            };
            texture.SetPixels(pixels);
            texture.Apply(true, true);
            return texture;
        }
    }
}
