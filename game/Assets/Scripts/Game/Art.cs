using System.Collections.Generic;
using UnityEngine;

namespace Starflare.Game
{
    /// <summary>
    /// Finds art by its scene tag: a plate, pose or insert tag is also the image's file name
    /// under Resources/Plates, Poses or Inserts. Missing art is logged once and skipped, so a
    /// scene can be written before its art exists.
    /// </summary>
    public static class Art
    {
        /// <summary>
        /// Where a portrait screen centres on a wide plate, as a fraction of its width. Only the
        /// 16:9 plates need one; a 9:16 plate under the same tag fills the screen as it is.
        /// </summary>
        static readonly Dictionary<string, float> PlateFocus = new Dictionary<string, float>
        {
            ["kasia_bar_night"] = 0.74f,
            ["lanes_front_night"] = 0.66f,
            ["motel_lot_night"] = 0.45f,
            ["motel_office_night"] = 0.4f,
        };

        static readonly HashSet<string> Reported = new HashSet<string>();

        public static Sprite Plate(string tag) => Load("Plates", tag);
        public static Sprite Pose(string tag) => Load("Poses", tag);
        public static Sprite Insert(string tag) => Load("Inserts", tag);

        public static float Focus(string plateTag) => PlateFocus.TryGetValue(plateTag, out float x) ? x : 0.5f;

        static Sprite Load(string folder, string tag)
        {
            var sprite = Resources.Load<Sprite>($"{folder}/{tag}");
            if (sprite == null && Reported.Add($"{folder}/{tag}"))
                Debug.LogWarning($"No art yet for {folder}/{tag}; skipping it.");
            return sprite;
        }
    }
}
