namespace Starflare.Bowling
{
    /// <summary>A place in a game's order: a regular, or you (no bowler), on a team, with a sheet.</summary>
    public sealed class Seat
    {
        public readonly Bowler Bowler;
        public readonly int Team;
        public readonly ScoreSheet Sheet = new ScoreSheet();

        public Seat(Bowler bowler, int team)
        {
            Bowler = bowler;
            Team = team;
        }

        public bool IsYou => Bowler == null;
        public string Name => Bowler?.Name ?? "You";
    }
}
