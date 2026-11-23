using System;
using System.Collections.Generic;

namespace Starflare.Scratchers
{
    /// <summary>
    /// The latex over one spot, as a grid of pixels that are still covered or scratched off. A
    /// stroke clears every pixel near the line the coin drew, with a ragged edge the way a real
    /// coating flakes. Once enough of a spot is off you can read it, so the rest goes too
    /// (<see cref="ShowsThrough"/>). View state: the card is settled before you scratch.
    /// </summary>
    public sealed class ScratchMask
    {
        /// <summary>How much of a spot has to come off before you can read it and the rest falls away.</summary>
        public const double ShowsThrough = 0.55;

        readonly bool[] _scratched;

        public ScratchMask(int width, int height)
        {
            if (width < 1) throw new ArgumentOutOfRangeException(nameof(width));
            if (height < 1) throw new ArgumentOutOfRangeException(nameof(height));
            Width = width;
            Height = height;
            _scratched = new bool[width * height];
        }

        public int Width { get; }
        public int Height { get; }
        public int ScratchedCount { get; private set; }

        /// <summary>The share of the spot scratched off, 0 to 1.</summary>
        public double Cleared => ScratchedCount / (double)_scratched.Length;

        /// <summary>Enough is off to read the spot.</summary>
        public bool Readable => Cleared >= ShowsThrough;

        public bool IsScratched(int x, int y) => _scratched[y * Width + x];

        /// <summary>
        /// A coin dragged from one point to another, in pixels; <paramref name="radiusX"/> and
        /// <paramref name="radiusY"/> are the coin's half-width across and down, so a coin stays
        /// round on a spot whose pixels aren't square. Pixels it clears are added to
        /// <paramref name="cleared"/> (as <c>y * Width + x</c>) for the picture to redraw.
        /// </summary>
        /// <returns>How many pixels came off.</returns>
        public int Scratch(float fromX, float fromY, float toX, float toY, float radiusX, float radiusY, List<int> cleared = null)
        {
            if (radiusX <= 0 || radiusY <= 0) return 0;
            int left = Math.Max(0, (int)Math.Floor(Math.Min(fromX, toX) - radiusX));
            int right = Math.Min(Width - 1, (int)Math.Ceiling(Math.Max(fromX, toX) + radiusX));
            int bottom = Math.Max(0, (int)Math.Floor(Math.Min(fromY, toY) - radiusY));
            int top = Math.Min(Height - 1, (int)Math.Ceiling(Math.Max(fromY, toY) + radiusY));

            // Measure in coin-radius units, where the coin is a circle of radius 1.
            double ax = fromX / radiusX, ay = fromY / radiusY;
            double dx = toX / radiusX - ax, dy = toY / radiusY - ay;
            double length = dx * dx + dy * dy;

            int count = 0;
            for (int y = bottom; y <= top; y++)
            for (int x = left; x <= right; x++)
            {
                int index = y * Width + x;
                if (_scratched[index]) continue;
                double px = (x + 0.5) / radiusX - ax, py = (y + 0.5) / radiusY - ay;
                double t = length > 0 ? Math.Max(0, Math.Min(1, (px * dx + py * dy) / length)) : 0;
                double ox = px - t * dx, oy = py - t * dy;
                if (ox * ox + oy * oy > Edge(x, y) * Edge(x, y)) continue;
                _scratched[index] = true;
                cleared?.Add(index);
                count++;
            }
            ScratchedCount += count;
            return count;
        }

        /// <summary>Everything left comes off at once: a spot read through, or "Scratch it all".</summary>
        public void ClearAll()
        {
            for (int i = 0; i < _scratched.Length; i++) _scratched[i] = true;
            ScratchedCount = _scratched.Length;
        }

        /// <summary>
        /// How far out the coin reaches at this pixel, 0.75 to 1 of its radius: the same pixel
        /// always flakes the same way, so the edge is ragged but steady.
        /// </summary>
        static double Edge(int x, int y)
        {
            uint h = (uint)(x * 73856093) ^ (uint)(y * 19349663);
            h ^= h >> 13;
            h *= 0x5bd1e995;
            h ^= h >> 15;
            return 0.75 + 0.25 * (h & 0xFFFF) / 65535.0;
        }
    }
}
