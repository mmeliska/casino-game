using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Starflare.Scratchers;

namespace Starflare.Tests
{
    public class ScratcherTests
    {
        static ScratchTicket Ticket(string id) => ScratchTicket.Bridgehead().Single(t => t.Id == id);

        /// <summary>
        /// What a player reads off a scratched ticket, from its face alone: three alike, a
        /// matching number, the symbol, or a number over theirs. Returns the prize it pays.
        /// </summary>
        static int Read(ScratchCard card)
        {
            var ticket = card.Ticket;
            var spots = card.Spots;
            switch (ticket.Play)
            {
                case ScratchPlay.MatchThree:
                    var triples = spots.GroupBy(s => s.Prize).Where(g => g.Count() >= 3).ToList();
                    Assert.That(triples.Count, Is.LessThanOrEqualTo(1), "never two amounts three times");
                    Assert.That(triples.All(g => g.Count() == 3), "a win shows its amount three times, not four");
                    return triples.Count == 0 ? 0 : triples[0].Key;
                case ScratchPlay.LuckyNumbers:
                    return spots.Where(s => card.Key.Contains(s.Number)).Sum(s => s.Prize);
                case ScratchPlay.FindSymbol:
                    return spots.Where(s => s.Symbol == ticket.Symbol).Sum(s => s.Prize);
                default:
                    Assert.That(spots.All(s => s.Number != card.Key[0]), "never a tie with theirs");
                    return spots.Where(s => s.Number > card.Key[0]).Sum(s => s.Prize);
            }
        }

        [Test]
        public void BridgeheadsRoll_IsTenTickets_FromOneDollarToFive()
        {
            var roll = ScratchTicket.Bridgehead();
            Assert.That(roll.Count, Is.EqualTo(10));
            Assert.That(roll.Select(t => t.Id).Distinct().Count(), Is.EqualTo(10));
            Assert.That(roll.Min(t => t.Price), Is.EqualTo(1));
            Assert.That(roll.Max(t => t.Price), Is.EqualTo(5));
            Assert.That(Enum.GetValues(typeof(ScratchPlay)).Cast<ScratchPlay>().All(p => roll.Any(t => t.Play == p)), "every kind of play is on the roll");
        }

        [TestCase("lucky7s", 0.589, 4.60)]
        [TestCase("pennyjar", 0.5975, 3.74)]
        [TestCase("icefishing", 0.570, 5.44)]
        [TestCase("lakeeffect", 0.617, 4.05)]
        [TestCase("northernlights", 0.5975, 3.94)]
        [TestCase("photofinish", 0.607, 4.24)]
        [TestCase("goldrush", 0.637, 3.98)]
        [TestCase("oreboat", 0.6283, 4.16)]
        [TestCase("diamonddeluxe", 0.686, 3.84)]
        [TestCase("cashblizzard", 0.678, 3.85)]
        public void EachTicket_PaysBackWhatItsPoolSays(string id, double returnToPlayer, double oddsOneIn)
        {
            var ticket = Ticket(id);
            Assert.That(ticket.ReturnToPlayer, Is.EqualTo(returnToPlayer).Within(0.0005));
            Assert.That(ticket.OddsOneIn, Is.EqualTo(oddsOneIn).Within(0.005));
        }

        [Test]
        public void TheRoll_IsTheWorstOddsInTown_AndDearerTicketsGiveMoreBack()
        {
            var roll = ScratchTicket.Bridgehead();
            Assert.That(roll.Max(t => t.ReturnToPlayer), Is.LessThan(0.86), "under Bridgehead's own Triple Bar");
            Assert.That(roll.Min(t => t.ReturnToPlayer), Is.GreaterThan(0.55));
            Assert.That(roll.Where(t => t.Price == 1).Max(t => t.ReturnToPlayer),
                Is.LessThan(roll.Where(t => t.Price == 5).Min(t => t.ReturnToPlayer)));
            foreach (var ticket in roll)
                Assert.That(ticket.Prizes[0].Amount, Is.EqualTo(ticket.Price), $"{ticket.Name}'s smallest prize is its price back");
        }

        [Test]
        public void EveryFace_PaysExactlyThePrizeDrawn_ForEveryTicketAndEveryPrize()
        {
            var random = new Random(11);
            foreach (var ticket in ScratchTicket.Bridgehead())
            foreach (int prize in ticket.Prizes.Select(p => p.Amount).Prepend(0))
            for (int i = 0; i < 60; i++)
            {
                var card = ScratchCard.Print(ticket, prize, random);
                Assert.That(card.Spots.Count, Is.EqualTo(ticket.Spots));
                Assert.That(card.Key.Count, Is.EqualTo(ticket.KeyCount));
                Assert.That(Read(card), Is.EqualTo(prize), $"{ticket.Name} printed for {prize}");
                Assert.That(card.Spots.Count(s => s.Wins), Is.EqualTo(prize == 0 ? 0 : ticket.Play == ScratchPlay.MatchThree ? 3 : 1));
                Assert.That(card.Spots.All(s => ticket.Prizes.Any(p => p.Amount == s.Prize)), "every printed amount is one the ticket pays");
                if (ticket.Highest > 0)
                    Assert.That(card.Spots.All(s => s.Number >= 1 && s.Number <= ticket.Highest) && card.Key.All(n => n >= 1 && n <= ticket.Highest));
            }
        }

