using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Ink.Runtime;
using Starflare.Ui;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Starflare.Ui.UiKit;

namespace Starflare.Game
{
    /// <summary>
    /// Plays an Ink scene as panels (writing.md's panel kit): a plate, a portrait pose, an
    /// insert, or one line on black, with one line of text per panel. Tap to advance; choices
    /// are buttons. It knows nothing about money or games: whoever owns the state binds the
    /// story's external functions and sets its variables.
    ///
    /// Tags: "# plate: tag", "# pose: tag" (or "none"), "# insert: tag" (this line only),
    /// "# black" (this line on black), "# point: control". Lines read "NAME: text" for
    /// dialogue, plain for narration.
    ///
    /// A scene can also play over a game (<see cref="PlayOver"/>), so a teacher talks while you
    /// see the table: no backdrop or plate, and along the bottom, the full width of the screen,
    /// a square with the speaker's face cropped from their pose and the line beside it (Mike,
    /// 2026-11-21). A line tagged
    /// "# point: stand" is an instruction: once it's read, the speaker and text step aside, that
    /// control glows and is the only one that works, and a Help button repeats what was said
    /// since the last instruction. Using the control carries the scene on.
    ///
    /// A new plate crossfades from the last and a line on black fades in. A scene played on its
    /// own leaves its last panel up when it ends, so the city can dip to black over it
    /// (<see cref="Clear"/>); over a game it steps straight out of the way.
    /// </summary>
    public sealed class StoryPlayer : MonoBehaviour
    {
        const float CharsPerSecond = 70f;
        const float PoseFadeSeconds = 0.18f;
        const float PlateFadeSeconds = 0.3f;
        const float BlackFadeSeconds = 0.25f;
        static readonly Color OverShade = new Color(0f, 0f, 0f, 0.3f);

        // Over a game: the speaker's face square, and how it's cut from a full-body pose. Every
        // pose stands head at the top, centred, so the top fifth of the picture is head and shoulders.
        const float FaceSide = 300f;
        const float FaceInset = 10f;
        const float FaceGap = 16f;
        const float FaceCropHeight = 0.2f;
        const float FaceCropTop = 0.01f;

        static readonly Regex SpeakerLine = new Regex(@"^([A-Z][A-Z .'\-]*):\s*(.*)$");

        Story _story;
        Func<string, string> _plateFor;
        Action _onDone;

        GameObject _root;
        RectTransform _insertArea;
        AspectRatioFitter _insertFit;
        Image _backdrop, _plate, _lastPlate, _pose, _insert, _insertPicture, _black, _rule, _glow, _faceFrame, _face;
        Button _help;
        CoverFit _plateFit;
        TMP_Text _speaker, _line, _blackLine, _more;
        RectTransform _bottom, _choices, _box;
        LayoutElement _boxLayout;
        Reveal _blackReveal;

        string _fullLine = "";
        float _shown;
        float _poseFade = 1f;
        float _plateFade = 1f;
        bool _playing, _waitingOnChoice;

        // Playing over a game: what's coached, the control the line just read points at, the
        // control being waited on, and the lines since the last instruction, for Help.
        ICoachable _game;
        string _pointAt, _waitingFor;
        readonly List<(string text, bool black, Sprite pose)> _step = new List<(string, bool, Sprite)>();
        int _replay = -1;

        public bool IsPlaying => _playing;

        /// <summary>
        /// <paramref name="plateFor"/> turns a "# plate:" tag into the plate to show (a bare view
        /// becomes its plate for the time of day); without it, tags are plates as written.
        /// </summary>
        public void Init(Story story, Func<string, string> plateFor = null)
        {
            _story = story;
            _plateFor = plateFor ?? (tag => tag);
            Build();
            _root.SetActive(false);
        }

        /// <summary>Plays one knot from the top and calls back when it ends.</summary>
        public void Play(string knot, Action onDone) => Begin(knot, null, onDone);

        /// <summary>Plays one knot over a game, which stays in view and is coached by "# point:" lines.</summary>
        public void PlayOver(string knot, ICoachable game, Action onDone) =>
            Begin(knot, game ?? throw new ArgumentNullException(nameof(game)), onDone);

        /// <summary>Takes down the last panel of a scene that's ended, once the screen is dark.</summary>
        public void Clear()
        {
            if (!_playing) _root.SetActive(false);
        }

