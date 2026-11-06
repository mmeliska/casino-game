using System;
using Starflare.Phone;
using Starflare.State;
using Starflare.Ui;
using Starflare.VideoPoker;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static Starflare.Ui.UiKit;

namespace Starflare.Game
{
    /// <summary>
    /// Kasia's Tap, the first playable venue. You arrive on the sidewalk across from the mill
    /// gate; inside, night one's scene (A1.2), one of her two jobs (A1.3), her nod toward the
    /// back door once both are done, or her hello plays as you walk in, then you're at the bar,
    /// where you can sit down at the video poker machine or, once she's let you in, go through
    /// to Rosa's table (<see cref="KasiasBackRoom"/>). Getting up from the machine is a break
    /// point, where Kasia's scene for the moment plays (her book when you're broke, settling up
    /// when you're flush). The machine never sees the story.
    /// </summary>
    public sealed class KasiasTap : MonoBehaviour, IPlace
    {
        public const string PlaceId = "kasias-tap";

        IPlaceHost _host;
        VideoPokerMachine _machine;
        KasiasBackRoom _backRoom;

        GameObject _menu;
        TMP_Text _menuText;

        public void Init(IPlaceHost host)
        {
            _host = host;
            _machine = gameObject.AddComponent<VideoPokerMachine>();
            _machine.Init(Art.Plate("kasia_bar_night"), Art.Focus("kasia_bar_night"));
            _machine.BreakPointReached += OnBreakPoint;
            _backRoom = gameObject.AddComponent<KasiasBackRoom>();
            _backRoom.Init(host, ShowBar);
            BuildMenu();
        }

        public string Id => PlaceId;
        public string Name => "Kasia's Tap";
        public PhoneMap.Place Pin => new PhoneMap.Place(PlaceId, Name, 0.41f, 0.30f, labelLeft: true);
        public bool IsOpen => true;

        public void Arrive() => ShowOutside();

        /// <summary>Puts the break menu away as you head somewhere else.</summary>
        public void Leave() => _menu.SetActive(false);

        void ShowOutside()
        {
            _host.ShowPlace("kasia_front",
                ("Go inside", () => _host.LeavePlaceScreen(Enter)));
        }

        void ShowBar()
        {
            (string, UnityAction) sitDown = ("Sit down at the machine", () => _host.LeavePlaceScreen(SitDown));
            if (_host.Story.Flag(StoryContract.BackRoomOpen))
                _host.ShowPlace("kasia_bar", sitDown, ("Go through the back door", _backRoom.Enter), NurseABeer, ("Step back outside", ShowOutside));
            else
                _host.ShowPlace("kasia_bar", sitDown, NurseABeer, ("Step back outside", ShowOutside));
        }

        /// <summary>Kasia's way to pass the time: three dollars a beer.</summary>
        (string, UnityAction) NurseABeer => ("Nurse a beer", () => _host.ShowWait("kasia_bar", "Nurse a beer", BeerPrice, ShowBar));

        const int BeerPrice = 3;

        /// <summary>
        /// Walking in. Night one plays the first time. After that night, until you meet Juan,
        /// Kasia has his coupons for you; once he has them, the bar-back shift; once both jobs
        /// are done, she nods you toward the back door (A1.3). Otherwise she says hello. Then
        /// you're at the bar.
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
            else PlayScene(StoryContract.Knots.WalkIn, ShowBar);
        }

        /// <summary>The break menu stays up until the dip to black covers it.</summary>
        void PlayScene(string knot, Action then)
        {
            _host.Story.Set(StoryContract.Broke, _host.State.BankrollNow < _machine.Rules.CreditValue);
            _host.Story.Set(StoryContract.SittingNet, _machine.SittingNet);
            _host.PlayScene(knot, () =>
            {
                _menu.SetActive(false);
                then();
            });
        }

        void SitDown()
        {
            _menu.SetActive(false);
            _machine.SitDown(_host.State, _host.Pace);
        }

        void OnBreakPoint(BreakPoint point) => PlayScene(StoryContract.Knots.GetUp, ShowMenu);

        void ShowMenu()
        {
            var state = _host.State;
            var clock = state.Clock;
            _menuText.text =
                $"Tonight: {_machine.SittingRounds} hands, {Money(_machine.SittingNet, true)}\n" +
                $"Cash {Money(state.BankrollNow)}" + (state.Debts > 0 ? $"   Kasia's book {Money(state.Debts)}" : "") + "\n" +
                $"Day {clock.Day}, {clock.Part} {clock.TimeLabel}";
            _menu.SetActive(true);
        }

        void BuildMenu()
        {
            var canvas = MakeCanvas(transform, "Break Canvas", 5);
            _menu = canvas.gameObject;

            var shade = MakeImage(canvas.transform, "Shade", new Color(0f, 0f, 0f, 0.85f));
            Stretch(shade.rectTransform);
            var column = AddVertical(shade.gameObject, 24, new RectOffset(80, 80, 300, 300));
            column.GetComponent<VerticalLayoutGroup>().childAlignment = TextAnchor.MiddleCenter;

            _menuText = MakeText(column, "", 44, TextAnchor.MiddleCenter, 400);
            MakeButton(column, "Back to the machine", SitDown);

            var safe = new GameObject("Safe Area", typeof(RectTransform), typeof(SafeAreaFitter));
            safe.transform.SetParent(canvas.transform, false);
            Stretch((RectTransform)safe.transform);
            var phoneButton = (RectTransform)MakePhoneButton(safe.transform, _host.OpenMap).transform;
            phoneButton.anchorMin = phoneButton.anchorMax = phoneButton.pivot = Vector2.one;
            phoneButton.anchoredPosition = new Vector2(-36, -36);
            OnTurn(canvas, landscape => column.GetComponent<VerticalLayoutGroup>().padding =
                landscape ? new RectOffset(560, 560, 80, 80) : new RectOffset(80, 80, 300, 300));
            _menu.SetActive(false);
        }
    }
}
