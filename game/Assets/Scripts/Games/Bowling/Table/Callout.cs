using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Starflare.Ui.UiKit;

namespace Starflare.Bowling
{
    /// <summary>
    /// The big call over the lane (Mike, 2026-11-24): a frame's marks the moment it's bowled,
    /// drawn as the frame on a paper sheet (the first mark plain, the second boxed at its left
    /// and foot, a strike just the X; the tenth's second and third boxed), and your score in big
    /// numbers when the game's over. White on the dark card, a soft white edge (Mike, 2026-11-24). It pops in over the
    /// deck and fades on its own; it never holds up the next throw or takes a tap from the
    /// lane, and a tap on the lane clears it. The score waits its turn behind the tenth's marks.
    /// </summary>
    public sealed class Callout : MonoBehaviour
    {
        /// <summary>How long a frame's marks stay up, seconds.</summary>
        public const float FrameSeconds = 1.3f;

        /// <summary>How long the game's score stays up, seconds.</summary>
        public const float ScoreSeconds = 3.5f;

        const float PopSeconds = 0.18f, FadeSeconds = 0.35f;

        /// <summary>A mark's slot, and the weight of a box's rules.</summary>
        const float SlotWidth = 150, SlotHeight = 170, Rule = 6;

        static readonly Color RuleColour = new Color(0.93f, 0.9f, 0.8f, 0.9f);

        sealed class Call
        {
            public string Over, Big, Under;
            /// <summary>A frame's slots, as boxes; null for the score.</summary>
            public string[] Slots;
            public int Size;
            public float Seconds;
        }

        readonly Queue<Call> _waiting = new Queue<Call>();
        Call _now;
        float _shown;
        CanvasGroup _group;
        RectTransform _card;
        TMP_Text _over, _big, _under;
        RectTransform _boxes;
        readonly List<RectTransform> _slots = new List<RectTransform>();
        readonly List<TMP_Text> _slotMarks = new List<TMP_Text>();
        readonly List<GameObject> _slotRules = new List<GameObject>();

        public bool Showing => _now != null;

        /// <summary>What's being called, the big line: for tests.</summary>
        public string Big => _now?.Big ?? "";

        public static Callout Make(RectTransform frame)
        {
            var go = new GameObject("Callout", typeof(RectTransform), typeof(CanvasGroup));
            go.transform.SetParent(frame, false);
            var view = go.AddComponent<Callout>();
            view._group = go.GetComponent<CanvasGroup>();
            view._group.blocksRaycasts = view._group.interactable = false;
            // Over the deck, where your eye is when the ball gets there.
            view._card = (RectTransform)go.transform;
            Pin(view._card, new Vector2(0.5f, 0.7f), new Vector2(0.5f, 0.7f), Vector2.zero, Vector2.zero);

            var card = MakeImage(go.transform, "Card", new Color(0.04f, 0.035f, 0.05f, 0.92f));
            card.sprite = Rounded;
            card.type = Image.Type.Sliced;
            card.raycastTarget = false;
            Stretch(card.rectTransform);
            var edge = MakeImage(go.transform, "Edge", new Color(1, 1, 1, 0.6f));
            edge.sprite = RoundedFrame;
            edge.type = Image.Type.Sliced;
            edge.raycastTarget = false;
            Stretch(edge.rectTransform);

            view._over = Line(go.transform, "Callout Over", 34, Color.white);
            Pin(view._over.rectTransform, new Vector2(0, 1), Vector2.one, new Vector2(16, -64), new Vector2(-16, -14));
            view._big = Line(go.transform, "Callout Big", 160, Color.white);
            view._big.fontStyle = FontStyles.Bold;
            Stretch(view._big.rectTransform);
            view._boxes = (RectTransform)new GameObject("Boxes", typeof(RectTransform)).transform;
            view._boxes.SetParent(go.transform, false);
            Stretch(view._boxes);
            for (int i = 0; i < 3; i++) view.MakeSlot(i);
            view._under = Line(go.transform, "Callout Under", 34, Color.white);
            Pin(view._under.rectTransform, Vector2.zero, new Vector2(1, 0), new Vector2(16, 14), new Vector2(-16, 64));
            go.SetActive(false);
            return view;
        }

