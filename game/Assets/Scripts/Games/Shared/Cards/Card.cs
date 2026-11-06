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

        /// <summary>
        /// A card from its short code, rank then suit letter (<c>AS</c>, <c>10H</c>, <c>7C</c>):
        /// how tests and checkpoints stack a shoe.
        /// </summary>
        public static Card Parse(string code)
        {
            if (string.IsNullOrEmpty(code) || code.Length < 2) throw new FormatException($"Not a card: '{code}'");
            string rank = code.Substring(0, code.Length - 1);
            var r = rank switch
            {
                "A" => Rank.Ace,
                "K" => Rank.King,
                "Q" => Rank.Queen,
                "J" => Rank.Jack,
                _ when int.TryParse(rank, out int n) && n >= 2 && n <= 10 => (Rank)n,
                _ => throw new FormatException($"Not a card: '{code}'"),
            };
            var s = code[code.Length - 1] switch
            {
                'C' => Suit.Clubs,
                'D' => Suit.Diamonds,
                'H' => Suit.Hearts,
                'S' => Suit.Spades,
                _ => throw new FormatException($"Not a card: '{code}'"),
            };
            return new Card(r, s);
        }

        public bool Equals(Card other) => Rank == other.Rank && Suit == other.Suit;
        public override bool Equals(object obj) => obj is Card other && Equals(other);
        public override int GetHashCode() => (int)Rank * 4 + (int)Suit;
        public override string ToString() => RankLabel + SuitSymbol;
    }
}
