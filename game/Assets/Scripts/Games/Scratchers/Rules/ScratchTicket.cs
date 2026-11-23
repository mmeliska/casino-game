using System;
using System.Collections.Generic;
using System.Linq;

namespace Starflare.Scratchers
{
    /// <summary>
    /// One kind of scratcher on the roll: its price, how its play area reads, and its prize pool.
    /// Every ticket on a roll is printed before it's sold, so a ticket's odds are only its pool:
    /// so many winners of each amount per million tickets, the rest losers. Scratching shows a
    /// ticket; it never decides one. Design data, not state.
    /// </summary>
    public sealed class ScratchTicket
    {
        public const int Million = 1_000_000;

        readonly ScratchPrize[] _prizes;
        string[] _decoys;

        ScratchTicket(string id, string name, int price, ScratchPlay play, int spots, ScratchPrize[] prizes)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentException("A ticket needs an id.", nameof(id));
            if (price < 1) throw new ArgumentOutOfRangeException(nameof(price));
            if (spots < 1) throw new ArgumentOutOfRangeException(nameof(spots));
            if (prizes == null || prizes.Length == 0) throw new ArgumentException("A ticket needs a prize pool.", nameof(prizes));
            if (prizes.Select(p => p.Amount).Distinct().Count() != prizes.Length)
                throw new ArgumentException("Each amount is one line of the pool.", nameof(prizes));
            if (prizes.Sum(p => (long)p.PerMillion) >= Million)
                throw new ArgumentException("A pool can't have more winners than tickets.", nameof(prizes));

            Id = id;
            Name = name ?? "";
            Price = price;
            Play = play;
            Spots = spots;
            _prizes = prizes.OrderBy(p => p.Amount).ToArray();
            _decoys = new string[0];
        }

        /// <param name="spots">Amounts under the coating. Every amount but a win's shows at most twice, so the pool needs half as many amounts as spots, and one more.</param>
        public static ScratchTicket MatchThree(string id, string name, int price, int spots, params ScratchPrize[] prizes)
        {
            var ticket = new ScratchTicket(id, name, price, ScratchPlay.MatchThree, spots, prizes);
            if (spots < 3) throw new ArgumentOutOfRangeException(nameof(spots), "Three of a kind needs three spots.");
            if (prizes.Length * 2 < spots || (prizes.Length - 1) * 2 < spots - 3)
                throw new ArgumentException("Too few amounts to fill a losing ticket without three alike.", nameof(prizes));
            ticket.YourLabel = "";
            ticket.HowToPlay = "Match 3 like amounts, win that amount.";
            return ticket;
        }

        /// <param name="winning">How many winning numbers there are.</param>
        /// <param name="spots">How many of your numbers, each over a prize.</param>
        /// <param name="highest">Numbers run from 1 to this.</param>
        public static ScratchTicket LuckyNumbers(string id, string name, int price, int winning, int spots, int highest, params ScratchPrize[] prizes)
        {
            var ticket = new ScratchTicket(id, name, price, ScratchPlay.LuckyNumbers, spots, prizes);
            if (winning < 1) throw new ArgumentOutOfRangeException(nameof(winning));
            if (highest < winning + spots) throw new ArgumentOutOfRangeException(nameof(highest), "Not enough numbers for a losing ticket.");
            ticket.KeyCount = winning;
            ticket.Highest = highest;
            ticket.KeyLabel = "WINNING NUMBERS";
            ticket.YourLabel = "YOUR NUMBERS";
            ticket.HowToPlay = "Match any of your numbers to a winning number, win the prize below it.";
            return ticket;
        }

