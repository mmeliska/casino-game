using System;
using System.Collections.Generic;
using Starflare.Cards;

namespace Starflare.Blackjack
{
    /// <summary>Insurance comes first, and only when the dealer shows an ace and the table offers it.</summary>
    public enum RoundPhase { InsuranceOffered, PlayerTurn, Settled }

    /// <summary>
    /// One round of blackjack from the deal to settlement. Pure rules: it never touches
    /// the bankroll or story state; the caller applies <see cref="Net"/> when it settles.
    /// </summary>
    public sealed class BlackjackRound
    {
        readonly BlackjackRules _rules;
        readonly Shoe _shoe;
        readonly int _bankroll;
        readonly List<PlayerHand> _hands = new List<PlayerHand>();

        public BlackjackRound(BlackjackRules rules, Shoe shoe, int bet, int bankroll)
        {
            _rules = rules ?? throw new ArgumentNullException(nameof(rules));
            _shoe = shoe ?? throw new ArgumentNullException(nameof(shoe));
            if (bet < rules.MinBet || bet > rules.MaxBet) throw new ArgumentOutOfRangeException(nameof(bet));
            if (bet > bankroll) throw new InvalidOperationException("Bet is more than the bankroll.");
            _bankroll = bankroll;

            var hand = new PlayerHand { Bet = bet };
            _hands.Add(hand);
            hand.Add(_shoe.Draw());
            Dealer.Add(_shoe.Draw());
            hand.Add(_shoe.Draw());
            Dealer.Add(_shoe.Draw());

            if (_rules.OfferInsurance && Dealer.Cards[0].Rank == Rank.Ace
                && InsuranceCost > 0 && bet + InsuranceCost <= bankroll)
            {
                Phase = RoundPhase.InsuranceOffered;
                return;
            }
            Peek();
        }

        /// <summary>The side bet insurance takes: half the bet, in whole dollars.</summary>
        public int InsuranceCost => _hands[0].Bet / 2;

        /// <summary>What's on insurance: nothing unless you took it.</summary>
        public int Insurance { get; private set; }

        /// <summary>Insurance pays 2:1 when the dealer has blackjack, and is lost otherwise.</summary>
        public int InsuranceNet { get; private set; }

        public bool CanInsure => Phase == RoundPhase.InsuranceOffered;

        /// <summary>
        /// The dealer shows an ace but the bet is too small to halve in whole dollars ($1), so
        /// there's no insurance, as at a real table with no half chips.
        /// </summary>
        public bool InsuranceTooSmall => _rules.OfferInsurance && Dealer.Cards[0].Rank == Rank.Ace && InsuranceCost == 0;

        public void TakeInsurance()
        {
            Require(CanInsure, nameof(TakeInsurance));
            Insurance = InsuranceCost;
            Peek();
        }

        public void DeclineInsurance()
        {
            Require(CanInsure, nameof(DeclineInsurance));
            Peek();
        }

        /// <summary>The dealer peeks under an ace or ten, so a dealer blackjack ends the round at once.</summary>
        void Peek()
        {
            var hand = _hands[0];
            if (Dealer.IsBlackjack || hand.IsBlackjack)
            {
                hand.Done = true;
                Settle();
                return;
            }
            Phase = RoundPhase.PlayerTurn;
        }

        public RoundPhase Phase { get; private set; }
        public IReadOnlyList<PlayerHand> Hands => _hands;
        public Hand Dealer { get; } = new Hand();
        public int ActiveHandIndex { get; private set; }
        public PlayerHand ActiveHand => Phase == RoundPhase.PlayerTurn ? _hands[ActiveHandIndex] : null;
        public bool DealerHoleRevealed => Phase == RoundPhase.Settled;

        public int TotalStake
        {
            get
            {
                int stake = Insurance;
                foreach (var hand in _hands) stake += hand.Bet;
                return stake;
            }
        }

        public int Net
        {
            get
            {
                int net = InsuranceNet;
                foreach (var hand in _hands) net += hand.Net;
                return net;
            }
        }

