using System;
using System.Collections.Generic;
using Starflare.Phone;
using Starflare.State;
using Starflare.VideoPoker;
using UnityEngine;
using UnityEngine.Events;

namespace Starflare.Game
{
    /// <summary>
    /// Kasia's Tap, the first playable venue. You arrive on the sidewalk across from the mill
    /// gate; inside, night one's scene (A1.2), one of her two jobs (A1.3), her nod toward the
    /// back door once both are done, or her hello plays as you walk in, then you're at the bar,
    /// where you can sit down at the video poker machine or, once she's let you in, go through
    /// to Rosa's table (<see cref="KasiasBackRoom"/>). On the Saturday night she's booked you for,
    /// the bar has a way up to the wedding in the hall (A1.4). Getting up from the machine is a break
    /// point, where Kasia's scene for the moment plays (her book when you're broke, settling up
    /// when you're flush), and then you're back at the bar. The machine never sees the story.
    /// </summary>
    public sealed class KasiasTap : MonoBehaviour, IPlace
    {
        public const string PlaceId = "kasias-tap";

        IPlaceHost _host;
        VideoPokerMachine _machine;
        KasiasBackRoom _backRoom;

        public void Init(IPlaceHost host)
        {
            _host = host;
            _machine = gameObject.AddComponent<VideoPokerMachine>();
            _machine.Init(Art.Plate("kasia_bar_night"), Art.Focus("kasia_bar_night"));
            _machine.BreakPointReached += OnBreakPoint;
            _backRoom = gameObject.AddComponent<KasiasBackRoom>();
            _backRoom.Init(host, ShowBar);
        }

        public string Id => PlaceId;
        public string Name => "Kasia's Tap";
        public PhoneMap.Place Pin => new PhoneMap.Place(PlaceId, Name, 0.41f, 0.30f, labelLeft: true);
        public bool IsOpen => true;

        public void Arrive() => ShowOutside();

        public void Leave() { }

        void ShowOutside()
        {
            _host.ShowPlace("kasia_front",
                ("Go inside", () => _host.LeavePlaceScreen(Enter)));
        }

        void ShowBar()
        {
            var actions = new List<(string, UnityAction)> { ("Sit down at the machine", () => _host.LeavePlaceScreen(SitDown)) };
            if (WeddingTonight) actions.Add(("Go up to the hall", () => PlayScene(StoryContract.Knots.Wedding, ShowOutside)));
            if (_host.Story.Flag(StoryContract.BackRoomOpen)) actions.Add(("Go through the back door", _backRoom.Enter));
            actions.Add(NurseABeer);
            actions.Add(("Step back outside", ShowOutside));
            _host.ShowPlace("kasia_bar", actions.ToArray());
        }

        /// <summary>The Saturday night she booked you for: the hall upstairs is open (A1.4).</summary>
        bool WeddingTonight =>
            !_host.Story.Flag(StoryContract.WeddingOver)
            && _host.Story.Int(StoryContract.WeddingDay) == _host.State.Clock.Day
            && _host.State.Clock.Part == PartOfDay.Night;

        /// <summary>The Saturday she booked you for has come and gone without you.</summary>
        bool WeddingMissed
        {
            get
            {
                int booked = _host.Story.Int(StoryContract.WeddingDay);
                return !_host.Story.Flag(StoryContract.WeddingOver) && booked > 0 && _host.State.Clock.Day > booked;
            }
        }

        /// <summary>You said no once; on a later day she asks once more.</summary>
        bool AsksAboutTheWeddingAgain =>
            !_host.Story.Flag(StoryContract.WeddingOver)
            && _host.Story.Int(StoryContract.WeddingDay) == 0
            && _host.Story.Int(StoryContract.WeddingMisses) == 1
            && _host.State.Clock.Day > _host.Story.Int(StoryContract.WeddingAskedDay);

        /// <summary>Kasia's way to pass the time: three dollars a beer.</summary>
        (string, UnityAction) NurseABeer => ("Nurse a beer", () => _host.ShowWait("kasia_bar", "Nurse a beer", BeerPrice, ShowBar));

        const int BeerPrice = 3;

        /// <summary>
        /// Walking in. Night one plays the first time. After that night, until you meet Juan,
        /// Kasia has his coupons for you; once he has them, the bar-back shift; once both jobs
        /// are done, she nods you toward the back door (A1.3) and asks you to work the wedding
        /// (A1.4). If you missed the Saturday, or said no, she has a word about it once.
        /// Otherwise she says hello. Then you're at the bar.
        /// </summary>
        void Enter()
        {
            var story = _host.Story;
            if (!story.Flag(StoryContract.MetKasia)) PlayScene(StoryContract.Knots.NightOne, ShowBar);
            else if (_host.State.Clock.Day > 1 && !story.Flag(StoryContract.CarryingCoupons) && !story.Flag(StoryContract.MetJuan))
                PlayScene(StoryContract.Knots.CouponJob, ShowBar);
            else if (story.Flag(StoryContract.MetJuan) && !story.Flag(StoryContract.DidBarBack))
                PlayScene(StoryContract.Knots.BarBackJob, ShowBar);
            else if (story.Flag(StoryContract.DidBarBack) && !story.Flag(StoryContract.BackRoomOpen))
                PlayScene(StoryContract.Knots.BackRoomInvite, ShowBar);
            else if (WeddingMissed) PlayScene(StoryContract.Knots.WeddingMissed, ShowBar);
            else if (AsksAboutTheWeddingAgain) PlayScene(StoryContract.Knots.WeddingAsk, ShowBar);
            else PlayScene(StoryContract.Knots.WalkIn, ShowBar);
        }

        void PlayScene(string knot, Action then)
        {
            _host.Story.Set(StoryContract.Broke, _host.State.BankrollNow < _machine.Rules.CreditValue);
            _host.Story.Set(StoryContract.SittingNet, _machine.SittingNet);
            _host.PlayScene(knot, then);
        }

        void SitDown() => _machine.SitDown(_host.State, _host.Pace);

        /// <summary>Getting up: Kasia's line for the moment, then the bar and what to do next.</summary>
        void OnBreakPoint(BreakPoint point) => PlayScene(StoryContract.Knots.GetUp, ShowBar);
    }
}
