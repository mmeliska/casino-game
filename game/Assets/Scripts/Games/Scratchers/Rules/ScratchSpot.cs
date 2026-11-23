namespace Starflare.Scratchers
{
    /// <summary>One of your spots under the coating: what it shows, the prize printed with it, and whether it's the win.</summary>
    public sealed class ScratchSpot
    {
        public ScratchSpot(string symbol, int number, int prize, bool wins)
        {
            Symbol = symbol ?? "";
            Number = number;
            Prize = prize;
            Wins = wins;
        }

        /// <summary>What's printed big: a number, or a symbol to find. Empty on a match-three ticket, whose spots are only amounts.</summary>
        public string Symbol { get; }

        /// <summary>The spot's number, on tickets with numbers; 0 otherwise.</summary>
        public int Number { get; }

        public int Prize { get; }

        /// <summary>This spot makes the ticket a winner (on a match-three, each of the three alike).</summary>
        public bool Wins { get; }
    }
}
