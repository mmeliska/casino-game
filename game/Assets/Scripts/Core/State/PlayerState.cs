using System;

namespace Starflare.State
{
    /// <summary>The box on Dot's register (A1.1): it sets honorifics, the rival and the first wedding dance.</summary>
    public enum RegisterBox { Unticked, Mr, Ms, PreferNotToAnswer }

    /// <summary>
    /// The slice of the GDD's player state model the prototype needs. NPC state, flags and
    /// Pockets join it as their systems are built.
    ///
    /// Every change goes through a method here and raises <see cref="Changed"/>, so the owner
    /// can save and the screens can redraw without polling. The fields stay public for the
    /// JSON save; nothing else should write them.
    /// </summary>
    [Serializable]
    public sealed class PlayerState
    {
        /// <summary>The save schema this code writes. Bump it with a migration step in the save store.</summary>
        public const int CurrentVersion = 5;

        /// <summary>You drive in with about $200 and a car (A1.1).</summary>
        public const int StartingBankroll = 200;

        public int Version = CurrentVersion;

        public int BankrollNow = StartingBankroll;

        /// <summary>Most of your own money ever held (cash minus debts). Only goes up; gates venues and acts.</summary>
        public int BestBankroll = StartingBankroll;

        /// <summary>Total owed, every creditor together: what the top bar shows as owed. The Wallet splits it by creditor later.</summary>
        public int Debts;

        /// <summary>
        /// Ray's part of <see cref="Debts"/>: "the envelope", the $2,000 plus interest he puts on
        /// you when Raffy names you (A1.5). There's no paying it off until Ray's loans are built.
        /// </summary>
        public int RayOwed;

        public int RoundsPlayed;

        /// <summary>Your name and box, signed once on Dot's register at check-in (A1.1).</summary>
        public string Name = "";
        public RegisterBox Box;

        /// <summary>Where you are: a place id, as on the docs' place pages. You start at the motel.</summary>
        public string Place = "slack-water";

        /// <summary>You arrive at night (A1.1).</summary>
        public GameClock Clock = new GameClock { MinuteOfDay = GameClock.NightStart };

        /// <summary>The four hidden scales, nudged by choices in scenes and jobs.</summary>
        public Scales Scales = new Scales();

        /// <summary>What the police know about your illegal play (GDD, "Heat").</summary>
        public Heat Heat = new Heat();

        /// <summary>Raised after every settled change, once per change. Never mid-round.</summary>
        [field: NonSerialized]
        public event Action Changed;

        public int OwnMoney => BankrollNow - Debts;

        /// <summary>What you owe on Kasia's book: everything owed but Ray's part.</summary>
        public int KasiasBook => Debts - RayOwed;

        public bool HasSigned => !string.IsNullOrEmpty(Name);

        public HeatStep HeatStep => Heat.Step;

        /// <summary>Signing the register: set once, used from then on.</summary>
        public void SignRegister(string name, RegisterBox box)
        {
            if (HasSigned) throw new InvalidOperationException("Already signed.");
            name = name?.Trim();
            if (string.IsNullOrEmpty(name)) throw new ArgumentException("A name is required.", nameof(name));
            if (box == RegisterBox.Unticked) throw new ArgumentException("A box must be ticked.", nameof(box));
            Name = name;
            Box = box;
            Changed?.Invoke();
        }

        /// <summary>
        /// The debug panel's switch, for testing who asks first at the wedding (A1.4). Play never
        /// calls it: the box is ticked once, at Dot's register.
        /// </summary>
        public void RetickBox(RegisterBox box)
        {
            if (!HasSigned) throw new InvalidOperationException("Not signed yet.");
            if (box == RegisterBox.Unticked) throw new ArgumentException("A box must be ticked.", nameof(box));
            Box = box;
            Changed?.Invoke();
        }

        /// <summary>Called by a game module when a round settles, with the story minutes played (<see cref="Sitting"/>).</summary>
        public void ApplyRoundResult(int net, int minutes)
        {
            BankrollNow += net;
            RoundsPlayed++;
            Clock.Advance(minutes);
            ClockMoved();
            RaiseBestBankroll();
            Changed?.Invoke();
        }

