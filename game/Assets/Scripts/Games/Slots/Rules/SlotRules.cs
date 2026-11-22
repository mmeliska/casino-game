using System;
using System.Collections.Generic;
using System.Linq;

namespace Starflare.Slots
{
    /// <summary>
    /// A machine: its reel strips, how many rows of them show in the window, its paylines and
    /// its paytable. One engine plays every venue's machine (the GDD's "one engine with
    /// per-venue reel sets"); the theme is only this data and the look the venue hands the
    /// table. Every stop on a strip is equally likely, and the strip you see is the strip that
    /// spins, so the odds are what the strips say: a short machine is honest, the way a short
    /// poker table is printed on the glass.
    /// </summary>
    public sealed class SlotRules
    {
        /// <summary>The empty stop between symbols on a classic reel. No pay names it.</summary>
        public const string Blank = "blank";

        readonly string[][] _reels;
        readonly int[][] _lines;
        readonly SlotPay[] _pays;

        /// <param name="id">For art tags: a symbol's picture is <c>{id}_{symbol}</c>.</param>
        /// <param name="name">What the glass says.</param>
        /// <param name="rows">Rows showing in the window, the same on every reel.</param>
        /// <param name="reels">Each reel's strip, top to bottom, left reel first.</param>
        /// <param name="lines">Each payline's row on every reel, 0 at the top.</param>
        /// <param name="pays">The paytable, top award first, as it reads on the glass.</param>
        public SlotRules(string id, string name, int rows, string[][] reels, int[][] lines, SlotPay[] pays)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentException("A machine needs an id.", nameof(id));
            if (rows < 1) throw new ArgumentOutOfRangeException(nameof(rows));
            if (reels == null || reels.Length == 0) throw new ArgumentException("A machine needs a reel.", nameof(reels));
            foreach (var reel in reels)
                if (reel == null || reel.Length < rows)
                    throw new ArgumentException("Every strip needs at least as many stops as the window shows.", nameof(reels));
            if (lines == null || lines.Length == 0) throw new ArgumentException("A machine needs a payline.", nameof(lines));
            foreach (var line in lines)
                if (line == null || line.Length != reels.Length || line.Any(row => row < 0 || row >= rows))
                    throw new ArgumentException("A payline names one row on every reel.", nameof(lines));
            if (pays == null || pays.Length == 0) throw new ArgumentException("A machine needs a paytable.", nameof(pays));
            if (pays.Any(p => p.Count > reels.Length))
                throw new ArgumentException("A pay can't count more reels than the machine has.", nameof(pays));

            Id = id;
            Name = name ?? "";
            Rows = rows;
            _reels = reels.Select(r => r.ToArray()).ToArray();
            _lines = lines.Select(l => l.ToArray()).ToArray();
            _pays = pays.ToArray();
        }

        public string Id { get; }
        public string Name { get; }
        public int Rows { get; }
        public int Columns => _reels.Length;
        public IReadOnlyList<IReadOnlyList<string>> Reels => _reels;
        public IReadOnlyList<IReadOnlyList<int>> Lines => _lines;
        public IReadOnlyList<SlotPay> Pays => _pays;

        public int CreditValue { get; set; } = 1;

        /// <summary>Credits per line; the bet is this many on every line.</summary>
        public int MaxCredits { get; set; } = 3;

        public int BetFor(int credits) => credits * _lines.Length * CreditValue;

        /// <summary>Every symbol on the strips, blanks included, in the order first seen.</summary>
        public IEnumerable<string> Symbols => _reels.SelectMany(r => r).Distinct();

        /// <summary>The best pay a line of symbols wins at this bet, or null.</summary>
        public SlotPay BestPay(IReadOnlyList<string> line, int credits)
        {
            SlotPay best = null;
            foreach (var pay in _pays)
                if (pay.Matches(line) && (best == null || pay.PayoutCredits(credits, MaxCredits) > best.PayoutCredits(credits, MaxCredits)))
                    best = pay;
            return best;
        }

        /// <summary>
        /// Bridgehead's cheap three-reel (the Bridges; Mike, 2026-11-22): cherries, bars and a 7,
        /// a blank between every symbol, one payline across the middle of three rows. 86% back at
        /// one credit and 88% at three, the worst machine in town (its 6/5 poker is about 95%); a
        /// win about one pull in four and a half; three 7s once in 10,648 pulls.
        /// </summary>
        public static SlotRules TripleBar()
        {
            const string seven = "seven", bar3 = "bar3", bar2 = "bar2", bar1 = "bar1", cherry = "cherry";
            return new SlotRules("triplebar", "TRIPLE BAR", 3,
                new[]
                {
                    WithBlanks(seven, bar1, cherry, bar2, bar1, cherry, bar3, cherry, bar1, bar2, cherry),
                    WithBlanks(seven, bar1, bar2, cherry, bar1, bar3, bar1, bar2, cherry, bar3, bar1),
                    WithBlanks(seven, bar1, bar2, bar1, bar3, cherry, bar1, bar2, bar1, bar3, bar2),
                },
                new[] { new[] { 1, 1, 1 } },
                new[]
                {
                    new SlotPay("7 7 7", 3, 200, seven) { PaysAtMaxBet = 400 },
                    new SlotPay("TRIPLE BAR", 3, 80, bar3),
                    new SlotPay("DOUBLE BAR", 3, 40, bar2),
                    new SlotPay("SINGLE BAR", 3, 20, bar1),
                    new SlotPay("CHERRY CHERRY CHERRY", 3, 20, cherry),
                    new SlotPay("CHERRY CHERRY", 2, 10, cherry),
                    new SlotPay("ANY BAR", 3, 5, bar1, bar2, bar3),
                    new SlotPay("CHERRY", 1, 2, cherry),
                })
            {
                CreditValue = 1,
                MaxCredits = 3,
            };
        }

        /// <summary>A classic strip: each symbol followed by a blank stop.</summary>
        public static string[] WithBlanks(params string[] symbols) =>
            symbols.SelectMany(s => new[] { s, Blank }).ToArray();
    }
}
