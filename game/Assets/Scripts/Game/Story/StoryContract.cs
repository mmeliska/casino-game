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
        public const string CantSleep = "cant_sleep";
        public const string AskedDotAboutTheBar = "asked_dot_about_bar";
        public const string MetKasia = "met_kasia";
        public const string MetJuan = "met_juan";
        public const string CarryingCoupons = "carrying_coupons";
        public const string DidBarBack = "did_bar_back";
        public const string BackRoomOpen = "back_room_open";
        public const string SatWithRosa = "sat_with_rosa";
        public const string RolledWithJuan = "rolled_with_juan";

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

        // Raffy's envelope (A1.2): met him, holding the envelope (until A1.5 settles it), and
        // whether you've looked inside.
        public const string MetRaffy = "met_raffy";
        public const string HasEnvelope = "has_envelope";
        public const string OpenedEnvelope = "opened_envelope";

        // Bridgehead (A1.6, S3): met Mitch and the day you did; the trucker's been at the pumps,
        // and whether you spotted him the twenty, with the day (the seed's reveal waits on it).
        public const string MetMitch = "met_mitch";
        public const string MetMitchDay = "met_mitch_day";
        public const string TruckerPassed = "trucker_passed";
        public const string HelpedTrucker = "helped_trucker";
        public const string HelpedTruckerDay = "helped_trucker_day";

        // The Flare (A1.6, A1.7, R1): met Gus and the day you did, the boot and the jar; visits on
        // separate days and the last one, blackjack sittings; Pete and Jalen and the days you met
        // them; Gus's secret (the notice, then kept or told to Pete); Pete saying your name; Gus
        // and Dot asking after each other; Gus's trust.
        public const string MetGus = "met_gus";
        public const string MetGusDay = "met_gus_day";
        public const string RubbedBoot = "rubbed_boot";
        public const string TippedJar = "tipped_jar";
        public const string FlareVisits = "flare_visits";
        public const string FlareLastDay = "flare_last_day";
        public const string FlareSittings = "flare_sittings";
        public const string MetPete = "met_pete";
        public const string MetPeteDay = "met_pete_day";
        public const string MetJalen = "met_jalen";
        public const string MetJalenDay = "met_jalen_day";
        public const string SawNotice = "saw_notice";
        public const string SawNoticeDay = "saw_notice_day";
        public const string KeptGusSecret = "kept_gus_secret";
        public const string ToldPete = "told_pete";
        public const string PeteSaidName = "pete_said_name";
        public const string GusAskedAfterDot = "gus_asked_after_dot";
        public const string DotAskedAfterGus = "dot_asked_after_gus";
        public const string GusTrust = "gus_trust";

        // Raffy's envelope comes due (A1.5, envelope.ink): Ray asking and the day he started,
        // Dot's warning, finding Raffy, how it was settled ("returned", "kept", "helped", "lied")
        // and the day, giving Raffy up, his payments, the lie found out, Kasia's word about his
        // mother, and the drive.
        public const string RayAsking = "ray_asking";
        public const string RayAskingDay = "ray_asking_day";
        public const string DotWarnedEnvelope = "dot_warned_envelope";
        public const string FoundRaffy = "found_raffy";
        public const string EnvelopeSettled = "envelope_settled";
        public const string EnvelopeSettledDay = "envelope_settled_day";
        public const string GaveUpRaffy = "gave_up_raffy";
        public const string RaffyOwes = "raffy_owes";
        public const string RaffyPaidDay = "raffy_paid_day";
        public const string RaffyStoppedPaying = "raffy_stopped_paying";
        public const string RaffyPaidOff = "raffy_paid_off";
        public const string RayNamedYou = "ray_named_you";
        public const string KasiaMomCalled = "kasia_mom_called";
        public const string HasDrive = "has_drive";
        public const string DrivePluggedIn = "drive_plugged_in";
        public const string DrivePhonedHome = "drive_phoned_home";

        // Millstone Lanes' bowling (bowling spec). Until Pockets is built, what you buy at Juan's
        // counter is here: owns_ball is the ball's id ("urethane", "reactive"), empty for none.
        public const string BowledAtLanes = "bowled_at_lanes";
        public const string SubNights = "sub_nights";
        public const string ChallengesWon = "challenges_won";
        public const string OwnsBall = "owns_ball";
        public const string OwnsShoes = "owns_shoes";
        public const string SubAskedDay = "sub_asked_day";
        public const string SubTonightDay = "sub_tonight_day";
        public const string ShirtsAsked = "shirts_asked";
        public const string ChippedInShirts = "chipped_in_shirts";

        // Set by the lanes at the break point, for the lines after.
        public const string GamesBowled = "games_bowled";
        public const string BestGame = "best_game";
        public const string ChallengesWonTonight = "challenges_won_tonight";
        public const string SubGamesWon = "sub_games_won";
        public const string SubFinished = "sub_finished";

        // Set only by the debug panel's checkpoints, so a beat's "again" lines play as they would.
        public const string OfferedCoupons = "offered_coupons";
        public const string OfferedBarBack = "offered_bar_back";
        public const string MetRosa = "met_rosa";
        public const string SawLateGame = "saw_late_game";

        public static readonly string[] Variables =
        {
            PlayerName, Bankroll, Book, Day, NextSaturday, Hour, RegisterBox, Broke, SittingNet, CheckedIn, CantSleep, AskedDotAboutTheBar,
            MetKasia, MetJuan, CarryingCoupons, DidBarBack, BackRoomOpen, SatWithRosa, WeddingDay, WeddingMisses,
            WeddingAskedDay, WorkedWedding, WeddingOver, WeddingRole, DancedWith, DanceKeen, MetRaffy, HasEnvelope, OpenedEnvelope, MetMitch, MetMitchDay, TruckerPassed, HelpedTrucker, HelpedTruckerDay,
            OfferedCoupons, OfferedBarBack, MetRosa, RolledWithJuan, SawLateGame,
            BowledAtLanes, SubNights, ChallengesWon, OwnsBall, OwnsShoes, SubAskedDay, SubTonightDay, ShirtsAsked, ChippedInShirts,
            GamesBowled, BestGame, ChallengesWonTonight, SubGamesWon, SubFinished,
            MetGus, MetGusDay, RubbedBoot, TippedJar, FlareVisits, FlareLastDay, FlareSittings, MetPete, MetPeteDay,
            MetJalen, MetJalenDay, SawNotice, SawNoticeDay, KeptGusSecret, ToldPete, PeteSaidName,
            GusAskedAfterDot, DotAskedAfterGus, GusTrust,
            RayAsking, RayAskingDay, DotWarnedEnvelope, FoundRaffy, EnvelopeSettled, EnvelopeSettledDay, GaveUpRaffy,
            RaffyOwes, RaffyPaidDay, RaffyStoppedPaying, RaffyPaidOff, RayNamedYou, KasiaMomCalled, HasDrive, DrivePluggedIn,
            DrivePhonedHome,
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
            public const string OweRay = "owe_ray";

            public static readonly string[] All = { Spend, Earn, Borrow, Repay, Sleep, Work, Lean, OweRay };
        }

        /// <summary>Knots the code starts, by the event they play.</summary>
        public static class Knots
        {
            // Slack Water Motel (A1.1)
            public const string Arrival = "arrival";
            public const string CheckingIn = "checking_in";
            public const string Office = "office";
            public const string TurnIn = "turn_in";
            public const string OpenEnvelope = "open_envelope";

            // Kasia's Tap (A1.2, A1.3)
            public const string NightOne = "night_one";
            public const string WalkIn = "walk_in";
            public const string GetUp = "get_up";
            public const string CouponJob = "coupon_job";
            public const string BarBackJob = "bar_back_job";
            public const string BackRoomInvite = "back_room_invite";
            public const string Raffy = "raffy";

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

            // Millstone Lanes' back lot: the late game (street craps)
            public const string BackLot = "back_lot";
            public const string BackLotEmpty = "back_lot_empty";
            public const string BackLotPractice = "back_lot_practice";
            public const string BackLotGetUp = "back_lot_get_up";

            // Millstone Lanes' bowling
            public const string BowlFirst = "bowl_first";
            public const string BowlGetUp = "bowl_get_up";
            public const string LeagueSubAsk = "league_sub_ask";
            public const string LeagueSubDone = "league_sub_done";
            public const string BowlShirts = "bowl_shirts";

            // Bridgehead (A1.6, S3)
            public const string BridgeheadFirst = "bridgehead_first";
            public const string Mitch = "mitch";
            public const string Trucker = "trucker";
            public const string BridgeheadGetUp = "bridgehead_get_up";
            public const string BridgeheadSlotsGetUp = "bridgehead_slots_get_up";
            public const string BridgeheadScratchersGetUp = "bridgehead_scratchers_get_up";

            // The Flare (A1.6, A1.7)
            public const string FlareFirst = "flare_first";
            public const string Spaceman = "spaceman";
            public const string PeteFirst = "pete_first";
            public const string JalenFirst = "jalen_first";
            public const string FlareDecline = "flare_decline";
            public const string PeteName = "pete_name";
            public const string FlareGetUp = "flare_get_up";
            public const string FlareMachineGetUp = "flare_machine_get_up";
            public const string FlareLounge = "flare_lounge";

            // Raffy's envelope comes due (A1.5)
            public const string RayAsks = "ray_asks";
            public const string RaffyHiding = "raffy_hiding";
            public const string KeepEnvelope = "keep_envelope";
            public const string RayReturn = "ray_return";
            public const string RayNamesYou = "ray_names_you";
            public const string RaffysMother = "raffys_mother";
            public const string RaffyPays = "raffy_pays";
            public const string PlugInDrive = "plug_in_drive";

            public static readonly string[] All =
            {
                Arrival, CheckingIn, Office, TurnIn, OpenEnvelope, NightOne, WalkIn, GetUp, CouponJob, BarBackJob,
                BackRoomInvite, Raffy, BackRoom, RosaFirstHand, RosaGetUp, WeddingAsk, WeddingMissed, Wedding, Coupons, Juan,
                BackLot, BackLotEmpty, BackLotPractice, BackLotGetUp,
                BowlFirst, BowlGetUp, LeagueSubAsk, LeagueSubDone, BowlShirts,
                BridgeheadFirst, Mitch, Trucker, BridgeheadGetUp, BridgeheadSlotsGetUp, BridgeheadScratchersGetUp,
                FlareFirst, Spaceman, PeteFirst, JalenFirst, FlareDecline, PeteName, FlareGetUp, FlareMachineGetUp, FlareLounge,
                RayAsks, RaffyHiding, KeepEnvelope, RayReturn, RayNamesYou, RaffysMother, RaffyPays, PlugInDrive,
            };
        }
    }
}
