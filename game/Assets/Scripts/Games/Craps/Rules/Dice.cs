using System;
using System.Collections.Generic;

namespace Starflare.Craps
{
    /// <summary>
    /// The pair of dice in the circle. Fair by construction: each die is an even draw from one
    /// to six, and nothing outside can weight it. The only other dice are set ones, for a
    /// practice turn a teacher walks you through (Juan's first roll with you), which run out
    /// when the lesson does. Loaded dice (R3) will be their own thing, flagged by a story moment.
    /// </summary>
    public sealed class Dice
    {
        readonly Random _rng;
        readonly Queue<Roll> _set;

        public Dice(Random rng) => _rng = rng ?? throw new ArgumentNullException(nameof(rng));

        Dice(IEnumerable<Roll> rolls) => _set = new Queue<Roll>(rolls);

        /// <summary>Dice that throw exactly these rolls in order, then no more. For practice turns and tests.</summary>
        public static Dice Set(params Roll[] rolls)
        {
            if (rolls == null || rolls.Length == 0) throw new ArgumentException("Set dice need at least one roll.", nameof(rolls));
            return new Dice(rolls);
        }

        /// <summary>Set dice have rolls left (fair dice always do).</summary>
        public bool HasMore => _set == null || _set.Count > 0;

        public Roll Throw()
        {
            if (_set == null) return new Roll(_rng.Next(1, 7), _rng.Next(1, 7));
            if (_set.Count == 0) throw new InvalidOperationException("The set dice have no rolls left.");
            return _set.Dequeue();
        }
    }
}
