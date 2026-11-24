using System;
using System.Linq;
using NUnit.Framework;
using Starflare.Bowling;

namespace Starflare.Tests
{
    /// <summary>What a regular says over a challenge (bowling spec, "A challenge"), held to the writing rules.</summary>
    public class ChallengeTalkTests
    {
        [Test]
        public void TheAsk_NamesTheStakeInWords()
        {
            Assert.That(ChallengeTalk.Ask(10, 0), Is.EqualTo("Game for ten?"));
            Assert.That(ChallengeTalk.Ask(5, 1), Is.EqualTo("Bowl one with me? I'll go five."));
            Assert.That(ChallengeTalk.Ask(20, 2), Does.Contain("twenty"));
        }

        [Test]
        public void ThreeStraightLosses_SendYouToAnotherRegular()
        {
            Assert.That(ChallengeTalk.Settled(Outcome.Won, true, "Chuy", 0), Is.EqualTo("I'm out. Go bowl Chuy."));
            Assert.That(ChallengeTalk.Settled(Outcome.Won, true, null, 0), Is.EqualTo("I'm out for the night."));
        }

        [Test]
        public void ThePick_WrapsAndReadsTheSameEachTime()
        {
            for (int pick = -4; pick < 12; pick++)
            {
                Assert.That(ChallengeTalk.Taken(pick), Is.EqualTo(ChallengeTalk.Taken(pick)).And.Not.Empty);
                Assert.That(ChallengeTalk.WavedOff(pick), Is.Not.Empty);
                foreach (var result in new[] { Outcome.Won, Outcome.Lost, Outcome.Push })
                    Assert.That(ChallengeTalk.Settled(result, false, "Chuy", pick), Is.Not.Empty);
            }
        }

        [Test]
        public void EveryLine_KeepsTheWritingRules()
        {
            foreach (var line in ChallengeTalk.All())
            {
                int words = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length;
                Assert.That(words, Is.LessThanOrEqualTo(15), line);
                Assert.That(line.ToLowerInvariant(), Does.Not.Contain("vig").And.Not.Contain("dry"), line);
            }
            Assert.That(ChallengeTalk.All().Distinct().Count(), Is.GreaterThan(10));
        }
    }
}
