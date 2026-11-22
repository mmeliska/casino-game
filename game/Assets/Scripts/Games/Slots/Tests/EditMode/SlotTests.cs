using System;
using System.Linq;
using NUnit.Framework;
using Starflare.Slots;

namespace Starflare.Tests
{
    public class SlotTests
    {
        // Bridgehead's strips, by index: a symbol on every even stop, a blank on every odd one.
        // The middle row is the payline, so a symbol at index i is on the line at stop i - 1.
        static int OnLine(int index, int length) => (index - 1 + length) % length;

        static SlotSpin TripleBar(int credits, params int[] symbolIndexes)
        {
            var rules = SlotRules.TripleBar();
            return new SlotSpin(rules, credits, symbolIndexes.Select((i, c) => OnLine(i, rules.Reels[c].Count)).ToArray());
        }

        [Test]
        public void TripleBar_IsTheWorstMachineInTown_AndSaysSoInItsOdds()
        {
            var rules = SlotRules.TripleBar();
            var one = SlotOdds.Of(rules, 1);
            var max = SlotOdds.Of(rules, rules.MaxCredits);
            Assert.That(one.ReturnToPlayer, Is.EqualTo(0.8603).Within(0.0005));
            Assert.That(max.ReturnToPlayer, Is.EqualTo(0.8790).Within(0.0005), "betting max earns the 7s' bonus");
            Assert.That(one.HitRate, Is.EqualTo(0.2225).Within(0.0005), "a win about one pull in four and a half");
            Assert.That(max.ReturnToPlayer, Is.LessThan(0.95), "under the 6/5 poker machine next to it");
        }

        [Test]
        public void TripleBar_ThreeSevens_Pay200ForOne_And1200AtMaxBet()
        {
            Assert.That(TripleBar(1, 0, 0, 0).Payout, Is.EqualTo(200));
            Assert.That(TripleBar(2, 0, 0, 0).Payout, Is.EqualTo(400));
            Assert.That(TripleBar(3, 0, 0, 0).Payout, Is.EqualTo(1200));
            Assert.That(TripleBar(3, 0, 0, 0).Net, Is.EqualTo(1197));
            Assert.That(TripleBar(1, 0, 0, 0).LineSymbols(0), Is.EqualTo(new[] { "seven", "seven", "seven" }));
        }

        [Test]
        public void ALine_PaysOnlyItsBestMatch()
        {
            var cherries = TripleBar(1, 4, 6, 10);
            Assert.That(cherries.LineSymbols(0), Is.EqualTo(new[] { "cherry", "cherry", "cherry" }));
            Assert.That(cherries.Payout, Is.EqualTo(20), "three cherries, not three plus two plus one");
            Assert.That(cherries.Wins.Single().Pay.Label, Is.EqualTo("CHERRY CHERRY CHERRY"));

            Assert.That(TripleBar(1, 12, 5, 8).Payout, Is.EqualTo(0), "a triple bar, a blank and a triple bar");
            Assert.That(TripleBar(1, 12, 10, 8).Payout, Is.EqualTo(80), "three triple bars");
            Assert.That(TripleBar(1, 2, 4, 8).Payout, Is.EqualTo(5), "one of each bar is any bar");
        }

        [Test]
        public void Cherries_CountFromTheLeftReel()
        {
            Assert.That(TripleBar(1, 4, 1, 0).Payout, Is.EqualTo(2), "one cherry on the left reel");
            Assert.That(TripleBar(1, 4, 6, 0).Payout, Is.EqualTo(10), "two from the left");
            Assert.That(TripleBar(1, 2, 6, 10).Payout, Is.EqualTo(0), "cherries not starting on the left reel pay nothing");
        }

        [Test]
        public void TheWindow_ShowsTheStripAroundEachStop_WrappingRound()
        {
            var rules = SlotRules.TripleBar();
            var spin = new SlotSpin(rules, 1, new[] { 21, 0, 1 });
            Assert.That(spin.At(0, 0), Is.EqualTo(SlotRules.Blank), "the last stop, a blank, above the 7");
            Assert.That(spin.At(0, 1), Is.EqualTo("seven"));
            Assert.That(spin.At(0, 2), Is.EqualTo(SlotRules.Blank));
            Assert.That(spin.At(1, 0), Is.EqualTo("seven"));
            Assert.That(spin.At(2, 1), Is.EqualTo("bar1"));
        }

