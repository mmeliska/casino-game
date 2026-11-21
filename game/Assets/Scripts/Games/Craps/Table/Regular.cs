using System;
using UnityEngine;

namespace Starflare.Craps
{
    /// <summary>
    /// One of the regulars in the circle: a walk-on, shown as a face only (GDD, "The circle").
    /// The venue names them and hands in their face; until the art lands, the seat shows a disc
    /// with the name's first letter.
    /// </summary>
    public sealed class Regular
    {
        public string Name { get; }

        /// <summary>A square head-and-shoulders portrait, or null for the placeholder disc.</summary>
        public Sprite Face { get; }

        public Regular(string name, Sprite face = null)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentException("A regular needs a name.", nameof(name));
            Name = name;
            Face = face;
        }
    }
}
