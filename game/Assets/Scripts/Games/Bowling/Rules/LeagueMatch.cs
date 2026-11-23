using System;
using System.Collections.Generic;

namespace Starflare.Bowling
{
    /// <summary>
    /// A sub night: a team a bowler short, you making up the four, three games against four
    /// across the lane, team pins winning each game and the side pot settling after each. The
    /// venue hands it in at sit-down; the table bowls it and reports the night.
    /// </summary>
    public sealed class LeagueMatch
    {
        public readonly string YourTeam, TheirTeam;
        public readonly IReadOnlyList<Bowler> Mates, Opponents;
        public readonly int Games, SidePot;

        readonly List<BowlingGame> _games = new List<BowlingGame>();

        public LeagueMatch(string yourTeam, string theirTeam, IReadOnlyList<Bowler> mates, IReadOnlyList<Bowler> opponents, int games = 3, int sidePot = 5)
        {
            YourTeam = yourTeam;
            TheirTeam = theirTeam;
            Mates = mates ?? throw new ArgumentNullException(nameof(mates));
            Opponents = opponents ?? throw new ArgumentNullException(nameof(opponents));
            if (opponents.Count != mates.Count + 1) throw new ArgumentException("Two teams of the same size, you making up yours.");
            Games = games;
            SidePot = sidePot;
        }

        /// <summary>The game under way, or the last one bowled.</summary>
        public BowlingGame Current => _games.Count == 0 ? null : _games[_games.Count - 1];

        public IReadOnlyList<BowlingGame> Played => _games;

        /// <summary>The next of the night's games. The bowlers keep their stands from game to game.</summary>
        public BowlingGame Next()
        {
            if (FinishedNight) throw new InvalidOperationException("That's the night.");
            if (Current != null && !Current.IsComplete) throw new InvalidOperationException("Finish this game first.");
            var game = BowlingGame.League(Mates, Opponents, SidePot);
            _games.Add(game);
            return game;
        }

        public int GamesFinished => Count(_ => true);
        public int GamesWon => Count(g => g.Result == Outcome.Won);
        public int GamesLost => Count(g => g.Result == Outcome.Lost);

        /// <summary>All three bowled: the night Juan counts.</summary>
        public bool FinishedNight => GamesFinished == Games;

        public bool TeamWonNight => FinishedNight && GamesWon > GamesLost;

        int Count(Func<BowlingGame, bool> which)
        {
            int n = 0;
            foreach (var g in _games) if (g.IsComplete && which(g)) n++;
            return n;
        }
    }
}