        /// <param name="symbol">The symbol that wins, as printed (a "7").</param>
        /// <param name="decoys">What the other spots show.</param>
        public static ScratchTicket FindSymbol(string id, string name, int price, int spots, string symbol, string[] decoys, params ScratchPrize[] prizes)
        {
            var ticket = new ScratchTicket(id, name, price, ScratchPlay.FindSymbol, spots, prizes);
            if (string.IsNullOrEmpty(symbol)) throw new ArgumentException("A ticket needs a symbol to find.", nameof(symbol));
            if (decoys == null || decoys.Length == 0 || decoys.Contains(symbol))
                throw new ArgumentException("The other spots need symbols that aren't the one to find.", nameof(decoys));
            ticket.Symbol = symbol;
            ticket._decoys = decoys.ToArray();
            ticket.YourLabel = "";
            ticket.HowToPlay = $"Reveal a {symbol}, win the prize below it.";
            return ticket;
        }

        /// <param name="spots">How many of your numbers, each over a prize.</param>
        /// <param name="highest">Numbers run from 1 to this.</param>
        public static ScratchTicket BeatTheirs(string id, string name, int price, int spots, int highest, params ScratchPrize[] prizes)
        {
            var ticket = new ScratchTicket(id, name, price, ScratchPlay.BeatTheirs, spots, prizes);
            if (highest < 6) throw new ArgumentOutOfRangeException(nameof(highest), "Too few numbers to beat.");
            ticket.KeyCount = 1;
            ticket.Highest = highest;
            ticket.KeyLabel = "THEIR NUMBER";
            ticket.YourLabel = "YOUR NUMBERS";
            ticket.HowToPlay = "Beat their number with any of yours, win the prize below it.";
            return ticket;
        }

        /// <summary>For art tags: a ticket's picture is <c>Scratchers/{id}</c>.</summary>
        public string Id { get; }

        /// <summary>What the ticket says across the top.</summary>
        public string Name { get; }
        public int Price { get; }
        public ScratchPlay Play { get; }

        /// <summary>Your spots: the amounts, numbers or symbols you scratch for.</summary>
        public int Spots { get; }

        /// <summary>The numbers above yours to scratch first: the winning numbers, or their number.</summary>
        public int KeyCount { get; private set; }

        /// <summary>The highest number printed, where a ticket has numbers.</summary>
        public int Highest { get; private set; }

        /// <summary>The symbol to find, where the ticket has one.</summary>
        public string Symbol { get; private set; } = "";

        public IReadOnlyList<string> Decoys => _decoys;

        /// <summary>The prize pool, smallest amount first.</summary>
        public IReadOnlyList<ScratchPrize> Prizes => _prizes;

        // What's printed on the face. Each play sets its own; a ticket can reword them.
        public string KeyLabel { get; set; } = "";
        public string YourLabel { get; set; } = "";
        public string HowToPlay { get; set; } = "";

        public int TopPrize => _prizes[_prizes.Length - 1].Amount;

        /// <summary>The share of tickets that win anything.</summary>
        public double HitRate => _prizes.Sum(p => (double)p.PerMillion) / Million;

        /// <summary>The overall odds the back prints: 1 in this many tickets wins something.</summary>
        public double OddsOneIn => 1 / HitRate;

        /// <summary>What comes back per dollar spent, over the whole roll.</summary>
        public double ReturnToPlayer => _prizes.Sum(p => (double)p.Amount * p.PerMillion) / ((double)Price * Million);

        /// <summary>A ticket off the roll: a prize from the pool, or nothing, each as likely as its share of the roll.</summary>
        public int DrawPrize(Random random)
        {
            if (random == null) throw new ArgumentNullException(nameof(random));
            int ticket = random.Next(Million);
            foreach (var prize in _prizes)
            {
                if (ticket < prize.PerMillion) return prize.Amount;
                ticket -= prize.PerMillion;
            }
            return 0;
        }

        static ScratchPrize P(int amount, int perMillion) => new ScratchPrize(amount, perMillion);

