using System;
using System.Collections.Generic;
using System.Linq;

namespace Starflare.Slots
{
    /// <summary>
    /// One line of a machine's paytable: a payline wins it when its first <see cref="Count"/>
    /// reels, from the left, all show one of <see cref="Symbols"/>. Three 7s is one symbol
    /// three times; "any bar" is three symbols; one cherry on the first reel is a count of one.
    /// A line pays only its best match.
    /// </summary>
    public sealed class SlotPay
    {
        public SlotPay(string label, int count, int pays, params string[] symbols)
        {
            if (string.IsNullOrEmpty(label)) throw new ArgumentException("A pay needs a label for the glass.", nameof(label));
            if (count < 1) throw new ArgumentOutOfRangeException(nameof(count));
            if (pays < 1) throw new ArgumentOutOfRangeException(nameof(pays));
            if (symbols == null || symbols.Length == 0) throw new ArgumentException("A pay needs a symbol.", nameof(symbols));
            Label = label;
            Count = count;
            Pays = pays;
            Symbols = symbols.ToArray();
        }

        public string Label { get; }
        public int Count { get; }

        /// <summary>Credits paid per credit bet on the line, the bet included ("for one").</summary>
        public int Pays { get; }

        public IReadOnlyList<string> Symbols { get; }

        /// <summary>
        /// The top award's bonus for betting max, per credit, as on video poker's royal; 0 when
        /// max pays the same as any other bet.
        /// </summary>
        public int PaysAtMaxBet { get; set; }

        /// <summary>A single symbol counted <see cref="Count"/> times, which the glass can draw as pictures.</summary>
        public bool IsOneSymbol => Symbols.Count == 1;

        public bool Matches(IReadOnlyList<string> line)
        {
            if (line.Count < Count) return false;
            for (int i = 0; i < Count; i++)
                if (!Symbols.Contains(line[i])) return false;
            return true;
        }

        /// <summary>Credits paid on one line at this bet per line.</summary>
        public int PayoutCredits(int credits, int maxCredits) =>
            PaysAtMaxBet > 0 && credits == maxCredits ? PaysAtMaxBet * credits : Pays * credits;
    }
}
