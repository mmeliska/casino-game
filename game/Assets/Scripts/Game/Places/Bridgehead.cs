using System;
using System.Collections.Generic;
using System.Linq;
using Starflare.Phone;
using Starflare.Scratchers;
using Starflare.Slots;
using Starflare.State;
using Starflare.VideoPoker;
using UnityEngine;

namespace Starflare.Game
{
    /// <summary>
    /// Bridgehead Fuel &amp; Food, at the Backwater foot of the Long Bridge: worst odds, best
    /// gossip. It opens once Juan has told you about the Starflare and the wedding is behind you,
    /// and your first stop is the start of going over the bridge (A1.6), where you meet Mitch.
    /// On a night after that, the trucker short on diesel is at the pumps (S3). Inside, the 6/5
    /// poker machine, the cheap three-reel (Triple Bar) and the scratcher rack behind the
    /// counter, ten tickets from $1 to $5; getting up from any of them is a break point, where
    /// Mitch has a word. The pull tabs and Mitch's race book out back (R5) aren't built yet.
    /// </summary>
    public sealed class Bridgehead : MonoBehaviour, IPlace
    {
        public const string PlaceId = "bridgehead";

        const string Forecourt = "bridgehead_forecourt";
        const string Store = "bridgehead_store";
        const int CoffeePrice = 1;

        IPlaceHost _host;
        VideoPokerMachine _machine;
        SlotMachine _slots;
        ScratcherCounter _scratchers;

        public void Init(IPlaceHost host)
        {
            _host = host;
            _machine = gameObject.AddComponent<VideoPokerMachine>();
            string plate = Art.PlateAt(Store, host.State.Clock);
            _machine.Init(Art.Plate(plate), Art.Focus(plate), VideoPokerRules.SixFive());
            _machine.BreakPointReached += OnBreakPoint;

            _slots = gameObject.AddComponent<SlotMachine>();
            var reels = SlotRules.TripleBar();
            _slots.Init(reels, Art.Plate(plate), Art.Focus(plate), WithArt(reels, SlotLook.TripleBar()));
            _slots.BreakPointReached += _ => PlayScene(StoryContract.Knots.BridgeheadSlotsGetUp, ShowStore);

            _scratchers = gameObject.AddComponent<ScratcherCounter>();
            var roll = ScratchTicket.Bridgehead();
            _scratchers.Init(roll, Art.Plate(plate), Art.Focus(plate), WithArt(roll, TicketLook.Bridgehead()));
            _scratchers.BreakPointReached += _ => PlayScene(StoryContract.Knots.BridgeheadScratchersGetUp, ShowStore);
        }

        /// <summary>
        /// Each ticket's cards, upright and on its side (<c>Scratchers/lucky7s</c>,
        /// <c>lucky7s_wide</c>), and the pictures under its latex, where they've been drawn:
        /// its symbols (<c>Scratchers/Symbols/goldrush_tin</c>), the pictures over its numbers
        /// (<c>icefishing_perch</c>) and its backing (<c>pennyjar_penny</c>). The counter prints
        /// the rest.
        /// </summary>
        static Dictionary<string, TicketLook> WithArt(IReadOnlyList<ScratchTicket> roll, Dictionary<string, TicketLook> looks)
        {
            foreach (var ticket in roll)
            {
                if (!looks.TryGetValue(ticket.Id, out var look)) continue;
                Sprite Picture(string name) => string.IsNullOrEmpty(name) ? null : Art.Scratcher($"Symbols/{ticket.Id}_{name.ToLowerInvariant()}");
                look.Art = Art.Scratcher(ticket.Id);
                look.WideArt = Art.Scratcher($"{ticket.Id}_wide");
                if (ticket.Play == ScratchPlay.FindSymbol)
                    foreach (string symbol in ticket.Decoys.Prepend(ticket.Symbol))
                        if (Picture(symbol) is Sprite drawn) look.Symbols[symbol] = drawn;
                foreach (string name in look.MarkNames) look.Marks.Add(Picture(name));
                look.KeyMark = Picture(look.KeyMarkName);
                look.Backing = Picture(look.BackingName);
            }
            return looks;
        }

        /// <summary>Each symbol's picture, where one's been drawn (<c>triplebar_seven</c>); the machine draws the rest.</summary>
        static SlotLook WithArt(SlotRules rules, SlotLook look)
        {
            foreach (string symbol in rules.Symbols)
            {
                var art = Art.Symbol($"{rules.Id}_{symbol}");
                if (art != null) look.For(symbol).Art = art;
            }
            return look;
        }

