namespace Starflare.Game
{
    /// <summary>
    /// Everything the C# side knows the Ink story by name: the variables it reads or writes, the
    /// external functions it binds, and the knots it starts. Nowhere else spells these out, and
    /// <c>StoryContractTests</c> compiles <c>northlight.ink</c> and checks every one exists, so a
    /// rename on either side fails the tests instead of a scene.
    /// </summary>
    public static class StoryContract
    {
        // Pushed from the player state before every scene (StoryBridge.Sync).
        public const string PlayerName = "player_name";
        public const string Bankroll = "bankroll";
        public const string Book = "book";

        // Set by the venue before a scene at its machine.
        public const string Broke = "broke";
        public const string SittingNet = "sitting_net";

        // Story flags the city and venues read to pick a scene or open a place.
        public const string CheckedIn = "checked_in";
        public const string MetKasia = "met_kasia";
        public const string MetJuan = "met_juan";
        public const string CarryingCoupons = "carrying_coupons";

        public static readonly string[] Variables =
        {
            PlayerName, Bankroll, Book, Broke, SittingNet, CheckedIn, MetKasia, MetJuan, CarryingCoupons,
        };

        /// <summary>EXTERNAL functions the story calls; the bridge binds each to the player state.</summary>
        public static class Externals
        {
            public const string Spend = "spend";
            public const string Borrow = "borrow";
            public const string Repay = "repay";
            public const string Sleep = "sleep";

            public static readonly string[] All = { Spend, Borrow, Repay, Sleep };
        }

        /// <summary>Knots the code starts, by the event they play.</summary>
        public static class Knots
        {
            // Slack Water Motel (A1.1)
            public const string Arrival = "arrival";
            public const string CheckingIn = "checking_in";
            public const string Office = "office";
            public const string TurnIn = "turn_in";

            // Kasia's Tap (A1.2, A1.3)
            public const string NightOne = "night_one";
            public const string WalkIn = "walk_in";
            public const string GetUp = "get_up";
            public const string CouponJob = "coupon_job";

            // Millstone Lanes (A1.3)
            public const string Coupons = "coupons";
            public const string Juan = "juan";

            public static readonly string[] All =
            {
                Arrival, CheckingIn, Office, TurnIn, NightOne, WalkIn, GetUp, CouponJob, Coupons, Juan,
            };
        }
    }
}
