using Starflare.State;

namespace Starflare.Game
{
    /// <summary>
    /// The debug panel's checkpoints, one per beat built so far, in story order. Each leaves you
    /// where the next beat starts: load "Night one done" and walking into the Tap plays Kasia's
    /// coupon job. Add one here as each beat is built.
    /// </summary>
    public static class Checkpoints
    {
        /// <summary>A1.1: signed Dot's register and checked in, the first night, on the lot.</summary>
        public static readonly Checkpoint CheckedIn = new Checkpoint("A1.1 Checked in", null, SlackWaterMotel.PlaceId,
            (state, story) =>
            {
                state.SignRegister("Lou", RegisterBox.Ms);
                story.Set(StoryContract.CheckedIn, true);
            });

        /// <summary>A1.2: met Kasia on night one and slept; the next morning, outside the Tap.</summary>
        public static readonly Checkpoint NightOneDone = new Checkpoint("A1.2 Night one done", CheckedIn, KasiasTap.PlaceId,
            (state, story) =>
            {
                story.Set(StoryContract.MetKasia, true);
                state.Sleep();
            });

        /// <summary>A1.2: tapped out at the machine with nothing on Kasia's book, so getting up offers it.</summary>
        public static readonly Checkpoint Broke = new Checkpoint("A1.2 Broke at the Tap", NightOneDone, KasiasTap.PlaceId,
            (state, story) => state.Spend(state.BankrollNow));

        /// <summary>A1.3: took Kasia's box of coupons; at Millstone Lanes, before Juan.</summary>
        public static readonly Checkpoint CarryingCoupons = new Checkpoint("A1.3 Carrying the coupons", NightOneDone, MillstoneLanes.PlaceId,
            (state, story) =>
            {
                story.Set(StoryContract.OfferedCoupons, true);
                story.Set(StoryContract.CarryingCoupons, true);
            });

        /// <summary>A1.3: Juan has his box; walking into the Tap offers the bar-back shift.</summary>
        public static readonly Checkpoint JuanHasTheBox = new Checkpoint("A1.3 Juan has the box", CarryingCoupons, KasiasTap.PlaceId,
            (state, story) =>
            {
                story.Set(StoryContract.CarryingCoupons, false);
                story.Set(StoryContract.MetJuan, true);
            });

        /// <summary>A1.3: worked the shift and ran the wallet out (straight, flaky); walking in plays Kasia's nod.</summary>
        public static readonly Checkpoint BarBackDone = new Checkpoint("A1.3 Bar-back done", JuanHasTheBox, KasiasTap.PlaceId,
            (state, story) =>
            {
                story.Set(StoryContract.OfferedBarBack, true);
                state.Lean("straight");
                state.Lean("flaky");
                state.WorkShift(60, 4);
                story.Set(StoryContract.DidBarBack, true);
            });

        /// <summary>A1.3: the back door's open, and you said yes to the wedding next Saturday; Rosa's first hand is next.</summary>
        public static readonly Checkpoint BackRoomOpen = new Checkpoint("A1.3 Back room open", BarBackDone, KasiasTap.PlaceId,
            (state, story) =>
            {
                story.Set(StoryContract.BackRoomOpen, true);
                story.Set(StoryContract.WeddingAskedDay, state.Clock.Day);
                story.Set(StoryContract.WeddingDay, state.Clock.NextSaturday);
            });

        /// <summary>A1.3: met Rosa and played her practice hand; her table is the shoe's from here.</summary>
        public static readonly Checkpoint SatWithRosa = new Checkpoint("A1.3 Sat with Rosa", BackRoomOpen, KasiasTap.PlaceId,
            (state, story) =>
            {
                story.Set(StoryContract.MetRosa, true);
                story.Set(StoryContract.SatWithRosa, true);
            });

        /// <summary>A1.4: the Saturday she booked, 6:00 p.m., at the bar; "Go up to the hall" plays the wedding.</summary>
        public static readonly Checkpoint WeddingNight = new Checkpoint("A1.4 Wedding night", SatWithRosa, KasiasTap.PlaceId,
            (state, story) =>
            {
                var clock = state.Clock;
                int booked = story.Int(StoryContract.WeddingDay);
                state.PassTime((booked - clock.Day) * GameClock.MinutesPerDay + GameClock.NightStart - clock.MinuteOfDay);
            });

        /// <summary>A1.4: worked the wedding as a server and said you were on the clock both times; slept, Sunday morning.</summary>
        public static readonly Checkpoint WorkedTheWedding = new Checkpoint("A1.4 Worked the wedding", WeddingNight, KasiasTap.PlaceId,
            (state, story) =>
            {
                story.Set(StoryContract.WeddingRole, "server");
                state.WorkShift(120, 7);
                story.Set(StoryContract.WorkedWedding, true);
                story.Set(StoryContract.WeddingOver, true);
                state.Sleep();
            });

        /// <summary>A1.6: the wedding's behind you; at Bridgehead, where pulling in plays the stop on the way over.</summary>
        public static readonly Checkpoint OverTheBridge = new Checkpoint("A1.6 Over the bridge", WorkedTheWedding, Bridgehead.PlaceId,
            (state, story) => { });

        /// <summary>S3: met Mitch the day before; 10:00 p.m., pulling into Bridgehead plays the trucker at the pumps.</summary>
        public static readonly Checkpoint TruckerAtThePumps = new Checkpoint("S3 Trucker at the pumps", OverTheBridge, Bridgehead.PlaceId,
            (state, story) =>
            {
                story.Set(StoryContract.MetMitch, true);
                story.Set(StoryContract.MetMitchDay, state.Clock.Day);
                state.Sleep();
                var clock = state.Clock;
                state.PassTime(GameClock.NightStart + 4 * 60 - clock.MinuteOfDay);
            });

        public static readonly Checkpoint[] All =
        {
            CheckedIn, NightOneDone, Broke, CarryingCoupons, JuanHasTheBox, BarBackDone, BackRoomOpen, SatWithRosa,
            WeddingNight, WorkedTheWedding, OverTheBridge, TruckerAtThePumps,
        };
    }
}
