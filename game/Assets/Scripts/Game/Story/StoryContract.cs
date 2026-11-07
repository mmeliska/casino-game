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
        public const string Day = "day";
        public const string NextSaturday = "next_saturday";
        public const string Hour = "hour";
        public const string RegisterBox = "register_box";

        // Set by the venue before a scene at its machine.
        public const string Broke = "broke";
        public const string SittingNet = "sitting_net";

        // Story flags the city and venues read to pick a scene or open a place.
        public const string CheckedIn = "checked_in";
        public const string MetKasia = "met_kasia";
        public const string MetJuan = "met_juan";
        public const string CarryingCoupons = "carrying_coupons";
        public const string DidBarBack = "did_bar_back";
        public const string BackRoomOpen = "back_room_open";
        public const string SatWithRosa = "sat_with_rosa";

        // Kasia's wedding (A1.4): the day it's booked for (0 until it is), how many times you've
        // said no or not shown up, the day she last asked, whether it's behind you, and (for the
        // romance characters later) the job you took and who you danced with.
        public const string WeddingDay = "wedding_day";
        public const string WeddingMisses = "wedding_misses";
        public const string WeddingAskedDay = "wedding_asked_day";
        public const string WorkedWedding = "worked_wedding";
        public const string WeddingOver = "wedding_over";
        public const string WeddingRole = "wedding_role";
        public const string DancedWith = "danced_with";
        public const string DanceKeen = "dance_keen";

        // Set only by the debug panel's checkpoints, so a beat's "again" lines play as they would.
        public const string OfferedCoupons = "offered_coupons";
        public const string OfferedBarBack = "offered_bar_back";
        public const string MetRosa = "met_rosa";

        public static readonly string[] Variables =
        {
            PlayerName, Bankroll, Book, Day, NextSaturday, Hour, RegisterBox, Broke, SittingNet, CheckedIn,
            MetKasia, MetJuan, CarryingCoupons, DidBarBack, BackRoomOpen, SatWithRosa, WeddingDay, WeddingMisses,
            WeddingAskedDay, WorkedWedding, WeddingOver, WeddingRole, DancedWith, DanceKeen, OfferedCoupons, OfferedBarBack, MetRosa,
        };

        /// <summary>EXTERNAL functions the story calls; the bridge binds each to the player state.</summary>
        public static class Externals
        {
            public const string Spend = "spend";
            public const string Earn = "earn";
            public const string Borrow = "borrow";
            public const string Repay = "repay";
            public const string Sleep = "sleep";
            public const string Work = "work";
            public const string Lean = "lean";

            public static readonly string[] All = { Spend, Earn, Borrow, Repay, Sleep, Work, Lean };
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
            public const string BarBackJob = "bar_back_job";
            public const string BackRoomInvite = "back_room_invite";

            // Kasia's back room (A1.3)
            public const string BackRoom = "back_room";
            public const string RosaFirstHand = "rosa_first_hand";
            public const string RosaGetUp = "rosa_get_up";

            // The wedding upstairs (A1.4)
            public const string WeddingAsk = "wedding_ask";
            public const string WeddingMissed = "wedding_missed";
            public const string Wedding = "wedding";

            // Millstone Lanes (A1.3)
            public const string Coupons = "coupons";
            public const string Juan = "juan";

            public static readonly string[] All =
            {
                Arrival, CheckingIn, Office, TurnIn, NightOne, WalkIn, GetUp, CouponJob, BarBackJob,
                BackRoomInvite, BackRoom, RosaFirstHand, RosaGetUp, WeddingAsk, WeddingMissed, Wedding, Coupons, Juan,
            };
        }
    }
}
