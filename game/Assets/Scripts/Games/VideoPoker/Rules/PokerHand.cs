using System;
using System.Collections.Generic;
using System.Linq;
using Starflare.Cards;

namespace Starflare.VideoPoker
{
    /// <summary>Jacks or Better hands, lowest to highest. Anything under a pair of jacks is Nothing.</summary>
    public enum PokerHandRank
    {
        Nothing,
        JacksOrBetter,
        TwoPair,
        ThreeOfAKind,
        Straight,
        Flush,
        FullHouse,
        FourOfAKind,
        StraightFlush,
        RoyalFlush,
    }

    public static class PokerHand
    {
        public static PokerHandRank Evaluate(IReadOnlyList<Card> cards)
        {
            if (cards == null || cards.Count != 5) throw new ArgumentException("A poker hand is five cards.", nameof(cards));

            bool flush = cards.All(c => c.Suit == cards[0].Suit);
            var ranks = cards.Select(c => (int)c.Rank).OrderBy(r => r).ToArray();
            bool distinct = ranks.Distinct().Count() == 5;
            bool wheel = ranks.SequenceEqual(new[] { 2, 3, 4, 5, (int)Rank.Ace });
            bool straight = distinct && (ranks[4] - ranks[0] == 4 || wheel);

            if (straight && flush)
                return ranks[0] == (int)Rank.Ten ? PokerHandRank.RoyalFlush : PokerHandRank.StraightFlush;

            var groups = cards.GroupBy(c => c.Rank).Select(g => (Rank: g.Key, Count: g.Count()))
                .OrderByDescending(g => g.Count).ToArray();

            if (groups[0].Count == 4) return PokerHandRank.FourOfAKind;
            if (groups[0].Count == 3 && groups[1].Count == 2) return PokerHandRank.FullHouse;
            if (flush) return PokerHandRank.Flush;
            if (straight) return PokerHandRank.Straight;
            if (groups[0].Count == 3) return PokerHandRank.ThreeOfAKind;
            if (groups[0].Count == 2 && groups[1].Count == 2) return PokerHandRank.TwoPair;
            if (groups[0].Count == 2 && groups[0].Rank >= Rank.Jack) return PokerHandRank.JacksOrBetter;
            return PokerHandRank.Nothing;
        }

        public static string Label(PokerHandRank rank) => rank switch
        {
            PokerHandRank.RoyalFlush => "Royal Flush",
            PokerHandRank.StraightFlush => "Straight Flush",
            PokerHandRank.FourOfAKind => "Four of a Kind",
            PokerHandRank.FullHouse => "Full House",
            PokerHandRank.Flush => "Flush",
            PokerHandRank.Straight => "Straight",
            PokerHandRank.ThreeOfAKind => "Three of a Kind",
            PokerHandRank.TwoPair => "Two Pair",
            PokerHandRank.JacksOrBetter => "Jacks or Better",
            _ => "",
        };
    }
}
