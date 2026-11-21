using System;
using System.Linq;
using NUnit.Framework;
using Starflare.Craps;

namespace Starflare.Tests
{
    /// <summary>Juan's late game as the GDD writes it ("Street craps", Mike 2026-11-20).</summary>
    public class CrapsGameTests
    {
        const int Cash = 500;

        static Dice Set(params int[] faces)
        {
            var rolls = new Roll[faces.Length / 2];
            for (int i = 0; i < rolls.Length; i++) rolls[i] = new Roll(faces[2 * i], faces[2 * i + 1]);
            return Dice.Set(rolls);
        }

        static CrapsGame Game(Dice dice, int shooter = CrapsGame.You) => new CrapsGame(new CrapsRules(), dice, shooter);

        // Come-out

        [TestCase(3, 4)]
        [TestCase(5, 6)]
        public void ANatural_WinsRight_AndLosesWrong(int a, int b)
        {
            var game = Game(Set(a, b), shooter: 1);
            game.Place(BetKind.Right, 4, Cash);
            var result = game.Throw();
            Assert.That(result.Net, Is.EqualTo(4));
            Assert.That(game.Phase, Is.EqualTo(CrapsPhase.ComeOut));
            Assert.That(game.Shooter, Is.EqualTo(1), "A natural doesn't pass the dice.");

            game = Game(Set(a, b), shooter: 1);
            game.Place(BetKind.Wrong, 4, Cash);
            Assert.That(game.Throw().Net, Is.EqualTo(-4));
        }

        [TestCase(1, 1)]
        [TestCase(1, 2)]
        public void TwoOrThree_LosesRight_AndWinsWrong(int a, int b)
        {
            var game = Game(Set(a, b, a, b), shooter: 1);
            game.Place(BetKind.Right, 4, Cash);
            Assert.That(game.Throw().Net, Is.EqualTo(-4));
            game.Place(BetKind.Wrong, 4, Cash);
            Assert.That(game.Throw().Net, Is.EqualTo(4));
            Assert.That(game.Shooter, Is.EqualTo(1), "Craps on the come-out doesn't pass the dice.");
        }

        [Test]
        public void Twelve_IsAPushForWrong_AndStillASettledRound()
        {
            var game = Game(Set(6, 6), shooter: 1);
            game.Place(BetKind.Wrong, 10, Cash);
            var result = game.Throw();
            Assert.That(result.Net, Is.EqualTo(0));
            Assert.That(result.SettledAny, Is.True);
            Assert.That(result.Settled[0].Outcome, Is.EqualTo(BetOutcome.Push));
            Assert.That(game.Bet(BetKind.Wrong), Is.EqualTo(0));
        }

        [TestCase(2, 2, 4)]
        [TestCase(4, 1, 5)]
        [TestCase(3, 3, 6)]
        [TestCase(6, 2, 8)]
        [TestCase(4, 5, 9)]
        [TestCase(6, 4, 10)]
        public void APointNumber_SetsThePoint_AndDecidesNothing(int a, int b, int point)
        {
            var game = Game(Set(a, b), shooter: 1);
            game.Place(BetKind.Right, 4, Cash);
            var result = game.Throw();
            Assert.That(result.PointSet, Is.EqualTo(point));
            Assert.That(result.SettledAny, Is.False);
            Assert.That(game.Phase, Is.EqualTo(CrapsPhase.Point));
            Assert.That(game.Point, Is.EqualTo(point));
            Assert.That(game.IsRiding(BetKind.Right), Is.True);
        }

        // The point

        [Test]
        public void MakingThePoint_PaysRightAndThePointBet_LessJuansCut()
        {
            var game = Game(Set(3, 3, 2, 3, 4, 2), shooter: 1);
            game.Place(BetKind.Right, 10, Cash);
            game.Throw();
            game.Place(BetKind.PointFor, 5, Cash);

            var nothing = game.Throw();
            Assert.That(nothing.SettledAny, Is.False, "A 5 decides nothing on a 6 point.");

            var made = game.Throw();
            Assert.That(made.PointMade, Is.True);
            var right = made.Settled.Single(b => b.Kind == BetKind.Right);
            var pointBet = made.Settled.Single(b => b.Kind == BetKind.PointFor);
            Assert.That(right.Cut, Is.EqualTo(1));
            Assert.That(right.Net, Is.EqualTo(10 - 1), "A $10 win pays Juan his dollar.");
            Assert.That(pointBet.Cut, Is.EqualTo(0));
            Assert.That(pointBet.Net, Is.EqualTo(6), "$5 on the 6 wins $6 at 6 to 5, under ten: no cut.");
            Assert.That(made.Net, Is.EqualTo(15));
            Assert.That(game.Phase, Is.EqualTo(CrapsPhase.ComeOut));
            Assert.That(game.Shooter, Is.EqualTo(1), "Making the point keeps the dice.");
        }

