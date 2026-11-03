namespace Starflare.VideoPoker
{
    /// <summary>
    /// Jacks or Better on the full-pay 9/6 table: a fresh 52-card deck every hand, so the
    /// odds are the real ones. Pays are "for one", per credit bet (a pair of jacks gives the
    /// bet back). Venues differ only in the credit value.
    /// </summary>
    public sealed class VideoPokerRules
    {
        /// <summary>Kasia's machines: about a dollar a go (A1.2).</summary>
        public int CreditValue { get; set; } = 1;
        public int MaxCredits { get; set; } = 5;

        /// <summary>Clock minutes per hand. Starting value; tune once the prototype runs.</summary>
        public int MinutesPerRound { get; set; } = 2;

        /// <summary>The royal's bonus for betting max: 800 per credit instead of 250.</summary>
        public int RoyalAtMaxPerCredit { get; set; } = 800;

        public int PaysPerCredit(PokerHandRank rank) => rank switch
        {
            PokerHandRank.RoyalFlush => 250,
            PokerHandRank.StraightFlush => 50,
            PokerHandRank.FourOfAKind => 25,
            PokerHandRank.FullHouse => 9,
            PokerHandRank.Flush => 6,
            PokerHandRank.Straight => 4,
            PokerHandRank.ThreeOfAKind => 3,
            PokerHandRank.TwoPair => 2,
            PokerHandRank.JacksOrBetter => 1,
            _ => 0,
        };

        /// <summary>Credits returned for a hand at this bet, the bet included.</summary>
        public int PayoutCredits(PokerHandRank rank, int credits) =>
            rank == PokerHandRank.RoyalFlush && credits == MaxCredits
                ? RoyalAtMaxPerCredit * credits
                : PaysPerCredit(rank) * credits;
    }
}
