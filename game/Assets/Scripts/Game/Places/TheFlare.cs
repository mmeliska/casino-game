using System;
using System.Collections.Generic;
using Starflare.Blackjack;
using Starflare.Phone;
using Starflare.State;
using Starflare.VideoPoker;
using UnityEngine;
using UnityEngine.Events;

namespace Starflare.Game
{
    /// <summary>
    /// The Starflare Club, "the Flare": Gus's fading rocket-age casino on the Levee waterfront.
    /// It opens once you've stopped at Bridgehead on the way over (A1.6), and your first arrival
    /// is the Flare half of A1.6: coffee with Gus under the rocket sign and the spaceman's boot.
    /// Becoming a regular (A1.7) plays a beat a visit, each on a later day than the one before:
    /// Pete at the pit (the first time you get up from blackjack, or at the door), Jalen at the
    /// bar, then the decline and Gus's secret, the notice on his desk, with Gus asking after Dot
    /// (R1). After a few sittings Pete says your name. While you hold Raffy's envelope and Ray's
    /// asking (A1.5), he's at the bar to hand it back to; if you lied to Raffy, Ray comes to you
    /// here a few days later. The floor has blackjack at the Flare's
    /// limits and a full-pay poker machine by the bar; the felt craps table, roulette, rocket
    /// slots, keno and the Rocket Wheel aren't built yet.
    /// </summary>
    public sealed class TheFlare : MonoBehaviour, IPlace
    {
        public const string PlaceId = "the-flare";

        /// <summary>Blackjack sittings before Pete says your name: "a few sittings (tuned)" (pete.md).</summary>
        public const int SittingsForYourName = 3;

        const string Front = "flare_front";
        const string Floor = "flare_floor";
        const string FloorDecline = "flare_floor_night_decline";
        const string Bar = "flare_bar";

        /// <summary>The Flare's table: $5 to $100 (the <see cref="BlackjackRules"/> defaults are the Flare's).</summary>
        static readonly int[] FlareChips = { 5, 25, 100 };

        IPlaceHost _host;
        BlackjackTable _table;
        VideoPokerMachine _machine;

        /// <summary>How the game you last got up from went, which the word after it is about.</summary>
        Func<int> _lastNet = () => 0;

        /// <summary>The least that game takes, so "broke" means broke for it ($5 at the table, $1 at the machine).</summary>
        int _lastCheapest = 1;

        public void Init(IPlaceHost host)
        {
            _host = host;
            _table = gameObject.AddComponent<BlackjackTable>();
            _table.Init(new BlackjackRules(), FlareChips);
            _table.BreakPointReached += OnTableBreakPoint;

            _machine = gameObject.AddComponent<VideoPokerMachine>();
            string plate = Art.PlateAt(Bar, host.State.Clock);
            _machine.Init(Art.Plate(plate), Art.Focus(plate), new VideoPokerRules());
            _machine.BreakPointReached += _ => PlayScene(StoryContract.Knots.FlareMachineGetUp, ShowFloor);
        }

        public string Id => PlaceId;
        public string Name => "The Starflare";
        public PhoneMap.Place Pin => new PhoneMap.Place(PlaceId, Name, 0.55f, 0.37f);

        /// <summary>Over the bridge: once you've made the stop at Bridgehead (A1.6).</summary>
        public bool IsOpen => _host.Story.Flag(StoryContract.MetMitch);

        public void Arrive()
        {
            var story = _host.Story;
            int today = _host.State.Clock.Day;
            bool newVisit = story.Int(StoryContract.FlareVisits) == 0 || story.Int(StoryContract.FlareLastDay) != today;
            if (newVisit)
            {
                story.Set(StoryContract.FlareVisits, story.Int(StoryContract.FlareVisits) + 1);
                story.Set(StoryContract.FlareLastDay, today);
            }
            string knot = RaffysEnvelope.RayNamesYouToday(story, today)
                ? StoryContract.Knots.RayNamesYou
                : ArrivalKnot(story, today, newVisit);
            if (knot == null) ShowFront();
            else PlayScene(knot, ShowFloor);
        }

        public void Leave() { }