        [Test]
        public void SevenOut_LosesRightAndFor_WinsWrongAndAgainst_AndPassesTheDice()
        {
            var game = Game(Set(4, 4, 3, 4), shooter: 1);
            game.Place(BetKind.Wrong, 4, Cash);
            game.Throw();
            game.Place(BetKind.PointAgainst, 6, Cash);

            var result = game.Throw();
            Assert.That(result.SevenOut, Is.True);
            Assert.That(result.Settled.Single(b => b.Kind == BetKind.Wrong).Net, Is.EqualTo(4), "Under $10 won: no cut.");
            Assert.That(result.Settled.Single(b => b.Kind == BetKind.PointAgainst).Net, Is.EqualTo(5), "$6 against the 8 wins $5.");
            Assert.That(game.Shooter, Is.EqualTo(2));
            Assert.That(game.Point, Is.EqualTo(0));
        }

        [Test]
        public void TheDice_GoAroundTheCircle()
        {
            var game = Game(Set(2, 2, 3, 4, 2, 2, 3, 4, 2, 2, 3, 4, 2, 2, 3, 4), shooter: 1);
            int[] shooters = new int[4];
            for (int hand = 0; hand < 4; hand++)
            {
                shooters[hand] = game.Shooter;
                if (game.YouShoot) game.Place(BetKind.Right, 1, Cash);
                game.Throw();
                game.Throw();
            }
            Assert.That(shooters, Is.EqualTo(new[] { 1, 2, 3, 0 }));
            Assert.That(game.Shooter, Is.EqualTo(1));
        }

        // Shooting

        [Test]
        public void ToShoot_YouBetRight()
        {
            var game = Game(Set(3, 4));
            Assert.That(game.CanThrow, Is.False);
            Assert.Throws<InvalidOperationException>(() => game.Throw());

            game.Place(BetKind.Wrong, 5, Cash);
            Assert.That(game.CanThrow, Is.False, "A wrong bet doesn't let you shoot.");

            game.Place(BetKind.Right, 1, Cash);
            Assert.That(game.CanThrow, Is.True);
        }

        [Test]
        public void YouCanPassTheDice_OnlyOnYourComeOut()
        {
            var game = Game(Set(4, 4));
            Assert.That(game.CanPassDice, Is.True);
            game.PassDice();
            Assert.That(game.Shooter, Is.EqualTo(1));
            Assert.That(game.CanPassDice, Is.False);
            Assert.Throws<InvalidOperationException>(() => game.PassDice());
        }

        [Test]
        public void YouNeedNoBet_ToCallARegularsThrow()
        {
            var game = Game(Set(2, 3), shooter: 2);
            Assert.That(game.CanThrow, Is.True);
            Assert.That(game.Throw().SettledAny, Is.False);
        }

        // Betting

        [Test]
        public void RightAndWrong_GoDownOnlyOnAComeOut_AndPointBetsOnlyOnAPoint()
        {
            var game = Game(Set(4, 4), shooter: 1);
            Assert.That(game.CanBet(BetKind.PointFor), Is.False);
            Assert.Throws<InvalidOperationException>(() => game.Place(BetKind.PointFor, 5, Cash));
            game.Throw();
            Assert.That(game.CanBet(BetKind.Right), Is.False);
            Assert.That(game.CanBet(BetKind.Wrong), Is.False);
            Assert.That(game.CanBet(BetKind.PointFor), Is.True);
            Assert.That(game.CanBet(BetKind.PointAgainst), Is.True);
        }

        [Test]
        public void ABet_CanBeTakenBack_UntilTheDiceAreThrown()
        {
            var game = Game(Set(4, 4), shooter: 1);
            game.Place(BetKind.Right, 5, Cash);
            game.Place(BetKind.Right, 8, Cash);
            Assert.That(game.Bet(BetKind.Right), Is.EqualTo(8), "Placing again replaces it.");
            game.TakeBack(BetKind.Right);
            Assert.That(game.Staked, Is.EqualTo(0));

            game.Place(BetKind.Right, 5, Cash);
            game.Throw();
            Assert.That(game.AnyRiding, Is.True);
            Assert.Throws<InvalidOperationException>(() => game.TakeBack(BetKind.Right));
        }

        [Test]
        public void Bets_StayWithinTheLimits_AndYourCash()
        {
            var game = Game(Set(4, 4), shooter: 1);
            Assert.That(game.CanPlace(BetKind.Right, 0, Cash), Is.False);
            Assert.That(game.CanPlace(BetKind.Right, 21, Cash), Is.False);
            Assert.That(game.CanPlace(BetKind.Right, 20, Cash), Is.True);

            game.Place(BetKind.Right, 12, 20);
            Assert.That(game.CanPlace(BetKind.Wrong, 9, 20), Is.False, "$12 and $9 is more than $20 in hand.");
            Assert.That(game.CanPlace(BetKind.Wrong, 8, 20), Is.True);
            Assert.That(game.CanPlace(BetKind.Right, 20, 20), Is.True, "Replacing a placed bet counts only the new amount.");
        }

