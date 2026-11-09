namespace Starflare.VideoPoker
{
    /// <summary>
    /// Jacks or Better: a fresh 52-card deck every hand, so the odds are the real ones. Pays are
    /// "for one", per credit bet (a pair of jacks gives the bet back). Venues differ in the credit
    /// value and in what a full house and a flush pay: Kasia's full-pay 9/6, Bridgehead's 6/5,
    /// the worst pay in town. A short table is honest; it's printed on the glass.
    /// </summary>
    public sealed class VideoPokerRules
    {
        /// <summary>Kasia's machines: about a dollar a go (A1.2).</summary>
        public int CreditValue { get; set; } = 1;
        public int MaxCredits { get; set; } = 5;

        /// <summary>The royal's bonus for betting max: 800 per credit instead of 250.</summary>
        public int RoyalAtMaxPerCredit { get; set; } = 800;

        public int FullHousePays { get; set; } = 9;
        public int FlushPays { get; set; } = 6;

        /// <summary>Bridgehead's machine (the Bridges): 6/5, the worst pay in town.</summary>
        public static VideoPokerRules SixFive() => new VideoPokerRules { FullHousePays = 6, FlushPays = 5 };

        public int PaysPerCredit(PokerHandRank rank) => rank switch
        {
            PokerHandRank.RoyalFlush => 250,
            PokerHandRank.StraightFlush => 50,
            PokerHandRank.FourOfAKind => 25,
            PokerHandRank.FullHouse => FullHousePays,
            PokerHandRank.Flush => FlushPays,
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
