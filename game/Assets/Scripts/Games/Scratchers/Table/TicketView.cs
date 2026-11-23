using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Starflare.Ui.UiKit;

namespace Starflare.Scratchers
{
    /// <summary>
    /// One bought ticket's face, laid out the way a gas-station scratcher is: its name, the
    /// price in a badge, how to play, the play area with every spot under latex, and the odds
    /// in small print at the foot. With the ticket's art the card takes the picture's shape,
    /// 800 across upright or 1200 on its side, and everything is printed on the flat colour
    /// below the picture; on the wide card the name, how to play and the odds stand to the
    /// left of the play area. Without art it's an 800 by 1200 card, printed all over. The
    /// counter scales the card to fit the screen.
    /// </summary>
    public sealed class TicketView
    {
        public const float TallWidth = 800, WideWidth = 1200, PrintedHeight = 1200;

        /// <summary>Where each part sits, as a share of the card (0-1 from the bottom left).</summary>
        sealed class Layout
        {
            public Rect Name, HowTo, Play, Fine;
            public Vector2 Badge;
            public float BadgeSize;
            public bool Printed, NameWraps;
        }

        readonly List<ScratchCoat> _coats = new List<ScratchCoat>();
        readonly List<Image> _winFrames = new List<Image>();
        readonly TicketLook _look;
        readonly Sprite _art;
        readonly Layout _layout;
        RectTransform _panel;
        Vector2 _playSize;

        /// <param name="wide">The phone's on its side: the landscape card, if the ticket has one.</param>
        public TicketView(Transform parent, ScratchCard card, TicketLook look, string serial, bool wide = false)
        {
            Card = card;
            Serial = serial;
            _look = look ?? new TicketLook();
            Wide = wide && _look.WideArt != null;
            _art = Wide ? _look.WideArt : _look.Art;

            var go = new GameObject($"Ticket {card.Ticket.Name}", typeof(RectTransform), typeof(Image), typeof(ScratchPad));
            go.transform.SetParent(parent, false);
            Rect = (RectTransform)go.transform;
            float width = Wide ? WideWidth : TallWidth;
            float height = _art == null ? PrintedHeight : width * _art.rect.height / _art.rect.width;
            Rect.sizeDelta = new Vector2(width, height);
            go.GetComponent<Image>().color = Color.clear;
            Pad = go.GetComponent<ScratchPad>();
            Pad.Stroke += Scratch;

            _layout = _art == null ? Printed() : Wide ? OnWideArt(_look.WideHeader) : OnTallArt(_look.Header);
            PrintFace(serial);
            PrintPlay();
        }

        public ScratchCard Card { get; }

        /// <summary>The number printed at the foot, kept when the ticket turns to its other card.</summary>
        public string Serial { get; }
        public RectTransform Rect { get; }
        public ScratchPad Pad { get; }
        public IReadOnlyList<ScratchCoat> Coats => _coats;

        /// <summary>This is the landscape card.</summary>
        public bool Wide { get; }

        /// <summary>The play area, as a share of the card, for checking it keeps clear of the picture.</summary>
        public Rect PlayArea => _layout.Play;

        public bool AllRevealed => _coats.All(c => c.Revealed);

        public void Scratch(Vector2 screenFrom, Vector2 screenTo, Camera camera)
        {
            foreach (var coat in _coats) coat.Scratch(screenFrom, screenTo, camera);
        }

        public void RevealAll()
        {
            foreach (var coat in _coats) coat.Reveal();
        }

        /// <summary>
        /// The same ticket turned the other way: whatever spots were read on the old card are
        /// read on this one. A spot only part scratched comes back covered; the cards' latex
        /// isn't the same shape.
        /// </summary>
        public void RevealAsOn(TicketView other)
        {
            for (int i = 0; i < _coats.Count && i < other._coats.Count; i++)
                if (other._coats[i].Revealed) _coats[i].Reveal();
        }

        public void Tick(float seconds)
        {
            foreach (var coat in _coats) coat.Tick(seconds);
        }

        /// <summary>The winning spots (and the number they matched) ringed in gold.</summary>
        public void ShowWin()
        {
            foreach (var frame in _winFrames) frame.enabled = true;
        }

        public void Destroy()
        {
            foreach (var coat in _coats) coat.Dispose();
            Object.Destroy(Rect.gameObject);
        }

        // ---- Layouts ----

        /// <summary>No art: the name across the top, the play area filling the middle.</summary>
        static Layout Printed() => new Layout
        {
            Printed = true,
            Name = new Rect(0.06f, 0.83f, 0.72f, 0.14f),
            HowTo = new Rect(0.07f, 0.74f, 0.86f, 0.085f),
            Play = new Rect(0.05f, 0.12f, 0.9f, 0.61f),
            Fine = new Rect(0.07f, 0.025f, 0.86f, 0.085f),
            Badge = new Vector2(0.885f, 0.9f),
            BadgeSize = 144,
        };

