using System;
using Starflare.State;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static Starflare.Game.UiKit;

namespace Starflare.Game
{
    /// <summary>
    /// The debug panel, for setting the game to the moment you want to test. A small DEV button
    /// on the left edge opens it over any screen. It loads a checkpoint (a beat's recipe) or a
    /// snapshot (a copy of a real save), and edits the live state through the same methods play
    /// uses: cash, Kasia's book, the clock, the hidden scales, every story flag, and where you
    /// stand. It also sets the clock's pace (story minutes per real minute, hours a sitting),
    /// which this device remembers, so a pace being tried survives reloads and relaunches. Flags and places change what a screen offers, so closing after either boots the
    /// city again from the save. The city adds it in Editor and Development builds only.
    /// </summary>
    public sealed class DebugPanel : MonoBehaviour
    {
        const float Row = 88;

        const string RateKey = "debug.pace.rate";
        const string CapKey = "debug.pace.cap";
        static readonly int[] Rates = { 15, 30, 60, 120 };
        static readonly int[] Caps = { 4, 8, 12 };

        IDebugHost _host;
        GameObject _root;
        RectTransform _content;
        RectTransform _body;
        bool _reloadOnClose;

        public void Init(IDebugHost host)
        {
            _host = host;
            _host.Pace.StoryMinutesPerRealMinute = PlayerPrefs.GetInt(RateKey, ClockPace.DefaultStoryMinutesPerRealMinute);
            _host.Pace.MaxHoursPerSitting = PlayerPrefs.GetInt(CapKey, ClockPace.DefaultMaxHoursPerSitting);
            _host.State.Changed += Redraw;
            Build();
        }

        void OnDestroy()
        {
            if (_host != null) _host.State.Changed -= Redraw;
        }

        public bool IsOpen => _root.activeSelf;

        public void Open()
        {
            _root.SetActive(true);
            Redraw();
        }

        void Close()
        {
            _root.SetActive(false);
            if (_reloadOnClose) _host.Reload();
        }

