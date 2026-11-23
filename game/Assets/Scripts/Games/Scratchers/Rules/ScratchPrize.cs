using System;

namespace Starflare.Scratchers
{
    /// <summary>One line of a ticket's prize pool: an amount and how many of every million tickets on the roll win it.</summary>
    public sealed class ScratchPrize
    {
        public ScratchPrize(int amount, int perMillion)
        {
            if (amount < 1) throw new ArgumentOutOfRangeException(nameof(amount));
            if (perMillion < 1) throw new ArgumentOutOfRangeException(nameof(perMillion));
            Amount = amount;
            PerMillion = perMillion;
        }

        public int Amount { get; }
        public int PerMillion { get; }

        /// <summary>The odds as the back of the ticket prints them: 1 in this many.</summary>
        public double OneIn => ScratchTicket.Million / (double)PerMillion;
    }
}