        public bool CanHit => Phase == RoundPhase.PlayerTurn;
        public bool CanStand => Phase == RoundPhase.PlayerTurn;

        public bool CanDouble =>
            Phase == RoundPhase.PlayerTurn
            && ActiveHand.Cards.Count == 2
            && (!ActiveHand.FromSplit || _rules.DoubleAfterSplit)
            && TotalStake + ActiveHand.Bet <= _bankroll;

        public bool CanSplit =>
            Phase == RoundPhase.PlayerTurn
            && _hands.Count < _rules.MaxHands
            && ActiveHand.Cards.Count == 2
            && ActiveHand.Cards[0].Rank == ActiveHand.Cards[1].Rank
            && TotalStake + ActiveHand.Bet <= _bankroll;

        public void Hit()
        {
            Require(CanHit, nameof(Hit));
            var hand = ActiveHand;
            hand.Add(_shoe.Draw());
            if (hand.Total >= 21) FinishActiveHand();
        }

        public void Stand()
        {
            Require(CanStand, nameof(Stand));
            FinishActiveHand();
        }

        public void Double()
        {
            Require(CanDouble, nameof(Double));
            var hand = ActiveHand;
            hand.Bet *= 2;
            hand.Doubled = true;
            hand.Add(_shoe.Draw());
            FinishActiveHand();
        }

        public void Split()
        {
            Require(CanSplit, nameof(Split));
            var first = ActiveHand;
            var second = new PlayerHand { Bet = first.Bet, FromSplit = true };
            first.FromSplit = true;
            second.Add(first.RemoveLast());
            _hands.Insert(ActiveHandIndex + 1, second);
            first.Add(_shoe.Draw());
            second.Add(_shoe.Draw());

            // Split aces get one card each and no more.
            if (first.IsSplitAces)
            {
                second.Done = true;
                FinishActiveHand();
            }
            else if (first.Total == 21)
            {
                FinishActiveHand();
            }
        }

        void FinishActiveHand()
        {
            _hands[ActiveHandIndex].Done = true;
            while (ActiveHandIndex < _hands.Count && _hands[ActiveHandIndex].Done) ActiveHandIndex++;

            if (ActiveHandIndex >= _hands.Count)
            {
                ActiveHandIndex = _hands.Count - 1;
                PlayDealer();
                Settle();
            }
            else if (_hands[ActiveHandIndex].Total == 21)
            {
                FinishActiveHand();
            }
        }

        void PlayDealer()
        {
            bool anyLive = false;
            foreach (var hand in _hands) anyLive |= !hand.IsBust;
            if (!anyLive) return;

            while (Dealer.Total < 17 || (Dealer.Total == 17 && Dealer.IsSoft && _rules.DealerHitsSoft17))
                Dealer.Add(_shoe.Draw());
        }

        void Settle()
        {
            InsuranceNet = Insurance == 0 ? 0 : Dealer.IsBlackjack ? 2 * Insurance : -Insurance;
            foreach (var hand in _hands)
            {
                hand.Outcome = Resolve(hand);
                hand.Net = hand.Outcome switch
                {
                    HandOutcome.Blackjack => _rules.BlackjackWin(hand.Bet),
                    HandOutcome.Win => hand.Bet,
                    HandOutcome.Push => 0,
                    _ => -hand.Bet,
                };
            }
            Phase = RoundPhase.Settled;
        }

        HandOutcome Resolve(PlayerHand hand)
        {
            if (hand.IsBust) return HandOutcome.Bust;
            if (Dealer.IsBlackjack) return hand.IsBlackjack ? HandOutcome.Push : HandOutcome.Lose;
            if (hand.IsBlackjack) return HandOutcome.Blackjack;
            if (Dealer.IsBust) return HandOutcome.Win;
            if (hand.Total > Dealer.Total) return HandOutcome.Win;
            return hand.Total == Dealer.Total ? HandOutcome.Push : HandOutcome.Lose;
        }

        static void Require(bool allowed, string action)
        {
            if (!allowed) throw new InvalidOperationException($"{action} isn't allowed right now.");
        }
    }
}
