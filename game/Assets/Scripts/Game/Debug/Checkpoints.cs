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

        /// <summary>A1.3: the back door's open; Rosa's first hand is next.</summary>
        public static readonly Checkpoint BackRoomOpen = new Checkpoint("A1.3 Back room open", BarBackDone, KasiasTap.PlaceId,
            (state, story) => story.Set(StoryContract.BackRoomOpen, true));

        /// <summary>A1.3: met Rosa and played her practice hand; her table is the shoe's from here.</summary>
        public static readonly Checkpoint SatWithRosa = new Checkpoint("A1.3 Sat with Rosa", BackRoomOpen, KasiasTap.PlaceId,
            (state, story) =>
            {
                story.Set(StoryContract.MetRosa, true);
                story.Set(StoryContract.SatWithRosa, true);
            });

        public static readonly Checkpoint[] All =
        {
            CheckedIn, NightOneDone, Broke, CarryingCoupons, JuanHasTheBox, BarBackDone, BackRoomOpen, SatWithRosa,
        };
    }
}
