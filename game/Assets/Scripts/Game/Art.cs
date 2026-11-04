using System.Collections.Generic;
using Starflare.State;
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
        /// Where a portrait screen centres on a wide plate, as a fraction of its width, by view:
        /// a view's day, night, rain and winter plates share a framing. Only the 16:9 plates need
        /// one; a 9:16 plate under the same tag fills the screen as it is.
        /// </summary>
        static readonly Dictionary<string, float> PlateFocus = new Dictionary<string, float>
        {
            ["kasia_bar"] = 0.74f,
            ["kasia_front"] = 0.58f,
            ["lanes_front"] = 0.66f,
            ["motel_lot"] = 0.5f,
            ["motel_office"] = 0.4f,
            ["motel_river"] = 0.4f,
            ["motel_room"] = 0.45f,
        };

        /// <summary>
        /// Views drawn on another view's plates, framed differently on a portrait screen: the
        /// motel lot looking across the river at the rocket sign (the arrival).
        /// </summary>
        static readonly Dictionary<string, string> SharedPlates = new Dictionary<string, string>
        {
            ["motel_river"] = "motel_lot",
        };

        /// <summary>
        /// Where you've looked round each view to, which frames its plates, in scenes too,
        /// until you next arrive somewhere (City).
        /// </summary>
        static readonly Dictionary<string, float> Looked = new Dictionary<string, float>();

        static readonly HashSet<string> Reported = new HashSet<string>();

        public static Sprite Plate(string tag) => Load("Plates", FileFor(tag));
        public static Sprite Pose(string tag) => Load("Poses", tag);
        public static Sprite Insert(string tag) => Load("Inserts", tag);

        public static float Focus(string plateTag)
        {
            if (Looked.TryGetValue(ViewOf(plateTag), out float looked)) return looked;
            foreach (var view in PlateFocus)
                if (plateTag.StartsWith(view.Key + "_")) return view.Value;
            return 0.5f;
        }

        /// <summary>Frames a view's plates where you left it after looking round.</summary>
        public static void KeepLook(string view, float focus) => Looked[view] = focus;

        /// <summary>Back to each view's own framing, as you arrive somewhere.</summary>
        public static void ForgetLooks() => Looked.Clear();

        /// <summary>The view a plate tag shows: the tag without its <c>_day</c>/<c>_night</c> and <c>_winter</c>.</summary>
        static string ViewOf(string plateTag)
        {
            string tag = plateTag.EndsWith("_winter") ? plateTag.Substring(0, plateTag.Length - "_winter".Length) : plateTag;
            int cut = tag.LastIndexOf('_');
            return cut < 0 ? tag : tag.Substring(0, cut);
        }

        /// <summary>
        /// The plate tag for a view at this time: <c>view_day</c> (morning and afternoon) or
        /// <c>view_night</c>, with <c>_winter</c> on the end in winter when that plate exists.
        /// A view drawn only at night shows its night plate all day.
        /// </summary>
        public static string PlateAt(string view, GameClock clock)
        {
            string tag = clock.Part == PartOfDay.Night ? $"{view}_night" : $"{view}_day";
            if (clock.Season == Season.Winter && HasPlate($"{tag}_winter")) return $"{tag}_winter";
            return HasPlate(tag) ? tag : $"{view}_night";
        }

        static bool HasPlate(string tag) => Resources.Load<Sprite>($"Plates/{FileFor(tag)}") != null;

        static string FileFor(string plateTag)
        {
            foreach (var view in SharedPlates)
                if (plateTag.StartsWith(view.Key + "_")) return view.Value + plateTag.Substring(view.Key.Length);
            return plateTag;
        }

        static Sprite Load(string folder, string tag)
        {
            var sprite = Resources.Load<Sprite>($"{folder}/{tag}");
            if (sprite == null && Reported.Add($"{folder}/{tag}"))
                Debug.LogWarning($"No art yet for {folder}/{tag}; skipping it.");
            return sprite;
        }
    }
}