        /// <summary>
        /// Bridgehead's roll behind the counter (the Bridges; Mike, 2026-11-23): ten tickets from
        /// $1 to $5. The worst odds in town, the way a lottery's are: the $1 tickets give back
        /// 57-60 cents on the dollar, the $5 ones about 68, and about one ticket in four wins
        /// something, mostly its price back.
        /// </summary>
        public static IReadOnlyList<ScratchTicket> Bridgehead() => new[]
        {
            FindSymbol("lucky7s", "LUCKY 7s", 1, 6, "7", new[] { "CHERRY", "BELL", "BAR", "HORSESHOE", "CLOVER", "LEMON" },
                P(1, 100000), P(2, 60000), P(4, 26000), P(5, 20000), P(10, 9000), P(20, 2000), P(50, 400), P(100, 100), P(500, 10)),
            MatchThree("pennyjar", "PENNY JAR", 1, 6,
                P(1, 120000), P(2, 80000), P(3, 40000), P(5, 20000), P(10, 6000), P(25, 1000), P(100, 100), P(250, 10)),
            Retitled(BeatTheirs("icefishing", "ICE FISHING", 1, 4, 40,
                P(1, 90000), P(2, 50000), P(5, 30000), P(10, 10000), P(25, 3000), P(50, 800), P(200, 60), P(1000, 3)),
                "THEIR CATCH", "YOUR CATCH", "Any of your fish outweighs theirs, win the prize below it."),
            LuckyNumbers("lakeeffect", "LAKE EFFECT CASH", 2, 2, 8, 30,
                P(2, 110000), P(4, 64000), P(5, 40000), P(10, 22000), P(20, 8000), P(50, 2400), P(100, 500), P(2000, 4)),
            MatchThree("northernlights", "NORTHERN LIGHTS", 2, 9,
                P(2, 115000), P(3, 60000), P(5, 45000), P(10, 25000), P(25, 6000), P(50, 2000), P(100, 500), P(1000, 10)),
            Retitled(BeatTheirs("photofinish", "PHOTO FINISH", 2, 6, 30,
                P(2, 100000), P(4, 66000), P(6, 40000), P(10, 22000), P(25, 6000), P(50, 2000), P(250, 120), P(2500, 4)),
                "THE FAVORITE", "YOUR HORSES", "Any of your horses beats the favorite, win the prize below it."),
            Reworded(FindSymbol("goldrush", "GOLD RUSH", 3, 9, "GOLD", new[] { "ROCK", "COAL", "TIN", "SAND", "IRON", "CLAY" },
                P(3, 110000), P(5, 66000), P(10, 46000), P(15, 18000), P(30, 9000), P(75, 2200), P(300, 220), P(5000, 4)),
                "Strike GOLD, win the prize below it."),
            LuckyNumbers("oreboat", "ORE BOAT BUCKS", 3, 3, 10, 40,
                P(3, 105000), P(6, 64000), P(9, 38000), P(15, 22000), P(30, 10000), P(100, 1400), P(500, 100), P(3000, 8)),
            MatchThree("diamonddeluxe", "DIAMOND DELUXE", 5, 12,
                P(5, 120000), P(10, 74000), P(20, 34000), P(25, 20000), P(50, 9000), P(100, 3000), P(500, 220), P(10000, 5)),
            LuckyNumbers("cashblizzard", "CASH BLIZZARD", 5, 4, 15, 50,
                P(5, 110000), P(10, 76000), P(15, 38000), P(25, 22000), P(50, 10000), P(100, 3400), P(1000, 70), P(10000, 5)),
        };

        static ScratchTicket Reworded(ScratchTicket ticket, string howToPlay)
        {
            ticket.HowToPlay = howToPlay;
            return ticket;
        }

        static ScratchTicket Retitled(ScratchTicket ticket, string keyLabel, string yourLabel, string howToPlay)
        {
            ticket.KeyLabel = keyLabel;
            ticket.YourLabel = yourLabel;
            ticket.HowToPlay = howToPlay;
            return ticket;
        }
    }
}
