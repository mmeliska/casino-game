using System;
using System.Collections.Generic;

namespace Starflare.Cards
{
    /// <summary>
    /// A multi-deck shoe with a cut card. Fair by construction: a full Fisher-Yates
    /// shuffle, and nothing outside the shoe can change the order.
    /// </summary>
    public sealed class Shoe
    {
        readonly List<Card> _cards = new List<Card>();
        readonly Random _rng;
        readonly int _cutCard;
        int _next;

        public Shoe(int decks, double penetration, Random rng)
        {
            if (decks < 1) throw new ArgumentOutOfRangeException(nameof(decks));
            if (penetration <= 0 || penetration > 1) throw new ArgumentOutOfRangeException(nameof(penetration));
            _rng = rng ?? throw new ArgumentNullException(nameof(rng));

            for (int d = 0; d < decks; d++)
                foreach (Suit suit in Enum.GetValues(typeof(Suit)))
                    foreach (Rank rank in Enum.GetValues(typeof(Rank)))
                        _cards.Add(new Card(rank, suit));

            _cutCard = (int)(_cards.Count * penetration);
            Shuffle();
        }

        Shoe(IEnumerable<Card> stackedOrder)
        {
            _cards.AddRange(stackedOrder);
            _cutCard = _cards.Count;
        }

        /// <summary>A shoe that deals exactly these cards in order. For tests and scripted scenes.</summary>
        public static Shoe Stacked(params Card[] order) => new Shoe(order);

        public int Count => _cards.Count;
        public int Remaining => _cards.Count - _next;

        /// <summary>True once the cut card is out; shuffle before the next round.</summary>
        public bool PastCutCard => _next >= _cutCard;

        public void Shuffle()
        {
            if (_rng == null) throw new InvalidOperationException("A stacked shoe can't be shuffled.");
            for (int i = _cards.Count - 1; i > 0; i--)
            {
                int j = _rng.Next(i + 1);
                (_cards[i], _cards[j]) = (_cards[j], _cards[i]);
            }
            _next = 0;
        }

        public Card Draw()
        {
            if (_next >= _cards.Count)
            {
                // Only reachable mid-round on a nearly empty shoe; a real table would do the same.
                Shuffle();
            }
            return _cards[_next++];
        }
    }
}
