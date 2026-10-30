using System;
using System.Linq;
using NUnit.Framework;
using Starflare.Blackjack;
using Starflare.Cards;

namespace Starflare.Tests
{
    public class BlackjackRoundTests
    {
        // Deal order is player, dealer, player, dealer, then draws in order.
        static Shoe Stack(params string[] cards) => Shoe.Stacked(cards.Select(C).ToArray());

        internal static Card C(string code)
        {
            string rank = code.Substring(0, code.Length - 1);
            var r = rank switch
            {
                "A" => Rank.Ace,
                "K" => Rank.King,
                "Q" => Rank.Queen,
                "J" => Rank.Jack,
                _ => (Rank)int.Parse(rank),
            };
            var s = code[code.Length - 1] switch
            {
                'C' => Suit.Clubs,
                'D' => Suit.Diamonds,
                'H' => Suit.Hearts,
                _ => Suit.Spades,
            };
            return new Card(r, s);
        }

        static Hand HandOf(params string[] cards)
        {
            var hand = new Hand();
            foreach (var card in cards) hand.Add(C(card));
            return hand;
        }

        static BlackjackRound Deal(Shoe shoe, int bet = 10, int bankroll = 500, BlackjackRules rules = null) =>
            new BlackjackRound(rules ?? new BlackjackRules(), shoe, bet, bankroll);

        [Test]
        public void HandTotals_CountAcesSoftThenHard()
        {
            Assert.That(HandOf("AS", "6H").Total, Is.EqualTo(17));
            Assert.That(HandOf("AS", "6H").IsSoft, Is.True);
            Assert.That(HandOf("AS", "6H", "10C").Total, Is.EqualTo(17));
            Assert.That(HandOf("AS", "6H", "10C").IsSoft, Is.False);
            Assert.That(HandOf("AS", "AH", "9C").Total, Is.EqualTo(21));
            Assert.That(HandOf("KS", "QH", "2C").IsBust, Is.True);
        }

        [Test]
        public void PlayerBlackjack_PaysThreeToTwo_AndSettlesAtOnce()
        {
            var round = Deal(Stack("AS", "9H", "KD", "7C"));
            Assert.That(round.Phase, Is.EqualTo(RoundPhase.Settled));
            Assert.That(round.Hands[0].Outcome, Is.EqualTo(HandOutcome.Blackjack));
            Assert.That(round.Net, Is.EqualTo(15));
        }

        [Test]
        public void BlackjackOnFiveDollars_RoundsHalfDollarDown()
        {
            var round = Deal(Stack("AS", "9H", "KD", "7C"), bet: 5);
            Assert.That(round.Net, Is.EqualTo(7));
        }

        [Test]
        public void DealerBlackjack_BeatsTwenty()
        {
            var round = Deal(Stack("10S", "AH", "KD", "KC"));
            Assert.That(round.Phase, Is.EqualTo(RoundPhase.Settled));
            Assert.That(round.Hands[0].Outcome, Is.EqualTo(HandOutcome.Lose));
            Assert.That(round.Net, Is.EqualTo(-10));
        }

        [Test]
        public void BothBlackjack_Push()
        {
            var round = Deal(Stack("AS", "AH", "KD", "KC"));
            Assert.That(round.Hands[0].Outcome, Is.EqualTo(HandOutcome.Push));
            Assert.That(round.Net, Is.EqualTo(0));
        }

        [Test]
        public void DealerStandsOnSoft17()
        {
            var round = Deal(Stack("10S", "AH", "9D", "6C"));
            round.Stand();
            Assert.That(round.Dealer.Cards.Count, Is.EqualTo(2));
            Assert.That(round.Hands[0].Outcome, Is.EqualTo(HandOutcome.Win));
            Assert.That(round.Net, Is.EqualTo(10));
        }

        [Test]
        public void DealerHitsSoft17_WhenTheTableSaysSo()
        {
            var rules = new BlackjackRules { DealerHitsSoft17 = true };
            var round = Deal(Stack("10S", "AH", "9D", "6C", "2S"), rules: rules);
            round.Stand();
            Assert.That(round.Dealer.Total, Is.EqualTo(19));
            Assert.That(round.Hands[0].Outcome, Is.EqualTo(HandOutcome.Push));
        }

