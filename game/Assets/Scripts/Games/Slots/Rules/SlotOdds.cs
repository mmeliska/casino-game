using System;
using System.Linq;

namespace Starflare.Slots
{
    /// <summary>
    /// A machine's exact odds, worked out over every way its reels can stop. For tuning a
    /// venue's strips and for the tests that hold them to it; the game never needs it.
    /// </summary>
    public readonly struct SlotOdds
    {
        /// <summary>The most stop combinations worth adding up (a 5-reel machine of 22 stops is about 5.2 million).</summary>
        public const long MaxCombinations = 6_000_000;

        SlotOdds(double returnToPlayer, double hitRate)
        {
            ReturnToPlayer = returnToPlayer;
            HitRate = hitRate;
        }

        /// <summary>What comes back per dollar bet, over the long run.</summary>
        public double ReturnToPlayer { get; }

        /// <summary>The share of pulls that pay anything.</summary>
        public double HitRate { get; }

        public static SlotOdds Of(SlotRules rules, int credits)
        {
            long combinations = rules.Reels.Aggregate(1L, (n, r) => n * r.Count);
            if (combinations > MaxCombinations)
                throw new InvalidOperationException($"{combinations} stop combinations is too many to add up.");

            var stops = new int[rules.Columns];
            long paid = 0, hits = 0;
            for (long i = 0; i < combinations; i++)
            {
                long rest = i;
                for (int c = 0; c < stops.Length; c++)
                {
                    int length = rules.Reels[c].Count;
                    stops[c] = (int)(rest % length);
                    rest /= length;
                }
                var spin = new SlotSpin(rules, credits, stops);
                paid += spin.Payout;
                if (spin.Payout > 0) hits++;
            }
            double bet = rules.BetFor(credits);
            return new SlotOdds(paid / (bet * combinations), hits / (double)combinations);
        }
    }
}