        void Begin(string knot, ICoachable game, Action onDone)
        {
            _game = game;
            _pointAt = _waitingFor = null;
            _step.Clear();
            _replay = -1;
            _backdrop.color = game != null ? OverShade : Color.black;
            if (game != null)
            {
                game.Lock("");
                game.ActionTaken += OnAction;
            }
            Rearrange(OrientationLayout.IsLandscape);
            SetTalking(true);
            _onDone = onDone;
            _story.ChoosePathString(knot);
            _root.SetActive(true);
            _playing = true;
            _plate.enabled = false;
            _lastPlate.enabled = false;
            _plateFade = 1f;
            _pose.enabled = false;
            _speaker.text = _fullLine = _line.text = "";
            _box.gameObject.SetActive(false);
            _faceFrame.gameObject.SetActive(false);
            _choices.gameObject.SetActive(false);
            Advance();
        }

        void Update()
        {
            if (_plateFade < 1f)
            {
                _plateFade = Mathf.Min(1f, _plateFade + Time.deltaTime / PlateFadeSeconds);
                _lastPlate.color = new Color(1, 1, 1, 1 - Mathf.SmoothStep(0, 1, _plateFade));
                if (_plateFade >= 1f) _lastPlate.enabled = false;
            }
            if (!_playing) return;

            if (_shown < _fullLine.Length)
            {
                _shown = Mathf.Min(_fullLine.Length, _shown + CharsPerSecond * Time.deltaTime);
                SetVisibleText(_fullLine.Substring(0, (int)_shown));
            }
            _more.enabled = !_waitingOnChoice && _shown >= _fullLine.Length && Mathf.Repeat(Time.time, 1f) < 0.65f;
            if (_glow.gameObject.activeSelf)
                _glow.color = new Color(Gold.r, Gold.g, Gold.b, 0.55f + 0.45f * Mathf.Sin(Time.time * 5f));

            if (_poseFade < 1f)
            {
                _poseFade = Mathf.Min(1f, _poseFade + Time.deltaTime / PoseFadeSeconds);
                _pose.color = new Color(1, 1, 1, _poseFade);
            }
        }

        void OnTap()
        {
            if (!_playing || _waitingOnChoice || _waitingFor != null && _replay < 0) return;
            if (_shown < _fullLine.Length)
            {
                _shown = _fullLine.Length;
                SetVisibleText(_fullLine);
                return;
            }
            if (_replay >= 0)
            {
                if (++_replay < _step.Count) ShowStepLine(_replay);
                else WaitFor(_waitingFor);
                return;
            }
            Advance();
        }

        void Advance()
        {
            _insert.gameObject.SetActive(false);
            _black.gameObject.SetActive(false);

            // The line just read was an instruction: step aside and wait for that control.
            if (_pointAt != null)
            {
                string id = _pointAt;
                _pointAt = null;
                WaitFor(id);
                return;
            }

            while (_story.canContinue)
            {
                string text = _story.Continue().Trim();
                bool black = ApplyTags();
                if (text.Length == 0) continue;
                ShowLine(text, black);
                _step.Add((text, black, _pose.enabled ? _pose.sprite : null));
                return;
            }

            if (_story.currentChoices.Count > 0)
            {
                ShowChoices();
                return;
            }

            _playing = false;
            _more.enabled = false;
            if (_game != null)
            {
                _game.ActionTaken -= OnAction;
                _game.Lock(null);
                _game = null;
                _root.SetActive(false);
            }
            var done = _onDone;
            _onDone = null;
            done?.Invoke();
        }

        /// <summary>Applies this line's tags; true if the line goes on a black panel.</summary>
        bool ApplyTags()
        {
            bool black = false;
            foreach (string tag in _story.currentTags)
            {
                int colon = tag.IndexOf(':');
                string kind = (colon < 0 ? tag : tag.Substring(0, colon)).Trim().ToLowerInvariant();
                string value = colon < 0 ? "" : tag.Substring(colon + 1).Trim();

                switch (kind)
                {
                    case "plate":
                        if (_game != null) break; // over a game, the game is the background
                        value = _plateFor(value);
                        var plate = Art.Plate(value);
                        if (_plate.enabled && plate != _plate.sprite) HoldLastPlate();
                        _plateFit.Show(_plate, plate, Art.Focus(value));
                        _pose.enabled = false;
                        break;
                    case "pose":
                        var pose = value == "none" ? null : Art.Pose(value);
                        if (pose != null && pose != _pose.sprite || !_pose.enabled)
                            _poseFade = 0f;
                        _pose.sprite = pose;
                        _pose.enabled = pose != null;
                        break;
                    case "insert":
                        var insert = Art.Insert(value);
                        _insertPicture.sprite = insert;
                        if (insert != null) _insertFit.aspectRatio = insert.rect.width / insert.rect.height;
                        _insert.gameObject.SetActive(insert != null);
                        break;
                    case "black":
                        black = true;
                        break;
                    case "point":
                        _pointAt = value;
                        break;
                }
            }
            return black;
        }

