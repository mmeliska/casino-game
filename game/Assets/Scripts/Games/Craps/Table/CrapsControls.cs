namespace Starflare.Craps
{
    /// <summary>
    /// The craps circle's controls by the id a scene's "# point:" tag names them by, so Juan can
    /// point at each one through his practice roll. Each bet spot has a step up and a step down:
    /// "right_up", "against_down" and so on.
    /// </summary>
    public static class CrapsControls
    {
        public const string Throw = "throw";
        public const string CallIt = "call_it";
        public const string PassDice = "pass_dice";
        public const string GetUp = "get_up";

        public static string Up(BetKind kind) => Spot(kind) + "_up";
        public static string Down(BetKind kind) => Spot(kind) + "_down";

        static string Spot(BetKind kind) => kind switch
        {
            BetKind.Right => "right",
            BetKind.Wrong => "wrong",
            BetKind.PointFor => "for",
            _ => "against",
        };

        public static readonly string[] All =
        {
            Throw, CallIt, PassDice, GetUp,
            "right_up", "right_down", "wrong_up", "wrong_down",
            "for_up", "for_down", "against_up", "against_down",
        };
    }
}
