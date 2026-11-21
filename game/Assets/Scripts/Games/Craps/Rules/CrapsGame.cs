using System;
using System.Collections.Generic;

namespace Starflare.Craps
{
    /// <summary>A come-out is the first throw of a shooter's hand, or the next after a point is decided.</summary>
    public enum CrapsPhase { ComeOut, Point }

    public enum BetOutcome { Won, Lost, Push }

    /// <summary>One of your bets, decided by a roll.</summary>
    public readonly struct SettledBet
    {
        public readonly BetKind Kind;
        public readonly int Amount;
        public readonly BetOutcome Outcome;

        /// <summary>Juan's dollars, taken from a win: one for every full ten it won.</summary>
        public readonly int Cut;

        /// <summary>What it does to your cash: the winnings less the cut, minus the stake, or nothing on a push.</summary>
        public readonly int Net;

        public SettledBet(BetKind kind, int amount, BetOutcome outcome, int cut, int net)
        {
            Kind = kind;
            Amount = amount;
            Outcome = outcome;
            Cut = cut;
            Net = net;
        }
    }

    /// <summary>What one throw did.</summary>
    public sealed class ThrowResult
    {
        public Roll Roll;

        /// <summary>The seat that threw: 0 is you.</summary>
        public int Shooter;

        public List<SettledBet> Settled = new List<SettledBet>();

        /// <summary>The point this throw set, or 0.</summary>
        public int PointSet;

        /// <summary>The shooter made their point.</summary>
        public bool PointMade;

        /// <summary>A 7 after a point: the hand is over and the dice pass.</summary>
        public bool SevenOut;

        public int Net
        {
            get
            {
                int net = 0;
                foreach (var bet in Settled) net += bet.Net;
                return net;
            }
        }

        /// <summary>A settled round for the player state: at least one of your bets was decided, pushes included.</summary>
        public bool SettledAny => Settled.Count > 0;
    }

    /// <summary>
    /// A craps game around one circle, throw after throw, as Juan runs the late game (GDD,
    /// "Street craps"). The dice go around: a shooter keeps them until they seven out, then they
    /// pass to the next seat. You're seat 0; the regulars always fade your bets, so only yours
    /// are tracked. To shoot a come-out you bet right; if you won't, the dice pass.
    ///
    /// Pure rules: it never touches the bankroll or story state. Bets are placed between
    /// throws and can be taken back until the dice are thrown; then they ride until decided.
    /// The caller applies <see cref="ThrowResult.Net"/> when a throw settles any.
    /// </summary>
    public sealed class CrapsGame
    {
        public const int You = 0;

        readonly CrapsRules _rules;
        readonly Dice _dice;
        readonly int[] _amounts = new int[4];
        readonly bool[] _riding = new bool[4];

        public CrapsGame(CrapsRules rules, Dice dice, int firstShooter = You)
        {
            _rules = rules ?? throw new ArgumentNullException(nameof(rules));
            _dice = dice ?? throw new ArgumentNullException(nameof(dice));
            if (firstShooter < 0 || firstShooter >= rules.Seats) throw new ArgumentOutOfRangeException(nameof(firstShooter));
            Shooter = firstShooter;
        }

        public CrapsRules Rules => _rules;
        public CrapsPhase Phase { get; private set; }

        /// <summary>The shooter's point, or 0 on a come-out.</summary>
        public int Point { get; private set; }

        public int Shooter { get; private set; }
        public bool YouShoot => Shooter == You;

        /// <summary>What's on a bet, placed or riding; 0 for none.</summary>
        public int Bet(BetKind kind) => _amounts[(int)kind];

        /// <summary>The bet rode a throw and stays until it's decided.</summary>
        public bool IsRiding(BetKind kind) => _riding[(int)kind];

        /// <summary>All your money on the asphalt, placed or riding.</summary>
        public int Staked
        {
            get
            {
                int total = 0;
                foreach (int amount in _amounts) total += amount;
                return total;
            }
        }

        /// <summary>Something of yours is riding, so you can't get up yet.</summary>
        public bool AnyRiding => Array.IndexOf(_riding, true) >= 0;

        /// <summary>Right and wrong go down on a come-out; point bets once there's a point.</summary>
        public bool CanBet(BetKind kind) =>
            !IsRiding(kind) && (kind == BetKind.Right || kind == BetKind.Wrong
                ? Phase == CrapsPhase.ComeOut
                : Phase == CrapsPhase.Point);

