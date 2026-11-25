using System;
using System.Collections.Generic;
using Starflare.State;
using Starflare.Ui;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static Starflare.Ui.UiKit;

namespace Starflare.Game
{
    /// <summary>
    /// The debug panel, for setting the game to the moment you want to test. A swipe up from the
    /// bottom edge of any screen pulls it up as a sheet; Done or a tap above it slides it away. It loads a checkpoint (a beat's recipe) or a
    /// snapshot (a copy of a real save), and edits the live state through the same methods play
    /// uses: cash, Kasia's book, the clock, the register box, the hidden scales, every story flag,
    /// and where you stand. It also sets the clock's pace (story minutes per real minute, hours a sitting) and the
    /// register box the checkpoints sign with, and the art style, which this device remembers, so a pace being tried survives reloads and relaunches. Flags and places change what a screen offers, so closing after either boots the
    /// city again from the save. Every build carries it for now, release builds included (Mike, 2026-11-25).
    /// </summary>
    public sealed class DebugPanel : MonoBehaviour
    {
        const float Row = 88;
        const float SlideSeconds = 0.25f;
        const float MaxStep = 1f / 30f;

        /// <summary>How tall the band a swipe starts in is, above the home indicator, as a share of the screen.</summary>
        const float EdgeShare = 0.08f;

        const string RateKey = "debug.pace.rate";
        const string CapKey = "debug.pace.cap";
        const string BoxKey = "debug.register.box";
        static readonly int[] Rates = { 15, 30, 60, 120 };
        static readonly int[] Caps = { 4, 8, 12 };

        IDebugHost _host;
        GameObject _root;
        RectTransform _canvas;
        RectTransform _content;
        RectTransform _body;
        RectTransform _viewport;
        Image _backdrop;
        EdgeSwipe _swipe;
        bool _open;
        float _shown;
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

        /// <summary>True from the tap that opens the sheet until the tap that closes it, slides included.</summary>
        public bool IsOpen => _open;

        public void Open()
        {
            if (_open) return;
            _open = true;
            _root.SetActive(true);
            Redraw();
            Slide();
        }

        /// <summary>Slides the sheet down; a reload the flags asked for waits until it's off the screen.</summary>
        void Close() => _open = false;

        void Redraw()
        {
            if (!IsOpen) return;
            ClearChildren(_content);
            var state = _host.State;
            var clock = state.Clock;

            Label($"{(state.HasSigned ? state.Name : "(unsigned)")}  •  {state.Place}  •  {clock.DateLabel}, {clock.Part} {clock.TimeLabel}\n" +
                  $"Cash {Money(state.BankrollNow)}   Owed {Money(state.Debts)}   Best {Money(state.BestBankroll)}   Rounds {state.RoundsPlayed}");

            // Who asks first at the wedding (A1.4) follows the box; Other is "Prefer not to answer".
            Heading("Register box (checkpoints sign with it; changes this game too)");
            Buttons(BoxButton("Mr.", RegisterBox.Mr), BoxButton("Ms.", RegisterBox.Ms), BoxButton("Other", RegisterBox.PreferNotToAnswer));

            Heading("Checkpoints (a new game, brought to the beat)");
            foreach (var checkpoint in Checkpoints.All)
                Wide(checkpoint.Name, () => _host.Restart(checkpoint, CheckpointBox));

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

            Heading("Bowling hand (this device keeps it)");
            var hand = BowlingHand.Current;
            Buttons(Array.ConvertAll(new[] { Bowling.Hand.Right, Bowling.Hand.Left }, h =>
                (h == hand ? $"<color=#5cc9d1>{h}</color>" : h.ToString(), (UnityAction)(() =>
                {
                    BowlingHand.Current = h;
                    Redraw();
                }))));

            // Missing pictures in a style show the base art, so a style can be compared before it's finished.
            Heading("Art style (boots the game again; this device keeps it)");
            string current = ArtStyles.Current;
            Buttons(Array.ConvertAll(ArtStyles.All, style => (style == current ? $"<color=#5cc9d1>{style}</color>" : style,
                (UnityAction)(() =>
                {
                    if (style == current) return;
                    ArtStyles.Choose(style);
                    _host.Reload();
                }))));

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

            Heading("Heat (illegal play; a step cools each week without it)");
            var heatRow = MakeRow(_content, "Heat");
            var heatLabel = MakeText(heatRow, $"{state.HeatStep}  ({state.Heat.Points} points)", 34, TextAnchor.MiddleLeft, Row);
            heatLabel.GetComponent<LayoutElement>().flexibleWidth = 1;
            Small(heatRow, "-1", () => state.SetHeatPoints(Math.Max(0, state.Heat.Points - 1)));
            Small(heatRow, "+1", () => state.SetHeatPoints(state.Heat.Points + 1));

            Heading("Go to (boots the game there)");
            foreach (var place in _host.Places)
            {
                Wide(place.Id == state.Place ? $"{place.Name}  (here)" : place.Name, () =>
                {
                    state.MoveTo(place.Id);
                    _host.Reload();
                });
            }

            Heading("Story flags (the game boots again when you close)");
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

        /// <summary>The box checkpoints sign with; this device remembers it, like the pace.</summary>
        static RegisterBox CheckpointBox => (RegisterBox)PlayerPrefs.GetInt(BoxKey, (int)RegisterBox.Ms);

        (string, UnityAction) BoxButton(string label, RegisterBox box) =>
            (CheckpointBox == box ? $"<color=#5cc9d1>{label}</color>" : label, () =>
            {
                PlayerPrefs.SetInt(BoxKey, (int)box);
                PlayerPrefs.Save();
                if (_host.State.HasSigned) _host.State.RetickBox(box);
                Redraw();
            });

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
            _canvas = (RectTransform)canvas.transform;

            // A swipe up from the bottom edge opens it (Mike, 2026-11-25; replaces the DEV
            // button in the bottom-right corner, 2026-11-21).
            _swipe = new EdgeSwipe(SomethingDragsAt);

            var root = new GameObject("Debug Panel", typeof(RectTransform));
            root.transform.SetParent(canvas.transform, false);
            Stretch((RectTransform)root.transform);
            _root = root;

            // The dim behind the sheet; a tap on it closes the sheet. A sibling of the sheet, not
            // its parent, so a tap on the sheet's own text doesn't bubble up to it.
            _backdrop = MakeImage(root.transform, "Backdrop", Color.clear);
            Stretch(_backdrop.rectTransform);
            _backdrop.gameObject.AddComponent<Button>().onClick.AddListener(Close);

            // The sheet sits on the screen's bottom edge, under the home indicator; the list
            // inside it stops above the indicator (FitBottomInset).
            var body = MakePanel(root.transform, "Sheet", Panel);
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
            _viewport = (RectTransform)viewport.transform;
            Pin(_viewport, Vector2.zero, Vector2.one, new Vector2(16, 16), new Vector2(-16, -124));

            var content = new GameObject("Content", typeof(RectTransform));
            content.transform.SetParent(viewport.transform, false);
            _content = AddVertical(content, 12, new RectOffset(16, 16, 0, 24));
            content.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            _content.anchorMin = new Vector2(0, 1);
            _content.anchorMax = Vector2.one;
            _content.pivot = new Vector2(0.5f, 1);
            _content.offsetMin = _content.offsetMax = Vector2.zero;

            var scroll = body.gameObject.AddComponent<ScrollRect>();
            scroll.viewport = _viewport;
            scroll.content = _content;
            scroll.horizontal = false;
            scroll.movementType = ScrollRect.MovementType.Clamped;
            scroll.scrollSensitivity = 40;

            // Portrait: the full width, up to a little below the top, so the screen shows behind
            // it. Landscape: a column up the middle.
            OnTurn(canvas, landscape =>
            {
                if (landscape)
                    Pin(_body, new Vector2(0.5f, 0), new Vector2(0.5f, 1), new Vector2(-640, 0), new Vector2(640, -48));
                else
                    Pin(_body, Vector2.zero, Vector2.one, Vector2.zero, new Vector2(0, -240));
                Slide();
            });
            _root.SetActive(false);
        }

