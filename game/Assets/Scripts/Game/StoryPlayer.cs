using System;
using System.Text.RegularExpressions;
using Ink.Runtime;
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
        Image _plate, _pose, _insert, _black;
        CoverFit _plateFit;
        Text _speaker, _line, _blackLine, _more;
        RectTransform _choices, _box;

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

            string speaker = "";
            var match = SpeakerLine.Match(text);
            if (match.Success)
            {
                speaker = match.Groups[1].Value;
                text = match.Groups[2].Value;
            }

            _black.gameObject.SetActive(black);
            _speaker.text = speaker;
            _line.fontStyle = speaker.Length == 0 ? FontStyle.Italic : FontStyle.Normal;
            _fullLine = text;
            _shown = 0;
            SetVisibleText("");
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
                var button = MakeButton(_choices, choice.text.Trim(), () => Choose(index));
                button.GetComponent<LayoutElement>().preferredHeight = 128;
                var label = button.GetComponentInChildren<Text>();
                label.fontSize = 42;
                label.color = Gold;
            }
        }

        void Choose(int index)
        {
            _waitingOnChoice = false;
            ClearChildren(_choices);
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
            BuildDialogueBox(safe.transform);

            var choicesGo = new GameObject("Choices", typeof(RectTransform));
            choicesGo.transform.SetParent(safe.transform, false);
            _choices = AddVertical(choicesGo, 20, new RectOffset(56, 56, 0, 0));
            _choices.GetComponent<VerticalLayoutGroup>().childAlignment = TextAnchor.LowerCenter;

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
        /// Portrait stacks the pose over the dialogue box with the choices above it. Landscape
        /// stands the pose (and any insert) on the left and puts the box and choices on the right.
        /// </summary>
        void Rearrange(bool landscape)
        {
            if (landscape)
            {
                var poseFoot = new Vector2(0.28f, 0);
                Pin(_pose.rectTransform, poseFoot, poseFoot, new Vector2(-380, 0), new Vector2(380, 1040));
                var middle = new Vector2(0.28f, 0.5f);
                Pin(_insert.rectTransform, middle, middle, new Vector2(-360, -360), new Vector2(360, 360));
                var right = new Vector2(0.5f, 0);
                Pin(_box, right, Vector2.right, new Vector2(24, 32), new Vector2(-32, 432));
                Pin(_choices, right, Vector2.right, new Vector2(-32, 460), new Vector2(24, 1060));
            }
            else
            {
                var foot = new Vector2(0.5f, 0);
                Pin(_pose.rectTransform, foot, foot, new Vector2(-500, 150), new Vector2(500, 1650));
                var top = new Vector2(0.5f, 1);
                Pin(_insert.rectTransform, top, top, new Vector2(-390, -1040), new Vector2(390, -260));
                Pin(_box, Vector2.zero, Vector2.right, new Vector2(32, 32), new Vector2(-32, 512));
                Pin(_choices, Vector2.zero, Vector2.right, new Vector2(0, 560), new Vector2(0, 1160));
            }
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
            var box = MakeImage(parent, "Dialogue Box", new Color(0.06f, 0.05f, 0.05f, 0.93f));
            box.sprite = Rounded;
            box.type = Image.Type.Sliced;
            box.raycastTarget = false;
            _box = box.rectTransform;

            var rule = MakeImage(box.transform, "Rule", new Color(Gold.r, Gold.g, Gold.b, 0.5f));
            rule.raycastTarget = false;
            rule.rectTransform.anchorMin = new Vector2(0, 1);
            rule.rectTransform.anchorMax = new Vector2(1, 1);
            rule.rectTransform.offsetMin = new Vector2(48, -100);
            rule.rectTransform.offsetMax = new Vector2(-48, -97);

            _speaker = MakeText(box.transform, "", 44, TextAnchor.LowerLeft, 0);
            _speaker.color = Gold;
            _speaker.fontStyle = FontStyle.Bold;
            _speaker.rectTransform.anchorMin = new Vector2(0, 1);
            _speaker.rectTransform.anchorMax = new Vector2(1, 1);
            _speaker.rectTransform.offsetMin = new Vector2(52, -96);
            _speaker.rectTransform.offsetMax = new Vector2(-52, -24);

            _line = MakeText(box.transform, "", 54, TextAnchor.UpperLeft, 0);
            Stretch(_line.rectTransform);
            _line.rectTransform.offsetMin = new Vector2(52, 64);
            _line.rectTransform.offsetMax = new Vector2(-52, -124);
            _line.lineSpacing = 1.1f;

            _more = MakeText(box.transform, "▼", 36, TextAnchor.LowerRight, 0);
            _more.color = Gold;
            Stretch(_more.rectTransform);
            _more.rectTransform.offsetMin = new Vector2(0, 24);
            _more.rectTransform.offsetMax = new Vector2(-40, 0);
        }
    }
}
