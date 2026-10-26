using System;

namespace Starflare.Blackjack
{
    /// <summary>
    /// Table rules for one blackjack table. Defaults are the Starflare Club (the Flare):
    /// six decks, dealer stands on soft 17, blackjack pays 3:2, $5 to $100.
    /// </summary>
    [Serializable]
    public sealed class BlackjackRules
    {
        public int Decks = 6;
        public double Penetration = 0.75;
        public bool DealerHitsSoft17 = false;
        public int BlackjackPayNumerator = 3;
        public int BlackjackPayDenominator = 2;
        public int MinBet = 5;
        public int MaxBet = 100;
        public int MaxHands = 2;
        public bool DoubleAfterSplit = true;

        /// <summary>
        /// In-game minutes a round takes off the clock, close to a real table's pace (Mike,
        /// 26 Oct 2026). 72 rounds fill a six-hour part of the day.
        /// </summary>
        public int MinutesPerRound = 5;

        /// <summary>Whole dollars; an odd bet's half dollar rounds down, as at a table with no half chips.</summary>
        public int BlackjackWin(int bet) => bet * BlackjackPayNumerator / BlackjackPayDenominator;
    }
}