        /// <summary>
        /// Puts <paramref name="amount"/> on a bet (replacing what was placed there and not yet
        /// thrown), if the rules take it and your cash covers everything you've staked.
        /// </summary>
        public bool CanPlace(BetKind kind, int amount, int cash) =>
            CanBet(kind) && _rules.Takes(kind, amount, Point) && Staked - Bet(kind) + amount <= cash;

        public void Place(BetKind kind, int amount, int cash)
        {
            if (!CanBet(kind)) throw new InvalidOperationException($"No {kind} bet now.");
            if (!_rules.Takes(kind, amount, Point)) throw new ArgumentOutOfRangeException(nameof(amount), $"The circle doesn't take ${amount} on {kind}.");
            if (Staked - Bet(kind) + amount > cash) throw new InvalidOperationException("Not enough cash.");
            _amounts[(int)kind] = amount;
        }

        /// <summary>A bet not yet thrown on can be picked back up.</summary>
        public void TakeBack(BetKind kind)
        {
            if (IsRiding(kind)) throw new InvalidOperationException($"The {kind} bet is riding.");
            _amounts[(int)kind] = 0;
        }

        /// <summary>The shooter can throw: on your come-out you need a right bet down.</summary>
        public bool CanThrow => !(YouShoot && Phase == CrapsPhase.ComeOut && Bet(BetKind.Right) == 0) && _dice.HasMore;

        /// <summary>You won't bet right on your come-out: the dice go to the next seat. Only between hands.</summary>
        public bool CanPassDice => YouShoot && Phase == CrapsPhase.ComeOut;

        public void PassDice()
        {
            if (!CanPassDice) throw new InvalidOperationException("You can pass the dice only on your come-out.");
            NextShooter();
        }

        public ThrowResult Throw()
        {
            if (!CanThrow) throw new InvalidOperationException(YouShoot ? "Bet right to shoot." : "The dice are done.");
            for (int i = 0; i < _amounts.Length; i++) _riding[i] = _amounts[i] > 0;

            var result = new ThrowResult { Roll = _dice.Throw(), Shooter = Shooter };
            int total = result.Roll.Total;

            if (Phase == CrapsPhase.ComeOut)
            {
                if (total == 7 || total == 11)
                {
                    Decide(result, BetKind.Right, BetOutcome.Won);
                    Decide(result, BetKind.Wrong, BetOutcome.Lost);
                }
                else if (total == 2 || total == 3)
                {
                    Decide(result, BetKind.Right, BetOutcome.Lost);
                    Decide(result, BetKind.Wrong, BetOutcome.Won);
                }
                else if (total == 12)
                {
                    Decide(result, BetKind.Right, BetOutcome.Lost);
                    Decide(result, BetKind.Wrong, BetOutcome.Push);
                }
                else
                {
                    Phase = CrapsPhase.Point;
                    Point = total;
                    result.PointSet = total;
                }
                return result;
            }

            if (total == Point)
            {
                Decide(result, BetKind.Right, BetOutcome.Won);
                Decide(result, BetKind.Wrong, BetOutcome.Lost);
                Decide(result, BetKind.PointFor, BetOutcome.Won);
                Decide(result, BetKind.PointAgainst, BetOutcome.Lost);
                result.PointMade = true;
                EndHand();
            }
            else if (total == 7)
            {
                Decide(result, BetKind.Right, BetOutcome.Lost);
                Decide(result, BetKind.Wrong, BetOutcome.Won);
                Decide(result, BetKind.PointFor, BetOutcome.Lost);
                Decide(result, BetKind.PointAgainst, BetOutcome.Won);
                result.SevenOut = true;
                EndHand();
                NextShooter();
            }
            return result;
        }

        void Decide(ThrowResult result, BetKind kind, BetOutcome outcome)
        {
            int amount = Bet(kind);
            if (amount == 0) return;
            int cut = 0, net = 0;
            if (outcome == BetOutcome.Won)
            {
                int winnings = CrapsRules.Winnings(kind, amount, Point);
                cut = _rules.CutOn(winnings);
                net = winnings - cut;
            }
            else if (outcome == BetOutcome.Lost)
            {
                net = -amount;
            }
            result.Settled.Add(new SettledBet(kind, amount, outcome, cut, net));
            _amounts[(int)kind] = 0;
            _riding[(int)kind] = false;
        }

        void EndHand()
        {
            Phase = CrapsPhase.ComeOut;
            Point = 0;
        }

        void NextShooter() => Shooter = (Shooter + 1) % _rules.Seats;
    }
}