        void Update()
        {
            FollowSwipe();
            if (!_root.activeSelf) return;
            FitBottomInset();
            if (_swipe.IsDragging) return;
            float target = _open ? 1f : 0f;
            if (_shown == target) return;
            float step = Mathf.Min(Time.unscaledDeltaTime, MaxStep) / SlideSeconds;
            _shown = Mathf.MoveTowards(_shown, target, step);
            Slide();
            if (_shown > 0f) return;
            _root.SetActive(false);
            if (!_reloadOnClose) return;
            _reloadOnClose = false;
            _host.Reload();
        }

        /// <summary>
        /// While the sheet is closed, watches for the swipe up from the bottom edge and pulls the
        /// sheet up under the finger. The edge is the band just above the home indicator.
        /// </summary>
        void FollowSwipe()
        {
            if (_open && !_swipe.IsDragging) return;
            var pointer = Pointer.current;
            if (pointer == null || Screen.height == 0) return;
            float toPixels = Screen.height / _canvas.rect.height;
            float edgeTop = Screen.safeArea.yMin + EdgeShare * Screen.height;
            switch (_swipe.Step(pointer.press.isPressed, pointer.position.ReadValue(), edgeTop, Screen.height, _body.rect.height * toPixels))
            {
                case EdgeSwipe.Outcome.Dragging:
                    Open();
                    _shown = _swipe.Progress;
                    Slide();
                    break;
                case EdgeSwipe.Outcome.Cancel:
                    Close();
                    break;
            }
        }

        /// <summary>
        /// A press the screen below would drag (the bowling ball, a scratcher, a list) is left to
        /// it. A place's backdrop is always there to drag, but it pans only while Look around is
        /// on, so the rest of the time the swipe wins.
        /// </summary>
        static bool SomethingDragsAt(Vector2 position)
        {
            var events = EventSystem.current;
            if (events == null) return false;
            var hits = new List<RaycastResult>();
            events.RaycastAll(new PointerEventData(events) { position = position }, hits);
            return hits.Count > 0 && Drags(hits[0].gameObject);
        }

        /// <summary>Whether a press on <paramref name="hit"/> would drag it, so the swipe leaves it alone.</summary>
        public static bool Drags(GameObject hit)
        {
            var handler = ExecuteEvents.GetEventHandler<IDragHandler>(hit);
            if (handler == null) return false;
            var look = handler.GetComponent<LookAround>();
            return look == null || look.On;
        }

        /// <summary>Puts the sheet where the slide has it: below the screen's edge at 0, all the way up at 1.</summary>
        void Slide()
        {
            float eased = 1f - (1f - _shown) * (1f - _shown);
            _body.anchoredPosition = new Vector2(_body.anchoredPosition.x, -(1f - eased) * _body.rect.height);
            _backdrop.color = new Color(0, 0, 0, 0.6f * eased);
        }

        /// <summary>Keeps the last rows of the list above the home indicator, in canvas units.</summary>
        void FitBottomInset()
        {
            float inset = Screen.height > 0 ? Screen.safeArea.yMin * _canvas.rect.height / Screen.height : 0f;
            _viewport.offsetMin = new Vector2(16, 16 + inset);
        }
    }
}
