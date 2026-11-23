using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Starflare.Bowling;

namespace Starflare.Tests
{
    /// <summary>The fee, the stakes, the side pot and the challenges (bowling spec, "Rounds, money and break points").</summary>
    public class BowlingMoneyTests
    {
        static readonly BowlingRules Rules = new BowlingRules();

        static void Finish(BowlingGame game, Func<Seat, int> pinsPerFrame)
        {
            while (!game.IsComplete)
            {
                var seat = game.Up;
                int pins = pinsPerFrame(seat);
                game.Record(pins);
                if (pins < 10 && !game.IsComplete && game.Up == seat) game.Record(0);
            }
        }

        // Open bowling

        [Test]
        public void OpenBowling_CostsTheFee_AndTheShoesTheFirstGame()
        {
            Assert.That(Rules.OpenGameCost(rentingShoes: true, shoesPaid: false), Is.EqualTo(4));
            Assert.That(Rules.OpenGameCost(rentingShoes: true, shoesPaid: true), Is.EqualTo(2));
            Assert.That(Rules.OpenGameCost(rentingShoes: false, shoesPaid: false), Is.EqualTo(2), "Your own shoes: no rental.");

            var game = BowlingGame.Open(Rules.OpenGameCost(true, false));
            Finish(game, _ => 9);
            Assert.That(game.Settle(), Is.EqualTo(-4));
            Assert.Throws<InvalidOperationException>(() => _ = game.Result, "Open bowling isn't won or lost.");
        }

        // Challenges

        [Test]
        public void AChallenge_PaysTheStake_ToTheHighScore_AndATieIsAPush()
        {
            var chuy = Bowler.WithAverage("Chuy", "", 185);

            var won = BowlingGame.Challenge(chuy, 10);
            Finish(won, s => s.IsYou ? 10 : 9);
            Assert.That(won.Result, Is.EqualTo(Outcome.Won));
            Assert.That(won.Settle(), Is.EqualTo(10));

            var lost = BowlingGame.Challenge(chuy, 20);
            Finish(lost, s => s.IsYou ? 5 : 9);
            Assert.That(lost.Settle(), Is.EqualTo(-20));

            var tie = BowlingGame.Challenge(chuy, 5);
            Finish(tie, _ => 7);
            Assert.That(tie.Result, Is.EqualTo(Outcome.Push));
            Assert.That(tie.Settle(), Is.EqualTo(0));
            Assert.That(tie.Fee, Is.EqualTo(0), "No lane fee on his lane.");
        }

        [Test]
        public void AChallenge_AlternatesFrames_YouFirst()
        {
            var game = BowlingGame.Challenge(Bowler.WithAverage("Chuy", "", 185), 5);
            Assert.That(game.YourTurn, Is.True);
            Assert.That(game.Record(3), Is.False, "Your second ball's still to come.");
            Assert.That(game.Record(4), Is.True);
            Assert.That(game.YourTurn, Is.False);
            Assert.That(game.Record(10), Is.True, "A strike ends his frame.");
            Assert.That(game.YourTurn, Is.True);
        }

        [TestCase(185, 500, 10)]
        [TestCase(160, 500, 5)]
        [TestCase(185, 7, 5)]
        [TestCase(185, 4, null)]
        public void AChallenger_NamesAStake_WithinYourBankroll(int average, int bankroll, int? stake)
        {
            var bowler = Bowler.WithAverage("Regular", "", average);
            Assert.That(bowler.StakeAgainst(new NightRecord(), bankroll, Rules), Is.EqualTo(stake));
        }

        [Test]
        public void AChallenger_RaisesWhenHesAhead_AndDropsWhenHesBehind()
        {
            var chuy = Bowler.WithAverage("Chuy", "", 185);
            Assert.That(chuy.StakeAgainst(new NightRecord { Won = 1 }, 500, Rules), Is.EqualTo(20));
            Assert.That(chuy.StakeAgainst(new NightRecord { Lost = 1 }, 500, Rules), Is.EqualTo(5));
            Assert.That(Rules.Stakes, Is.EqualTo(new[] { 5, 10, 20 }));
        }

        [Test]
        public void AboutAQuarterOfGames_DrawAnOffer_NeverTwoRunning_NeverBeforeTheFirstGame()
        {
            var present = new List<Bowler> { Bowler.WithAverage("Chuy", "", 185), Bowler.WithAverage("A", "", 160), Bowler.WithAverage("B", "", 135) };
            int offers = 0, games = 0;
            for (int night = 0; night < 1000; night++)
            {
                var challenges = new Challenges(Rules, night);
                Assert.That(challenges.GamesBowled, Is.EqualTo(0), "Nobody asks before you've bowled.");
                bool last = false;
                for (int g = 0; g < 10; g++)
                {
                    var offer = challenges.AfterGame(present, 500);
                    games++;
                    if (offer == null) { last = false; continue; }
                    Assert.That(last, Is.False, "Never two in a row.");
                    Assert.That(present, Does.Contain(offer.Bowler));
                    offers++;
                    last = true;
                }
            }
            Assert.That(offers / (double)games, Is.InRange(0.22, 0.28));
        }

