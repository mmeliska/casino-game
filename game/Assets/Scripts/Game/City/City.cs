using System;
using System.Collections.Generic;
using Starflare.State;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Starflare.Game.UiKit;

namespace Starflare.Game
{
    /// <summary>
    /// The city layer of the core loop: it owns the player state and the one Ink story (through
    /// <see cref="StoryBridge"/>), saves both on every settled change, plays the arrival at the
    /// Slack Water (A1.1), then moves you between the places. Each place owns its own screens
    /// and scene triggers (<see cref="IPlace"/>) and sees the city as an <see cref="IPlaceHost"/>.
    /// Every change of screen goes through the <see cref="ScreenFader"/>, so nothing cuts.
    /// Editor and Development builds also get the debug panel (<c>City.Debug.cs</c>).
    /// </summary>
    public sealed partial class City : MonoBehaviour, IPlaceHost
    {
        /// <summary>Where the save files go; null means the device's persistent data path. Tests set it.</summary>
        public static string SaveFolder;

        /// <summary>
        /// A trip on the map. Every place built so far is in Backwater, a trip across one district
        /// (starting value, Mike 2026-11-05); the districts get their own once the Levee is built.
        /// </summary>
        const int TravelMinutes = 30;

        /// <summary>After a trip, the place's plate shows on its own this long before its actions fade in.</summary>
        const float ArrivalHoldSeconds = 0.6f;
        const float ActionsFadeSeconds = 0.2f;

        public PlayerState State { get; private set; }
        public StoryBridge Story { get; private set; }
        public ClockPace Pace { get; } = new ClockPace();

        SaveStore _saves;

        /// <summary>Every place built so far; its pins are the phone map's.</summary>
        IPlace[] _places;
        IPlace _current;

        ScreenFader _fader;
        StoryPlayer _player;
        RegisterCard _register;
        PhoneMap _map;
        Hud _hud;

        GameObject _placeScreen;
        Image _plate;
        CoverFit _plateFit;
        LookAround _look;
        RectTransform _actions;
        Reveal _actionsReveal;
        string _view, _shownPlate;
        bool _arriving;

        void Awake()
        {
            _saves = new SaveStore(SaveFolder ?? Application.persistentDataPath);
            State = _saves.Load();
            // The city is the one owner of the save: every settled change writes it.
            State.Changed += SaveAll;
            Story = StoryBridge.Load(_saves.LoadStory(), State);

            _fader = gameObject.AddComponent<ScreenFader>();
            _player = gameObject.AddComponent<StoryPlayer>();
            _player.Init(Story.Ink);
            _register = gameObject.AddComponent<RegisterCard>();
            _map = gameObject.AddComponent<PhoneMap>();
            _hud = gameObject.AddComponent<Hud>();
            _hud.Init(State, OpenMap, () => Look(true), () => Look(false));
            BuildPlaceScreen();

            var tap = gameObject.AddComponent<KasiasTap>();
            tap.Init(this);
            _places = new IPlace[] { new SlackWaterMotel(this), tap, new MillstoneLanes(this) };
            AddDebugPanel();
        }

        /// <summary>The debug panel, in Editor and Development builds only (<c>City.Debug.cs</c>).</summary>
        partial void AddDebugPanel();

        void Start()
        {
            if (!Story.Flag(StoryContract.CheckedIn)) CheckIn();
            else GoTo(State.Place);
        }

        void OnApplicationPause(bool paused)
        {
            if (paused) SaveAll();
        }

        void OnApplicationQuit() => SaveAll();

        void OnDestroy() => State.Changed -= SaveAll;

        /// <summary>A quick dip into the scene; at its end, the last panel stays up for the dip back out.</summary>
        public void PlayScene(string knot, Action then) => _fader.Cover(() =>
        {
            _placeScreen.SetActive(false);
            _hud.Show(Find(State.Place).Name, buttons: false);
            Story.Sync(State);
            _player.Play(knot, () =>
            {
                SaveAll();
                _fader.Cover(() =>
                {
                    _player.Clear();
                    _hud.Hide();
                    then();
                });
            });
        });

        public void PlaySceneOver(string knot, ICoachable game, Action then)
        {
            Story.Sync(State);
            _player.PlayOver(knot, game, () =>
            {
                SaveAll();
                then();
            });
        }

        /// <summary>Arrival, the register, then Dot's question. Picks up after the register if you'd signed it.</summary>
        void CheckIn()
        {
            if (State.HasSigned) PlayScene(StoryContract.Knots.CheckingIn, () => GoTo(SlackWaterMotel.PlaceId));
            else PlayScene(StoryContract.Knots.Arrival, () => _register.Show(SignRegister));
        }

        void SignRegister(string name, RegisterBox box)
        {
            State.SignRegister(name, box);
            PlayScene(StoryContract.Knots.CheckingIn, () => GoTo(SlackWaterMotel.PlaceId));
        }

        public void OpenMap()
        {
            var open = new List<PhoneMap.Place>();
            foreach (var place in _places)
                if (place.IsOpen) open.Add(place.Pin);
            _map.Show(open, State.Place, State.Clock, TravelMinutes, id =>
            {
                if (id != State.Place) GoTo(id);
            });
        }