        public string Id => PlaceId;
        public string Name => "Bridgehead Fuel & Food";
        public PhoneMap.Place Pin => new PhoneMap.Place(PlaceId, Name, 0.355f, 0.335f, labelLeft: true);

        /// <summary>After Juan's word about the Starflare (A1.3) and the wedding (A1.4).</summary>
        public bool IsOpen => _host.Story.Flag(StoryContract.MetJuan) && _host.Story.Flag(StoryContract.WeddingOver);

        /// <summary>
        /// Pulling in: the first time, the stop on the way over (A1.6), after which the phone points
        /// you on to the Flare; on a later night, once,
        /// the trucker at the pumps (S3). Otherwise the forecourt.
        /// </summary>
        public void Arrive()
        {
            var story = _host.Story;
            if (!story.Flag(StoryContract.MetMitch)) PlayScene(StoryContract.Knots.BridgeheadFirst, PointOverTheBridge);
            else if (TruckerTonight) PlayScene(StoryContract.Knots.Trucker, ShowForecourt);
            else ShowForecourt();
        }

        public void Leave() { }

        /// <summary>After the first stop the Flare is on the map, and the phone glows until you open it.</summary>
        void PointOverTheBridge()
        {
            _host.PointAtPhone(true);
            ShowStore();
        }

        /// <summary>He's at the pumps at night, on a day after your first stop, until he's been.</summary>
        bool TruckerTonight
        {
            get
            {
                var clock = _host.State.Clock;
                bool dark = clock.Part == PartOfDay.Night || clock.Part == PartOfDay.BeforeDawn;
                return dark && !_host.Story.Flag(StoryContract.TruckerPassed)
                    && clock.Day > _host.Story.Int(StoryContract.MetMitchDay);
            }
        }

        void ShowForecourt()
        {
            _host.ShowPlace(Forecourt,
                ("Go inside", ShowStore));
        }

        void ShowStore()
        {
            _host.ShowPlace(Store,
                ("Play the poker machine", () => _host.LeavePlaceScreen(SitDown)),
                ("Play the slots", () => _host.LeavePlaceScreen(SitAtSlots)),
                ("Buy scratchers", () => _host.LeavePlaceScreen(StepUpToScratchers)),
                ("Talk to Mitch", () => PlayScene(StoryContract.Knots.Mitch, ShowStore)),
                ("Nurse a coffee", () => _host.ShowWait(Store, "Nurse a coffee", CoffeePrice, ShowStore)),
                ("Step back outside", ShowForecourt));
        }

        void PlayScene(string knot, Action then)
        {
            int cheapest = Math.Min(Math.Min(_machine.Rules.CreditValue, _slots.Rules.BetFor(1)), _scratchers.CheapestPrice);
            _host.Story.Set(StoryContract.Broke, _host.State.BankrollNow < cheapest);
            _host.Story.Set(StoryContract.SittingNet, _lastNet?.Invoke() ?? 0);
            _host.PlayScene(knot, then);
        }

        /// <summary>The store's plate for now, day or night, behind the machine.</summary>
        void SitDown()
        {
            string plate = Art.PlateAt(Store, _host.State.Clock);
            _machine.SetBackdrop(Art.Plate(plate), Art.Focus(plate));
            _machine.SitDown(_host.State, _host.Pace);
            _lastNet = () => _machine.SittingNet;
        }

        void SitAtSlots()
        {
            string plate = Art.PlateAt(Store, _host.State.Clock);
            _slots.SetBackdrop(Art.Plate(plate), Art.Focus(plate));
            _slots.SitDown(_host.State, _host.Pace);
            _lastNet = () => _slots.SittingNet;
        }

        void StepUpToScratchers()
        {
            string plate = Art.PlateAt(Store, _host.State.Clock);
            _scratchers.SetBackdrop(Art.Plate(plate), Art.Focus(plate));
            _scratchers.SitDown(_host.State, _host.Pace);
            _lastNet = () => _scratchers.SittingNet;
        }

        /// <summary>How the game you last got up from went, which Mitch's word is about.</summary>
        Func<int> _lastNet;

        /// <summary>Getting up: Mitch's word, then back in the store.</summary>
        void OnBreakPoint(BreakPoint point) => PlayScene(StoryContract.Knots.BridgeheadGetUp, ShowStore);
    }
}
