using System;

namespace Starflare.Craps
{
    /// <summary>
    /// A practice turn: you shoot from set dice rather than fair ones, so a teacher can walk you
    /// through how the dice go, like Juan's first roll with you in the back lot (GDD, "Juan's
    /// practice roll"). It's played for show: no money changes hands, no round counts and the
    /// clock doesn't move (Mike, 2026-11-21). It ends when the set dice run out. Regular play
    /// stays fair; only a practice turn is ever set.
    /// </summary>
    public sealed class PracticeTurn
    {
        /// <summary>The pretend cash the bet buttons let you put down. Nothing is taken from yours.</summary>
        public int Stake { get; }

        /// <summary>Every throw of the turn, in order, whoever's shooting.</summary>
        public Roll[] Rolls { get; }

        public PracticeTurn(int stake, params Roll[] rolls)
        {
            if (stake <= 0) throw new ArgumentOutOfRangeException(nameof(stake));
            if (rolls == null || rolls.Length == 0) throw new ArgumentException("A practice turn needs at least one roll.", nameof(rolls));
            Stake = stake;
            Rolls = rolls;
        }
    }
}
