using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Starflare.Ui;
using TMPro;
using UnityEngine;

namespace Starflare.Tests
{
    /// <summary>
    /// Every character the game puts on screen has a glyph, so no label shows a box and the
    /// console stays quiet. The main font is TextMesh Pro's shipped Liberation Sans atlas, kept
    /// static; anything outside it (the card suits, the ▼ prompt) comes from the essentials'
    /// dynamic fallback, which has the TTF. A character the TTF doesn't have either (a star, an
    /// emoji) fails here, naming the file.
    /// </summary>
    public class FontTests
    {
        static readonly Regex StringLiteral = new Regex("\"(?:[^\"\\\\\\n]|\\\\.)*\"");

        /// <summary>
        /// A dynamic main font has no TTF and an atlas that isn't readable, so it warns for every
        /// glyph outside its atlas before the fallback draws it. Starflare > Rebuild Project Setup
        /// sets it back.
        /// </summary>
        [Test]
        public void TheMainFont_IsStatic()
        {
            Assert.That(UiKit.FontAsset.atlasPopulationMode, Is.EqualTo(AtlasPopulationMode.Static));
        }

        [Test]
        public void EveryCharacterOnScreen_HasAGlyph()
        {
            var font = UiKit.FontAsset;
            var missing = new SortedSet<string>();
            foreach (var (file, text) in OnScreenText())
                foreach (char c in text)
                    if (c > '~' && !char.IsSurrogate(c) && !font.HasCharacter(c, searchFallbacks: true, tryAddCharacter: true))
                        missing.Add($"'{c}' (U+{(int)c:X4}) in {file}");
                    else if (char.IsSurrogate(c))
                        missing.Add($"a character beyond U+FFFF (an emoji?) in {file}");
            Assert.That(missing, Is.Empty);
        }

        /// <summary>String literals in the game's scripts, and the story's lines (not its comments).</summary>
        static IEnumerable<(string file, string text)> OnScreenText()
        {
            foreach (string assembly in new[] { "Core", "Game" })
            {
                string scripts = Path.Combine(Application.dataPath, "Scripts", assembly);
                foreach (string path in Directory.GetFiles(scripts, "*.cs", SearchOption.AllDirectories))
                    foreach (Match literal in StringLiteral.Matches(File.ReadAllText(path)))
                        yield return (Path.GetFileName(path), literal.Value);
            }

            string story = Path.Combine(Application.dataPath, "Resources", "Story");
            foreach (string path in Directory.GetFiles(story, "*.ink"))
                foreach (string line in File.ReadAllLines(path))
                    if (!line.TrimStart().StartsWith("//"))
                        yield return (Path.GetFileName(path), line);
        }
    }
}
