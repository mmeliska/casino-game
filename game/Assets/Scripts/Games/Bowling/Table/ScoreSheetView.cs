using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Starflare.Ui.UiKit;

namespace Starflare.Bowling
{
    /// <summary>
    /// One bowler's line on the scoresheet: the name, ten boxes with the marks and the running
    /// totals, and the score so far. The same strip in portrait and in the landscape column.
    /// </summary>
    public sealed class ScoreSheetView : MonoBehaviour
    {
        TMP_Text _name, _total;
        readonly List<TMP_Text> _marks = new List<TMP_Text>(), _totals = new List<TMP_Text>();
        readonly List<Image> _boxes = new List<Image>();

        static readonly Color Box = new Color(1, 1, 1, 0.08f);
        static readonly Color UpBox = new Color(0.95f, 0.76f, 0.25f, 0.25f);

        public static ScoreSheetView Make(Transform parent, string name)
        {
            var row = MakeRow(parent, name);
            row.GetComponent<LayoutElement>().preferredHeight = 76;
            var group = row.GetComponent<HorizontalLayoutGroup>();
            group.spacing = 4;
            group.childForceExpandHeight = true;
            var view = row.gameObject.AddComponent<ScoreSheetView>();
            view._name = Cell(row, "Name", 28, 120, TextAnchor.MiddleLeft);
            for (int i = 0; i < ScoreSheet.Frames; i++)
            {
                var box = MakeImage(row, $"Frame {i + 1}", Box);
                box.raycastTarget = false;
                box.gameObject.AddComponent<LayoutElement>().flexibleWidth = i == ScoreSheet.Frames - 1 ? 1.4f : 1;
                var marks = MakeText(box.transform, "", 24, TextAnchor.UpperRight, 0);
                marks.name = "Marks";
                NoWrap(marks);
                Pin(marks.rectTransform, new Vector2(0, 0.5f), Vector2.one, new Vector2(2, 0), new Vector2(-4, -2));
                var total = MakeText(box.transform, "", 26, TextAnchor.LowerCenter, 0);
                total.name = "Total";
                NoWrap(total);
                Pin(total.rectTransform, Vector2.zero, new Vector2(1, 0.55f), Vector2.zero, new Vector2(0, 0));
                view._boxes.Add(box);
                view._marks.Add(marks);
                view._totals.Add(total);
            }
            view._total = Cell(row, "Score", 34, 90, TextAnchor.MiddleRight);
            view._total.fontStyle = FontStyles.Bold;
            return view;
        }

        static TMP_Text Cell(Transform row, string name, int size, float width, TextAnchor anchor)
        {
            var text = MakeText(row, "", size, anchor, 0);
            text.name = name;
            NoWrap(text);
            var layout = text.gameObject.AddComponent<LayoutElement>();
            layout.preferredWidth = layout.minWidth = width;
            layout.flexibleWidth = 0;
            return text;
        }

        /// <summary>Shows a sheet under a name; the frame being bowled lights up when it's their turn.</summary>
        public void Show(string who, ScoreSheet sheet, bool up)
        {
            gameObject.SetActive(true);
            _name.text = who;
            _name.color = up ? Gold : Color.white;
            for (int f = 1; f <= ScoreSheet.Frames; f++)
            {
                var frame = sheet.FrameList[f - 1];
                var marks = new List<string>();
                for (int b = 0; b < frame.Balls.Count; b++) marks.Add(sheet.Mark(f, b));
                _marks[f - 1].text = string.Join(" ", marks);
                _totals[f - 1].text = frame.Total.HasValue ? frame.Total.Value.ToString() : "";
                _boxes[f - 1].color = up && !sheet.IsComplete && sheet.CurrentFrame == f ? UpBox : Box;
            }
            _total.text = sheet.Score.ToString();
        }

        public void Hide() => gameObject.SetActive(false);
    }
}
