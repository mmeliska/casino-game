using System;
using System.Collections.Generic;
using System.Linq;

namespace Starflare.Scratchers
{
    /// <summary>
    /// A ticket you've bought, settled the moment Mitch tears it off: its prize comes off the roll
    /// first, then its face is printed to show that prize and nothing more. Every losing face is
    /// a real loser (no three alike, no matching number, no symbol, nothing beating theirs) and
    /// every winning face wins exactly once. Scratching only shows it; walking off can't change it.
    /// </summary>
    public sealed class ScratchCard
    {
        readonly ScratchSpot[] _spots;
        readonly int[] _key;

        ScratchCard(ScratchTicket ticket, int prize, int[] key, ScratchSpot[] spots)
        {
            Ticket = ticket;
            Prize = prize;
            _key = key;
            _spots = spots;
        }

        public ScratchTicket Ticket { get; }

        /// <summary>What the ticket pays, 0 for a loser.</summary>
        public int Prize { get; }

        public int Net => Prize - Ticket.Price;
        public bool IsWinner => Prize > 0;

        /// <summary>The winning numbers, or their number; empty on tickets without.</summary>
        public IReadOnlyList<int> Key => _key;

        public IReadOnlyList<ScratchSpot> Spots => _spots;

        /// <summary>Buys a ticket off the roll with this much cash on you.</summary>
        public static ScratchCard Buy(ScratchTicket ticket, int bankroll, Random random)
        {
            if (ticket == null) throw new ArgumentNullException(nameof(ticket));
            if (random == null) throw new ArgumentNullException(nameof(random));
            if (ticket.Price > bankroll) throw new InvalidOperationException("Not enough cash for that ticket.");
            return Print(ticket, ticket.DrawPrize(random), random);
        }

        /// <summary>Prints a ticket's face for a prize already drawn: 0, or one of the pool's amounts.</summary>
        public static ScratchCard Print(ScratchTicket ticket, int prize, Random random)
        {
            if (ticket == null) throw new ArgumentNullException(nameof(ticket));
            if (random == null) throw new ArgumentNullException(nameof(random));
            if (prize != 0 && ticket.Prizes.All(p => p.Amount != prize))
                throw new ArgumentException("A ticket only pays what's in its pool.", nameof(prize));

            switch (ticket.Play)
            {
                case ScratchPlay.MatchThree: return MatchThree(ticket, prize, random);
                case ScratchPlay.LuckyNumbers: return LuckyNumbers(ticket, prize, random);
                case ScratchPlay.FindSymbol: return FindSymbol(ticket, prize, random);
                default: return BeatTheirs(ticket, prize, random);
            }
        }

        /// <summary>
        /// The win's amount three times; every other spot an amount that isn't the win's, none
        /// more than twice. Taking spots from a pile of two of each amount keeps that true.
        /// </summary>
        static ScratchCard MatchThree(ScratchTicket ticket, int prize, Random random)
        {
            int others = prize > 0 ? ticket.Spots - 3 : ticket.Spots;
            var pile = ticket.Prizes.Select(p => p.Amount).Where(a => a != prize).SelectMany(a => new[] { a, a }).ToList();
            Shuffle(pile, random);
            var spots = pile.Take(others).Select(a => new ScratchSpot("", 0, a, false)).ToList();
            for (int i = 0; prize > 0 && i < 3; i++) spots.Add(new ScratchSpot("", 0, prize, true));
            Shuffle(spots, random);
            return new ScratchCard(ticket, prize, new int[0], spots.ToArray());
        }

        /// <summary>Winning numbers, and none of yours among them but the one over the win.</summary>
        static ScratchCard LuckyNumbers(ScratchTicket ticket, int prize, Random random)
        {
            var numbers = Enumerable.Range(1, ticket.Highest).ToList();
            Shuffle(numbers, random);
            var key = numbers.Take(ticket.KeyCount).ToArray();
            var yours = numbers.Skip(ticket.KeyCount).Take(ticket.Spots).ToArray();
            var spots = yours.Select(n => new ScratchSpot(n.ToString(), n, Decoy(ticket, random), false)).ToArray();
            if (prize > 0)
            {
                int n = key[random.Next(key.Length)];
                spots[random.Next(spots.Length)] = new ScratchSpot(n.ToString(), n, prize, true);
            }
            Array.Sort(key);
            return new ScratchCard(ticket, prize, key, spots);
        }

        /// <summary>The other symbols everywhere, and the ticket's own over the win.</summary>
        static ScratchCard FindSymbol(ScratchTicket ticket, int prize, Random random)
        {
            var spots = new ScratchSpot[ticket.Spots];
            for (int i = 0; i < spots.Length; i++)
                spots[i] = new ScratchSpot(ticket.Decoys[random.Next(ticket.Decoys.Count)], 0, Decoy(ticket, random), false);
            if (prize > 0) spots[random.Next(spots.Length)] = new ScratchSpot(ticket.Symbol, 0, prize, true);
            return new ScratchCard(ticket, prize, new int[0], spots);
        }

        /// <summary>
        /// Their number, with room above it for a win and below it for the losers: yours all
        /// under it, but the one over the win, which beats it. A tie never shows, so none needs a rule.
        /// </summary>
        static ScratchCard BeatTheirs(ScratchTicket ticket, int prize, Random random)
        {
            int theirs = random.Next(ticket.Highest / 3 + 1, ticket.Highest);
            var spots = new ScratchSpot[ticket.Spots];
            for (int i = 0; i < spots.Length; i++)
            {
                int n = random.Next(1, theirs);
                spots[i] = new ScratchSpot(n.ToString(), n, Decoy(ticket, random), false);
            }
            if (prize > 0)
            {
                int n = random.Next(theirs + 1, ticket.Highest + 1);
                spots[random.Next(spots.Length)] = new ScratchSpot(n.ToString(), n, prize, true);
            }
            return new ScratchCard(ticket, prize, new[] { theirs }, spots);
        }

        /// <summary>
        /// A prize printed under a spot that doesn't win: any amount in the pool, the small ones
        /// most often and now and then the top one, as on a real ticket.
        /// </summary>
        static int Decoy(ScratchTicket ticket, Random random)
        {
            int count = ticket.Prizes.Count;
            double total = 0;
            for (int i = 0; i < count; i++) total += 1.0 / (i + 1);
            double pick = random.NextDouble() * total;
            for (int i = 0; i < count; i++)
            {
                pick -= 1.0 / (i + 1);
                if (pick < 0) return ticket.Prizes[i].Amount;
            }
            return ticket.Prizes[0].Amount;
        }

        static void Shuffle<T>(IList<T> items, Random random)
        {
            for (int i = items.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (items[i], items[j]) = (items[j], items[i]);
            }
        }
    }
}