        /// <summary>There's no change in the circle, so a bet must pay whole dollars.</summary>
        [TestCase(BetKind.Right, 6, new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 })]
        [TestCase(BetKind.PointFor, 4, new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 })]
        [TestCase(BetKind.PointFor, 9, new[] { 2, 4, 6, 8, 10, 12, 14, 16, 18, 20 })]
        [TestCase(BetKind.PointFor, 6, new[] { 5, 10, 15, 20 })]
        [TestCase(BetKind.PointAgainst, 10, new[] { 2, 4, 6, 8, 10, 12, 14, 16, 18, 20 })]
        [TestCase(BetKind.PointAgainst, 5, new[] { 3, 6, 9, 12, 15, 18 })]
        [TestCase(BetKind.PointAgainst, 8, new[] { 6, 12, 18 })]
        public void BetAmounts_PayWholeDollars(BetKind kind, int point, int[] amounts)
        {
            Assert.That(new CrapsRules().Amounts(kind, point), Is.EqualTo(amounts));
        }

        /// <summary>
        /// Point bets pay true odds, so on their own they're an even game; Juan's cut is the only
        /// edge. Checked exactly: the winnings times the ways to win equal the stake times the
        /// ways to lose (a 7 has six ways).
        /// </summary>
        [TestCase(4, 3)]
        [TestCase(5, 4)]
        [TestCase(6, 5)]
        [TestCase(8, 5)]
        [TestCase(9, 4)]
        [TestCase(10, 3)]
        public void PointBets_PayTrueOdds(int point, int waysToMake)
        {
            int forIt = new CrapsRules().Amounts(BetKind.PointFor, point).Last();
            Assert.That(CrapsRules.Winnings(BetKind.PointFor, forIt, point) * waysToMake, Is.EqualTo(forIt * 6));

            int against = new CrapsRules().Amounts(BetKind.PointAgainst, point).Last();
            Assert.That(CrapsRules.Winnings(BetKind.PointAgainst, against, point) * 6, Is.EqualTo(against * waysToMake));
        }

        [TestCase(1, 0)]
        [TestCase(9, 0)]
        [TestCase(10, 1)]
        [TestCase(19, 1)]
        [TestCase(20, 2)]
        [TestCase(40, 4)]
        public void JuansCut_IsADollarForEveryFullTenWon(int winnings, int cut)
        {
            Assert.That(new CrapsRules().CutOn(winnings), Is.EqualTo(cut));
        }

        [Test]
        public void JuansCut_ComesFromTheWin_NotTheBet()
        {
            var game = Game(Set(2, 2, 2, 2), shooter: 1);
            game.Throw();
            game.Place(BetKind.PointFor, 20, Cash);

            var won = game.Throw().Settled.Single();
            Assert.That(won.Cut, Is.EqualTo(4), "$20 on the 4 wins $40 at 2 to 1.");
            Assert.That(won.Net, Is.EqualTo(36));
        }

        // Dice

        /// <summary>Regular play is always fair: every face and every total comes up as often as it should.</summary>
        [Test]
        public void FairDice_ComeUpEvenly()
        {
            var dice = new Dice(new Random(1234));
            const int throws = 360000;
            var faces = new int[7];
            var totals = new int[13];
            for (int i = 0; i < throws; i++)
            {
                var roll = dice.Throw();
                faces[roll.Die1]++;
                faces[roll.Die2]++;
                totals[roll.Total]++;
            }
            for (int face = 1; face <= 6; face++)
                Assert.That(faces[face], Is.EqualTo(2 * throws / 6).Within(2000), $"face {face}");
            int[] ways = { 0, 0, 1, 2, 3, 4, 5, 6, 5, 4, 3, 2, 1 };
            for (int total = 2; total <= 12; total++)
                Assert.That(totals[total], Is.EqualTo(throws * ways[total] / 36).Within(1500), $"total {total}");
        }

        /// <summary>
        /// Juan's practice turn (Mike, 2026-11-20): a come-out 6 sets the point, a 6 makes it, a
        /// come-out 10 sets a new one, and a 7 ends the turn and the dice pass. Then the set dice
        /// are done and the table goes back to fair ones.
        /// </summary>
        [Test]
        public void JuansPracticeTurn_PlaysAsWritten()
        {
            var game = Game(Set(3, 3, 5, 1, 5, 5, 4, 3));
            game.Place(BetKind.Right, 5, Cash);
            Assert.That(game.Throw().PointSet, Is.EqualTo(6));
            Assert.That(game.Throw().PointMade, Is.True);

            game.Place(BetKind.Right, 5, Cash);
            Assert.That(game.Throw().PointSet, Is.EqualTo(10));
            var last = game.Throw();
            Assert.That(last.SevenOut, Is.True);
            Assert.That(game.Shooter, Is.EqualTo(1));
            Assert.That(game.CanThrow, Is.False, "The set dice are done.");
        }
    }
}
