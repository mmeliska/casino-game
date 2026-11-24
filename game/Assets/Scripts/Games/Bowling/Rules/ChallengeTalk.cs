namespace Starflare.Bowling
{
    /// <summary>
    /// What a regular says over a challenge, under his face (Mike, 2026-11-24): the ask ("Game
    /// for ten?"), his answer when you take it or wave him off, and his word when the game's
    /// settled. Short and plain, Great Lakes understatement. Which line is picked by a number the
    /// lane passes in (the games bowled), so a night reads the same on a reload.
    /// </summary>
    public static class ChallengeTalk
    {
        static readonly string[] Asks = { "Game for {0}?", "Bowl one with me? I'll go {0}.", "Got a game in you? Say {0}." };
        static readonly string[] Takes = { "Rack 'em.", "Good. Don't get comfortable.", "Alright. Bowl your game." };
        static readonly string[] WaveOffs = { "Suit yourself.", "Another night, then.", "Fair enough." };
        static readonly string[] YouWon = { "Nice game. Here.", "You earned that one.", "Fair and square. Here's your money." };
        static readonly string[] YouLost = { "Good game. I'll take that.", "Close. Not that close.", "Next time, maybe." };
        static readonly string[] Pushed = { "Dead even. Call it a wash.", "Nobody wins. Fine by me." };

        /// <summary>He names the stake, in words.</summary>
        public static string Ask(int stake, int pick) => string.Format(Of(Asks, pick), Words(stake));

        public static string Taken(int pick) => Of(Takes, pick);

        public static string WavedOff(int pick) => Of(WaveOffs, pick);

        /// <summary>
        /// His word when the game's settled, by your result. On his third straight loss he's
        /// done ("I'm out. Go bowl Chuy."), sending you to another regular if one's on the lanes.
        /// </summary>
        public static string Settled(Outcome yours, bool done, string sendTo, int pick)
        {
            if (done) return string.IsNullOrEmpty(sendTo) ? "I'm out for the night." : $"I'm out. Go bowl {sendTo}.";
            switch (yours)
            {
                case Outcome.Won: return Of(YouWon, pick);
                case Outcome.Lost: return Of(YouLost, pick);
                default: return Of(Pushed, pick);
            }
        }

        /// <summary>Every line he can say, for the writing rules' checks.</summary>
        public static string[] All()
        {
            var all = new System.Collections.Generic.List<string>();
            foreach (int stake in new[] { 5, 10, 20 })
                for (int i = 0; i < Asks.Length; i++) all.Add(Ask(stake, i));
            all.AddRange(Takes);
            all.AddRange(WaveOffs);
            all.AddRange(YouWon);
            all.AddRange(YouLost);
            all.AddRange(Pushed);
            all.Add(Settled(Outcome.Won, true, "Chuy", 0));
            all.Add(Settled(Outcome.Won, true, null, 0));
            return all.ToArray();
        }

        static string Of(string[] lines, int pick) => lines[(pick % lines.Length + lines.Length) % lines.Length];

        static string Words(int stake)
        {
            switch (stake)
            {
                case 5: return "five";
                case 10: return "ten";
                case 15: return "fifteen";
                case 20: return "twenty";
                default: return stake.ToString();
            }
        }
    }
}