        /// <summary>Upright art: the picture on top, then the name, how to play, the play area and the odds stacked below it.</summary>
        Layout OnTallArt(float header)
        {
            float top = 1 - header;
            return new Layout
            {
                Name = new Rect(0.07f, top - 0.08f, 0.86f, 0.07f),
                HowTo = new Rect(0.08f, top - 0.135f, 0.84f, 0.05f),
                Play = new Rect(0.07f, 0.105f, 0.86f, top - 0.145f - 0.105f),
                Fine = new Rect(0.08f, 0.035f, 0.84f, 0.06f),
                Badge = Corner(64),
                BadgeSize = 120,
            };
        }

        /// <summary>Art on its side: the picture across the top; below it the name, how to play and the odds on the left, the play area on the right.</summary>
        Layout OnWideArt(float header)
        {
            float top = 1 - header;
            return new Layout
            {
                NameWraps = true,
                Name = new Rect(0.06f, top - 0.2f, 0.28f, 0.17f),
                HowTo = new Rect(0.06f, 0.16f, 0.28f, top - 0.22f - 0.16f),
                Play = new Rect(0.37f, 0.06f, 0.57f, top - 0.09f),
                Fine = new Rect(0.06f, 0.05f, 0.28f, 0.1f),
                Badge = Corner(76),
                BadgeSize = 108,
            };
        }

        /// <summary>The badge's centre this far in from the card's top right corner, in card units.</summary>
        Vector2 Corner(float inset) =>
            new Vector2(1 - inset / Rect.sizeDelta.x, 1 - inset / Rect.sizeDelta.y);

        // ---- The face ----

        void PrintFace(string serial)
        {
            var ticket = Card.Ticket;
            if (_layout.Printed)
            {
                var paper = MakeImage(Rect, "Paper", _look.Bottom);
                Stretch(paper.rectTransform);
                var sweep = MakeImage(Rect, "Sweep", _look.Top);
                sweep.sprite = Fade;
                sweep.raycastTarget = false;
                Stretch(sweep.rectTransform);
                var border = MakeImage(Rect, "Border", _look.Accent);
                border.sprite = RoundedFrame;
                border.type = Image.Type.Sliced;
                border.pixelsPerUnitMultiplier = 0.5f;
                border.raycastTarget = false;
                Pin(border.rectTransform, Vector2.zero, Vector2.one, new Vector2(16, 16), new Vector2(-16, -16));
            }
            else
            {
                var art = MakeImage(Rect, "Art", Color.white);
                art.sprite = _art;
                art.raycastTarget = false;
                Stretch(art.rectTransform);
            }

            var title = Label(Rect, "Name", ticket.Name, _layout.Name, 110, _look.TitleInk);
            title.fontStyle = FontStyles.Bold | FontStyles.Italic;
            title.fontSizeMin = 40;
            title.outlineWidth = 0.22f;
            title.outlineColor = _look.TitleOutline;
            if (_layout.NameWraps) title.textWrappingMode = TextWrappingModes.Normal;

            var badge = MakeImage(Rect, "Price Badge", _look.Accent);
            badge.sprite = Circle;
            badge.raycastTarget = false;
            float half = _layout.BadgeSize / 2;
            Pin(badge.rectTransform, _layout.Badge, _layout.Badge, new Vector2(-half, -half), new Vector2(half, half));
            var price = Label(badge.rectTransform, "Price", Money(ticket.Price), new Rect(0, 0, 1, 1), 66, _look.BadgeInk);
            price.fontStyle = FontStyles.Bold;

            var how = Label(Rect, "How To Play", ticket.HowToPlay, _layout.HowTo, 32, _look.TitleInk);
            how.fontStyle = FontStyles.Bold;
            how.textWrappingMode = TextWrappingModes.Normal;
            how.outlineWidth = 0.18f;
            how.outlineColor = _look.TitleOutline;

            string odds = $"Overall odds 1 in {ticket.OddsOneIn:0.00}.  Top prize {Money(ticket.TopPrize)}.";
            var fine = Label(Rect, "Fine Print", $"{odds}\nTicket {serial}", _layout.Fine, 24, _look.FootInk);
            fine.textWrappingMode = TextWrappingModes.Normal;

            var panel = MakeImage(Rect, "Play Area", _look.Panel);
            panel.sprite = Rounded;
            panel.type = Image.Type.Sliced;
            panel.raycastTarget = false;
            _panel = panel.rectTransform;
            Place(_panel, _layout.Play);
            _playSize = new Vector2(Rect.sizeDelta.x * _layout.Play.width, Rect.sizeDelta.y * _layout.Play.height);
            var edge = MakeImage(_panel, "Edge", _look.Accent);
            edge.sprite = RoundedFrame;
            edge.type = Image.Type.Sliced;
            edge.raycastTarget = false;
            Stretch(edge.rectTransform);
        }