        /// <summary>A five-reel, three-row machine with three lines, one of them a V.</summary>
        static SlotRules FiveByThree() => new SlotRules("test", "TEST", 3,
            Enumerable.Repeat(new[] { "A", "B", "C", "D" }, 5).ToArray(),
            new[] { new[] { 1, 1, 1, 1, 1 }, new[] { 0, 0, 0, 0, 0 }, new[] { 0, 1, 2, 1, 0 } },
            new[] { new SlotPay("A x5", 5, 50, "A"), new SlotPay("A x3", 3, 5, "A"), new SlotPay("B x2", 2, 1, "B") });

        [Test]
        public void AnyGrid_BetsEveryLine_AndPaysEachLineThatWins()
        {
            var rules = FiveByThree();
            Assert.That(rules.Columns, Is.EqualTo(5));
            Assert.That(rules.BetFor(2), Is.EqualTo(6), "two credits on each of three lines");

            var flat = new SlotSpin(rules, 2, new[] { 0, 0, 0, 0, 0 });
            Assert.That(flat.Wins.Select(w => w.Line), Is.EqualTo(new[] { 0, 1 }), "B x5 on the middle row, A x5 on the top; the V reads A B C");
            Assert.That(flat.Payout, Is.EqualTo(2 * 1 + 2 * 50));
            Assert.That(flat.IsWinningCell(4, 0), Is.True, "all five A's on the top row");
            Assert.That(flat.IsWinningCell(2, 1), Is.False, "the middle row's B x2 is the first two only");

            // The V: A on reel 1 at the top, A on reel 2 in the middle, A on reel 3 at the bottom.
            var v = new SlotSpin(rules, 1, new[] { 0, 3, 2, 3, 0 });
            Assert.That(v.LineSymbols(2), Is.EqualTo(new[] { "A", "A", "A", "A", "A" }));
            Assert.That(v.Wins.Single(w => w.Line == 2).Credits, Is.EqualTo(50));
        }