        [Test]
        public void Double_TakesOneCardAndDoublesTheBet()
        {
            var round = Deal(Stack("6S", "10H", "5D", "7C", "10S"));
            Assert.That(round.CanDouble, Is.True);
            round.Double();
            Assert.That(round.Phase, Is.EqualTo(RoundPhase.Settled));
            Assert.That(round.Hands[0].Cards.Count, Is.EqualTo(3));
            Assert.That(round.Hands[0].Bet, Is.EqualTo(20));
            Assert.That(round.Net, Is.EqualTo(20));
        }

        [Test]
        public void Double_NotAllowedWithoutTheMoney()
        {
            var round = Deal(Stack("6S", "10H", "5D", "7C"), bet: 10, bankroll: 15);
            Assert.That(round.CanDouble, Is.False);
            Assert.Throws<InvalidOperationException>(() => round.Double());
        }

        [Test]
        public void SplitAces_GetOneCardEach_AndTwentyOneIsNotBlackjack()
        {
            var round = Deal(Stack("AS", "10H", "AD", "7C", "KS", "9S"));
            Assert.That(round.CanSplit, Is.True);
            round.Split();

            Assert.That(round.Phase, Is.EqualTo(RoundPhase.Settled));
            Assert.That(round.Hands.Count, Is.EqualTo(2));
            Assert.That(round.Hands[0].IsBlackjack, Is.False);
            Assert.That(round.Hands[0].Outcome, Is.EqualTo(HandOutcome.Win));
            Assert.That(round.Hands[1].Total, Is.EqualTo(20));
            Assert.That(round.Net, Is.EqualTo(20));
        }

        [Test]
        public void Split_PlaysEachHandInTurn_AndOnlyOnce()
        {
            var round = Deal(Stack("8S", "10H", "8D", "7C", "3S", "10S", "10D"));
            round.Split();
            Assert.That(round.CanSplit, Is.False, "one split per round");
            Assert.That(round.ActiveHandIndex, Is.EqualTo(0));

            round.Hit(); // 8 3 10 = 21 finishes the first hand
            Assert.That(round.ActiveHandIndex, Is.EqualTo(1));
            round.Stand(); // 8 10 = 18 against the dealer's 17

            Assert.That(round.Phase, Is.EqualTo(RoundPhase.Settled));
            Assert.That(round.Hands.Select(h => h.Outcome), Is.EqualTo(new[] { HandOutcome.Win, HandOutcome.Win }));
            Assert.That(round.Net, Is.EqualTo(20));
        }

        [Test]
        public void PlayerBust_LosesAndDealerDoesNotDraw()
        {
            var round = Deal(Stack("10S", "10H", "6D", "6C", "10D"));
            round.Hit();
            Assert.That(round.Phase, Is.EqualTo(RoundPhase.Settled));
            Assert.That(round.Hands[0].Outcome, Is.EqualTo(HandOutcome.Bust));
            Assert.That(round.Dealer.Cards.Count, Is.EqualTo(2));
            Assert.That(round.Net, Is.EqualTo(-10));
        }

        [Test]
        public void BetOutsideTableLimits_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Deal(Stack("10S", "10H", "6D", "6C"), bet: 2));
            Assert.Throws<ArgumentOutOfRangeException>(() => Deal(Stack("10S", "10H", "6D", "6C"), bet: 500));
        }

        [Test]
        public void SixDeckShoe_HasEveryCardSixTimes_AndACutCard()
        {
            var shoe = new Shoe(6, 0.75, new Random(1));
            Assert.That(shoe.Count, Is.EqualTo(312));

            var drawn = Enumerable.Range(0, 312).Select(_ => shoe.Draw()).ToList();
            Assert.That(drawn.GroupBy(c => c).All(g => g.Count() == 6), Is.True);
            Assert.That(drawn.Distinct().Count(), Is.EqualTo(52));
            Assert.That(shoe.PastCutCard, Is.True);

            shoe.Shuffle();
            for (int i = 0; i < 233; i++) shoe.Draw();
            Assert.That(shoe.PastCutCard, Is.False);
            shoe.Draw();
            Assert.That(shoe.PastCutCard, Is.True);
        }
    }
}