        /// <summary>Lays the plate that's showing over the new one, to fade out.</summary>
        void HoldLastPlate()
        {
            RectTransform from = _plate.rectTransform, to = _lastPlate.rectTransform;
            to.anchorMin = from.anchorMin;
            to.anchorMax = from.anchorMax;
            to.pivot = from.pivot;
            to.sizeDelta = from.sizeDelta;
            to.anchoredPosition = from.anchoredPosition;
            _lastPlate.sprite = _plate.sprite;
            _lastPlate.color = Color.white;
            _lastPlate.enabled = true;
            _plateFade = 0f;
        }

        void ShowLine(string text, bool black)
        {
            _waitingOnChoice = false;
            ClearChildren(_choices);
            _choices.gameObject.SetActive(false);

            string speaker = "";
            var match = SpeakerLine.Match(text);
            if (match.Success)
            {
                speaker = match.Groups[1].Value;
                text = match.Groups[2].Value;
            }

            if (black && !_black.gameObject.activeSelf) _blackReveal.Play(BlackFadeSeconds);
            _black.gameObject.SetActive(black);
            _speaker.text = speaker;
            _line.fontStyle = speaker.Length == 0 ? FontStyles.Italic : FontStyles.Normal;
            _fullLine = text;
            _shown = 0;
            SetVisibleText("");
            FitBox();
        }

        /// <summary>
        /// Sizes the dialogue box to the whole line before it types out, so it doesn't grow as
        /// it goes. Narration has no name, so its box drops the name and rule.
        /// </summary>
        void FitBox()
        {
            bool showing = _fullLine.Length > 0;
            bool speaking = _speaker.text.Length > 0;
            ShowFace(showing && speaking);
            bool face = _faceFrame.gameObject.activeSelf;
            _speaker.enabled = _rule.enabled = speaking;
            float top = speaking ? 112 : 40;
            _line.rectTransform.offsetMax = new Vector2(-48, -top);

            var padding = _bottom.GetComponent<VerticalLayoutGroup>().padding;
            float width = _bottom.rect.width - padding.horizontal - 96 - (face ? FaceSide + FaceGap : 0);
            float height = _line.GetPreferredValues(_fullLine, width, 0).y;
            _boxLayout.preferredHeight = Mathf.Max(top + height + 44, face ? FaceSide : 0);
            _box.gameObject.SetActive(showing);
        }

        /// <summary>
        /// Over a game, the speaker's face beside the line: the top of their pose, scaled so its
        /// head and shoulders fill the square. Narration, or a line with no pose, has none, even
        /// while the last pose is still up.
        /// </summary>
        void ShowFace(bool showing)
        {
            var sprite = _pose.enabled ? _pose.sprite : null;
            bool show = showing && _game != null && sprite != null;
            _faceFrame.gameObject.SetActive(show);
            if (!show) return;
            _face.sprite = sprite;
            float window = FaceSide - 2 * FaceInset;
            float height = window / FaceCropHeight;
            var rect = _face.rectTransform;
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 1);
            rect.sizeDelta = new Vector2(height * sprite.rect.width / sprite.rect.height, height);
            rect.anchoredPosition = new Vector2(0, FaceCropTop * height);
        }

        void SetVisibleText(string text)
        {
            if (_black.gameObject.activeSelf) _blackLine.text = text;
            else _line.text = text;
        }

        /// <summary>
        /// The speaker and text step aside; the control glows and is the only one that works,
        /// with Help to hear it again. A control the game doesn't have is skipped, with a warning.
        /// </summary>
        void WaitFor(string id)
        {
            var control = _game?.Control(id);
            if (control == null)
            {
                Debug.LogWarning($"\"# point: {id}\" names no control here; carrying on.");
                _step.Clear();
                Advance();
                return;
            }
            _waitingFor = id;
            _replay = -1;
            SetTalking(false);
            _glow.transform.SetParent(control, false);
            Stretch(_glow.rectTransform);
            _glow.rectTransform.offsetMin = new Vector2(-14, -14);
            _glow.rectTransform.offsetMax = new Vector2(14, 14);
            _glow.gameObject.SetActive(true);
            _help.gameObject.SetActive(true);
            _game.Lock(id);
        }