        /// <summary>Cash in hand changes outside a game: a tip, a drink, a job's pay.</summary>
        public void Spend(int amount)
        {
            if (amount > BankrollNow) throw new InvalidOperationException("Not enough cash.");
            BankrollNow -= amount;
            Changed?.Invoke();
        }

        public void Earn(int amount)
        {
            BankrollNow += amount;
            RaiseBestBankroll();
            Changed?.Invoke();
        }

        /// <summary>A job's shift: its pay in cash, and its hours off the clock (the bar-back shift is about 4).</summary>
        public void WorkShift(int pay, int hours)
        {
            if (pay < 0) throw new ArgumentOutOfRangeException(nameof(pay));
            if (hours < 0) throw new ArgumentOutOfRangeException(nameof(hours));
            BankrollNow += pay;
            Clock.Advance(hours * 60);
            ClockMoved();
            RaiseBestBankroll();
            Changed?.Invoke();
        }

        /// <summary>A choice nudges a hidden scale one step toward a pole, such as "crooked".</summary>
        public void Lean(string pole)
        {
            Scales.Lean(pole);
            Changed?.Invoke();
        }

        /// <summary>Credit: cash now, owed later. Own money, and so best bankroll, doesn't move.</summary>
        public void Borrow(int amount)
        {
            if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
            BankrollNow += amount;
            Debts += amount;
            Changed?.Invoke();
        }

        /// <summary>Paying down Kasia's book. Ray's part isn't hers to take.</summary>
        public void Repay(int amount)
        {
            if (amount <= 0 || amount > KasiasBook) throw new ArgumentOutOfRangeException(nameof(amount));
            if (amount > BankrollNow) throw new InvalidOperationException("Not enough cash.");
            BankrollNow -= amount;
            Debts -= amount;
            Changed?.Invoke();
        }

        /// <summary>
        /// Ray puts money on you with no cash changing hands: the envelope (A1.5). Own money,
        /// and so best bankroll's next rise, waits until it's paid.
        /// </summary>
        public void OweRay(int amount)
        {
            if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
            RayOwed += amount;
            Debts += amount;
            Changed?.Invoke();
        }

        /// <summary>
        /// Time passing with nothing else changing: waiting at a place, or the last of a sitting's
        /// play when you get up. No time, no change.
        /// </summary>
        public void PassTime(int minutes)
        {
            if (minutes < 0) throw new ArgumentOutOfRangeException(nameof(minutes));
            if (minutes == 0) return;
            Clock.Advance(minutes);
            ClockMoved();
            Changed?.Invoke();
        }

        /// <summary>Your room, or a night anywhere: the clock moves to the next 6:00 a.m.</summary>
        public void Sleep()
        {
            Clock.Sleep();
            ClockMoved();
            Changed?.Invoke();
        }

        /// <summary>Arriving somewhere on the map, after the trip's minutes. Staying put takes none.</summary>
        public void MoveTo(string place, int travelMinutes = 0)
        {
            if (string.IsNullOrEmpty(place)) throw new ArgumentException("A place is required.", nameof(place));
            if (travelMinutes < 0) throw new ArgumentOutOfRangeException(nameof(travelMinutes));
            if (place == Place) return;
            Place = place;
            Clock.Advance(travelMinutes);
            ClockMoved();
            Changed?.Invoke();
        }

        /// <summary>
        /// Sitting down at an illegal game: <paramref name="place"/>'s heat points count once a
        /// night, however many sittings or rounds. A venue calls it, never a game, since the
        /// same table can be legal in one place and not in another.
        /// </summary>
        public void PlayIllegalGame(string place, int points)
        {
            if (Heat.Raise(place, points, Clock.Night)) Changed?.Invoke();
        }

        /// <summary>The debug panel's switch for testing heat. Play never calls it.</summary>
        public void SetHeatPoints(int points)
        {
            if (points < 0) throw new ArgumentOutOfRangeException(nameof(points));
            Heat.Points = points;
            Heat.CoolingFrom = Clock.Night;
            Changed?.Invoke();
        }

        /// <summary>Every move of the clock lets heat cool, a step per week with no illegal play.</summary>
        void ClockMoved() => Heat.Cool(Clock.Night);

        void RaiseBestBankroll()
        {
            if (OwnMoney > BestBankroll) BestBankroll = OwnMoney;
        }
    }
}