        void Redraw()
        {
            if (!IsOpen) return;
            ClearChildren(_content);
            var state = _host.State;
            var clock = state.Clock;

            Label($"{(state.HasSigned ? state.Name : "(unsigned)")}  •  {state.Place}  •  {clock.DateLabel}, {clock.Part} {clock.TimeLabel}\n" +
                  $"Cash {Money(state.BankrollNow)}   Owed {Money(state.Debts)}   Best {Money(state.BestBankroll)}   Rounds {state.RoundsPlayed}");

            Heading("Checkpoints (a new game, brought to the beat)");
            foreach (var checkpoint in Checkpoints.All)
                Wide(checkpoint.Name, () => _host.Restart(checkpoint));

            Heading("Snapshots (copies of your save)");
            Wide("Take a snapshot now", () =>
            {
                _host.TakeSnapshot();
                Redraw();
            });
            foreach (string name in _host.Snapshots)
            {
                var row = MakeRow(_content, "Snapshot");
                var load = Small(row, name, () => _host.LoadSnapshot(name));
                load.GetComponentInChildren<TMP_Text>().alignment = TextAlignmentOptions.Left;
                var delete = Small(row, "Delete", () =>
                {
                    _host.DeleteSnapshot(name);
                    Redraw();
                });
                var deleteLayout = delete.GetComponent<LayoutElement>();
                deleteLayout.flexibleWidth = 0;
                deleteLayout.preferredWidth = 200;
            }

            Heading("Cash and Kasia's book");
            Buttons(("-$100", () => state.Spend(Math.Min(100, state.BankrollNow))),
                    ("-$10", () => state.Spend(Math.Min(10, state.BankrollNow))),
                    ("+$10", () => state.Earn(10)),
                    ("+$100", () => state.Earn(100)));
            Buttons(("Go broke", () => state.Spend(state.BankrollNow)),
                    ("Borrow $50", () => state.Borrow(50)),
                    ("Repay all", () =>
                    {
                        int amount = Math.Min(state.Debts, state.BankrollNow);
                        if (amount > 0) state.Repay(amount);
                    }));

            Heading("Clock");
            Buttons(("+1 hour", () => state.PassTime(60)),
                    ("Until " + (clock.NextPart == PartOfDay.BeforeDawn ? "midnight" : clock.NextPart.ToString().ToLowerInvariant()), () => state.PassTime(clock.MinutesToNextPart)),
                    ("Sleep till morning", state.Sleep));

            var pace = _host.Pace;
            Heading("Pace (story minutes per real minute; hours a sitting)");
            Buttons(Array.ConvertAll(Rates, rate => PaceButton($"{rate} min", rate == pace.StoryMinutesPerRealMinute,
                () => pace.StoryMinutesPerRealMinute = rate, RateKey, rate)));
            Buttons(Array.ConvertAll(Caps, cap => PaceButton($"{cap} h", cap == pace.MaxHoursPerSitting,
                () => pace.MaxHoursPerSitting = cap, CapKey, cap)));

            Heading("Hidden scales");
            var scales = state.Scales;
            int[] values = { scales.Generous, scales.Reliable, scales.Ruthless, scales.Straight };
            for (int i = 0; i < values.Length; i++)
            {
                string up = Scales.Poles[2 * i], down = Scales.Poles[2 * i + 1];
                var row = MakeRow(_content, "Scale");
                var label = MakeText(row, $"{up}/{down}  {values[i]:+0;-0;0}", 34, TextAnchor.MiddleLeft, Row);
                label.GetComponent<LayoutElement>().flexibleWidth = 1;
                Small(row, down, () => state.Lean(down));
                Small(row, up, () => state.Lean(up));
            }

            Heading("Go to (boots the city there)");
            foreach (var place in _host.Places)
            {
                Wide(place.Id == state.Place ? $"{place.Name}  (here)" : place.Name, () =>
                {
                    state.MoveTo(place.Id);
                    _host.Reload();
                });
            }

            Heading("Story flags (the city boots again when you close)");
            var story = _host.Story;
            foreach (string flag in story.FlagNames())
            {
                bool on = story.Flag(flag);
                var button = Wide((on ? "<color=#5cc9d1>ON</color>    " : "off    ") + flag, () =>
                {
                    story.Set(flag, !on);
                    _reloadOnClose = true;
                    Redraw();
                });
                button.GetComponentInChildren<TMP_Text>().alignment = TextAlignmentOptions.Left;
            }

            Heading("Start again");
            Wide("Start over (wipe save)", _host.StartOver);
        }

        /// <summary>A pace setting: takes effect at once, even mid-sitting, and this device keeps it.</summary>
        (string, UnityAction) PaceButton(string label, bool on, Action set, string key, int value) =>
            (on ? $"<color=#5cc9d1>{label}</color>" : label, () =>
            {
                set();
                PlayerPrefs.SetInt(key, value);
                PlayerPrefs.Save();
                Redraw();
            });

        void Heading(string text)
        {
            var heading = MakeText(_content, text, 34, TextAnchor.LowerLeft, 76);
            heading.color = Teal;
            heading.fontStyle = FontStyles.Bold;
        }

        void Label(string text) => MakeText(_content, text, 30, TextAnchor.UpperLeft, 96).color = Paper;

        Button Wide(string label, UnityAction onClick) => Small(_content, label, onClick);

        void Buttons(params (string label, UnityAction action)[] buttons)
        {
            var row = MakeRow(_content, "Buttons");
            foreach (var (label, action) in buttons) Small(row, label, action);
        }

        /// <summary>A shorter button than the game's, so the panel fits more on a screen.</summary>
        static Button Small(Transform parent, string label, UnityAction onClick)
        {
            var button = MakeButton(parent, label, onClick);
            button.GetComponent<LayoutElement>().preferredHeight = Row;
            var text = button.GetComponentInChildren<TMP_Text>();
            text.fontSize = 32;
            text.overflowMode = TextOverflowModes.Ellipsis;
            NoWrap(text);
            return button;
        }