        void OnAction(string id)
        {
            if (id != _waitingFor) return;
            _waitingFor = null;
            _replay = -1;
            HideGlow();
            _help.gameObject.SetActive(false);
            _game.Lock("");
            _step.Clear();
            SetTalking(true);
            Advance();
        }

        /// <summary>Help: the speaker comes back and says again what she said since the last instruction.</summary>
        void Repeat()
        {
            if (_waitingFor == null || _step.Count == 0) return;
            _game.Lock("");
            HideGlow();
            _help.gameObject.SetActive(false);
            SetTalking(true);
            _replay = 0;
            ShowStepLine(0);
        }

        void ShowStepLine(int index)
        {
            var (text, black, pose) = _step[index];
            _pose.sprite = pose;
            _pose.enabled = pose != null;
            ShowLine(text, black);
        }

        void HideGlow()
        {
            _glow.gameObject.SetActive(false);
            _glow.transform.SetParent(_root.transform, false);
        }

        /// <summary>The backdrop (which takes the taps), speaker and text, on or out of the way.</summary>
        void SetTalking(bool on)
        {
            _backdrop.enabled = on;
            _pose.gameObject.SetActive(on && _game == null);
            _bottom.gameObject.SetActive(on);
            if (!on) _insert.gameObject.SetActive(false);
        }

        void ShowChoices()
        {
            _waitingOnChoice = true;
            ClearChildren(_choices);
            foreach (var choice in _story.currentChoices)
            {
                int index = choice.index;
                MakeChoiceButton(_choices, choice.text.Trim(), () => Choose(index));
            }
            _choices.gameObject.SetActive(true);
        }

        void Choose(int index)
        {
            _waitingOnChoice = false;
            ClearChildren(_choices);
            _choices.gameObject.SetActive(false);
            _story.ChooseChoiceIndex(index);
            Advance();
        }

        // ---- UI construction ----

        void Build()
        {
            var canvas = MakeCanvas(transform, "Story Canvas", 10);
            _root = canvas.gameObject;

            var backdrop = MakeImage(canvas.transform, "Backdrop", Color.black);
            Stretch(backdrop.rectTransform);
            _backdrop = backdrop;
            _plate = MakeImage(backdrop.transform, "Plate", Color.white);
            _plateFit = _plate.gameObject.AddComponent<CoverFit>();
            _plate.raycastTarget = false;
            _lastPlate = MakeImage(backdrop.transform, "Last Plate", Color.white);
            _lastPlate.raycastTarget = false;
            _lastPlate.enabled = false;

            // Tapping anywhere that isn't a choice advances.
            var tap = backdrop.gameObject.AddComponent<Button>();
            tap.transition = Selectable.Transition.None;
            tap.onClick.AddListener(OnTap);

            var safe = new GameObject("Safe Area", typeof(RectTransform), typeof(SafeAreaFitter));
            safe.transform.SetParent(canvas.transform, false);
            Stretch((RectTransform)safe.transform);

            _pose = MakeImage(safe.transform, "Pose", Color.white);
            _pose.preserveAspect = true;
            _pose.raycastTarget = false;

            BuildInsert(safe.transform);

            // The dialogue box with the choices under it, stacked up from the bottom. The
            // choices sit a little narrower than the box.
            var bottomGo = new GameObject("Dialogue", typeof(RectTransform));
            bottomGo.transform.SetParent(safe.transform, false);
            _bottom = AddVertical(bottomGo, 24, new RectOffset(0, 0, 0, 0));
            _bottom.GetComponent<VerticalLayoutGroup>().childAlignment = TextAnchor.LowerCenter;
            BuildDialogueBox(_bottom);

            var choicesGo = new GameObject("Choices", typeof(RectTransform));
            choicesGo.transform.SetParent(_bottom, false);
            _choices = AddVertical(choicesGo, 20, new RectOffset(28, 28, 0, 0));

            _black = MakeImage(canvas.transform, "Black Panel", Color.black);
            Stretch(_black.rectTransform);
            _black.raycastTarget = false;
            _blackLine = MakeText(_black.transform, "", 56, TextAnchor.MiddleCenter, 0);
            Stretch(_blackLine.rectTransform);
            _blackLine.rectTransform.offsetMin = new Vector2(100, 0);
            _blackLine.rectTransform.offsetMax = new Vector2(-100, 0);
            _blackReveal = _black.gameObject.AddComponent<Reveal>();
            _black.gameObject.SetActive(false);

            _help = MakeButton(safe.transform, "Help", Repeat);
            var help = (RectTransform)_help.transform;
            help.anchorMin = help.anchorMax = help.pivot = Vector2.one;
            help.sizeDelta = new Vector2(220, 110);
            help.anchoredPosition = new Vector2(-36, -36);
            _help.gameObject.SetActive(false);

            _glow = MakeImage(canvas.transform, "Glow", Gold);
            _glow.sprite = RoundedFrame;
            _glow.type = Image.Type.Sliced;
            _glow.raycastTarget = false;
            _glow.gameObject.AddComponent<LayoutElement>().ignoreLayout = true;
            _glow.gameObject.SetActive(false);

            OnTurn(canvas, Rearrange);
        }

