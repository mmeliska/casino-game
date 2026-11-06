namespace Starflare.Blackjack
{
    /// <summary>
    /// The blackjack table's controls by the id a scene's "# point:" tag names them by, so a
    /// test can check every tag against this list. Chips are "chip_" and their value.
    /// </summary>
    public static class BlackjackControls
    {
        public const string Deal = "deal";
        public const string Clear = "clear";
        public const string Hit = "hit";
        public const string Stand = "stand";
        public const string Double = "double";
        public const string Split = "split";
        public const string Insurance = "insurance";
        public const string NoInsurance = "no_insurance";
        public const string GetUp = "get_up";

        public static string Chip(int value) => "chip_" + value;

        public static readonly string[] All = { Deal, Clear, Hit, Stand, Double, Split, Insurance, NoInsurance, GetUp };
    }
}