        void Build()
        {
            // Over everything, the phone (13) included.
            var canvas = MakeCanvas(transform, "Debug Canvas", 50);

            var safe = new GameObject("Safe Area", typeof(RectTransform), typeof(SafeAreaFitter));
            safe.transform.SetParent(canvas.transform, false);
            Stretch((RectTransform)safe.transform);

            // The DEV button, on the left edge, where no screen puts anything; gold, so it shows on a night plate.
            var open = MakePanel(safe.transform, "Dev Button", InkBlack);
            open.gameObject.AddComponent<Button>().onClick.AddListener(Open);
            Pin(open.rectTransform, new Vector2(0, 0.42f), new Vector2(0, 0.42f), new Vector2(16, -56), new Vector2(136, 56));
            var devText = MakeText(open.transform, "DEV", 36, TextAnchor.MiddleCenter, 0);
            devText.color = Gold;
            devText.fontStyle = FontStyles.Bold;
            Stretch(devText.rectTransform);

            var root = MakeImage(canvas.transform, "Debug Panel", new Color(0, 0, 0, 0.6f));
            Stretch(root.rectTransform);
            _root = root.gameObject;

            var rootSafe = new GameObject("Safe Area", typeof(RectTransform), typeof(SafeAreaFitter));
            rootSafe.transform.SetParent(root.transform, false);
            Stretch((RectTransform)rootSafe.transform);

            var body = MakePanel(rootSafe.transform, "Body", Panel);
            _body = body.rectTransform;

            var title = MakeText(body.transform, "Debug", 44, TextAnchor.MiddleLeft, 0);
            title.color = Gold;
            title.fontStyle = FontStyles.Bold;
            Pin(title.rectTransform, Vector2.up, Vector2.one, new Vector2(32, -112), new Vector2(-260, -16));
            var done = Small(body.transform, "Done", Close);
            Pin((RectTransform)done.transform, Vector2.one, Vector2.one, new Vector2(-232, -104), new Vector2(-24, -24));

            var viewport = new GameObject("Viewport", typeof(RectTransform), typeof(RectMask2D), typeof(Image));
            viewport.transform.SetParent(body.transform, false);
            // A clear image, so a drag anywhere in the list scrolls it.
            viewport.GetComponent<Image>().color = Color.clear;
            var viewportRect = (RectTransform)viewport.transform;
            Pin(viewportRect, Vector2.zero, Vector2.one, new Vector2(16, 16), new Vector2(-16, -124));

            var content = new GameObject("Content", typeof(RectTransform));
            content.transform.SetParent(viewport.transform, false);
            _content = AddVertical(content, 12, new RectOffset(16, 16, 0, 24));
            content.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            _content.anchorMin = new Vector2(0, 1);
            _content.anchorMax = Vector2.one;
            _content.pivot = new Vector2(0.5f, 1);
            _content.offsetMin = _content.offsetMax = Vector2.zero;

            var scroll = body.gameObject.AddComponent<ScrollRect>();
            scroll.viewport = viewportRect;
            scroll.content = _content;
            scroll.horizontal = false;
            scroll.movementType = ScrollRect.MovementType.Clamped;
            scroll.scrollSensitivity = 40;

            // Portrait: nearly the whole screen. Landscape: a column down the middle.
            OnTurn(canvas, landscape =>
            {
                if (landscape)
                    Pin(_body, new Vector2(0.5f, 0), new Vector2(0.5f, 1), new Vector2(-640, 32), new Vector2(640, -32));
                else
                    Pin(_body, Vector2.zero, Vector2.one, new Vector2(32, 48), new Vector2(-32, -48));
            });
            _root.SetActive(false);
        }
    }
}
