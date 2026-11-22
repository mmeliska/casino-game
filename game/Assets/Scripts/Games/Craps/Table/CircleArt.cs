using System;
using UnityEngine;

namespace Starflare.Craps
{
    /// <summary>
    /// A painted ground for the craps circle: a plate with the chalk circle and the wall the dice
    /// go off drawn in, and where each sits in it, so the dice land in the painted circle and
    /// bounce off the painted bricks. A venue measures its own art and hands it over; without
    /// it, the table draws its own ring over the venue's plate.
    /// </summary>
    public sealed class CircleArt
    {
        public Sprite Plate { get; }

        /// <summary>
        /// The chalk circle's bounds, in fractions of the plate from its bottom-left corner. It may
        /// run off the plate's edge, as long as its middle, where the dice rest, is on it.
        /// </summary>
        public Rect Circle { get; }

        /// <summary>Where the wall meets the ground, in fractions of the plate's height from the bottom.</summary>
        public float WallFoot { get; }

        public CircleArt(Sprite plate, Rect circle, float wallFoot)
        {
            Plate = plate ? plate : throw new ArgumentNullException(nameof(plate));
            var middle = circle.center;
            if (middle.x <= 0 || middle.y <= 0 || middle.x >= 1 || middle.y >= 1 || circle.width <= 0 || circle.height <= 0)
                throw new ArgumentOutOfRangeException(nameof(circle), "The circle is measured in fractions of the plate, its middle on it.");
            if (wallFoot <= circle.center.y || wallFoot > 1)
                throw new ArgumentOutOfRangeException(nameof(wallFoot), "The wall stands behind the circle, above its middle.");
            Circle = circle;
            WallFoot = wallFoot;
        }
    }
}
