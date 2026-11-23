using System;
using System.Collections.Generic;

namespace Starflare.Bowling
{
    /// <summary>A regular at your lane between games: "Game for ten?"</summary>
    public sealed class Offer
    {
        public readonly Bowler Bowler;
        public readonly int Stake;

        public Offer(Bowler bowler, int stake)
        {
            Bowler = bowler;
            Stake = stake;
        }
    }

    /// <summary>
    /// Who comes over tonight, and when: about one game in four once you've bowled one, never
    /// two in a row, only a regular who's on the lanes, and never one who's lost three straight
    /// to you tonight ("I'm out. Go bowl Chuy."). Seeded, so a night plays the same on a reload.
    /// It's the table's own event, between rounds; it needs no story.
    /// </summary>
    public sealed class Challenges
    {
        readonly BowlingRules _rules;
        readonly Random _rng;
        readonly Dictionary<Bowler, NightRecord> _records = new Dictionary<Bowler, NightRecord>();
        bool _offeredLast;

        public Challenges(BowlingRules rules, int seed)
        {
            _rules = rules ?? throw new ArgumentNullException(nameof(rules));
            _rng = new Random(seed);
        }

        public int GamesBowled { get; private set; }

        public NightRecord RecordOf(Bowler bowler)
        {
            if (!_records.TryGetValue(bowler, out var record)) _records[bowler] = record = new NightRecord();
            return record;
        }

        /// <summary>He's lost three straight to you: done for the night.</summary>
        public bool IsDone(Bowler bowler) => RecordOf(bowler).LosingRun >= _rules.StraightLossesToQuit;

        /// <summary>
        /// A game's just finished: maybe someone comes over. After an offer (taken or waved off),
        /// never; otherwise one in three, which comes to about one game in four over a night.
        /// </summary>
        public Offer AfterGame(IReadOnlyList<Bowler> present, int bankroll)
        {
            GamesBowled++;
            if (_offeredLast)
            {
                _offeredLast = false;
                return null;
            }
            if (_rng.Next(_rules.ChallengeOdds - 1) != 0) return null;
            var willing = new List<Offer>();
            if (present != null)
                foreach (var bowler in present)
                {
                    if (IsDone(bowler)) continue;
                    int? stake = bowler.StakeAgainst(RecordOf(bowler), bankroll, _rules);
                    if (stake.HasValue) willing.Add(new Offer(bowler, stake.Value));
                }
            if (willing.Count == 0) return null;
            _offeredLast = true;
            return willing[_rng.Next(willing.Count)];
        }

        /// <summary>A challenge game's result, from your side, onto his record.</summary>
        public void Settled(Bowler bowler, Outcome yours)
        {
            var record = RecordOf(bowler);
            switch (yours)
            {
                case Outcome.Won:
                    record.Lost++;
                    record.LosingRun++;
                    break;
                case Outcome.Lost:
                    record.Won++;
                    record.LosingRun = 0;
                    break;
                default:
                    record.Pushed++;
                    break;
            }
        }
    }
}
