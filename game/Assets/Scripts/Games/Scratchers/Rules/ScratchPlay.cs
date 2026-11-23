namespace Starflare.Scratchers
{
    /// <summary>How a ticket's play area reads: what you scratch off and what makes it a winner.</summary>
    public enum ScratchPlay
    {
        /// <summary>Prize amounts under every spot; three of the same wins that amount.</summary>
        MatchThree,

        /// <summary>Winning numbers, then your numbers each over a prize; match one, win its prize.</summary>
        LuckyNumbers,

        /// <summary>A symbol and a prize under every spot; find the ticket's symbol, win its prize.</summary>
        FindSymbol,

        /// <summary>Their number, then yours each over a prize; beat theirs, win that prize.</summary>
        BeatTheirs,
    }
}
