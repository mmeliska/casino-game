using System.Collections.Generic;
using Starflare.State;
using UnityEngine;

namespace Starflare.Game
{
    /// <summary>
    /// Finds art by its scene tag: a plate, pose or insert tag is also the image's file name
    /// under Resources/Plates, Poses or Inserts, or the chosen art style's copy of them
    /// (<see cref="ArtStyles"/>). Missing art is logged once and skipped, so a scene can be
    /// written before its art exists.
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
            ["bridgehead_backroom"] = 0.38f,
            ["bridgehead_forecourt"] = 0.3f,
            ["bridgehead_store"] = 0.48f,
            ["flare_bar"] = 0.5f,
            ["flare_floor"] = 0.6f,
            ["flare_front"] = 0.33f,
            ["flare_lounge"] = 0.5f,
            ["flare_office"] = 0.72f,
            ["kasia_backroom"] = 0.58f,
            ["kasia_bar"] = 0.74f,
            ["kasia_front"] = 0.64f,
            ["lanes_backlot"] = 0.38f,
            ["lanes_counter"] = 0.55f,
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

        /// <summary>
        /// A slot symbol's picture, <c>{machine}_{symbol}</c> under Resources/Symbols, or null.
        /// Quiet when it's missing: a machine draws its own symbols until the art exists.
        /// </summary>
        public static Sprite Symbol(string tag) => Find($"Symbols/{tag}");

        /// <summary>
        /// A scratcher's card, Resources/Scratchers/{ticket id} (or {ticket id}_wide, the landscape
        /// card), or null. Quiet when it's missing: the counter prints the ticket in its own
        /// colours until the art exists.
        /// </summary>
        public static Sprite Scratcher(string id) => Find($"Scratchers/{id}");

        /// <summary>
        /// A texture the bowling lane is drawn with, Resources/Bowling/{tag} (<c>lane_boards</c>
        /// and so on; tools/import_bowling.py), or null. Quiet when it's missing: the lane
        /// makes a plain one until the picture exists.
        /// </summary>
        public static Texture2D Texture(string tag) => Find<Texture2D>($"Bowling/{tag}");

        /// <summary>
        /// A picture the bowling game draws, Resources/Bowling/{tag} (the deck, the balls, the
        /// pins; tools/import_bowling.py), or null. Quiet when it's missing: the game draws that
        /// thing in shapes until the picture exists.
        /// </summary>
        public static Sprite BowlingSprite(string tag) => Find($"Bowling/{tag}");

        /// <summary>A crowd face by its id in the roster, Resources/Faces/faces.json (tools/import_faces.py).</summary>
        public static Sprite Face(string id) => Load("Faces", id);

        /// <summary>The crowd roster, Resources/Faces/faces.json; empty (and logged once) if it's missing.</summary>
        public static CrowdFace[] Faces()
        {
            var roster = Resources.Load<TextAsset>("Faces/faces");
            if (roster == null && Reported.Add("Faces/faces"))
                Debug.LogWarning("No crowd roster at Faces/faces; the crowds stand empty.");
            return roster == null ? new CrowdFace[0] : FaceRoster.Parse(roster.text);
        }

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

        /// <summary>
        /// The view a plate tag shows: the tag without its weather or season (<c>_rain</c>,
        /// <c>_winter</c>) and then its time of day (<c>_day</c>, <c>_night</c>), so
        /// <c>kasia_front_night_winter</c> and <c>kasia_bar_winter</c> are both their view's.
        /// </summary>
        public static string ViewOf(string plateTag)
        {
            string tag = StripSuffix(StripSuffix(plateTag, "_winter"), "_rain");
            return StripSuffix(StripSuffix(tag, "_day"), "_night");
        }

        static string StripSuffix(string tag, string suffix) =>
            tag.EndsWith(suffix) ? tag.Substring(0, tag.Length - suffix.Length) : tag;

        /// <summary>
        /// The plate tag for a view at this time: <c>view_day</c> (morning and afternoon) or
        /// <c>view_night</c> (night and before dawn). In winter it's the snow plate when the view has one: first
        /// <c>view_day_winter</c> or <c>view_night_winter</c>, else <c>view_winter</c>, one snow
        /// plate for any time of day. A view drawn only at night shows its night plate all day.
        /// Rain plates follow the same rule (<c>view_night_rain</c>, else <c>view_rain</c>) once
        /// there's weather to choose them. A view that is itself a whole plate, such as a decline
        /// stage (<c>flare_floor_night_decline</c>), shows as it is.
        /// </summary>
        public static string PlateAt(string view, GameClock clock) => PlateAt(view, clock, HasPlate);

        /// <summary><see cref="PlateAt(string, GameClock)"/> with the plates that exist given, for tests.</summary>
        public static string PlateAt(string view, GameClock clock, System.Func<string, bool> hasPlate)
        {
            if (hasPlate(view)) return view;
            bool dark = clock.Part == PartOfDay.Night || clock.Part == PartOfDay.BeforeDawn;
            string tag = dark ? $"{view}_night" : $"{view}_day";
            if (clock.Season == Season.Winter)
            {
                if (hasPlate($"{tag}_winter")) return $"{tag}_winter";
                if (hasPlate($"{view}_winter")) return $"{view}_winter";
            }
            return hasPlate(tag) ? tag : $"{view}_night";
        }

        /// <summary>
        /// A scene's "# plate:" tag: a whole plate (<c>kasia_bar_night</c>) shows as it is; a bare
        /// view (<c>bridgehead_store</c>) shows its plate for this time and season, as the place
        /// screen does, for scenes that play by day and by night.
        /// </summary>
        public static string ScenePlate(string tag, GameClock clock) => ScenePlate(tag, clock, HasPlate);

        /// <summary><see cref="ScenePlate(string, GameClock)"/> with the plates that exist given, for tests.</summary>
        public static string ScenePlate(string tag, GameClock clock, System.Func<string, bool> hasPlate) =>
            ViewOf(tag) == tag ? PlateAt(tag, clock, hasPlate) : tag;

        static bool HasPlate(string tag) => Find($"Plates/{FileFor(tag)}") != null;

        static string FileFor(string plateTag)
        {
            foreach (var view in SharedPlates)
                if (plateTag.StartsWith(view.Key + "_")) return view.Value + plateTag.Substring(view.Key.Length);
            return plateTag;
        }

        static Sprite Load(string folder, string tag)
        {
            var sprite = Find($"{folder}/{tag}");
            if (sprite == null && Reported.Add($"{folder}/{tag}"))
                Debug.LogWarning($"No art yet for {folder}/{tag}; skipping it.");
            return sprite;
        }

        /// <summary>The picture in the chosen art style, else the base art (<see cref="ArtStyles"/>).</summary>
        static Sprite Find(string path) => Find<Sprite>(path);

        static T Find<T>(string path) where T : Object
        {
            foreach (string candidate in ArtStyles.PathsFor(ArtStyles.Current, path))
            {
                var found = Resources.Load<T>(candidate);
                if (found != null) return found;
            }
            return null;
        }
    }
}
