using System;
using System.Collections.Generic;
using System.Linq;

namespace Starflare.Slots
{
    /// <summary>A payline that won: which line, what it matched, and what it paid.</summary>
    public sealed class LineWin
    {
        public LineWin(int line, SlotPay pay, int credits)
        {
            Line = line;
            Pay = pay;
            Credits = credits;
        }

        public int Line { get; }
        public SlotPay Pay { get; }

        /// <summary>Credits paid on this line.</summary>
        public int Credits { get; }
    }

    /// <summary>
    /// One pull, settled the moment it's made: where each reel stops, what the window shows,
    /// which lines won. The reels spinning on screen only show it; quitting mid-spin can't take
    /// a pull back.
    /// </summary>
    public sealed class SlotSpin
    {
        readonly SlotRules _rules;
        readonly int[] _stops;
        readonly LineWin[] _wins;

        /// <param name="stops">Each reel's stop: the strip index showing in the window's top row.</param>
        public SlotSpin(SlotRules rules, int credits, IReadOnlyList<int> stops)
        {
            _rules = rules ?? throw new ArgumentNullException(nameof(rules));
            if (credits < 1 || credits > rules.MaxCredits) throw new ArgumentOutOfRangeException(nameof(credits));
            if (stops == null || stops.Count != rules.Columns) throw new ArgumentException("One stop per reel.", nameof(stops));
            for (int c = 0; c < stops.Count; c++)
                if (stops[c] < 0 || stops[c] >= rules.Reels[c].Count) throw new ArgumentOutOfRangeException(nameof(stops));

            Credits = credits;
            _stops = stops.ToArray();
            var wins = new List<LineWin>();
            for (int l = 0; l < rules.Lines.Count; l++)
            {
                var pay = rules.BestPay(LineSymbols(l), credits);
                if (pay != null) wins.Add(new LineWin(l, pay, pay.PayoutCredits(credits, rules.MaxCredits)));
            }
            _wins = wins.ToArray();
        }

        /// <summary>A fair pull: every stop on every reel equally likely.</summary>
        public static SlotSpin Pull(SlotRules rules, int credits, int bankroll, Random random)
        {
            if (rules == null) throw new ArgumentNullException(nameof(rules));
            if (random == null) throw new ArgumentNullException(nameof(random));
            if (credits < 1 || credits > rules.MaxCredits) throw new ArgumentOutOfRangeException(nameof(credits));
            if (rules.BetFor(credits) > bankroll) throw new InvalidOperationException("Not enough bankroll for that bet.");
            var stops = rules.Reels.Select(r => random.Next(r.Count)).ToArray();
            return new SlotSpin(rules, credits, stops);
        }

        public int Credits { get; }
        public IReadOnlyList<int> Stops => _stops;
        public IReadOnlyList<LineWin> Wins => _wins;

        public int Bet => _rules.BetFor(Credits);
        public int Payout => _wins.Sum(w => w.Credits) * _rules.CreditValue;
        public int Net => Payout - Bet;

        /// <summary>The symbol in the window at this reel and row (0 at the top).</summary>
        public string At(int column, int row)
        {
            var reel = _rules.Reels[column];
            return reel[(_stops[column] + row) % reel.Count];
        }

        /// <summary>The symbols along a payline, left reel first.</summary>
        public IReadOnlyList<string> LineSymbols(int line)
        {
            var rows = _rules.Lines[line];
            var symbols = new string[_rules.Columns];
            for (int c = 0; c < symbols.Length; c++) symbols[c] = At(c, rows[c]);
            return symbols;
        }

        /// <summary>Whether this cell is part of a winning line's match, for the window to light it.</summary>
        public bool IsWinningCell(int column, int row) =>
            _wins.Any(w => column < w.Pay.Count && _rules.Lines[w.Line][column] == row);
    }
}
