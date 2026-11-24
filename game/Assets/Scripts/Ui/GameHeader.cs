using Starflare.State;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static Starflare.Ui.UiKit;

namespace Starflare.Ui
{
    /// <summary>
    /// The bar across the top of every game: your cash (and what you owe), the date and time, and
    /// the way out. One look for all the games, the glass of the place screens' bar (<see cref="Hud"/>):
    /// a rounded dark panel with a teal edge, the cash glyph, padding all round, solid enough to read
    /// over any table, lane or machine (Mike, 2026-11-24). The game says what the numbers are; the
    /// header only shows them.
    /// </summary>
    public sealed class GameHeader : MonoBehaviour
    {
        public const float Height = 128;

        TMP_Text _cash, _owed, _clock;

        /// <summary>The way out. A game registers it with its coach like its other controls.</summary>
        public Button Leave { get; private set; }

        public RectTransform Rect => (RectTransform)transform;

        /// <summary>A header in a layout column, its Leave game button calling <paramref name="leave"/> (or nothing, for a game that wires it to its coach itself).</summary>
        public static GameHeader Make(Transform parent, UnityAction leave)
        {
            var panel = MakePanel(parent, "Header", new Color(Panel.r, Panel.g, Panel.b, 0.95f));
            panel.raycastTarget = false;
            var layout = panel.gameObject.AddComponent<LayoutElement>();
            layout.preferredHeight = layout.minHeight = Height;
            layout.flexibleHeight = 0;
            var row = panel.gameObject.AddComponent<HorizontalLayoutGroup>();
            row.padding = new RectOffset(26, 16, 14, 14);
            row.spacing = 18;
            row.childAlignment = TextAnchor.MiddleLeft;
            row.childControlWidth = row.childControlHeight = true;
            row.childForceExpandWidth = row.childForceExpandHeight = false;
            panel.transform.Find("Edge").gameObject.AddComponent<LayoutElement>().ignoreLayout = true;
            var header = panel.gameObject.AddComponent<GameHeader>();

            var icon = MakeImage(panel.transform, "Icon", Paper);
            icon.sprite = CashGlyph;
            icon.raycastTarget = false;
            var iconLayout = icon.gameObject.AddComponent<LayoutElement>();
            iconLayout.preferredWidth = iconLayout.preferredHeight = 56;

            var info = new GameObject("Info", typeof(RectTransform), typeof(LayoutElement));
            info.transform.SetParent(panel.transform, false);
            info.GetComponent<LayoutElement>().flexibleWidth = 1;
            var lines = AddVertical(info, 2, new RectOffset()).GetComponent<VerticalLayoutGroup>();
            lines.childAlignment = TextAnchor.MiddleLeft;

            var money = new GameObject("Money", typeof(RectTransform), typeof(LayoutElement));
            money.transform.SetParent(info.transform, false);
            money.GetComponent<LayoutElement>().preferredHeight = 52;
            var moneyRow = money.AddComponent<HorizontalLayoutGroup>();
            moneyRow.spacing = 18;
            moneyRow.childAlignment = TextAnchor.LowerLeft;
            moneyRow.childControlWidth = moneyRow.childControlHeight = true;
            moneyRow.childForceExpandWidth = moneyRow.childForceExpandHeight = false;
            header._cash = MakeText(money.transform, "", 44, TextAnchor.LowerLeft, 0);
            header._cash.name = "Cash";
            header._cash.fontStyle = FontStyles.Bold;
            NoWrap(header._cash);
            header._owed = MakeText(money.transform, "", 28, TextAnchor.LowerLeft, 0);
            header._owed.name = "Owed";
            header._owed.color = Gold;
            NoWrap(header._owed);

            header._clock = MakeText(info.transform, "", 30, TextAnchor.MiddleLeft, 40);
            header._clock.name = "Clock";
            header._clock.color = new Color(Paper.r, Paper.g, Paper.b, 0.85f);
            NoWrap(header._clock);
            header._clock.overflowMode = TextOverflowModes.Ellipsis;

            header.Leave = MakeButton(panel.transform, "Leave game", leave ?? (() => { }));
            var leaveLayout = header.Leave.GetComponent<LayoutElement>();
            leaveLayout.preferredHeight = leaveLayout.minHeight = Height - 28;
            leaveLayout.preferredWidth = leaveLayout.minWidth = 250;
            leaveLayout.flexibleWidth = 0;
            var label = header.Leave.GetComponentInChildren<TMP_Text>();
            label.fontSize = 38;
            NoWrap(label);
            header.Leave.GetComponent<Image>().color = new Color(1, 1, 1, 0.06f);
            var edge = MakeImage(header.Leave.transform, "Edge", new Color(1, 1, 1, 0.6f));
            edge.sprite = RoundedFrame;
            edge.type = Image.Type.Sliced;
            edge.raycastTarget = false;
            Stretch(edge.rectTransform);
            edge.transform.SetAsFirstSibling();
            return header;
        }

        /// <summary>
        /// Shows the cash and what's owed (nothing shows when nothing is), and the clock line: the
        /// date and time, and anything the game adds after it (" (late)").
        /// </summary>
        public void Show(int cash, int owed, GameClock clock, string after = "") =>
            Show(Money(cash), owed > 0 ? $"Owed {Money(owed)}" : "", ClockLine(clock, after));

        /// <summary>
        /// Shows a game's own words in the money line (a coached roll's "Practice"), with a gold
        /// note beside it ("Down $10"), and a clock line.
        /// </summary>
        public void Show(string money, string beside, string clockLine, Color? moneyColour = null)
        {
            _cash.text = money;
            _cash.color = moneyColour ?? Color.white;
            _owed.text = beside ?? "";
            _owed.gameObject.SetActive(!string.IsNullOrEmpty(beside));
            _clock.text = clockLine;
        }

        /// <summary>The date and time, as the place screens' bar shows them, and anything after (" (late)").</summary>
        public static string ClockLine(GameClock clock, string after = "") =>
            clock == null ? after : $"{clock.DateLabel}  •  {clock.TimeLabel}{after}";
    }
}