        // ---- The play area ----

        void PrintPlay()
        {
            var ticket = Card.Ticket;
            var spots = new Rect(0.03f, 0.03f, 0.94f, 0.94f);
            if (Card.Key.Count > 0)
            {
                Heading(ticket.KeyLabel, new Rect(0.04f, 0.885f, 0.92f, 0.09f));
                PrintKey(new Rect(0.08f, 0.68f, 0.84f, 0.2f));
                var rule = MakeImage(_panel, "Rule", _look.PanelInk);
                rule.raycastTarget = false;
                Place(rule.rectTransform, new Rect(0.06f, 0.664f, 0.88f, 0.004f));
                Heading(ticket.YourLabel, new Rect(0.04f, 0.565f, 0.92f, 0.09f));
                spots = new Rect(0.03f, 0.03f, 0.94f, 0.53f);
            }
            else if (!string.IsNullOrEmpty(ticket.YourLabel))
            {
                Heading(ticket.YourLabel, new Rect(0.04f, 0.885f, 0.92f, 0.09f));
                spots = new Rect(0.03f, 0.03f, 0.94f, 0.85f);
            }
            PrintSpots(spots);
        }

        void Heading(string text, Rect at)
        {
            var heading = Label(_panel, "Heading", text, at, 34, _look.PanelInk);
            heading.fontStyle = FontStyles.Bold;
            heading.characterSpacing = 4;
        }

        /// <summary>The winning numbers, or their one number, each under its own latex.</summary>
        void PrintKey(Rect row)
        {
            var key = Card.Key;
            float width = Mathf.Min(row.width / key.Count, 0.24f);
            float left = row.x + (row.width - width * key.Count) / 2;
            int winning = Card.Spots.FirstOrDefault(s => s.Wins)?.Number ?? 0;
            for (int i = 0; i < key.Count; i++)
            {
                var at = new Rect(left + width * i + 0.012f, row.y, width - 0.024f, row.height);
                var face = Face($"Key {i + 1}", at);
                if (_look.KeyMark != null)
                {
                    Picture(face, _look.KeyMark, new Rect(0.08f, 0.4f, 0.84f, 0.57f));
                    Label(face, "Number", key[i].ToString(), new Rect(0.04f, 0.03f, 0.92f, 0.38f), 56, _look.PanelInk).fontStyle = FontStyles.Bold;
                }
                else
                {
                    Backing(face);
                    Label(face, "Number", key[i].ToString(), new Rect(0, 0, 1, 1), 76, _look.PanelInk).fontStyle = FontStyles.Bold;
                }
                WinFrame(at, Card.Ticket.Play == ScratchPlay.LuckyNumbers && key[i] == winning);
                Coat($"Key Coat {i + 1}", at);
            }
        }

        /// <summary>
        /// Your spots. A symbol with a picture shows the picture, captioned with its word; a
        /// number shows the spot's own picture above it where the ticket has them (a perch, a
        /// horse in red silks), else the ticket's faint backing behind it.
        /// </summary>
        void PrintSpots(Rect area)
        {
            var spots = Card.Spots;
            int columns = Columns(spots.Count), rows = (spots.Count + columns - 1) / columns;
            float w = area.width / columns, h = area.height / rows;
            bool amountsOnly = Card.Ticket.Play == ScratchPlay.MatchThree;
            for (int i = 0; i < spots.Count; i++)
            {
                int column = i % columns, row = i / columns;
                var at = new Rect(area.x + w * column + 0.01f, area.yMax - h * (row + 1) + 0.01f, w - 0.02f, h - 0.02f);
                var face = Face($"Spot {i + 1}", at);
                var spot = spots[i];
                var picture = amountsOnly ? null
                    : Card.Ticket.Play == ScratchPlay.FindSymbol ? (_look.Symbols.TryGetValue(spot.Symbol, out var drawn) ? drawn : null)
                    : _look.MarkFor(i);
                if (amountsOnly)
                {
                    Backing(face);
                    var amount = Label(face, "Amount", Money(spot.Prize), new Rect(0.04f, 0.1f, 0.92f, 0.8f), 72, _look.PanelInk);
                    amount.fontStyle = FontStyles.Bold;
                }
                else if (picture != null)
                {
                    Picture(face, picture, new Rect(0.1f, 0.42f, 0.8f, 0.55f));
                    bool caption = Card.Ticket.Play == ScratchPlay.FindSymbol;
                    var symbol = Label(face, "Symbol", spot.Symbol, new Rect(0.04f, 0.22f, 0.92f, 0.2f), caption ? 28 : 48, _look.PanelInk);
                    symbol.fontStyle = FontStyles.Bold;
                    if (caption) symbol.characterSpacing = 3;
                    Label(face, "Prize", Money(spot.Prize), new Rect(0.04f, 0.03f, 0.92f, 0.2f), 34, _look.PanelInk);
                }
                else
                {
                    if (Card.Ticket.Play == ScratchPlay.LuckyNumbers) Backing(face);
                    var symbol = Label(face, "Symbol", spot.Symbol, new Rect(0.04f, 0.36f, 0.92f, 0.6f), 72, _look.PanelInk);
                    symbol.fontStyle = FontStyles.Bold;
                    Label(face, "Prize", Money(spot.Prize), new Rect(0.04f, 0.04f, 0.92f, 0.32f), 36, _look.PanelInk);
                }
                WinFrame(at, spot.Wins);
                Coat($"Coat {i + 1}", at);
            }
        }

