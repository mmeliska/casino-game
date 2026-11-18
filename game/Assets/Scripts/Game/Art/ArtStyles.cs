using UnityEngine;

namespace Starflare.Game
{
    /// <summary>
    /// Art styles being tried side by side. The base style is the art in Resources/Plates, Poses
    /// and Inserts; another style is the same folders under Resources/Styles/&lt;style&gt;/, with
    /// files under the same tag names. A style needn't be complete: anything it hasn't drawn yet
    /// shows the base art, so a new look can be compared a plate at a time. The debug panel
    /// picks the style and this device remembers it, like the clock pace; release builds always
    /// show the base style.
    /// </summary>
    public static class ArtStyles
    {
        public const string Base = "base";

        /// <summary>Every style the debug panel offers. A new style is a line here and a folder.</summary>
        public static readonly string[] All = { Base, "alt" };

        const string Key = "debug.art.style";

        public static string Current => Debug.isDebugBuild ? PlayerPrefs.GetString(Key, Base) : Base;

        /// <summary>Takes effect as each picture next loads; the panel reboots the city so every screen does at once.</summary>
        public static void Choose(string style)
        {
            PlayerPrefs.SetString(Key, style);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Where to look for a picture (<c>Plates/kasia_bar_night</c>), in order: the style's own
        /// first, then the base art.
        /// </summary>
        public static string[] PathsFor(string style, string path) =>
            string.IsNullOrEmpty(style) || style == Base ? new[] { path } : new[] { $"Styles/{style}/{path}", path };
    }
}