        /// <summary>
        /// The scene arriving plays, or null for the front: the first time, Gus (A1.6); then on
        /// each new visit, a day after the last beat, the next of A1.7's: Pete (if you haven't
        /// sat down yet), Jalen, then Gus's office and the notice.
        /// </summary>
        public static string ArrivalKnot(StoryBridge story, int today, bool newVisit)
        {
            if (!story.Flag(StoryContract.MetGus)) return StoryContract.Knots.FlareFirst;
            if (!newVisit) return null;
            if (!story.Flag(StoryContract.MetPete)) return StoryContract.Knots.PeteFirst;
            if (!story.Flag(StoryContract.MetJalen))
                return today > story.Int(StoryContract.MetPeteDay) ? StoryContract.Knots.JalenFirst : null;
            if (!story.Flag(StoryContract.SawNotice))
                return today > story.Int(StoryContract.MetJalenDay) ? StoryContract.Knots.FlareDecline : null;
            return null;
        }

        /// <summary>
        /// The word at the pit when you get up from blackjack: meeting Pete the first time; after
        /// the notice, on a later day and a few sittings in, your name; otherwise his word.
        /// </summary>
        public static string GetUpKnot(StoryBridge story, int today)
        {
            if (!story.Flag(StoryContract.MetPete)) return StoryContract.Knots.PeteFirst;
            bool nameDue = story.Flag(StoryContract.SawNotice) && !story.Flag(StoryContract.PeteSaidName)
                && today > story.Int(StoryContract.SawNoticeDay)
                && story.Int(StoryContract.FlareSittings) >= SittingsForYourName;
            return nameDue ? StoryContract.Knots.PeteName : StoryContract.Knots.FlareGetUp;
        }

        void ShowFront()
        {
            _host.ShowPlace(Front,
                ("Go inside", ShowFloor),
                ("Rub the spaceman's boot", () => PlayScene(StoryContract.Knots.Spaceman, ShowFront)));
        }

        /// <summary>The floor, lit in act 1 until you've seen the notice; then its decline plate (A1.7).</summary>
        void ShowFloor()
        {
            string view = _host.Story.Flag(StoryContract.SawNotice) ? FloorDecline : Floor;
            var actions = new List<(string, UnityAction)>
            {
                ("Sit down at blackjack", () => _host.LeavePlaceScreen(SitDown)),
                ("Play the poker machines", () => _host.LeavePlaceScreen(SitAtMachine)),
            };
            if (RaffysEnvelope.RayAtTheBar(_host.Story))
                actions.Add(("Find Ray at the bar", () => PlayScene(StoryContract.Knots.RayReturn, ShowFloor)));
            actions.Add(("Look in on the lounge", () => PlayScene(StoryContract.Knots.FlareLounge, ShowFloor)));
            actions.Add(("Nurse a coffee", () => _host.ShowWait(view, "Nurse a coffee", 0, ShowFloor)));
            actions.Add(("Step outside", ShowFront));
            _host.ShowPlace(view, actions.ToArray());
        }

        void SitDown()
        {
            _table.SitDown(_host.State, _host.Pace);
            _lastNet = () => _table.SittingNet;
            _lastCheapest = _table.Rules.MinBet;
        }

        /// <summary>The bar's plate for now behind the machine.</summary>
        void SitAtMachine()
        {
            string plate = Art.PlateAt(Bar, _host.State.Clock);
            _machine.SetBackdrop(Art.Plate(plate), Art.Focus(plate));
            _machine.SitDown(_host.State, _host.Pace);
            _lastNet = () => _machine.SittingNet;
            _lastCheapest = _machine.Rules.CreditValue;
        }

        /// <summary>Getting up from blackjack counts a sitting toward Pete's name scene, then his word.</summary>
        void OnTableBreakPoint(BreakPoint point)
        {
            var story = _host.Story;
            story.Set(StoryContract.FlareSittings, story.Int(StoryContract.FlareSittings) + 1);
            PlayScene(GetUpKnot(story, _host.State.Clock.Day), ShowFloor);
        }

        void PlayScene(string knot, Action then)
        {
            _host.Story.Set(StoryContract.Broke, _host.State.BankrollNow < _lastCheapest);
            _host.Story.Set(StoryContract.SittingNet, _lastNet());
            _lastNet = () => 0;
            _host.PlayScene(knot, then);
        }
    }
}
