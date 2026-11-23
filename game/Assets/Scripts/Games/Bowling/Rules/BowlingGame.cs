using System;
using System.Collections.Generic;

namespace Starflare.Bowling
{
    /// <summary>What kind of game: open bowling on your own, a regular's challenge, or a league game you sub in.</summary>
    public enum GameKind { Open, Challenge, League }

    /// <summary>How a money game came out, from your side.</summary>
    public enum Outcome { Won, Lost, Push }

    /// <summary>
    /// One game of ten frames: who bowls in what order, a frame at a time, and what it settles
    /// for. Open bowling costs the lane fee; a challenge is high score for the stake he named, a
    /// tie a push, nobody taking a cut; a league game is team pins for the side pot. No fee on a
    /// challenge or a league game.
    /// </summary>
    public sealed class BowlingGame
    {
        public const int Yours = 0, Theirs = 1;

        readonly List<Seat> _seats;
        int _up;

        public readonly GameKind Kind;

        /// <summary>Open bowling: the fee (and the shoes, the first game of a visit).</summary>
        public readonly int Fee;

        /// <summary>A challenge: what each puts up. A league game: the side pot, a bowler.</summary>
        public readonly int Stake;

        BowlingGame(GameKind kind, int fee, int stake, List<Seat> seats)
        {
            Kind = kind;
            Fee = fee;
            Stake = stake;
            _seats = seats;
        }

        public static BowlingGame Open(int fee) =>
            new BowlingGame(GameKind.Open, fee, 0, new List<Seat> { new Seat(null, Yours) });

        /// <summary>You and him, alternating frames, you first.</summary>
        public static BowlingGame Challenge(Bowler opponent, int stake)
        {
            if (opponent == null) throw new ArgumentNullException(nameof(opponent));
            return new BowlingGame(GameKind.Challenge, 0, stake, new List<Seat> { new Seat(null, Yours), new Seat(opponent, Theirs) });
        }

        /// <summary>
        /// Your team (you and three) against theirs (four), alternating a bowler from each side, a
        /// frame at a time: you lead off.
        /// </summary>
        public static BowlingGame League(IReadOnlyList<Bowler> mates, IReadOnlyList<Bowler> opponents, int sidePot)
        {
            if (mates == null) throw new ArgumentNullException(nameof(mates));
            if (opponents == null) throw new ArgumentNullException(nameof(opponents));
            if (opponents.Count != mates.Count + 1) throw new ArgumentException("Two teams of the same size, you making up yours.");
            var seats = new List<Seat>();
            for (int i = 0; i < opponents.Count; i++)
            {
                seats.Add(new Seat(i == 0 ? null : mates[i - 1], Yours));
                seats.Add(new Seat(opponents[i], Theirs));
            }
            return new BowlingGame(GameKind.League, 0, sidePot, seats);
        }

        public IReadOnlyList<Seat> Seats => _seats;

        /// <summary>Who's up: the bowler the next ball belongs to.</summary>
        public Seat Up => _seats[_up];

        public bool YourTurn => Up.IsYou;

        /// <summary>Your sheet.</summary>
        public ScoreSheet You
        {
            get
            {
                foreach (var seat in _seats) if (seat.IsYou) return seat.Sheet;
                throw new InvalidOperationException("You're not in this game.");
            }
        }

        public bool IsComplete
        {
            get
            {
                foreach (var seat in _seats) if (!seat.Sheet.IsComplete) return false;
                return true;
            }
        }

        /// <summary>
        /// Pins down for whoever's up. When their frame's over, the next seat is up (and faces a
        /// fresh rack). Returns true when the turn passed.
        /// </summary>
        public bool Record(int pins)
        {
            if (IsComplete) throw new InvalidOperationException("The game's over.");
            var sheet = Up.Sheet;
            int frame = sheet.CurrentFrame;
            sheet.Record(pins);
            if (!sheet.IsComplete && sheet.CurrentFrame == frame) return false;
            if (IsComplete) return true;
            do _up = (_up + 1) % _seats.Count;
            while (_seats[_up].Sheet.IsComplete);
            return true;
        }

        /// <summary>A team's pins so far.</summary>
        public int TeamPins(int team)
        {
            int pins = 0;
            foreach (var seat in _seats) if (seat.Team == team) pins += seat.Sheet.Score;
            return pins;
        }

        /// <summary>A finished money game's result, from your side; open bowling has none.</summary>
        public Outcome Result
        {
            get
            {
                if (!IsComplete) throw new InvalidOperationException("The game isn't over.");
                if (Kind == GameKind.Open) throw new InvalidOperationException("Open bowling isn't won or lost.");
                int yours = TeamPins(Yours), theirs = TeamPins(Theirs);
                return yours > theirs ? Outcome.Won : yours < theirs ? Outcome.Lost : Outcome.Push;
            }
        }

        /// <summary>What the game settles for you, once it's over: the round's net.</summary>
        public int Settle()
        {
            if (!IsComplete) throw new InvalidOperationException("The game isn't over.");
            if (Kind == GameKind.Open) return -Fee;
            var result = Result;
            return result == Outcome.Won ? Stake : result == Outcome.Lost ? -Stake : 0;
        }
    }
}
