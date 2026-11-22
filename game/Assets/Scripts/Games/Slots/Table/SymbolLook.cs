using UnityEngine;

namespace Starflare.Slots
{
    /// <summary>How a symbol is drawn in code while it has no picture.</summary>
    public enum SymbolDrawing { Blank, Glyph, Bars, Cherries }

    /// <summary>
    /// What one reel symbol looks like. A picture (<see cref="Art"/>) wins when the venue has
    /// one; until then the machine draws the symbol itself: a lettered glyph (the 7), stacked
    /// BAR slabs, or a pair of cherries.
    /// </summary>
    public sealed class SymbolLook
    {
        public SymbolDrawing Drawing { get; set; }

        /// <summary>The glyph's letters, for <see cref="SymbolDrawing.Glyph"/>.</summary>
        public string Glyph { get; set; } = "";

        /// <summary>How many slabs, for <see cref="SymbolDrawing.Bars"/>.</summary>
        public int Bars { get; set; }

        public Color Ink { get; set; } = Color.black;

        /// <summary>The symbol's picture, set by the venue once the art exists.</summary>
        public Sprite Art { get; set; }

        public static SymbolLook Blank() => new SymbolLook { Drawing = SymbolDrawing.Blank };
        public static SymbolLook Lettered(string glyph, Color ink) => new SymbolLook { Drawing = SymbolDrawing.Glyph, Glyph = glyph, Ink = ink };
        public static SymbolLook BarStack(int bars, Color ink) => new SymbolLook { Drawing = SymbolDrawing.Bars, Bars = bars, Ink = ink };
        public static SymbolLook Cherries() => new SymbolLook { Drawing = SymbolDrawing.Cherries };
    }
}
