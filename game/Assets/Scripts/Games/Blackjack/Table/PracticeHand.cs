using System;
using Starflare.Cards;

namespace Starflare.Blackjack
{
    /// <summary>
    /// A practice hand: one hand dealt from a script rather than the shoe, so a teacher can show
    /// what a rule means, like Rosa's first hand in Kasia's back room (A1.3). It's played for
    /// show at a set bet and doesn't count: no money changes hands and the clock doesn't move.
    /// Regular play stays fair; only a practice hand is ever stacked.
    /// </summary>
    public sealed class PracticeHand
    {
        /// <summary>The bet shown on the felt. Nothing is taken from your cash.</summary>
        public int Bet { get; }

        /// <summary>Deal order: player, dealer, player, dealer, then every card drawn after.</summary>
        public Card[] Cards { get; }

        public PracticeHand(int bet, params Card[] cards)
        {
            if (bet <= 0) throw new ArgumentOutOfRangeException(nameof(bet));
            if (cards == null || cards.Length < 4) throw new ArgumentException("A hand needs at least the four cards of the deal.", nameof(cards));
            Bet = bet;
            Cards = cards;
        }
    }
}
