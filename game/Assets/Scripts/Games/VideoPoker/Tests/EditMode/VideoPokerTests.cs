using System;
using System.Linq;
using NUnit.Framework;
using Starflare.Cards;
using Starflare.VideoPoker;

namespace Starflare.Tests
{
    public class VideoPokerTests
    {
        static Card[] Cards(string hand) => hand.Split(' ').Select(Card.Parse).ToArray();
        static Shoe Stack(string order) => Shoe.Stacked(Cards(order));

        [TestCase("AS KS QS JS 10S", PokerHandRank.RoyalFlush)]
        [TestCase("9H 8H 7H 6H 5H", PokerHandRank.StraightFlush)]
        [TestCase("AD 2D 3D 4D 5D", PokerHandRank.StraightFlush)]
        [TestCase("7C 7D 7H 7S 2C", PokerHandRank.FourOfAKind)]
        [TestCase("3C 3D 3H 9S 9C", PokerHandRank.FullHouse)]
        [TestCase("2H 9H JH 4H KH", PokerHandRank.Flush)]
        [TestCase("10C JD QH KS AC", PokerHandRank.Straight)]
        [TestCase("AC 2D 3H 4S 5C", PokerHandRank.Straight)]
        [TestCase("QC KD AH 2S 3C", PokerHandRank.Nothing)]
        [TestCase("5C 5D 5H KS 2C", PokerHandRank.ThreeOfAKind)]
        [TestCase("5C 5D KH KS 2C", PokerHandRank.TwoPair)]
        [TestCase("JC JD 4H 8S 2C", PokerHandRank.JacksOrBetter)]
        [TestCase("AC AD 4H 8S 2C", PokerHandRank.JacksOrBetter)]
        [TestCase("10C 10D 4H 8S 2C", PokerHandRank.Nothing)]
        [TestCase("2C 7D 9H JS KC", PokerHandRank.Nothing)]
        public void Evaluate_RanksTheHand(string hand, PokerHandRank expected) =>
            Assert.That(PokerHand.Evaluate(Cards(hand)), Is.EqualTo(expected));

        [Test]
        public void Rules_Pay96ForOne_WithTheMaxBetRoyalBonus()
        {
            var rules = new VideoPokerRules();
            Assert.That(rules.PayoutCredits(PokerHandRank.FullHouse, 1), Is.EqualTo(9));
            Assert.That(rules.PayoutCredits(PokerHandRank.Flush, 5), Is.EqualTo(30));
            Assert.That(rules.PayoutCredits(PokerHandRank.JacksOrBetter, 3), Is.EqualTo(3));
            Assert.That(rules.PayoutCredits(PokerHandRank.Nothing, 5), Is.EqualTo(0));
            Assert.That(rules.PayoutCredits(PokerHandRank.RoyalFlush, 4), Is.EqualTo(1000));
            Assert.That(rules.PayoutCredits(PokerHandRank.RoyalFlush, 5), Is.EqualTo(4000));
        }

        [Test]
        public void Round_HeldCardsStay_OthersComeFromTheSameDeck()
        {
            var round = new VideoPokerRound(new VideoPokerRules(), Stack("JC JD 4H 8S 2C 9D JS 3H"), 5, 200);
            Assert.That(round.Rank, Is.EqualTo(PokerHandRank.JacksOrBetter));

            round.ToggleHold(0);
            round.ToggleHold(1);
            round.ToggleHold(3);
            round.ToggleHold(3);
            round.Draw();

            Assert.That(round.Cards.Select(c => c.ToString()), Is.EqualTo(Cards("JC JD 9D JS 3H").Select(c => c.ToString())));
            Assert.That(round.Rank, Is.EqualTo(PokerHandRank.ThreeOfAKind));
            Assert.That(round.Payout, Is.EqualTo(15));
            Assert.That(round.Net, Is.EqualTo(10));
        }

        [Test]
        public void Round_JacksGiveTheBetBack()
        {
            var round = new VideoPokerRound(new VideoPokerRules(), Stack("QC QD 4H 8S 2C"), 2, 200);
            for (int i = 0; i < 5; i++) round.ToggleHold(i);
            round.Draw();
            Assert.That(round.Net, Is.EqualTo(0));
        }

        [Test]
        public void Round_LosingHandCostsTheBet_AndNothingPaysBeforeTheDraw()
        {
            var round = new VideoPokerRound(new VideoPokerRules(), Stack("2C 7D 9H JS KC 3D 4S 5H 6C 8D"), 3, 200);
            Assert.That(round.Net, Is.EqualTo(0));
            round.Draw();
            Assert.That(round.Phase, Is.EqualTo(VideoPokerPhase.Settled));
            Assert.That(round.Net, Is.EqualTo(-3));
            Assert.Throws<InvalidOperationException>(() => round.ToggleHold(0));
        }

        [Test]
        public void Round_CreditValueScalesTheBet()
        {
            var rules = new VideoPokerRules { CreditValue = 5 };
            var round = new VideoPokerRound(rules, Stack("3C 3D 3H 9S 9C"), 5, 200);
            for (int i = 0; i < 5; i++) round.ToggleHold(i);
            round.Draw();
            Assert.That(round.Bet, Is.EqualTo(25));
            Assert.That(round.Net, Is.EqualTo(9 * 25 - 25));
        }

        [Test]
        public void Round_RefusesBetsOutsideTheMachineOrTheBankroll()
        {
            var rules = new VideoPokerRules();
            Assert.Throws<ArgumentOutOfRangeException>(() => new VideoPokerRound(rules, Stack("2C 7D 9H JS KC"), 6, 200));
            Assert.Throws<ArgumentOutOfRangeException>(() => new VideoPokerRound(rules, Stack("2C 7D 9H JS KC"), 0, 200));
            Assert.Throws<InvalidOperationException>(() => new VideoPokerRound(rules, Stack("2C 7D 9H JS KC"), 5, 4));
        }
    }
}
