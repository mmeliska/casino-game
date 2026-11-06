using System;
using NUnit.Framework;
using Starflare.Cards;

namespace Starflare.Tests
{
    public class CardTests
    {
        [TestCase("AS", Rank.Ace, Suit.Spades)]
        [TestCase("10H", Rank.Ten, Suit.Hearts)]
        [TestCase("2C", Rank.Two, Suit.Clubs)]
        [TestCase("QD", Rank.Queen, Suit.Diamonds)]
        public void ParsesItsShortCode(string code, Rank rank, Suit suit) =>
            Assert.That(Card.Parse(code), Is.EqualTo(new Card(rank, suit)));

        [TestCase("")]
        [TestCase("A")]
        [TestCase("1S")]
        [TestCase("11S")]
        [TestCase("AX")]
        public void RefusesAnythingElse(string code) =>
            Assert.Throws<FormatException>(() => Card.Parse(code));

        [Test]
        public void EveryCardRoundTripsThroughItsRankLabel()
        {
            foreach (Rank rank in Enum.GetValues(typeof(Rank)))
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                var card = new Card(rank, suit);
                Assert.That(Card.Parse(card.RankLabel + suit.ToString()[0]), Is.EqualTo(card));
            }
        }
    }
}
