using System;
using System.Collections.Generic;
using Starflare.Cards;

namespace Starflare.VideoPoker
{
    public enum VideoPokerPhase { Holding, Settled }

    /// <summary>
    /// One hand of draw poker: deal five, hold any of them, draw replacements from the same
    /// deck, settle. The caller shuffles a fresh deck before each hand.
    /// </summary>
    public sealed class VideoPokerRound
    {
        readonly VideoPokerRules _rules;
        readonly Shoe _deck;
        readonly Card[] _cards = new Card[5];
        readonly bool[] _held = new bool[5];

        public VideoPokerRound(VideoPokerRules rules, Shoe deck, int credits, int bankroll)
        {
            _rules = rules ?? throw new ArgumentNullException(nameof(rules));
            _deck = deck ?? throw new ArgumentNullException(nameof(deck));
            if (credits < 1 || credits > rules.MaxCredits) throw new ArgumentOutOfRangeException(nameof(credits));
            Credits = credits;
            if (Bet > bankroll) throw new InvalidOperationException("Not enough bankroll for that bet.");

            for (int i = 0; i < 5; i++) _cards[i] = _deck.Draw();
            Phase = VideoPokerPhase.Holding;
        }

        public VideoPokerPhase Phase { get; private set; }
        public int Credits { get; }
        public int Bet => Credits * _rules.CreditValue;
        public IReadOnlyList<Card> Cards => _cards;
        public bool IsHeld(int index) => _held[index];

        /// <summary>What the cards on screen make right now; after the draw, the final hand.</summary>
        public PokerHandRank Rank => PokerHand.Evaluate(_cards);

        public int Payout => Phase == VideoPokerPhase.Settled ? _rules.PayoutCredits(Rank, Credits) * _rules.CreditValue : 0;
        public int Net => Phase == VideoPokerPhase.Settled ? Payout - Bet : 0;

        public void ToggleHold(int index)
        {
            if (Phase != VideoPokerPhase.Holding) throw new InvalidOperationException("The hand is over.");
            _held[index] = !_held[index];
        }

        public void Draw()
        {
            if (Phase != VideoPokerPhase.Holding) throw new InvalidOperationException("The hand is over.");
            for (int i = 0; i < 5; i++)
                if (!_held[i]) _cards[i] = _deck.Draw();
            Phase = VideoPokerPhase.Settled;
        }
    }
}