        /// <summary>
        /// Portrait stacks the pose over the dialogue box, with the choices under the box. Landscape
        /// stands the pose (and any insert) on the left and puts the box and choices on the right,
        /// under the top bar (Hud). Over a game, the face square and the box run the full width
        /// along the bottom, so the table above stays in view.
        /// </summary>
        void Rearrange(bool landscape)
        {
            if (_game != null)
            {
                if (landscape)
                {
                    var middle = new Vector2(0.3f, 0.55f);
                    Pin(_insertArea, middle, middle, new Vector2(-440, -300), new Vector2(440, 300));
                    Pin(_bottom, Vector2.zero, Vector2.right, new Vector2(0, 32), new Vector2(0, 640));
                }
                else
                {
                    var top = new Vector2(0.5f, 1);
                    Pin(_insertArea, Vector2.zero, Vector2.one, new Vector2(48, 520), new Vector2(-48, -240));
                    Pin(_bottom, Vector2.zero, Vector2.right, new Vector2(0, 40), new Vector2(0, 1100));
                }
                FitBox();
                return;
            }
            if (landscape)
            {
                // In the foreground, cut off at the knees by the bottom edge, so the figure
                // doesn't float over the plate's floor; the head stays clear of the top bar
                // (about 170 high).
                var poseFoot = new Vector2(0.28f, 0);
                Pin(_pose.rectTransform, poseFoot, poseFoot, new Vector2(-450, -440), new Vector2(450, 880));
                Pin(_insertArea, Vector2.zero, new Vector2(0.5f, 1), new Vector2(48, 48), new Vector2(-24, -190));
                Pin(_bottom, new Vector2(0.5f, 0), Vector2.one, new Vector2(0, 32), new Vector2(-8, -170));
            }
            else
            {
                var foot = new Vector2(0.5f, 0);
                Pin(_pose.rectTransform, foot, foot, new Vector2(-500, 150), new Vector2(500, 1650));
                Pin(_insertArea, Vector2.zero, Vector2.one, new Vector2(48, 520), new Vector2(-48, -240));
                Pin(_bottom, Vector2.zero, Vector2.right, new Vector2(0, 40), new Vector2(0, 1400));
            }
            FitBox();
        }

        /// <summary>
        /// A close-up (an insert) as a comic panel laid over the scene: the scene dims behind a
        /// scrim, still there for context, and the picture sits in the clear space above the
        /// dialogue box at its own shape, in a thin ink border. The scrim doesn't take taps, so
        /// tapping anywhere still advances.
        /// </summary>
        void BuildInsert(Transform parent)
        {
            _insert = MakeImage(parent, "Insert Scrim", new Color(0, 0, 0, InsertDim));
            Stretch(_insert.rectTransform);
            // Past the safe area's edges, so the notch and the corners dim too.
            _insert.rectTransform.offsetMin = new Vector2(-ScrimBleed, -ScrimBleed);
            _insert.rectTransform.offsetMax = new Vector2(ScrimBleed, ScrimBleed);
            _insert.raycastTarget = false;

            var area = new GameObject("Insert Area", typeof(RectTransform));
            area.transform.SetParent(_insert.transform, false);
            _insertArea = (RectTransform)area.transform;

            var frame = MakeImage(_insertArea, "Insert Frame", InkBlack);
            frame.raycastTarget = false;
            _insertFit = frame.gameObject.AddComponent<AspectRatioFitter>();
            _insertFit.aspectMode = AspectRatioFitter.AspectMode.FitInParent;

            _insertPicture = MakeImage(frame.transform, "Insert", Color.white);
            _insertPicture.preserveAspect = true;
            _insertPicture.raycastTarget = false;
            Stretch(_insertPicture.rectTransform);
            _insertPicture.rectTransform.offsetMin = new Vector2(InsertBorder, InsertBorder);
            _insertPicture.rectTransform.offsetMax = new Vector2(-InsertBorder, -InsertBorder);
            _insert.gameObject.SetActive(false);
        }

