using System;
using Starflare.Phone;
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
    /// poker machine; getting up is a break point, where Mitch has a word. The slots, pull tabs,
    /// scratchers and Mitch's race book out back (R5) aren't built yet.
    /// </summary>
    public sealed class Bridgehead : MonoBehaviour, IPlace
    {
        public const string PlaceId = "bridgehead";

        const string Forecourt = "bridgehead_forecourt";
        const string Store = "bridgehead_store";
        const int CoffeePrice = 1;

        IPlaceHost _host;
        VideoPokerMachine _machine;

        public void Init(IPlaceHost host)
        {
            _host = host;
            _machine = gameObject.AddComponent<VideoPokerMachine>();
            string plate = Art.PlateAt(Store, host.State.Clock);
            _machine.Init(Art.Plate(plate), Art.Focus(plate), VideoPokerRules.SixFive());
            _machine.BreakPointReached += OnBreakPoint;
        }

        public string Id => PlaceId;
        public string Name => "Bridgehead Fuel & Food";
        public PhoneMap.Place Pin => new PhoneMap.Place(PlaceId, Name, 0.355f, 0.335f, labelLeft: true);

        /// <summary>After Juan's word about the Starflare (A1.3) and the wedding (A1.4).</summary>
        public bool IsOpen => _host.Story.Flag(StoryContract.MetJuan) && _host.Story.Flag(StoryContract.WeddingOver);

        /// <summary>
        /// Pulling in: the first time, the stop on the way over (A1.6); on a later night, once,
        /// the trucker at the pumps (S3). Otherwise the forecourt.
        /// </summary>
        public void Arrive()
        {
            var story = _host.Story;
            if (!story.Flag(StoryContract.MetMitch)) PlayScene(StoryContract.Knots.BridgeheadFirst, ShowStore);
            else if (TruckerTonight) PlayScene(StoryContract.Knots.Trucker, ShowForecourt);
            else ShowForecourt();
        }

        public void Leave() { }

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
                ("Talk to Mitch", () => PlayScene(StoryContract.Knots.Mitch, ShowStore)),
                ("Nurse a coffee", () => _host.ShowWait(Store, "Nurse a coffee", CoffeePrice, ShowStore)),
                ("Step back outside", ShowForecourt));
        }

        void PlayScene(string knot, Action then)
        {
            _host.Story.Set(StoryContract.Broke, _host.State.BankrollNow < _machine.Rules.CreditValue);
            _host.Story.Set(StoryContract.SittingNet, _machine.SittingNet);
            _host.PlayScene(knot, then);
        }

        /// <summary>The store's plate for now, day or night, behind the machine.</summary>
        void SitDown()
        {
            string plate = Art.PlateAt(Store, _host.State.Clock);
            _machine.SetBackdrop(Art.Plate(plate), Art.Focus(plate));
            _machine.SitDown(_host.State, _host.Pace);
        }

        /// <summary>Getting up: Mitch's word, then back in the store.</summary>
        void OnBreakPoint(BreakPoint point) => PlayScene(StoryContract.Knots.BridgeheadGetUp, ShowStore);
    }
}
