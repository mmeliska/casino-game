using System;
using Starflare.State;
using UnityEngine;
using UnityEngine.UI;
using static Starflare.Game.UiKit;

namespace Starflare.Game
{
    /// <summary>
    /// Kasia's Tap, the first playable venue: night one's scene (A1.2), Juan's coupons (A1.3) or
    /// Kasia's hello as you walk in, then the bar, where you can sit down at the video poker machine. Getting up is a break point, where Kasia's scene for the moment
    /// plays (her book when you're broke, settling up when you're flush). The city holds the
    /// player state and the story; the machine never sees the story.
    /// </summary>
    public sealed class KasiasTap : MonoBehaviour
    {
        City _city;
        Action _atBar;
        VideoPokerMachine _machine;

        GameObject _menu;
        Text _menuText;

        public void Init(City city, Action atBar)
        {
            _city = city;
            _atBar = atBar;
            _machine = gameObject.AddComponent<VideoPokerMachine>();
            _machine.BreakPointReached += OnBreakPoint;
            BuildMenu();
        }

        /// <summary>
        /// Walking in. Night one plays the first time. After that night, until you meet Juan,
        /// Kasia has his coupons for you (A1.3); otherwise she says hello. Then you're at the bar.
        /// </summary>
        public void Enter()
        {
            var story = _city.Story.variablesState;
            if (!(bool)story["met_kasia"]) PlayScene("night_one", _atBar);
            else if (_city.State.Clock.Day > 1 && !(bool)story["carrying_coupons"] && !(bool)story["met_juan"])
                PlayScene("coupon_job", _atBar);
            else PlayScene("walk_in", _atBar);
        }

        void PlayScene(string knot, Action then)
        {
            _menu.SetActive(false);
            _city.Story.variablesState["broke"] = _city.State.BankrollNow < _machine.Rules.CreditValue;
            _city.Story.variablesState["sitting_net"] = _machine.SittingNet;
            _city.PlayScene(knot, then);
        }

        public void SitDown()
        {
            _menu.SetActive(false);
            _machine.SitDown(_city.State);
        }

        void OnBreakPoint(BreakPoint point) => PlayScene("get_up", ShowMenu);

        void ShowMenu()
        {
            var state = _city.State;
            var clock = state.Clock;
            _menuText.text =
                $"Tonight: {_machine.SittingRounds} hands, {Money(_machine.SittingNet, true)}\n" +
                $"Cash {Money(state.BankrollNow)}" + (state.Debts > 0 ? $"   Kasia's book {Money(state.Debts)}" : "") + "\n" +
                $"Day {clock.Day}, {clock.Part} {clock.TimeLabel}";
            _menu.SetActive(true);
        }

        /// <summary>Puts the break menu away as you head somewhere else.</summary>
        public void Close() => _menu.SetActive(false);

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
            var phoneButton = (RectTransform)MakePhoneButton(safe.transform, _city.OpenMap).transform;
            phoneButton.anchorMin = phoneButton.anchorMax = phoneButton.pivot = Vector2.one;
            phoneButton.anchoredPosition = new Vector2(-36, -36);
            OnTurn(canvas, landscape => column.GetComponent<VerticalLayoutGroup>().padding =
                landscape ? new RectOffset(560, 560, 80, 80) : new RectOffset(80, 80, 300, 300));
            _menu.SetActive(false);
        }
    }
}