        [Test]
        public void ARegular_WhosLostThreeStraight_IsDoneForTheNight()
        {
            var chuy = Bowler.WithAverage("Chuy", "", 185);
            var challenges = new Challenges(Rules, 1);
            challenges.Settled(chuy, Outcome.Won);
            challenges.Settled(chuy, Outcome.Push);
            challenges.Settled(chuy, Outcome.Won);
            Assert.That(challenges.IsDone(chuy), Is.False, "A push doesn't break the run, but it isn't a loss.");
            challenges.Settled(chuy, Outcome.Won);
            Assert.That(challenges.IsDone(chuy), Is.True);
            for (int g = 0; g < 200; g++)
                Assert.That(challenges.AfterGame(new[] { chuy }, 500), Is.Null);

            challenges = new Challenges(Rules, 1);
            challenges.Settled(chuy, Outcome.Won);
            challenges.Settled(chuy, Outcome.Won);
            challenges.Settled(chuy, Outcome.Lost);
            challenges.Settled(chuy, Outcome.Won);
            Assert.That(challenges.IsDone(chuy), Is.False, "A win for him resets the run.");
        }

        // A sub night

        static LeagueMatch Match() => new LeagueMatch("Millstone Parts", "Lane Six",
            new[] { Bowler.WithAverage("Chuy", "", 185), Bowler.WithAverage("A", "", 160), Bowler.WithAverage("B", "", 150) },
            new[] { Bowler.WithAverage("C", "", 170), Bowler.WithAverage("D", "", 160), Bowler.WithAverage("E", "", 140), Bowler.WithAverage("F", "", 130) });

        [Test]
        public void EightBowlers_BowlInTurn_AFrameAtATime_YouLeadingOff()
        {
            var match = Match();
            var game = match.Next();
            var order = new List<string>();
            for (int i = 0; i < 8; i++)
            {
                order.Add(game.Up.Name);
                Assert.That(game.Record(10), Is.True);
            }
            Assert.That(order, Is.EqualTo(new[] { "You", "C", "Chuy", "D", "A", "E", "B", "F" }));
            Assert.That(game.YourTurn, Is.True, "Back to you for the second frame.");
            Assert.That(game.Seats.Count(s => s.Team == BowlingGame.Yours), Is.EqualTo(4));
        }

        [Test]
        public void TeamPins_AreTheSumOfFourSheets_AndThePotPaysTheWinners()
        {
            var match = Match();
            var game = match.Next();
            Finish(game, s => s.Team == BowlingGame.Yours ? 8 : 7);
            Assert.That(game.TeamPins(BowlingGame.Yours), Is.EqualTo(game.Seats.Where(s => s.Team == 0).Sum(s => s.Sheet.Score)));
            Assert.That(game.TeamPins(BowlingGame.Yours), Is.EqualTo(4 * 80));
            Assert.That(game.Settle(), Is.EqualTo(5));
            Assert.That(game.Fee, Is.EqualTo(0));

            var lost = match.Next();
            Finish(lost, s => s.Team == BowlingGame.Yours ? 6 : 7);
            Assert.That(lost.Settle(), Is.EqualTo(-5));

            Assert.That(match.FinishedNight, Is.False);
            var tie = match.Next();
            Finish(tie, _ => 7);
            Assert.That(tie.Settle(), Is.EqualTo(0));

            Assert.That(match.FinishedNight, Is.True);
            Assert.That(match.GamesFinished, Is.EqualTo(3));
            Assert.That(match.GamesWon, Is.EqualTo(1));
            Assert.That(match.TeamWonNight, Is.False, "One up, one down, one tied.");
            Assert.Throws<InvalidOperationException>(() => match.Next());
        }

        // The counter

        [Test]
        public void TheCounter_SellsTheBallsYouDontHave()
        {
            Assert.That(Rules.BallsForSale(BowlingBall.House), Is.EqualTo(new[] { BowlingBall.Urethane, BowlingBall.Reactive }));
            Assert.That(Rules.BallsForSale(BowlingBall.Urethane), Is.EqualTo(new[] { BowlingBall.Reactive }));
            Assert.That(Rules.BallsForSale(BowlingBall.Reactive), Is.Empty);
            Assert.That(Rules.PriceOf(BowlingBall.Urethane), Is.EqualTo(40));
            Assert.That(Rules.PriceOf(BowlingBall.Reactive), Is.EqualTo(120));
            Assert.That(Rules.ShoesPrice, Is.EqualTo(30));
            Assert.That(BowlingBall.ById("reactive"), Is.SameAs(BowlingBall.Reactive));
            Assert.That(BowlingBall.ById(null), Is.SameAs(BowlingBall.House));
        }

        [Test]
        public void TheMeters_AreExact_UnlessTheJitterIsOn()
        {
            var t = new Throw(20, 10, 50, 16);
            Assert.That(Rules.Apply(t, new Random(1)), Is.EqualTo(t));
            var jittery = new BowlingRules { AimJitter = 1, MeterJitter = 2 };
            var thrown = jittery.Apply(t, new Random(1));
            Assert.That(thrown.Aim, Is.InRange(9.0, 11.0));
            Assert.That(thrown.Hook, Is.InRange(48.0, 52.0));
        }

        // The lane's wear

        [Test]
        public void TheLane_IsFreshAtFive_AndTheLeagueWearsIt()
        {
            Assert.That(LaneWear.BallsSinceOiling(DayOfWeek.Wednesday, 17 * 60), Is.EqualTo(0));
            Assert.That(LaneWear.BallsSinceOiling(DayOfWeek.Wednesday, 22 * 60), Is.EqualTo(50));
            Assert.That(LaneWear.BallsSinceOiling(DayOfWeek.Tuesday, 22 * 60), Is.EqualTo(20 + 180));
            Assert.That(LaneWear.BallsSinceOiling(DayOfWeek.Wednesday, 2 * 60), Is.EqualTo(200 + 10), "After midnight it's still Tuesday's oil.");
            Assert.That(LaneWear.BallsSinceOiling(DayOfWeek.Wednesday, 16 * 60), Is.EqualTo(210 + 20));
        }
    }
}
