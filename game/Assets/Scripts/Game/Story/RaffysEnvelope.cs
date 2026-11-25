namespace Starflare.Game
{
    /// <summary>
    /// When Raffy's envelope comes due (A1.5) and what follows it, read off the story's flags, so
    /// each place asks one question and the rules sit in one place: Ray at Kasia's Tap, Raffy
    /// hiding at Millstone Lanes, Ray at the Flare's bar, the lie found out, Raffy's payments,
    /// and Kasia's word about his mother. The scenes are in <c>envelope.ink</c>.
    /// </summary>
    public static class RaffysEnvelope
    {
        /// <summary>"Weeks after night one": two weeks on (proposed, to tune).</summary>
        public const int DueDay = 15;

        /// <summary>Ray finds Raffy "within days" of the lie (proposed).</summary>
        public const int DaysTillRayFindsRaffy = 3;

        static bool Holding(StoryBridge story) => story.Flag(StoryContract.HasEnvelope);
        static string Settled(StoryBridge story) => story.Text(StoryContract.EnvelopeSettled);

        /// <summary>
        /// Walking into the Tap from the due day, still holding it: Ray, first on screen. He sends
        /// you to his spot at the Starflare's bar, so it waits until the Flare is on your map.
        /// </summary>
        public static bool RayAsksToday(StoryBridge story, int day) =>
            Holding(story) && !story.Flag(StoryContract.RayAsking) && day >= DueDay && story.Flag(StoryContract.MetMitch);

        /// <summary>Raffy, hiding at the Lanes, the first time you go in after Ray's been asking.</summary>
        public static bool RaffyHidingToday(StoryBridge story) =>
            Holding(story) && story.Flag(StoryContract.RayAsking) && !story.Flag(StoryContract.FoundRaffy);

        /// <summary>Ray's at the Flare's bar to hand it back to, while you hold it.</summary>
        public static bool RayAtTheBar(StoryBridge story) => Holding(story) && story.Flag(StoryContract.RayAsking);

        /// <summary>Your room offers keeping the cash once Ray's been asking.</summary>
        public static bool CanKeepIt(StoryBridge story) => RayAtTheBar(story);

        /// <summary>The lie found out: a few days on, Ray comes to your table at the Flare.</summary>
        public static bool RayNamesYouToday(StoryBridge story, int day) =>
            Settled(story) == "lied" && !story.Flag(StoryContract.RayNamedYou)
            && day >= story.Int(StoryContract.EnvelopeSettledDay) + DaysTillRayFindsRaffy;

        /// <summary>
        /// A payment a visit to the Lanes, a day after the last: on the help path until he's paid
        /// off; on the lie path, "a payment or two at most", until Ray finds him (raffy.md).
        /// </summary>
        public static bool RaffyPaysToday(StoryBridge story, int day)
        {
            if (day <= story.Int(StoryContract.RaffyPaidDay) || story.Flag(StoryContract.RaffyPaidOff)) return false;
            string settled = Settled(story);
            return settled == "helped" || (settled == "lied" && !story.Flag(StoryContract.RayNamedYou));
        }

        /// <summary>
        /// Raffy's gone (kept, given up, or the lie found out): on a later day, Kasia tells you his
        /// mother called the bar.
        /// </summary>
        public static bool KasiaHeardFromHisMother(StoryBridge story, int day)
        {
            if (story.Flag(StoryContract.KasiaMomCalled) || day <= story.Int(StoryContract.EnvelopeSettledDay)) return false;
            string settled = Settled(story);
            return settled == "kept" || story.Flag(StoryContract.GaveUpRaffy)
                || (settled == "lied" && story.Flag(StoryContract.RayNamedYou));
        }

        /// <summary>The drive is in your pocket and you haven't plugged it in yet.</summary>
        public static bool CanPlugInTheDrive(StoryBridge story) =>
            story.Flag(StoryContract.HasDrive) && !story.Flag(StoryContract.DrivePluggedIn);
    }
}
