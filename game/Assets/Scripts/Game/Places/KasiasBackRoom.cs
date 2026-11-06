using System;
using Starflare.Blackjack;
using Starflare.Cards;
using Starflare.State;
using UnityEngine;

namespace Starflare.Game
{
    /// <summary>
    /// Kasia's back room (A1.3): one lamp over Rosa's blackjack table, a dollar to fifty. Kasia
    /// lets you in once you've done her two jobs. Your first hand at Rosa's table is a practice
    /// hand on the house: your eighteen against her ace. She coaches it over the table, and you
    /// press the real buttons she points at: deal, insurance, stand. From then on the table is
    /// the shoe's, and always fair. The table never sees the
    /// story; this room plays the scenes around it.
    /// </summary>
    public sealed class KasiasBackRoom : MonoBehaviour
    {
        /// <summary>Rosa's table: the back room's limits, otherwise the house rules.</summary>
        static readonly BlackjackRules RosasRules = new BlackjackRules { MinBet = 1, MaxBet = 50 };

        static readonly int[] RosasChips = { 1, 5, 25 };

        /// <summary>Your 10 and 8 against her ace with a 6 under it, at $10 on the house; then cards for any hits.</summary>
        static PracticeHand RosasFirstHand() => new PracticeHand(10,
            new Card(Rank.Ten, Suit.Spades), new Card(Rank.Ace, Suit.Hearts),
            new Card(Rank.Eight, Suit.Diamonds), new Card(Rank.Six, Suit.Clubs),
            new Card(Rank.Four, Suit.Clubs), new Card(Rank.Nine, Suit.Hearts), new Card(Rank.Two, Suit.Spades),
            new Card(Rank.King, Suit.Diamonds), new Card(Rank.Three, Suit.Hearts), new Card(Rank.Five, Suit.Spades),
            new Card(Rank.Seven, Suit.Diamonds), new Card(Rank.Queen, Suit.Clubs), new Card(Rank.Two, Suit.Hearts),
            new Card(Rank.Three, Suit.Spades), new Card(Rank.Four, Suit.Diamonds), new Card(Rank.Five, Suit.Hearts),
            new Card(Rank.Six, Suit.Spades), new Card(Rank.Seven, Suit.Clubs), new Card(Rank.Eight, Suit.Hearts),
            new Card(Rank.Nine, Suit.Spades));

        IPlaceHost _host;
        BlackjackTable _table;
        Action _backToBar;

        public void Init(IPlaceHost host, Action backToBar)
        {
            _host = host;
            _backToBar = backToBar;
            _table = gameObject.AddComponent<BlackjackTable>();
            _table.Init(RosasRules, RosasChips);
            _table.BreakPointReached += OnBreakPoint;
        }

        /// <summary>Through the back door: Rosa says hello (the first time, you meet her).</summary>
        public void Enter() => PlayScene(StoryContract.Knots.BackRoom, Show);

        void Show()
        {
            _host.ShowPlace("kasia_backroom",
                ("Sit down at Rosa's table", SitDown),
                ("Back out to the bar", () => _backToBar()));
        }

        /// <summary>Sitting down. The first time, Rosa deals you a practice hand and coaches it.</summary>
        void SitDown() => _host.LeavePlaceScreen(SitDownNow);

        void SitDownNow()
        {
            if (_host.Story.Flag(StoryContract.SatWithRosa))
            {
                _table.SitDown(_host.State, _host.Pace);
                return;
            }
            _table.SitDown(_host.State, _host.Pace, RosasFirstHand());
            _host.PlaySceneOver(StoryContract.Knots.RosaFirstHand, _table, () => { });
        }

        void OnBreakPoint(BreakPoint point) => PlayScene(StoryContract.Knots.RosaGetUp, Show);

        void PlayScene(string knot, Action then)
        {
            _host.Story.Set(StoryContract.Broke, _host.State.BankrollNow < _table.Rules.MinBet);
            _host.Story.Set(StoryContract.SittingNet, _table.SittingNet);
            _host.PlayScene(knot, then);
        }
    }
}
