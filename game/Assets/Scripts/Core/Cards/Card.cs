using System;

namespace Starflare.Cards
{
    public enum Suit { Clubs, Diamonds, Hearts, Spades }

    public enum Rank { Two = 2, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, Ace }

    public readonly struct Card : IEquatable<Card>
    {
        public readonly Rank Rank;
        public readonly Suit Suit;

        public Card(Rank rank, Suit suit)
        {
            Rank = rank;
            Suit = suit;
        }

        /// <summary>Blackjack count with aces as 11; <see cref="Blackjack.Hand"/> softens them.</summary>
        public int BlackjackValue => Rank == Rank.Ace ? 11 : Rank >= Rank.Ten ? 10 : (int)Rank;

        public bool IsRed => Suit is Suit.Diamonds or Suit.Hearts;

        public string RankLabel => Rank switch
        {
            Rank.Ace => "A",
            Rank.King => "K",
            Rank.Queen => "Q",
            Rank.Jack => "J",
            _ => ((int)Rank).ToString(),
        };

        public string SuitSymbol => Suit switch
        {
            Suit.Clubs => "♣",
            Suit.Diamonds => "♦",
            Suit.Hearts => "♥",
            _ => "♠",
        };

        public bool Equals(Card other) => Rank == other.Rank && Suit == other.Suit;
        public override bool Equals(object obj) => obj is Card other && Equals(other);
        public override int GetHashCode() => (int)Rank * 4 + (int)Suit;
        public override string ToString() => RankLabel + SuitSymbol;
    }
}