        /// <summary>How dark the scene goes behind a close-up, and the close-up's ink border.</summary>
        const float InsertDim = 0.9f;
        const float InsertBorder = 8f;
        const float ScrimBleed = 400f;

        /// <summary>
        /// A row: the speaker's face square (over a game only) and the dialogue box, which takes
        /// the rest of the width.
        /// </summary>
        void BuildDialogueBox(Transform parent)
        {
            var rowGo = new GameObject("Talk Row", typeof(RectTransform));
            rowGo.transform.SetParent(parent, false);
            var row = rowGo.AddComponent<HorizontalLayoutGroup>();
            row.spacing = FaceGap;
            row.childControlWidth = row.childControlHeight = true;
            row.childForceExpandWidth = row.childForceExpandHeight = false;
            row.childAlignment = TextAnchor.UpperLeft;

            _faceFrame = MakePanel(rowGo.transform, "Face Frame", Panel);
            _faceFrame.raycastTarget = false;
            var frameLayout = _faceFrame.gameObject.AddComponent<LayoutElement>();
            frameLayout.minWidth = frameLayout.preferredWidth = FaceSide;
            frameLayout.minHeight = frameLayout.preferredHeight = FaceSide;
            frameLayout.flexibleWidth = 0;
            var window = new GameObject("Face Window", typeof(RectTransform), typeof(RectMask2D));
            window.transform.SetParent(_faceFrame.transform, false);
            var windowRect = (RectTransform)window.transform;
            Stretch(windowRect);
            windowRect.offsetMin = new Vector2(FaceInset, FaceInset);
            windowRect.offsetMax = new Vector2(-FaceInset, -FaceInset);
            _face = MakeImage(window.transform, "Face", Color.white);
            _face.raycastTarget = false;
            _faceFrame.gameObject.SetActive(false);

            var box = MakePanel(rowGo.transform, "Dialogue Box", Panel);
            box.raycastTarget = false;
            _box = box.rectTransform;
            _boxLayout = box.gameObject.AddComponent<LayoutElement>();
            _boxLayout.flexibleWidth = 1;

            _rule = MakeImage(box.transform, "Rule", new Color(Teal.r, Teal.g, Teal.b, 0.6f));
            _rule.raycastTarget = false;
            _rule.rectTransform.anchorMin = new Vector2(0, 1);
            _rule.rectTransform.anchorMax = new Vector2(1, 1);
            _rule.rectTransform.offsetMin = new Vector2(48, -90);
            _rule.rectTransform.offsetMax = new Vector2(-48, -88);

            _speaker = MakeText(box.transform, "", 40, TextAnchor.LowerLeft, 0);
            _speaker.color = Teal;
            _speaker.fontStyle = FontStyles.Bold;
            _speaker.rectTransform.anchorMin = new Vector2(0, 1);
            _speaker.rectTransform.anchorMax = new Vector2(1, 1);
            _speaker.rectTransform.offsetMin = new Vector2(48, -84);
            _speaker.rectTransform.offsetMax = new Vector2(-48, -20);

            // FitBox sets the line's top, below the name or not.
            _line = MakeText(box.transform, "", 52, TextAnchor.UpperLeft, 0);
            Stretch(_line.rectTransform);
            _line.rectTransform.offsetMin = new Vector2(48, 36);
            _line.rectTransform.offsetMax = new Vector2(-48, -112);
            _line.lineSpacing = 10; // a tenth of the size between lines, as the legacy 1.1 did

            _more = MakeText(box.transform, "▼", 32, TextAnchor.LowerRight, 0);
            _more.color = Teal;
            Stretch(_more.rectTransform);
            _more.rectTransform.offsetMin = new Vector2(0, 18);
            _more.rectTransform.offsetMax = new Vector2(-28, 0);
        }
    }
}