        [Test]
        public void APull_IsFair_AndOnlyWhatYouCanCover()
        {
            var rules = SlotRules.TripleBar();
            var random = new Random(7);
            var seen = new int[22];
            for (int i = 0; i < 22 * 200; i++) seen[SlotSpin.Pull(rules, 1, 100, random).Stops[0]]++;
            Assert.That(seen.Min(), Is.GreaterThan(140), "every stop comes up about as often as any other");

            Assert.That(() => SlotSpin.Pull(rules, 3, 2, random), Throws.InvalidOperationException);
            Assert.That(() => SlotSpin.Pull(rules, 4, 100, random), Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(() => new SlotSpin(rules, 1, new[] { 0, 0 }), Throws.ArgumentException);
        }

        [Test]
        public void AMachine_RefusesReelsAndLinesThatDontFit()
        {
            var pays = new[] { new SlotPay("A", 1, 2, "A") };
            Assert.That(() => new SlotRules("x", "X", 3, new[] { new[] { "A", "B" } }, new[] { new[] { 1 } }, pays),
                Throws.ArgumentException, "a strip shorter than the window");
            Assert.That(() => new SlotRules("x", "X", 1, new[] { new[] { "A" } }, new[] { new[] { 1 } }, pays),
                Throws.ArgumentException, "a line through a row that isn't there");
            Assert.That(() => new SlotRules("x", "X", 1, new[] { new[] { "A" } }, new[] { new[] { 0, 0 } }, pays),
                Throws.ArgumentException, "a line across more reels than the machine has");
            Assert.That(() => new SlotRules("x", "X", 1, new[] { new[] { "A" } }, new[] { new[] { 0 } }, new[] { new SlotPay("AA", 2, 2, "A") }),
                Throws.ArgumentException, "a pay counting more reels than there are");
        }

        // ---- The reels on screen ----

        [Test]
        public void AReel_KicksUp_Spins_AndLandsExactlyOnItsStop()
        {
            var reel = new ReelMotion(22, 5);
            reel.Spin(17, 1.0);
            reel.Advance(ReelMotion.KickSeconds / 2);
            Assert.That(reel.Position, Is.GreaterThan(5), "the kick up as the handle goes");

            double passedStop = double.MaxValue;
            double t = ReelMotion.KickSeconds / 2;
            while (reel.Turning && t < 5)
            {
                reel.Advance(1 / 60.0);
                t += 1 / 60.0;
                if (t > 1.0) passedStop = Math.Min(passedStop, reel.Position);
            }
            Assert.That(reel.Turning, Is.False);
            Assert.That(reel.Position, Is.EqualTo(17));
            Assert.That(reel.Top, Is.EqualTo(17));
            Assert.That(passedStop, Is.LessThan(17), "it swings past the stop and settles back");
            Assert.That(t, Is.EqualTo(1.0 + ReelMotion.LandingSeconds).Within(0.2));
        }

        [Test]
        public void TappingStop_LandsTheReelSooner_NeverLater()
        {
            var reel = new ReelMotion(22);
            reel.Spin(3, 2.0);
            reel.Advance(0.3);
            reel.StopWithin(0);
            reel.StopWithin(5);
            double t = 0;
            while (reel.Turning && t < 3) { reel.Advance(1 / 60.0); t += 1 / 60.0; }
            Assert.That(t, Is.LessThan(ReelMotion.LandingSeconds + 0.25));
            Assert.That(reel.Top, Is.EqualTo(3));
        }

        [Test]
        public void AReel_WrapsRoundItsStrip()
        {
            var reel = new ReelMotion(4, 0);
            reel.Spin(2, 0.5);
            for (int i = 0; i < 200 && reel.Turning; i++)
            {
                reel.Advance(1 / 60.0);
                Assert.That(reel.Top, Is.InRange(0, 3));
                Assert.That(reel.Offset, Is.InRange(0, 1));
            }
            Assert.That(reel.Top, Is.EqualTo(2));
        }

        // ---- The bulbs ----

        [Test]
        public void AWin_FlashesEveryBulbTogether()
        {
            for (double t = 0; t < 1; t += 0.05)
            {
                bool first = LightShow.IsLit(LightPattern.Win, 0, 22, t);
                Assert.That(Enumerable.Range(0, 22).All(b => LightShow.IsLit(LightPattern.Win, b, 22, t) == first));
            }
            Assert.That(LightShow.IsLit(LightPattern.Win, 0, 22, 0), Is.Not.EqualTo(LightShow.IsLit(LightPattern.Win, 0, 22, LightShow.WinStep)));
        }

        [Test]
        public void TheChase_LightsOneBulbInThreeWhileSpinning_AndMovesOn()
        {
            int Lit(LightPattern p, double t) => Enumerable.Range(0, 21).Count(b => LightShow.IsLit(p, b, 21, t));
            Assert.That(Lit(LightPattern.Spinning, 0), Is.EqualTo(7));
            Assert.That(Lit(LightPattern.Idle, 0), Is.EqualTo(6), "one in four, slower, waiting for a pull");
            Assert.That(LightShow.IsLit(LightPattern.Spinning, 0, 21, 0), Is.True);
            Assert.That(LightShow.IsLit(LightPattern.Spinning, 1, 21, LightShow.SpinStep), Is.True, "the lit bulb has moved one along");
            Assert.That(LightShow.IsLit(LightPattern.Spinning, 0, 21, LightShow.SpinStep), Is.False);
        }

        [Test]
        public void TheJackpot_FlashesTheHalvesAgainstEachOther()
        {
            Assert.That(LightShow.IsLit(LightPattern.Jackpot, 0, 22, 0), Is.Not.EqualTo(LightShow.IsLit(LightPattern.Jackpot, 1, 22, 0)));
            Assert.That(LightShow.IsLit(LightPattern.Jackpot, 0, 22, 0), Is.Not.EqualTo(LightShow.IsLit(LightPattern.Jackpot, 0, 22, LightShow.JackpotStep)));
        }
    }
}