        [Test]
        public void LuckyNumbers_YourNumbersNeverRepeat_AndTheWinningNumbersAreInOrder()
        {
            var ticket = Ticket("cashblizzard");
            var random = new Random(5);
            for (int i = 0; i < 200; i++)
            {
                var card = ScratchCard.Print(ticket, i % 2 == 0 ? 0 : 25, random);
                Assert.That(card.Spots.Select(s => s.Number).Distinct().Count(), Is.EqualTo(ticket.Spots));
                Assert.That(card.Key, Is.Ordered);
                Assert.That(card.Key.Distinct().Count(), Is.EqualTo(4));
            }
        }

        [Test]
        public void ADrawnTicket_WinsAsOftenAsThePoolSays()
        {
            var ticket = Ticket("lucky7s");
            var random = new Random(3);
            const int draws = 200_000;
            var won = new Dictionary<int, int>();
            for (int i = 0; i < draws; i++)
            {
                int prize = ticket.DrawPrize(random);
                won[prize] = won.TryGetValue(prize, out int n) ? n + 1 : 1;
            }
            Assert.That(1 - won[0] / (double)draws, Is.EqualTo(ticket.HitRate).Within(0.004));
            Assert.That(won[1] / (double)draws, Is.EqualTo(0.1).Within(0.003), "a dollar back, one ticket in ten");
            Assert.That(won.Keys.Where(k => k > 0), Is.SubsetOf(ticket.Prizes.Select(p => p.Amount)));
        }

        [Test]
        public void Buying_TakesThePrice_AndNeedsTheCash()
        {
            var ticket = Ticket("diamonddeluxe");
            var random = new Random(1);
            Assert.That(() => ScratchCard.Buy(ticket, 4, random), Throws.InvalidOperationException);
            for (int i = 0; i < 50; i++)
            {
                var card = ScratchCard.Buy(ticket, 5, random);
                Assert.That(card.Net, Is.EqualTo(card.Prize - 5));
                Assert.That(card.IsWinner, Is.EqualTo(card.Prize > 0));
            }
            Assert.That(() => ScratchCard.Print(ticket, 7, random), Throws.ArgumentException, "only the pool's amounts");
        }

        [Test]
        public void ATicket_RefusesAPoolItCantPrintHonestly()
        {
            Assert.That(() => ScratchTicket.MatchThree("x", "X", 1, 9, new ScratchPrize(1, 10), new ScratchPrize(2, 10), new ScratchPrize(3, 10)),
                Throws.ArgumentException, "three amounts can't fill nine spots without three alike");
            Assert.That(() => ScratchTicket.LuckyNumbers("x", "X", 1, 3, 10, 12, new ScratchPrize(1, 10)),
                Throws.InstanceOf<ArgumentOutOfRangeException>(), "not enough numbers to miss with");
            Assert.That(() => ScratchTicket.FindSymbol("x", "X", 1, 6, "7", new[] { "7", "8" }, new ScratchPrize(1, 10)),
                Throws.ArgumentException, "the symbol to find can't be a decoy");
            Assert.That(() => ScratchTicket.MatchThree("x", "X", 1, 3, new ScratchPrize(1, 600_000), new ScratchPrize(2, 400_000)),
                Throws.ArgumentException, "more winners than tickets");
        }

        // ---- The latex ----

        [Test]
        public void AStroke_ClearsABandAlongItsLine_AndNothingFarFromIt()
        {
            var mask = new ScratchMask(100, 60);
            int cleared = mask.Scratch(0, 30, 100, 30, 10, 10);

            Assert.That(cleared, Is.GreaterThan(100 * 14), "at least three quarters of the coin's width all the way along");
            Assert.That(mask.IsScratched(50, 30), Is.True);
            Assert.That(mask.IsScratched(50, 36), Is.True, "inside three quarters of the radius always comes off");
            Assert.That(mask.IsScratched(50, 45), Is.False, "beyond the coin stays on");
            Assert.That(mask.IsScratched(50, 5), Is.False);
            Assert.That(mask.Scratch(0, 30, 100, 30, 10, 10), Is.EqualTo(0), "the same stroke again takes nothing more");
            Assert.That(mask.ScratchedCount, Is.EqualTo(cleared));
        }

        [Test]
        public void TheCoin_StaysRound_WhenTheSpotsPixelsArent()
        {
            var mask = new ScratchMask(40, 40);
            mask.Scratch(20, 20, 20, 20, 4, 12);
            Assert.That(mask.IsScratched(20, 27), Is.True, "reaches further down than across");
            Assert.That(mask.IsScratched(26, 20), Is.False);
        }

        [Test]
        public void ASpot_IsReadable_OnceMoreThanHalfIsOff()
        {
            var mask = new ScratchMask(50, 50);
            mask.Scratch(0, 10, 50, 10, 10, 10);
            Assert.That(mask.Readable, Is.False);
            mask.Scratch(0, 30, 50, 30, 10, 10);
            mask.Scratch(0, 45, 50, 45, 10, 10);
            Assert.That(mask.Cleared, Is.GreaterThanOrEqualTo(ScratchMask.ShowsThrough));
            Assert.That(mask.Readable, Is.True);
            mask.ClearAll();
            Assert.That(mask.Cleared, Is.EqualTo(1));
        }

        [Test]
        public void AStrokeOffTheSpot_TakesNothing()
        {
            var mask = new ScratchMask(30, 30);
            Assert.That(mask.Scratch(-100, -100, -60, -40, 8, 8), Is.EqualTo(0));
            Assert.That(mask.Scratch(200, 10, 300, 10, 8, 8), Is.EqualTo(0));
        }
    }
}
