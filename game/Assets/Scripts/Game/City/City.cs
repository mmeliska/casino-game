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
    /// Slack Water (A1.1), then lets you move between places. A venue gets the state and story
    /// through it and hands control back when you walk out.
    /// </summary>
    public sealed class City : MonoBehaviour
    {
        const string Motel = "slack-water";
        const string Tap = "kasias-tap";
        const string Lanes = "millstone-lanes";

        /// <summary>Every place built so far, with its map pin (from the pin table on the Northlight page).</summary>
        static readonly PhoneMap.Place[] Places =
        {
            new PhoneMap.Place(Motel, "Slack Water Motel", 0.45f, 0.32f),
            new PhoneMap.Place(Tap, "Kasia's Tap", 0.41f, 0.30f, labelLeft: true),
            new PhoneMap.Place(Lanes, "Millstone Lanes", 0.22f, 0.44f),
        };

        public PlayerState State { get; private set; }
        public StoryBridge Story { get; private set; }

        SaveStore _saves;

        StoryPlayer _player;
        RegisterCard _register;
        KasiasTap _tap;
        PhoneMap _map;
        Hud _hud;

        GameObject _placeScreen;
        Image _plate;
        CoverFit _plateFit;
        LookAround _look;
        RectTransform _actions;
        string _view;

        void Awake()
        {
            _saves = new SaveStore(Application.persistentDataPath);
            State = _saves.Load();
            // The city is the one owner of the save: every settled change writes it.
            State.Changed += SaveAll;
            Story = StoryBridge.Load(_saves.LoadStory(), State);

            _player = gameObject.AddComponent<StoryPlayer>();
            _player.Init(Story.Ink);
            _register = gameObject.AddComponent<RegisterCard>();
            _tap = gameObject.AddComponent<KasiasTap>();
            _tap.Init(this, ShowTap);
            _map = gameObject.AddComponent<PhoneMap>();
            _hud = gameObject.AddComponent<Hud>();
            _hud.Init(State, OpenMap, () => Look(true), () => Look(false));
            BuildPlaceScreen();
        }

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

        /// <summary>Plays one knot with the shared variables up to date, saves, then calls back.</summary>
        public void PlayScene(string knot, Action then)
        {
            _placeScreen.SetActive(false);
            _hud.Show(PlaceName(State.Place), buttons: false);
            Story.Sync(State);
            _player.Play(knot, () =>
            {
                _hud.Hide();
                SaveAll();
                then();
            });
        }

        /// <summary>Arrival, the register, then Dot's question. Picks up after the register if you'd signed it.</summary>
        void CheckIn()
        {
            if (State.HasSigned) PlayScene(StoryContract.Knots.CheckingIn, () => GoTo(Motel));
            else PlayScene(StoryContract.Knots.Arrival, () => _register.Show(SignRegister));
        }

        void SignRegister(string name, RegisterBox box)
        {
            State.SignRegister(name, box);
            PlayScene(StoryContract.Knots.CheckingIn, () => GoTo(Motel));
        }

        /// <summary>Opens the map on your phone with the places open to you; tap one to go there.</summary>
        public void OpenMap()
        {
            var open = new List<PhoneMap.Place>();
            foreach (var place in Places)
                if (IsOpen(place.Id)) open.Add(place);
            _map.Show(open, State.Place, place =>
            {
                if (place != State.Place) GoTo(place);
            });
        }

        static string PlaceName(string id) => Array.Find(Places, place => place.Id == id).Name ?? "";

        bool IsOpen(string place) =>
            // The Lanes opens once Kasia's given you the box to carry.
            place != Lanes || Story.Flag(StoryContract.CarryingCoupons) || Story.Flag(StoryContract.MetJuan);

        void GoTo(string place)
        {
            _tap.Close();
            Art.ForgetLooks();
            State.MoveTo(place);
            if (place == Tap) ShowTapOutside();
            else if (place == Lanes) ShowLanesOutside();
            else ShowMotel();
        }

        void ShowMotel()
        {
            ShowPlace("Slack Water Motel", "motel_lot",
                ("Step into the office", () => PlayScene(StoryContract.Knots.Office, ShowMotel)),
                ("Go to your room", () => PlayScene(StoryContract.Knots.TurnIn, ShowMotel)),
                ("Start over (wipe save)", StartOver));
        }

        /// <summary>You arrive at the Tap on the sidewalk, across from the mill gate.</summary>
        void ShowTapOutside()
        {
            ShowPlace("Kasia's Tap", "kasia_front",
                ("Go inside", () =>
                {
                    LeavePlaceScreen();
                    _tap.Enter();
                }));
        }

        void ShowTap()
        {
            ShowPlace("Kasia's Tap", "kasia_bar",
                ("Sit down at the machine", () =>
                {
                    LeavePlaceScreen();
                    _tap.SitDown();
                }),
                ("Step back outside", ShowTapOutside));
        }

        /// <summary>You arrive at the Lanes in its lot, under the sign.</summary>
        void ShowLanesOutside()
        {
            ShowPlace("Millstone Lanes", "lanes_front",
                ("Go inside", () =>
                {
                    // Kasia's coupons are how you first walk in (A1.3).
                    if (!Story.Flag(StoryContract.MetJuan)) PlayScene(StoryContract.Knots.Coupons, ShowLanes);
                    else ShowLanes();
                }));
        }

        void ShowLanes()
        {
            ShowPlace("Millstone Lanes", "lanes_lanes",
                ("Find Juan at the counter", () => PlayScene(StoryContract.Knots.Juan, ShowLanes)),
                ("Step back outside", ShowLanesOutside));
        }

        /// <summary>Shows a place screen over the view's plate for the time of day and season.</summary>
        void ShowPlace(string name, string view, params (string label, UnityAction action)[] actions)
        {
            Look(false);
            _view = view;
            string plate = Art.PlateAt(view, State.Clock);
            _plateFit.Show(_plate, Art.Plate(plate), Art.Focus(plate));
            ClearChildren(_actions);
            foreach (var (label, action) in actions) MakeChoiceButton(_actions, label, action);
            _hud.Show(name, buttons: true);
            _placeScreen.SetActive(true);
        }

        void LeavePlaceScreen()
        {
            _placeScreen.SetActive(false);
            _hud.Hide();
        }

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

        void StartOver()
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
