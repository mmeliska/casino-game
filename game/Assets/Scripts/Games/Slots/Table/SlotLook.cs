using System.Collections.Generic;
using UnityEngine;

namespace Starflare.Slots
{
    /// <summary>
    /// A machine's colours and symbols, kept apart from its layout so each venue's theme is
    /// only data: the rules say what's on the reels, the look says how it's drawn. A venue
    /// hands its own look to <see cref="SlotMachine.Init"/>.
    /// </summary>
    public sealed class SlotLook
    {
        /// <summary>Fills the screen around the reels: the cabinet's paint.</summary>
        public Color Cabinet { get; set; } = new Color(0.13f, 0.09f, 0.17f);

        /// <summary>The glass over the paytable and behind the meters.</summary>
        public Color Glass { get; set; } = new Color(0.04f, 0.03f, 0.06f);

        public Color GlassText { get; set; } = new Color(0.95f, 0.93f, 0.86f);

        /// <summary>The machine's name on the glass, and the payline across the window.</summary>
        public Color Accent { get; set; } = new Color(0.86f, 0.16f, 0.16f);

        /// <summary>The stripe behind the paytable column for the current bet.</summary>
        public Color LitColumn { get; set; } = new Color(0.32f, 0.22f, 0.4f);

        public Color ReelFace { get; set; } = new Color(0.96f, 0.95f, 0.91f);
        public Color Trim { get; set; } = new Color(0.72f, 0.72f, 0.76f);
        public Color BulbOn { get; set; } = new Color(1f, 0.9f, 0.62f);
        public Color BulbOff { get; set; } = new Color(0.28f, 0.22f, 0.2f);
        public Color Button { get; set; } = new Color(0.93f, 0.91f, 0.86f);
        public Color ButtonText { get; set; } = new Color(0.1f, 0.08f, 0.08f);

        /// <summary>The spin button, the one you press most.</summary>
        public Color SpinButton { get; set; } = new Color(0.8f, 0.12f, 0.12f);
        public Color SpinButtonText { get; set; } = Color.white;

        public Dictionary<string, SymbolLook> Symbols { get; } = new Dictionary<string, SymbolLook>();

        /// <summary>
        /// A symbol's look; one the look doesn't name shows its id in capitals, so a new reel set
        /// plays before it's drawn. That stand-in is kept, so a picture set on it sticks.
        /// </summary>
        public SymbolLook For(string symbol)
        {
            if (Symbols.TryGetValue(symbol, out var look)) return look;
            look = symbol == SlotRules.Blank ? SymbolLook.Blank() : SymbolLook.Lettered(symbol.ToUpperInvariant(), Color.black);
            Symbols[symbol] = look;
            return look;
        }

        /// <summary>
        /// Bridgehead's cheap three-reel: a scuffed purple-black cabinet under the canopy
        /// fluorescents (the Bridges' flat white and oil-slick purple), warm bulbs, a red 7 and
        /// red spin button, black bars.
        /// </summary>
        public static SlotLook TripleBar()
        {
            var look = new SlotLook();
            var bar = new Color(0.08f, 0.07f, 0.08f);
            look.Symbols["seven"] = SymbolLook.Lettered("7", new Color(0.82f, 0.1f, 0.1f));
            look.Symbols["bar3"] = SymbolLook.BarStack(3, bar);
            look.Symbols["bar2"] = SymbolLook.BarStack(2, bar);
            look.Symbols["bar1"] = SymbolLook.BarStack(1, bar);
            look.Symbols["cherry"] = SymbolLook.Cherries();
            look.Symbols[SlotRules.Blank] = SymbolLook.Blank();
            return look;
        }
    }
}
