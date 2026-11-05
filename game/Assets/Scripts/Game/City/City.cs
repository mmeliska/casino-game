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
    /// </summary>
    public sealed class City : MonoBehaviour, IPlaceHost
    {
        /// <summary>Where the save files go; null means the device's persistent data path. Tests set it.</summary>
        public static string SaveFolder;

        public PlayerState State { get; private set; }
        public StoryBridge Story { get; private set; }

        SaveStore _saves;

        /// <summary>Every place built so far; its pins are the phone map's.</summary>
        IPlace[] _places;
        IPlace _current;

        StoryPlayer _player;
        RegisterCard _register;
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
            _saves = new SaveStore(SaveFolder ?? Application.persistentDataPath);
            State = _saves.Load();
            // The city is the one owner of the save: every settled change writes it.
            State.Changed += SaveAll;
            Story = StoryBridge.Load(_saves.LoadStory(), State);

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

        public void PlayScene(string knot, Action then)
        {
            _placeScreen.SetActive(false);
            _hud.Show(Find(State.Place).Name, buttons: false);
            Story.Sync(State);
            _player.Play(knot, () =>
            {
                _hud.Hide();
                SaveAll();
                then();
            });
        }

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
            _map.Show(open, State.Place, id =>
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

        void GoTo(string id)
        {
            _current?.Leave();
            Art.ForgetLooks();
            State.MoveTo(id);
            _current = Find(id);
            _current.Arrive();
        }

        public void ShowPlace(string view, params (string label, UnityAction action)[] actions)
        {
            Look(false);
            _view = view;
            string plate = Art.PlateAt(view, State.Clock);
            _plateFit.Show(_plate, Art.Plate(plate), Art.Focus(plate));
            ClearChildren(_actions);
            foreach (var (label, action) in actions) MakeChoiceButton(_actions, label, action);
            _hud.Show(_current.Name, buttons: true);
            _placeScreen.SetActive(true);
        }

        public void LeavePlaceScreen()
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
