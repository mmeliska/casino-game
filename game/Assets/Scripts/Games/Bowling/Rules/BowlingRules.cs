using System;
using System.Collections.Generic;

namespace Starflare.Bowling
{
    /// <summary>
    /// What the Lanes charge and pay, and how the meters behave. The defaults are Millstone's
    /// (Mike, 2026-11-23, with Claude's numbers where marked on the spec page; starting values to
    /// tune): a low fee so a fair bowler can come out ahead, rentals until you own shoes, stakes
    /// the bowler names, and Juan's counter.
    /// </summary>
    [Serializable]
    public sealed class BowlingRules
    {
        /// <summary>Open bowling, a game.</summary>
        public int LaneFee = 2;

        /// <summary>Rental shoes, once a visit, until you own a pair.</summary>
        public int ShoeRental = 2;

        /// <summary>What a regular names for a game: from his average and how tonight's gone.</summary>
        public int[] Stakes = { 5, 10, 20 };

        /// <summary>About one game in this many, a regular comes over (never twice running).</summary>
        public int ChallengeOdds = 4;

        /// <summary>Losses in a row to you before a regular's done for the night.</summary>
        public int StraightLossesToQuit = 3;

        /// <summary>The league's side pot, a bowler a game; winners split it.</summary>
        public int SidePot = 5;

        public int LeagueGames = 3;
        public int TeamSize = 4;

        /// <summary>Juan's counter.</summary>
        public int UrethanePrice = 40, ReactivePrice = 120, ShoesPrice = 30;

        /// <summary>How fast the sweeps run: boards a second for aim, per cent a second for hook and speed.</summary>
        public double AimSweep = 16, HookSweep = 70, SpeedSweep = 70;

        /// <summary>
        /// Off by default: what you stop is what you throw. If playtesting finds that too exact,
        /// up to this many boards and per cent of error go on top.
        /// </summary>
        public double AimJitter = 0, MeterJitter = 0;

        public Hand Hand = Hand.Right;

        /// <summary>What open bowling costs this game: the fee, and the shoes if they're still to pay.</summary>
        public int OpenGameCost(bool rentingShoes, bool shoesPaid) =>
            LaneFee + (rentingShoes && !shoesPaid ? ShoeRental : 0);

        /// <summary>The price of a ball on the counter; the house ball isn't for sale.</summary>
        public int PriceOf(BowlingBall ball) =>
            ball == BowlingBall.Urethane ? UrethanePrice : ball == BowlingBall.Reactive ? ReactivePrice
            : throw new ArgumentException("The house ball isn't for sale.", nameof(ball));

        /// <summary>The balls on Juan's counter you don't have yet.</summary>
        public IEnumerable<BowlingBall> BallsForSale(BowlingBall owned)
        {
            if (owned != BowlingBall.Urethane && owned != BowlingBall.Reactive) yield return BowlingBall.Urethane;
            if (owned != BowlingBall.Reactive) yield return BowlingBall.Reactive;
        }

        /// <summary>
        /// The throw the meters give, with the optional jitter on top. With the jitter off (the
        /// default) this is the throw you stopped.
        /// </summary>
        public Throw Apply(Throw stopped, Random rng)
        {
            if (AimJitter <= 0 && MeterJitter <= 0) return stopped.Clamp();
            double J(double size) => size * (2 * rng.NextDouble() - 1);
            return new Throw(stopped.Stand, stopped.Aim + J(AimJitter),
                stopped.Hook + J(MeterJitter), stopped.Speed * (1 + J(MeterJitter) / 100)).Clamp();
        }
    }
}
