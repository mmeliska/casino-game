using System;
using System.Text.RegularExpressions;
using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Starflare.Game.UiKit;

namespace Starflare.Game
{
    /// <summary>
    /// Plays an Ink scene as panels (writing.md's panel kit): a plate, a portrait pose, an
    /// insert, or one line on black, with one line of text per panel. Tap to advance; choices
    /// are buttons. It knows nothing about money or games: whoever owns the state binds the
    /// story's external functions and sets its variables.
    ///
    /// Tags: "# plate: tag", "# pose: tag" (or "none"), "# insert: tag" (this line only),
    /// "# black" (this line on black). Lines read "NAME: text" for dialogue, plain for narration.
    /// </summary>
    public sealed class StoryPlayer : MonoBehaviour
    {
        const float CharsPerSecond = 70f;
        const float PoseFadeSeconds = 0.18f;

        static readonly Regex SpeakerLine = new Regex(@"^([A-Z][A-Z .'\-]*):\s*(.*)$");

        Story _story;
        Action _onDone;

        GameObject _root;
        Image _plate, _pose, _insert, _black, _rule;
        CoverFit _plateFit;
        TMP_Text _speaker, _line, _blackLine, _more;
        RectTransform _bottom, _choices, _box;
        LayoutElement _boxLayout;

        string _fullLine = "";
        float _shown;
        float _poseFade = 1f;
        bool _waitingOnChoice;

        public bool IsPlaying => _root != null && _root.activeSelf;

        public void Init(Story story)
        {
            _story = story;
            Build();
            _root.SetActive(false);
        }

        /// <summary>Plays one knot from the top and calls back when it ends.</summary>
        public void Play(string knot, Action onDone)
        {
            _onDone = onDone;
            _story.ChoosePathString(knot);
            _root.SetActive(true);
            _plate.enabled = false;
            _pose.enabled = false;
            _speaker.text = _fullLine = _line.text = "";
            _box.gameObject.SetActive(false);
            _choices.gameObject.SetActive(false);
            Advance();
        }

        void Update()
        {
            if (!IsPlaying) return;

            if (_shown < _fullLine.Length)
            {
                _shown = Mathf.Min(_fullLine.Length, _shown + CharsPerSecond * Time.deltaTime);
                SetVisibleText(_fullLine.Substring(0, (int)_shown));
            }
            _more.enabled = !_waitingOnChoice && _shown >= _fullLine.Length && Mathf.Repeat(Time.time, 1f) < 0.65f;

            if (_poseFade < 1f)
            {
                _poseFade = Mathf.Min(1f, _poseFade + Time.deltaTime / PoseFadeSeconds);
                _pose.color = new Color(1, 1, 1, _poseFade);
            }
        }

        void OnTap()
        {
            if (_waitingOnChoice) return;
            if (_shown < _fullLine.Length)
            {
                _shown = _fullLine.Length;
                SetVisibleText(_fullLine);
                return;
            }
            Advance();
        }

        void Advance()
        {
            _insert.gameObject.SetActive(false);
            _black.gameObject.SetActive(false);

            while (_story.canContinue)
            {
                string text = _story.Continue().Trim();
                bool black = ApplyTags();
                if (text.Length == 0) continue;
                ShowLine(text, black);
                return;
            }

            if (_story.currentChoices.Count > 0)
            {
                ShowChoices();
                return;
            }

            _root.SetActive(false);
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
                        _plateFit.Show(_plate, Art.Plate(value), Art.Focus(value));
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
                        _insert.transform.GetChild(0).GetComponent<Image>().sprite = insert;
                        _insert.gameObject.SetActive(insert != null);
                        break;
                    case "black":
                        black = true;
                        break;
                }
            }
            return black;
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
            bool speaking = _speaker.text.Length > 0;
            _speaker.enabled = _rule.enabled = speaking;
            float top = speaking ? 112 : 40;
            _line.rectTransform.offsetMax = new Vector2(-48, -top);

            var padding = _bottom.GetComponent<VerticalLayoutGroup>().padding;
            float width = _bottom.rect.width - padding.horizontal - 96;
            float height = _line.GetPreferredValues(_fullLine, width, 0).y;
            _boxLayout.preferredHeight = top + height + 44;
            _box.gameObject.SetActive(_fullLine.Length > 0);
        }

        void SetVisibleText(string text)
        {
            if (_black.gameObject.activeSelf) _blackLine.text = text;
            else _line.text = text;
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
            _plate = MakeImage(backdrop.transform, "Plate", Color.white);
            _plateFit = _plate.gameObject.AddComponent<CoverFit>();
            _plate.raycastTarget = false;

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
            _bottom = AddVertical(bottomGo, 24, new RectOffset(32, 32, 0, 0));
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
            _black.gameObject.SetActive(false);

            OnTurn(canvas, Rearrange);
        }

        /// <summary>
        /// Portrait stacks the pose over the dialogue box, with the choices under the box. Landscape
        /// stands the pose (and any insert) on the left and puts the box and choices on the right,
        /// under the top bar (Hud).
        /// </summary>
        void Rearrange(bool landscape)
        {
            if (landscape)
            {
                var poseFoot = new Vector2(0.28f, 0);
                Pin(_pose.rectTransform, poseFoot, poseFoot, new Vector2(-380, 0), new Vector2(380, 1040));
                var middle = new Vector2(0.28f, 0.5f);
                Pin(_insert.rectTransform, middle, middle, new Vector2(-360, -360), new Vector2(360, 360));
                Pin(_bottom, new Vector2(0.5f, 0), Vector2.one, new Vector2(0, 32), new Vector2(-8, -170));
            }
            else
            {
                var foot = new Vector2(0.5f, 0);
                Pin(_pose.rectTransform, foot, foot, new Vector2(-500, 150), new Vector2(500, 1650));
                var top = new Vector2(0.5f, 1);
                Pin(_insert.rectTransform, top, top, new Vector2(-390, -1040), new Vector2(390, -260));
                Pin(_bottom, Vector2.zero, Vector2.right, new Vector2(0, 40), new Vector2(0, 1400));
            }
            FitBox();
        }

        void BuildInsert(Transform parent)
        {
            _insert = MakeImage(parent, "Insert Frame", new Color(0.93f, 0.9f, 0.82f));
            _insert.sprite = Rounded;
            _insert.type = Image.Type.Sliced;
            _insert.raycastTarget = false;

            var picture = MakeImage(_insert.transform, "Insert", Color.white);
            picture.preserveAspect = true;
            picture.raycastTarget = false;
            Stretch(picture.rectTransform);
            picture.rectTransform.offsetMin = new Vector2(14, 14);
            picture.rectTransform.offsetMax = new Vector2(-14, -14);
            _insert.gameObject.SetActive(false);
        }

        void BuildDialogueBox(Transform parent)
        {
            var box = MakePanel(parent, "Dialogue Box", Panel);
            box.raycastTarget = false;
            _box = box.rectTransform;
            _boxLayout = box.gameObject.AddComponent<LayoutElement>();

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