        /// <summary>One mark's slot, with the rules down its left and along its foot.</summary>
        void MakeSlot(int i)
        {
            var slot = (RectTransform)new GameObject($"Slot {i + 1}", typeof(RectTransform)).transform;
            slot.SetParent(_boxes, false);
            slot.anchorMin = slot.anchorMax = new Vector2(0.5f, 0.5f);
            slot.sizeDelta = new Vector2(SlotWidth, SlotHeight);
            var rules = new GameObject("Rules", typeof(RectTransform));
            rules.transform.SetParent(slot, false);
            Stretch((RectTransform)rules.transform);
            var left = MakeImage(rules.transform, "Left", RuleColour);
            left.raycastTarget = false;
            Pin(left.rectTransform, Vector2.zero, new Vector2(0, 1), Vector2.zero, new Vector2(Rule, 0));
            var foot = MakeImage(rules.transform, "Foot", RuleColour);
            foot.raycastTarget = false;
            Pin(foot.rectTransform, Vector2.zero, new Vector2(1, 0), Vector2.zero, new Vector2(0, Rule));
            var mark = Line(slot, "Mark", 130, Color.white);
            mark.fontStyle = FontStyles.Bold;
            Pin(mark.rectTransform, Vector2.zero, Vector2.one, new Vector2(Rule, Rule), Vector2.zero);
            _slots.Add(slot);
            _slotMarks.Add(mark);
            _slotRules.Add(rules);
        }

        static TMP_Text Line(Transform parent, string name, int size, Color colour)
        {
            var text = MakeText(parent, "", size, TextAnchor.MiddleCenter, 0);
            text.name = name;
            NoWrap(text);
            text.color = colour;
            text.raycastTarget = false;
            Shadowed(text);
            return text;
        }

        /// <summary>
        /// A frame's slots as boxes (<see cref="ScoreSheet.FrameBoxes"/>), under the bowler's name
        /// when it isn't you; <paramref name="marks"/> is the same frame in a line, for tests. It
        /// takes over from an older call: they never back up.
        /// </summary>
        public void Frame(string who, string[] slots, string marks)
        {
            Clear();
            Add(new Call { Over = who ?? "", Big = marks, Under = "", Slots = slots, Size = (int)SlotHeight, Seconds = FrameSeconds });
        }

        /// <summary>Your score at the end of a game, with the other side's under it when there is one.</summary>
        public void Score(int score, string under) =>
            Add(new Call { Over = "Your game", Big = score.ToString(), Under = under ?? "", Size = 200, Seconds = ScoreSeconds });

        /// <summary>Clears what's up and what's waiting: a new game, or getting up.</summary>
        public void Clear()
        {
            _waiting.Clear();
            _now = null;
            gameObject.SetActive(false);
        }

        /// <summary>A tap on the lane: the next call, or none.</summary>
        public void Skip()
        {
            if (_now == null) return;
            Next();
        }

        void Add(Call call)
        {
            _waiting.Enqueue(call);
            if (_now == null) Next();
        }

        void Next()
        {
            if (_waiting.Count == 0)
            {
                Clear();
                return;
            }
            _now = _waiting.Dequeue();
            _shown = 0;
            _over.text = _now.Over;
            _under.text = _now.Under;
            // The middle sits between the name over it and the line under it, when they're there.
            var middle = new Vector2(0, _now.Under.Length > 0 ? 60 : 0);
            var top = new Vector2(0, _now.Over.Length > 0 ? -60 : 0);
            bool boxes = _now.Slots != null;
            _big.gameObject.SetActive(!boxes);
            _boxes.gameObject.SetActive(boxes);
            Pin(_big.rectTransform, Vector2.zero, Vector2.one, middle, top);
            Pin(_boxes, Vector2.zero, Vector2.one, middle, top);
            float wide;
            if (boxes)
            {
                int n = _now.Slots.Length;
                for (int i = 0; i < _slots.Count; i++)
                {
                    bool used = i < n;
                    _slots[i].gameObject.SetActive(used);
                    if (!used) continue;
                    _slots[i].anchoredPosition = new Vector2((i - (n - 1) / 2f) * SlotWidth, 0);
                    _slotMarks[i].text = _now.Slots[i];
                    // The first mark stands plain; the ones after it are boxed.
                    _slotRules[i].SetActive(i > 0);
                }
                wide = n * SlotWidth + 100;
            }
            else
            {
                _big.text = _now.Big;
                _big.fontSize = _now.Size;
                wide = Mathf.Max(380, _now.Big.Length * _now.Size * 0.62f + 120);
            }
            float tall = _now.Size + 60 - top.y + middle.y;
            _card.sizeDelta = new Vector2(wide, tall);
            gameObject.SetActive(true);
            Animate();
        }

        void Update()
        {
            if (_now == null) return;
            _shown += Time.unscaledDeltaTime;
            if (_shown >= _now.Seconds)
            {
                Next();
                return;
            }
            Animate();
        }

        /// <summary>A quick pop from a touch larger, then a fade at the end.</summary>
        void Animate()
        {
            float pop = Mathf.Clamp01(_shown / PopSeconds);
            float scale = Mathf.Lerp(1.25f, 1, 1 - (1 - pop) * (1 - pop));
            _card.localScale = new Vector3(scale, scale, 1);
            float left = _now.Seconds - _shown;
            _group.alpha = Mathf.Min(pop * 1.5f, Mathf.Clamp01(left / FadeSeconds));
        }
    }
}
