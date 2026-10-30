using System.Collections.Generic;
using Starflare.Cards;

namespace Starflare.Blackjack
{
    public class Hand
    {
        readonly List<Card> _cards = new List<Card>();

        public IReadOnlyList<Card> Cards => _cards;

        public void Add(Card card) => _cards.Add(card);

        internal Card RemoveLast()
        {
            var card = _cards[_cards.Count - 1];
            _cards.RemoveAt(_cards.Count - 1);
            return card;
        }

        public int Total
        {
            get
            {
                Evaluate(out int total, out _);
                return total;
            }
        }

        /// <summary>True when an ace is still counted as 11.</summary>
        public bool IsSoft
        {
            get
            {
                Evaluate(out _, out bool soft);
                return soft;
            }
        }

        public bool IsBust => Total > 21;

        public virtual bool IsBlackjack => _cards.Count == 2 && Total == 21;

        void Evaluate(out int total, out bool soft)
        {
            total = 0;
            int elevenAces = 0;
            foreach (var card in _cards)
            {
                total += card.BlackjackValue;
                if (card.Rank == Rank.Ace) elevenAces++;
            }
            while (total > 21 && elevenAces > 0)
            {
                total -= 10;
                elevenAces--;
            }
            soft = elevenAces > 0;
        }
    }

    public enum HandOutcome { Pending, Blackjack, Win, Push, Lose, Bust }

    public sealed class PlayerHand : Hand
    {
        public int Bet { get; internal set; }
        public bool FromSplit { get; internal set; }
        public bool Doubled { get; internal set; }
        public bool Done { get; internal set; }
        public HandOutcome Outcome { get; internal set; }

        /// <summary>Money won (positive) or lost (negative) on this hand once settled.</summary>
        public int Net { get; internal set; }

        /// <summary>21 on two cards after a split is just 21, not a blackjack.</summary>
        public override bool IsBlackjack => !FromSplit && base.IsBlackjack;

        public bool IsSplitAces => FromSplit && Cards.Count > 0 && Cards[0].Rank == Rank.Ace;
    }
}
