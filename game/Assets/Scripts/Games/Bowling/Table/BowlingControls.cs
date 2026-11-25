namespace Starflare.Bowling
{
    /// <summary>
    /// The lane's controls by the id a scene's "# point:" tag names them by, so Juan can point at
    /// each one through his first frame with you.
    /// </summary>
    public static class BowlingControls
    {
        /// <summary>The ball on the approach, dragged along the foul line.</summary>
        public const string Stand = "stand";
        public const string StandLeft = "stand_left";
        public const string StandRight = "stand_right";

        /// <summary>The aim meter: a tap starts the sweep over the arrows, the next stops it.</summary>
        public const string Aim = "aim";
        public const string Hook = "hook";
        public const string Speed = "speed";

        public const string TakeGame = "take_game";
        public const string WaveOff = "wave_off";

        /// <summary>After a challenge: his word read, back to your own lane.</summary>
        public const string Continue = "continue";

        /// <summary>Between games: another game.</summary>
        public const string Bowl = "bowl";

        public const string GetUp = "get_up";

        public static readonly string[] All = { Stand, StandLeft, StandRight, Aim, Hook, Speed, TakeGame, WaveOff, Continue, Bowl, GetUp };
    }
}
