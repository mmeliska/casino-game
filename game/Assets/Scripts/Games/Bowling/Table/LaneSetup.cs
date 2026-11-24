using System.Collections.Generic;
using UnityEngine;

namespace Starflare.Bowling
{
    /// <summary>
    /// What the venue hands the lane at sit-down: tonight's oil (worn by the clock), the ball you
    /// own and whether you've got shoes, who's on the lanes and their faces, a sub night's match if you're
    /// subbing, a coached frame if a teacher's over your shoulder, and the night's seed. The lane
    /// never works any of it out.
    /// </summary>
    public sealed class LaneSetup
    {
        public LaneOil Oil;
        public BowlingBall Ball = BowlingBall.House;
        public bool OwnShoes;
        public IReadOnlyList<Bowler> Regulars = new Bowler[0];

        /// <summary>The regulars' and a sub night's bowlers' faces, by <see cref="Bowler.Face"/>; a missing one shows an initial.</summary>
        public IReadOnlyDictionary<string, Sprite> Faces = new Dictionary<string, Sprite>();

        public LeagueMatch Match;
        public PracticeFrame Practice;
        public int Seed;
    }
}
