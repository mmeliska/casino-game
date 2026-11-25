using System;
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

        /// <summary>A1.2: met Kasia on night one, Raffy left you his envelope (unopened) and fifty, and you slept; the next morning, outside the Tap.</summary>
        public static readonly Checkpoint NightOneDone = new Checkpoint("A1.2 Night one done", CheckedIn, KasiasTap.PlaceId,
            (state, story) =>
            {
                story.Set(StoryContract.MetKasia, true);
                story.Set(StoryContract.MetRaffy, true);
                story.Set(StoryContract.HasEnvelope, true);
                state.Earn(RaffysFifty);
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

        /// <summary>
        /// A1.3: Juan has his box; 10:30 that night at Millstone Lanes, the late game on out back.
        /// "Step out back", then "Step into the circle", plays Juan's practice roll.
        /// </summary>
        public static readonly Checkpoint LateGame = new Checkpoint("A1.3 Late game out back", JuanHasTheBox, MillstoneLanes.PlaceId,
            (state, story) =>
            {
                var clock = state.Clock;
                int target = MillstoneBackLot.OpensAt + 30;
                state.PassTime(((target - clock.MinuteOfDay) % GameClock.MinutesPerDay + GameClock.MinutesPerDay) % GameClock.MinutesPerDay);
            });

        /// <summary>
        /// A1.3: Juan has his box; Wednesday 8 p.m. at Millstone Lanes, never bowled. "Bowl" plays
        /// Juan's coached frame over the lane, then your first open game.
        /// </summary>
        public static readonly Checkpoint OnTheLanes = new Checkpoint("A1.3 On the lanes", JuanHasTheBox, MillstoneLanes.PlaceId,
            (state, story) => GoTo(state, DayOfWeek.Wednesday, 20 * 60));

        /// <summary>
        /// A1.3: you've bowled here before; 6:45 p.m. on a Tuesday a team's short, so going
        /// inside plays the sub ask, and saying yes seats you for Chuy's team.
        /// </summary>
        public static readonly Checkpoint ShortABowler = new Checkpoint("A1.3 Short a bowler", OnTheLanes, MillstoneLanes.PlaceId,
            (state, story) =>
            {
                story.Set(StoryContract.BowledAtLanes, true);
                GoTo(state, DayOfWeek.Tuesday, 18 * 60 + 45, MillstoneLanes.ShortTuesday);
            });

        /// <summary>
        /// A1.3: three sub nights for his league (standing 6), shirts chipped in; Wednesday 8 p.m.,
        /// Juan's easy with you at the counter.
        /// </summary>
        public static readonly Checkpoint LeagueToJuan = new Checkpoint("A1.3 League to Juan", ShortABowler, MillstoneLanes.PlaceId,
            (state, story) =>
            {
                story.Set(StoryContract.SubNights, 3);
                story.Set(StoryContract.ShirtsAsked, true);
                story.Set(StoryContract.ChippedInShirts, true);
                GoTo(state, DayOfWeek.Wednesday, 20 * 60);
            });

        /// <summary>Moves the clock on to the next such weekday (today counts if it's still ahead) at a minute of the day.</summary>
        static void GoTo(PlayerState state, DayOfWeek weekday, int minute, Func<int, bool> dayOk = null)
        {
            var clock = state.Clock;
            int day = clock.Day;
            while ((DayOfWeek)(day % 7) != weekday || (day == clock.Day && clock.MinuteOfDay >= minute) || (dayOk != null && !dayOk(day))) day++;
            state.PassTime((day - clock.Day) * GameClock.MinutesPerDay + minute - clock.MinuteOfDay);
        }

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

        /// <summary>A1.6: stopped at Bridgehead and met Mitch; at the Flare, where arriving plays coffee with Gus.</summary>
        public static readonly Checkpoint AtTheFlare = new Checkpoint("A1.6 At the Flare", OverTheBridge, TheFlare.PlaceId,
            (state, story) =>
            {
                story.Set(StoryContract.MetMitch, true);
                story.Set(StoryContract.MetMitchDay, state.Clock.Day);
            });

        /// <summary>
        /// A1.6: had coffee with Gus and rubbed the boot; 8:00 p.m. the next day at the Flare, where
        /// arriving plays Pete at the door.
        /// </summary>
        public static readonly Checkpoint MetGus = new Checkpoint("A1.6 Met Gus", AtTheFlare, TheFlare.PlaceId,
            (state, story) =>
            {
                story.Set(StoryContract.MetGus, true);
                story.Set(StoryContract.MetGusDay, state.Clock.Day);
                story.Set(StoryContract.RubbedBoot, true);
                VisitTheFlare(state, story);
                NextNight(state);
            });

        /// <summary>A1.7: met Pete; 8:00 p.m. the next day at the Flare, where arriving plays Jalen at the bar. Dot's desk now plays her asking after Gus (R1).</summary>
        public static readonly Checkpoint MetPete = new Checkpoint("A1.7 Met Pete", MetGus, TheFlare.PlaceId,
            (state, story) =>
            {
                story.Set(StoryContract.MetPete, true);
                story.Set(StoryContract.MetPeteDay, state.Clock.Day);
                VisitTheFlare(state, story);
                NextNight(state);
            });

        /// <summary>A1.7: met Jalen; 8:00 p.m. the next day at the Flare, where arriving plays the decline and Gus's office.</summary>
        public static readonly Checkpoint MetJalen = new Checkpoint("A1.7 Met Jalen", MetPete, TheFlare.PlaceId,
            (state, story) =>
            {
                story.Set(StoryContract.MetJalen, true);
                story.Set(StoryContract.MetJalenDay, state.Clock.Day);
                VisitTheFlare(state, story);
                NextNight(state);
            });

        /// <summary>
        /// A1.7: saw the notice, told Gus about Dot, kept his secret from Pete, and sat at his
        /// table twice; 8:00 p.m. the next day at the Flare. Getting up from one more sitting plays
        /// Pete saying your name.
        /// </summary>
        public static readonly Checkpoint KeptGusSecret = new Checkpoint("A1.7 Kept Gus's secret", MetJalen, TheFlare.PlaceId,
            (state, story) =>
            {
                story.Set(StoryContract.SawNotice, true);
                story.Set(StoryContract.SawNoticeDay, state.Clock.Day);
                story.Set(StoryContract.GusAskedAfterDot, true);
                story.Set(StoryContract.KeptGusSecret, true);
                story.Set(StoryContract.GusTrust, 1);
                story.Set(StoryContract.FlareSittings, TheFlare.SittingsForYourName - 1);
                state.Lean("reliable");
                VisitTheFlare(state, story);
                NextNight(state);
            });

        /// <summary>A1.7: Pete has said your name; you're a regular. The next night, at the Flare.</summary>
        public static readonly Checkpoint ARegular = new Checkpoint("A1.7 A regular", KeptGusSecret, TheFlare.PlaceId,
            (state, story) =>
            {
                story.Set(StoryContract.FlareSittings, TheFlare.SittingsForYourName);
                story.Set(StoryContract.PeteSaidName, true);
                VisitTheFlare(state, story);
                NextNight(state);
            });

        /// <summary>
        /// A1.5: still holding Raffy's envelope (unopened), the Flare on the map; 8:00 p.m. on the
        /// due day at Kasia's Tap, where walking in plays Ray asking around.
        /// </summary>
        public static readonly Checkpoint EnvelopeDue = new Checkpoint("A1.5 Envelope due", AtTheFlare, KasiasTap.PlaceId,
            (state, story) =>
            {
                while (state.Clock.Day < RaffysEnvelope.DueDay) state.Sleep();
                NightOf(state);
            });

        /// <summary>A1.5: Ray's been asking since yesterday; 8:00 p.m. at Millstone Lanes, where going inside finds Raffy.</summary>
        public static readonly Checkpoint RayAsking = new Checkpoint("A1.5 Ray asking", EnvelopeDue, MillstoneLanes.PlaceId,
            (state, story) =>
            {
                story.Set(StoryContract.RayAsking, true);
                story.Set(StoryContract.RayAskingDay, state.Clock.Day);
                NextNight(state);
            });

        /// <summary>
        /// A1.5: told Raffy it's settled and kept the $2,000; three days on, 8:00 p.m. at the Flare,
        /// where arriving plays Ray coming to your table.
        /// </summary>
        public static readonly Checkpoint ToldRaffySettled = new Checkpoint("A1.5 Told Raffy it's settled", RayAsking, TheFlare.PlaceId,
            (state, story) =>
            {
                story.Set(StoryContract.FoundRaffy, true);
                Settle(state, story, "lied");
                state.Earn(EnvelopeCash);
                state.Lean("crooked");
                story.Set(StoryContract.RaffyOwes, RaffysShort);
                story.Set(StoryContract.RaffyPaidDay, state.Clock.Day);
                for (int i = 0; i < RaffysEnvelope.DaysTillRayFindsRaffy; i++) state.Sleep();
                NightOf(state);
            });

        /// <summary>A1.5: covered Raffy's $500; 8:00 p.m. the next day at Millstone Lanes, where going inside has his first payment.</summary>
        public static readonly Checkpoint HelpedRaffy = new Checkpoint("A1.5 Helped Raffy", RayAsking, MillstoneLanes.PlaceId,
            (state, story) =>
            {
                story.Set(StoryContract.FoundRaffy, true);
                if (state.BankrollNow < RaffysShort + 100) state.Earn(RaffysShort + 100 - state.BankrollNow); // a good night at the Flare
                Settle(state, story, "helped");
                state.Spend(RaffysShort);
                state.Lean("generous");
                story.Set(StoryContract.RaffyOwes, RaffysShort);
                story.Set(StoryContract.RaffyPaidDay, state.Clock.Day);
                NextNight(state);
            });

        /// <summary>The envelope settled as Ink's settle() leaves it: the drive in your pocket.</summary>
        static void Settle(PlayerState state, StoryBridge story, string how)
        {
            story.Set(StoryContract.EnvelopeSettled, how);
            story.Set(StoryContract.EnvelopeSettledDay, state.Clock.Day);
            story.Set(StoryContract.HasEnvelope, false);
            story.Set(StoryContract.OpenedEnvelope, true);
            story.Set(StoryContract.HasDrive, true);
        }

        /// <summary>The cash in Raffy's envelope, and what Ray's short of his $2,500 (raffy.md).</summary>
        const int EnvelopeCash = 2000, RaffysShort = 500;

        /// <summary>Today's visit to the Flare, counted the way arriving counts it.</summary>
        static void VisitTheFlare(PlayerState state, StoryBridge story)
        {
            story.Set(StoryContract.FlareVisits, story.Int(StoryContract.FlareVisits) + 1);
            story.Set(StoryContract.FlareLastDay, state.Clock.Day);
        }

        /// <summary>Sleeps, then on to 8:00 p.m. that day.</summary>
        static void NextNight(PlayerState state)
        {
            state.Sleep();
            NightOf(state);
        }

        /// <summary>On to 8:00 p.m. today, if it isn't later already.</summary>
        static void NightOf(PlayerState state)
        {
            int minutes = 20 * 60 - state.Clock.MinuteOfDay;
            if (minutes > 0) state.PassTime(minutes);
        }

        /// <summary>Raffy's tip on night one, out of the envelope's cash (raffy.md).</summary>
        const int RaffysFifty = 50;

        public static readonly Checkpoint[] All =
        {
            CheckedIn, NightOneDone, Broke, CarryingCoupons, JuanHasTheBox, LateGame, OnTheLanes, ShortABowler, LeagueToJuan,
            BarBackDone, BackRoomOpen, SatWithRosa,
            WeddingNight, WorkedTheWedding, OverTheBridge, TruckerAtThePumps,
            AtTheFlare, MetGus, MetPete, MetJalen, KeptGusSecret, ARegular,
            EnvelopeDue, RayAsking, ToldRaffySettled, HelpedRaffy,
        };
    }
}