        IPlace Find(string id)
        {
            var place = Array.Find(_places, p => p.Id == id);
            if (place == null) throw new InvalidOperationException($"No place is built for '{id}'.");
            return place;
        }

        /// <summary>The slow dip: the phone goes away in the dark and you arrive at the new time.</summary>
        void GoTo(string id) => _fader.Cover(() =>
        {
            _map.Hide();
            _current?.Leave();
            Art.ForgetLooks();
            State.MoveTo(id, TravelMinutes);
            _current = Find(id);
            _arriving = true;
            try { _current.Arrive(); }
            finally { _arriving = false; }
        }, away: true);

        /// <summary>
        /// Same plate, the actions just change; a new plate (another view, or the same one at a
        /// new time of day) dips to black first.
        /// </summary>
        public void ShowPlace(string view, params (string label, UnityAction action)[] actions)
        {
            string plate = Art.PlateAt(view, State.Clock);
            if (_placeScreen.activeSelf && plate == _shownPlate) ShowPlace(view, plate, actions);
            else _fader.Cover(() => ShowPlace(view, plate, actions));
        }

        void ShowPlace(string view, string plate, (string label, UnityAction action)[] actions)
        {
            Look(false);
            _view = view;
            _shownPlate = plate;
            _plateFit.Show(_plate, Art.Plate(plate), Art.Focus(plate));
            ClearChildren(_actions);
            foreach (var (label, action) in actions) MakeChoiceButton(_actions, label, action);
            _actionsReveal.Play(ActionsFadeSeconds, _arriving ? ArrivalHoldSeconds : 0f);
            _hud.Show(_current.Name, buttons: true);
            _placeScreen.SetActive(true);
        }

        public void ShowWait(string view, string how, int price, Action then)
        {
            var clock = State.Clock;
            string next = clock.NextPart switch
            {
                PartOfDay.Morning => "morning",
                PartOfDay.Afternoon => "the afternoon",
                PartOfDay.Night => "night",
                _ => "after midnight",
            };
            string cost = price > 0 ? $" ({Money(price)})" : "";
            if (State.BankrollNow < price)
            {
                ShowPlace(view, ($"{how}{cost}: not enough cash", () => then()));
                return;
            }
            ShowPlace(view,
                ($"{how}{cost}, an hour", () => Wait(price, 60, then)),
                ($"{how}{cost}, until {next}", () => Wait(price, clock.MinutesToNextPart, then)),
                ("Never mind", () => then()));
        }

        void Wait(int price, int minutes, Action then)
        {
            if (price > 0) State.Spend(price);
            State.PassTime(minutes);
            then();
        }

        public void LeavePlaceScreen(Action then) => _fader.Cover(() =>
        {
            _placeScreen.SetActive(false);
            _hud.Hide();
            then();
        });

        /// <summary>
        /// Looking around: the top bar and actions step aside so you can pan the plate. When you
        /// close it, the view keeps where you panned to, in scenes too (Art.KeepLook).
        /// </summary>
        void Look(bool on)
        {
            if (_look.On && !on) Art.KeepLook(_view, _plateFit.KeepLook());
            _look.On = on;
            _actions.gameObject.SetActive(!on);
            _hud.Looking(on);
        }

        public void StartOver()
        {
            State.Changed -= SaveAll;
            _saves.Wipe();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        /// <summary>Both files together, so the story's flags and the cash they refer to agree.</summary>
        void SaveAll()
        {
            if (Story == null) return;
            _saves.Save(State);
            _saves.SaveStory(Story.SaveJson());
        }

        void BuildPlaceScreen()
        {
            var canvas = MakeCanvas(transform, "Place Canvas", 1);
            _placeScreen = canvas.gameObject;

            var backdrop = MakeImage(canvas.transform, "Backdrop", Color.black);
            Stretch(backdrop.rectTransform);
            _plate = MakeImage(backdrop.transform, "Plate", Color.white);
            _plate.raycastTarget = false;
            _plateFit = _plate.gameObject.AddComponent<CoverFit>();
            _look = backdrop.gameObject.AddComponent<LookAround>();
            _look.Init(_plateFit);

            var safe = new GameObject("Safe Area", typeof(RectTransform), typeof(SafeAreaFitter));
            safe.transform.SetParent(canvas.transform, false);
            Stretch((RectTransform)safe.transform);

            var actionsGo = new GameObject("Actions", typeof(RectTransform));
            actionsGo.transform.SetParent(safe.transform, false);
            _actions = AddVertical(actionsGo, 20, new RectOffset(60, 60, 0, 0));
            _actionsReveal = actionsGo.AddComponent<Reveal>();
            _actions.GetComponent<VerticalLayoutGroup>().childAlignment = TextAnchor.LowerCenter;

            // The top bar (Hud) is shared with the scenes. Under it, portrait puts the actions
            // along the bottom; landscape puts them down the right half, so the plate shows beside.
            OnTurn(canvas, landscape =>
            {
                Look(false);
                if (landscape)
                    Pin(_actions, new Vector2(0.5f, 0), Vector2.one, new Vector2(0, 40), new Vector2(-16, -180));
                else
                    Pin(_actions, Vector2.zero, Vector2.right, new Vector2(0, 40), new Vector2(0, 780));
            });
            _placeScreen.SetActive(false);
        }
    }
}
