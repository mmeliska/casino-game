using System;
using System.Collections.Generic;

namespace Starflare.Craps
{
    /// <summary>
    /// The bets a craps game takes. Right and wrong are the street's names for pass and don't
    /// pass; a point bet is for or against the shooter's point coming before a 7.
    /// </summary>
    public enum BetKind { Right, Wrong, PointFor, PointAgainst }

    /// <summary>
    /// Limits and the house's take for one craps game. The defaults are Juan's late game in
    /// Millstone Lanes' back lot (GDD, "Street craps", Mike 2026-11-20; starting values to tune):
    /// $1 to $20 a bet, point bets at true odds, and Juan's cut of $1 for every full $10 a bet
    /// wins (Mike, 2026-11-21), the only edge, and the parts money. The Flare's felt table will
    /// set its own.
    /// </summary>
    [Serializable]
    public sealed class CrapsRules
    {
        public int MinBet = 1;
        public int MaxBet = 20;

        /// <summary>
        /// Juan takes a dollar for every this many dollars a bet wins, rounded down: "a dollar out
        /// of every ten you win". Tied to the win, not the bet, so no bet size dodges it or pays
        /// more than a tenth of a win.
        /// </summary>
        public int CutPer = 10;

        /// <summary>Who stands in the circle: you (seat 0) and three regulars.</summary>
        public int Seats = 4;

        /// <summary>
        /// What a bet of <paramref name="amount"/> wins with <paramref name="point"/> as the point
        /// (ignored for right and wrong), before the cut. True odds: for the point, 2 to 1 on 4
        /// and 10, 3 to 2 on 5 and 9, 6 to 5 on 6 and 8; against it, the reverse.
        /// </summary>
        public static int Winnings(BetKind kind, int amount, int point)
        {
            var (pays, per) = Odds(kind, point);
            return amount * pays / per;
        }

        /// <summary>
        /// A bet is takeable if it's within the limits and pays whole dollars: there's no change
        /// in the circle. A $1 bet on the 6 would win $1.20, so bets on the 6 come in fives.
        /// </summary>
        public bool Takes(BetKind kind, int amount, int point) =>
            amount >= MinBet && amount <= MaxBet && amount % Odds(kind, point).per == 0;

        /// <summary>Every amount the bet takes, smallest first, for the bet buttons.</summary>
        public List<int> Amounts(BetKind kind, int point)
        {
            var amounts = new List<int>();
            for (int amount = MinBet; amount <= MaxBet; amount++)
                if (Takes(kind, amount, point)) amounts.Add(amount);
            return amounts;
        }

        /// <summary>Juan's cut from <paramref name="winnings"/>, before it's paid.</summary>
        public int CutOn(int winnings) => winnings / CutPer;

        public static bool IsPointNumber(int total) => total == 4 || total == 5 || total == 6 || total == 8 || total == 9 || total == 10;

        static (int pays, int per) Odds(BetKind kind, int point)
        {
            if (kind == BetKind.Right || kind == BetKind.Wrong) return (1, 1);
            if (!IsPointNumber(point)) throw new ArgumentOutOfRangeException(nameof(point), "A point bet needs a point.");
            var forIt = point == 4 || point == 10 ? (2, 1)
                : point == 5 || point == 9 ? (3, 2)
                : (6, 5);
            return kind == BetKind.PointFor ? forIt : (forIt.Item2, forIt.Item1);
        }
    }
}
