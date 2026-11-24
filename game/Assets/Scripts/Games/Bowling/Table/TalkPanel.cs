using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Starflare.Ui.UiKit;

namespace Starflare.Bowling
{
    /// <summary>
    /// A regular's face with a line beside it, in the block under the lane (Mike, 2026-11-24):
    /// what he says over a challenge ("Game for ten?"), and while he's up, "Leon's Turn" in
    /// place of your meters. Until his face is drawn, the card shows his initial on a disc, as
    /// the back lot's circle does.
    /// </summary>
    public sealed class TalkPanel : MonoBehaviour
    {
        const float CardSize = 168;

        static readonly Color CardEdge = new Color(1, 1, 1, 0.3f);
        static readonly Color CardFill = new Color(0.32f, 0.32f, 0.34f);

        Image _portrait, _disc;
        TMP_Text _initial, _title, _line;

        public RectTransform Rect => (RectTransform)transform;

        /// <summary>The name or "Leon's Turn": for tests.</summary>
        public string Title => _title.text;

        public string Line => _line.text;

        public static TalkPanel Make(Transform parent)
        {
            var row = MakeRow(parent, "Talk");
            row.GetComponent<LayoutElement>().preferredHeight = CardSize + 16;
            var group = row.GetComponent<HorizontalLayoutGroup>();
            group.spacing = 24;
            group.childForceExpandWidth = false;
            group.childAlignment = TextAnchor.MiddleLeft;
            var panel = row.gameObject.AddComponent<TalkPanel>();
            panel.Build(row);
            return panel;
        }

        void Build(RectTransform row)
        {
            var frame = MakeImage(row, "Face", CardEdge);
            frame.sprite = Rounded;
            frame.type = Image.Type.Sliced;
            frame.raycastTarget = false;
            var size = frame.gameObject.AddComponent<LayoutElement>();
            size.preferredWidth = size.minWidth = CardSize;
            size.preferredHeight = size.minHeight = CardSize;

            var card = MakeImage(frame.transform, "Card", CardFill);
            card.sprite = Rounded;
            card.type = Image.Type.Sliced;
            card.raycastTarget = false;
            Pin(card.rectTransform, Vector2.zero, Vector2.one, new Vector2(5, 5), new Vector2(-5, -5));
            card.gameObject.AddComponent<Mask>().showMaskGraphic = true;

            _portrait = MakeImage(card.transform, "Portrait", Color.white);
            _portrait.raycastTarget = false;
            _portrait.preserveAspect = true;
            Stretch(_portrait.rectTransform);

            _disc = MakeImage(card.transform, "Disc", new Color(0.2f, 0.2f, 0.22f));
            _disc.sprite = Circle;
            _disc.raycastTarget = false;
            _disc.rectTransform.sizeDelta = new Vector2(104, 104);
            _initial = MakeText(_disc.transform, "", 64, TextAnchor.MiddleCenter, 0);
            _initial.fontStyle = FontStyles.Bold;
            Stretch(_initial.rectTransform);

            var words = new GameObject("Words", typeof(RectTransform), typeof(LayoutElement));
            words.transform.SetParent(row, false);
            words.GetComponent<LayoutElement>().flexibleWidth = 1;
            var column = AddVertical(words, 6, new RectOffset());
            column.GetComponent<VerticalLayoutGroup>().childAlignment = TextAnchor.MiddleLeft;

            _title = MakeText(column, "", 40, TextAnchor.MiddleLeft, 52);
            _title.name = "Talk Title";
            _title.fontStyle = FontStyles.Bold;
            _title.color = Gold;
            Shadowed(_title);
            _line = MakeText(column, "", 36, TextAnchor.UpperLeft, 0);
            _line.name = "Talk Line";
            _line.enableAutoSizing = true;
            _line.fontSizeMin = 26;
            _line.fontSizeMax = 36;
            _line.GetComponent<RectTransform>().gameObject.AddComponent<LayoutElement>().preferredHeight = 100;
            Shadowed(_line);
        }

        /// <summary>A face, a title (his name, or whose turn it is) and what he says, if anything.</summary>
        public void Show(string name, Sprite face, string title, string line)
        {
            _portrait.sprite = face;
            _portrait.enabled = face != null;
            _disc.enabled = face == null;
            _initial.text = face != null || string.IsNullOrEmpty(name) ? "" : name.Substring(0, 1).ToUpperInvariant();
            _title.text = title;
            _line.text = string.IsNullOrEmpty(line) ? "" : $"“{line}”";
            _line.gameObject.SetActive(!string.IsNullOrEmpty(line));
            gameObject.SetActive(true);
        }

        public void Hide() => gameObject.SetActive(false);
    }
}