        /// <summary>A picture fitted to a box in the spot, its shape kept.</summary>
        static void Picture(RectTransform face, Sprite sprite, Rect at)
        {
            var image = MakeImage(face, "Picture", Color.white);
            image.sprite = sprite;
            image.preserveAspect = true;
            image.raycastTarget = false;
            Place(image.rectTransform, at);
        }

        /// <summary>The ticket's backing, faint behind an amount or a number, where it has one.</summary>
        void Backing(RectTransform face)
        {
            if (_look.Backing == null) return;
            var image = MakeImage(face, "Backing", new Color(1, 1, 1, 0.3f));
            image.sprite = _look.Backing;
            image.preserveAspect = true;
            image.raycastTarget = false;
            Place(image.rectTransform, new Rect(0.1f, 0.08f, 0.8f, 0.84f));
        }

        /// <summary>How many spots across, so the play area reads like a real ticket's grid.</summary>
        static int Columns(int spots) => spots switch
        {
            <= 3 => spots,
            4 => 2,
            6 or 9 => 3,
            8 or 12 => 4,
            10 or 15 => 5,
            _ => Mathf.CeilToInt(Mathf.Sqrt(spots)),
        };

        /// <summary>The printed spot under the latex.</summary>
        RectTransform Face(string name, Rect at)
        {
            var face = MakeImage(_panel, name, Color.Lerp(_look.Panel, Color.white, 0.6f));
            face.sprite = Rounded;
            face.type = Image.Type.Sliced;
            face.pixelsPerUnitMultiplier = 2;
            face.raycastTarget = false;
            Place(face.rectTransform, at);
            return face.rectTransform;
        }

        void Coat(string name, Rect at)
        {
            var coat = new ScratchCoat(_panel, name, at.width * _playSize.x, at.height * _playSize.y, _look.Coat, Card.GetHashCode() ^ _coats.Count);
            Place(coat.Rect, at);
            _coats.Add(coat);
        }

        void WinFrame(Rect at, bool wins)
        {
            if (!wins) return;
            var frame = MakeImage(_panel, "Win", Gold);
            frame.sprite = RoundedFrame;
            frame.type = Image.Type.Sliced;
            frame.pixelsPerUnitMultiplier = 0.4f;
            frame.raycastTarget = false;
            Place(frame.rectTransform, at);
            frame.rectTransform.offsetMin = new Vector2(-6, -6);
            frame.rectTransform.offsetMax = new Vector2(6, 6);
            frame.enabled = false;
            _winFrames.Add(frame);
        }

        /// <summary>Text fitted to a box in its parent's 0-1 space, shrinking to fit rather than spilling.</summary>
        static TMP_Text Label(RectTransform parent, string name, string text, Rect at, int size, Color ink)
        {
            var label = MakeText(parent, text, size, TextAnchor.MiddleCenter, 0);
            label.name = name;
            label.color = ink;
            label.enableAutoSizing = true;
            label.fontSizeMin = Mathf.Min(16, size);
            label.fontSizeMax = size;
            NoWrap(label);
            Place(label.rectTransform, at);
            return label;
        }

        /// <summary>Anchors a rect to a box in its parent's 0-1 space.</summary>
        static void Place(RectTransform rect, Rect at) =>
            Pin(rect, new Vector2(at.xMin, at.yMin), new Vector2(at.xMax, at.yMax), Vector2.zero, Vector2.zero);
    }
}
