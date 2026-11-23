using System;
using System.Collections.Generic;
using System.Linq;
using Starflare.State;
using Starflare.Ui;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Starflare.Ui.UiKit;

namespace Starflare.Scratchers
{
    /// <summary>
    /// The scratcher rack behind a counter: pick a ticket off the roll, pay, scratch it. A ticket
    /// settles into the player state the moment it's bought (<see cref="ScratchCard"/>);
    /// scratching only shows it. Like the machines it raises a break point when you get up, and
    /// never reads story state or saves. UI built in code, portrait or landscape.
    /// </summary>
    public sealed class ScratcherCounter : MonoBehaviour
    {
        const float Gap = 18;

        /// <summary>Raised at a break point, where story beats and seed reveals may fire.</summary>
        public event Action<BreakPoint> BreakPointReached;

        public IReadOnlyList<ScratchTicket> Roll { get; private set; }

        /// <summary>The cheapest ticket on the roll: less cash than this and you're done here.</summary>
        public int CheapestPrice => Roll.Min(t => t.Price);

        /// <summary>This sitting so far; still readable at the break point after you get up.</summary>
        public int SittingNet => _state == null ? 0 : _state.BankrollNow - _sittingStartBankroll;
        public int SittingRounds { get; private set; }

        /// <summary>The ticket in your hand, settled; null at the rack.</summary>
        public ScratchCard Card => _ticket?.Card;

        /// <summary>The ticket in your hand is all scratched off.</summary>
        public bool Finished { get; private set; }

        public TicketView Ticket => _ticket;

        /// <summary>One ticket on the rack: a button with its picture, name and top prize.</summary>
        sealed class Tile
        {
            public ScratchTicket Ticket;
            public TicketLook Look;
            public Button Button;
            public CanvasGroup Group;
            public RawImage Picture;
        }

        PlayerState _state;
        Sitting _sitting;
        System.Random _random;
        IReadOnlyDictionary<string, TicketLook> _looks;
        int _sittingStartBankroll;
        bool _landscape;

        GameObject _root;
        RectTransform _column, _body, _stage, _side, _rack, _holder, _buttonsRow, _buttonsColumn;
        GridLayoutGroup _rackGrid;
        TMP_Text _title, _bankrollText, _clockText, _statusText;
        Button _getUpButton, _revealButton, _anotherButton, _rackButton;
        readonly List<Tile> _tiles = new List<Tile>();
        TicketView _ticket;

        Sprite _plate;
        float _plateFocus;
        Image _plateImage;
        CoverFit _plateFit;

        /// <summary>
        /// Sets the counter up over the venue's plate with its roll of tickets and how each one
        /// looks. It stays hidden until you step up. A seeded random is for tests.
        /// </summary>
        public void Init(IReadOnlyList<ScratchTicket> roll, Sprite plate, float plateFocus,
            IReadOnlyDictionary<string, TicketLook> looks = null, System.Random random = null)
        {
            if (roll == null || roll.Count == 0) throw new ArgumentException("A counter needs tickets.", nameof(roll));
            Roll = roll;
            _looks = looks ?? new Dictionary<string, TicketLook>();
            _plate = plate;
            _plateFocus = plateFocus;
            _random = random ?? new System.Random();
            BuildUi();
            _root.SetActive(false);
        }

        /// <summary>Puts the counter over another plate, for a venue you visit by day and by night.</summary>
        public void SetBackdrop(Sprite plate, float plateFocus)
        {
            _plate = plate;
            _plateFocus = plateFocus;
            _plateFit.Show(_plateImage, plate, plateFocus);
        }

        /// <summary>Steps you up to the counter with this state, the clock moving at this pace while you play.</summary>
        public void SitDown(PlayerState state, ClockPace pace)
        {
            _state = state;
            _sitting = new Sitting(pace);
            _sittingStartBankroll = state.BankrollNow;
            SittingRounds = 0;
            _root.SetActive(true);
            ShowRack();
        }

        public TicketLook LookFor(ScratchTicket ticket) =>
            _looks.TryGetValue(ticket.Id, out var look) && look != null ? look : new TicketLook();

        void Update()
        {
            if (_sitting == null || !_root.activeSelf) return;
            float seconds = Mathf.Min(Time.unscaledDeltaTime, Sitting.MaxFrameSeconds);
            _sitting.Play(seconds);
            if (_ticket != null)
            {
                _ticket.Tick(seconds);
                if (!Finished && _ticket.AllRevealed) Finish();
                FitTicket();
            }
            else FitRack();
            RefreshMeters();
        }

        bool Broke => _state.BankrollNow < CheapestPrice;

        // ---- Play ----

        void Buy(ScratchTicket ticket)
        {
            if (ticket.Price > _state.BankrollNow || (_ticket != null && !Finished)) return;
            var card = ScratchCard.Buy(ticket, _state.BankrollNow, _random);
            _state.ApplyRoundResult(card.Net, _sitting.TakeMinutes());
            SittingRounds++;

            _ticket?.Destroy();
            string serial = $"{ticket.Price:000}-{_random.Next(10000000):0000000}-{_random.Next(1000):000}";
            _ticket = new TicketView(_holder, card, LookFor(ticket), serial, _landscape);
            Finished = false;
            _rack.gameObject.SetActive(false);
            _holder.gameObject.SetActive(true);
            FitTicket();
            Refresh();
        }

        void RevealAll()
        {
            if (_ticket == null || Finished) return;
            _ticket.RevealAll();
            Finish();
        }

        void Finish()
        {
            Finished = true;
            _ticket.ShowWin();
            Refresh();
        }

        void Another()
        {
            if (_ticket != null) Buy(_ticket.Card.Ticket);
        }

        void ShowRack()
        {
            _ticket?.Destroy();
            _ticket = null;
            Finished = false;
            _holder.gameObject.SetActive(false);
            _rack.gameObject.SetActive(true);
            FitRack();
            Refresh();
        }

        void GetUp()
        {
            if (_ticket != null && !Finished) return;
            _state.PassTime(_sitting.TakeMinutes());
            _ticket?.Destroy();
            _ticket = null;
            _root.SetActive(false);
            BreakPointReached?.Invoke(BreakPoint.GotUpFromTable);
        }

        // ---- Display ----

        void Refresh()
        {
            bool atRack = _ticket == null;
            bool scratching = !atRack && !Finished;

            if (atRack) SetStatus(Broke ? "You're broke. Get up and find another way back in." : "Pick a ticket. Mitch tears it off the roll.", Color.white);
            else if (scratching) SetStatus("Scratch it off with your finger.", Color.white);
            else if (Card.IsWinner) SetStatus($"Winner!  {Money(Card.Prize)}", Gold);
            else SetStatus("Not a winner.", new Color(1, 1, 1, 0.8f));

            foreach (var tile in _tiles)
            {
                bool affordable = tile.Ticket.Price <= _state.BankrollNow;
                tile.Button.interactable = affordable;
                tile.Group.alpha = affordable ? 1 : 0.4f;
            }

            _revealButton.gameObject.SetActive(scratching);
            _anotherButton.gameObject.SetActive(!atRack && Finished);
            _rackButton.gameObject.SetActive(!atRack && Finished);
            if (!atRack)
            {
                int price = Card.Ticket.Price;
                _anotherButton.GetComponentInChildren<TMP_Text>().text = $"Another, {Money(price)}";
                _anotherButton.interactable = price <= _state.BankrollNow;
            }
            _getUpButton.interactable = !scratching;
            _buttonsRow.gameObject.SetActive(!_landscape && !atRack);
            _buttonsColumn.gameObject.SetActive(_landscape && !atRack);
            RefreshMeters();
        }

        void SetStatus(string text, Color ink)
        {
            _statusText.text = text;
            _statusText.color = ink;
        }

        /// <summary>
        /// Cash and the clock. The ticket's settled when it's bought, so until it's scratched
        /// off the cash shows what you had less the price, as if the prize were still to claim.
        /// </summary>
        void RefreshMeters()
        {
            int notYetShown = _ticket != null && !Finished ? Card.Prize : 0;
            _bankrollText.text = $"Cash {Money(_state.BankrollNow - notYetShown)}" + (_state.Debts > 0 ? $"    Owed {Money(_state.Debts)}" : "");
            var clock = _state.Clock;
            _clockText.text = $"{clock.DateLabel} · {clock.Part} {clock.TimeLabel}" + (_sitting.ClockStopped ? " (late)" : "");
        }

        /// <summary>The card at the largest size that fits the stage, centred.</summary>
        void FitTicket()
        {
            var size = _holder.rect.size;
            if (size.x <= 0 || size.y <= 0) return;
            var card = _ticket.Rect.sizeDelta;
            float scale = Mathf.Min(size.x / card.x, size.y / card.y);
            _ticket.Rect.localScale = new Vector3(scale, scale, 1);
        }

        /// <summary>The rack's tiles share the stage: two across in portrait, five in landscape.</summary>
        void FitRack()
        {
            var size = _stage.rect.size;
            if (size.x <= 0 || size.y <= 0) return;
            int columns = _landscape ? 5 : 2, rows = (_tiles.Count + columns - 1) / columns;
            _rackGrid.constraintCount = columns;
            _rackGrid.cellSize = new Vector2(
                Mathf.Max(40, (size.x - Gap * (columns - 1)) / columns),
                Mathf.Max(40, Mathf.Min((size.y - Gap * (rows - 1)) / rows, 520)));
            foreach (var tile in _tiles)
                if (tile.Picture != null) FramePicture(tile);
        }

        /// <summary>
        /// A tile shows its ticket's picture, cropped to fill the top of the tile: the upright
        /// or the wide card's, whichever is nearer the tile's shape, centred in the crop.
        /// </summary>
        static void FramePicture(Tile tile)
        {
            var box = tile.Picture.rectTransform.rect.size;
            if (box.x <= 0 || box.y <= 0) return;
            float want = box.x / box.y;
            var look = tile.Look;
            float Shape(Sprite art, float header) => art == null ? float.NaN : art.rect.width / (art.rect.height * header);
            float tall = Shape(look.Art, look.Header), wide = Shape(look.WideArt, look.WideHeader);
            bool useWide = float.IsNaN(tall) || (!float.IsNaN(wide) && Mathf.Abs(Mathf.Log(wide / want)) < Mathf.Abs(Mathf.Log(tall / want)));
            var sprite = useWide ? look.WideArt : look.Art;
            float headerShare = useWide ? look.WideHeader : look.Header;
            float shape = useWide ? wide : tall;

            var texture = sprite.texture;
            var rect = sprite.textureRect;
            float u = rect.x / texture.width, uw = rect.width / texture.width;
            float vh = rect.height / texture.height * headerShare;
            float v = (rect.y + rect.height) / texture.height - vh;
            if (want > shape)
            {
                float keep = shape / want;
                v += vh * (1 - keep) / 2;
                vh *= keep;
            }
            else
            {
                float keep = want / shape;
                u += uw * (1 - keep) / 2;
                uw *= keep;
            }
            tile.Picture.texture = texture;
            tile.Picture.uvRect = new Rect(u, v, uw, vh);
        }

        // ---- UI construction ----

        void BuildUi()
        {
            var canvas = MakeCanvas(transform, "Scratcher Canvas", 0);
            _root = canvas.gameObject;

            var backdrop = MakeImage(canvas.transform, "Backdrop", Color.black);
            Stretch(backdrop.rectTransform);
            _plateImage = MakeImage(backdrop.transform, "Plate", Color.white);
            _plateImage.raycastTarget = false;
            _plateFit = _plateImage.gameObject.AddComponent<CoverFit>();
            _plateFit.Show(_plateImage, _plate, _plateFocus);
            // The store goes dim behind the counter, so the tickets read.
            var shade = MakeImage(canvas.transform, "Shade", new Color(0, 0, 0, 0.62f));
            Stretch(shade.rectTransform);

            var safe = new GameObject("Safe Area", typeof(RectTransform), typeof(SafeAreaFitter));
            safe.transform.SetParent(canvas.transform, false);
            Stretch((RectTransform)safe.transform);
            _column = AddVertical(safe, 16, new RectOffset(36, 36, 28, 28));

            BuildHeader(_column);

            _title = MakeText(_column, "SCRATCHERS", 64, TextAnchor.MiddleCenter, 80);
            _title.name = "Title";
            _title.color = Gold;
            _title.fontStyle = FontStyles.Bold | FontStyles.Italic;
            _title.characterSpacing = 6;
            NoWrap(_title);

            _body = MakeRow(_column, "Body");
            _body.GetComponent<LayoutElement>().flexibleHeight = 1;
            var bodyGroup = _body.GetComponent<HorizontalLayoutGroup>();
            bodyGroup.spacing = 40;
            bodyGroup.childForceExpandWidth = false;
            bodyGroup.childForceExpandHeight = true;

            var stage = new GameObject("Stage", typeof(RectTransform), typeof(LayoutElement));
            stage.transform.SetParent(_body, false);
            _stage = (RectTransform)stage.transform;
            var stageLayout = stage.GetComponent<LayoutElement>();
            stageLayout.flexibleWidth = 1;
            stageLayout.minWidth = stageLayout.preferredWidth = 0;
            BuildRack();
            var holder = new GameObject("Ticket Holder", typeof(RectTransform));
            holder.transform.SetParent(_stage, false);
            _holder = (RectTransform)holder.transform;
            Stretch(_holder);

            var side = new GameObject("Side", typeof(RectTransform), typeof(LayoutElement));
            side.transform.SetParent(_body, false);
            _side = AddVertical(side, 24, new RectOffset());
            _side.GetComponent<VerticalLayoutGroup>().childAlignment = TextAnchor.MiddleCenter;
            var sideLayout = side.GetComponent<LayoutElement>();
            sideLayout.preferredWidth = 560;
            sideLayout.flexibleWidth = 0;

            _statusText = MakeText(_column, "", 46, TextAnchor.MiddleCenter, 70);
            _statusText.name = "Status";
            _statusText.fontStyle = FontStyles.Bold;

            _buttonsRow = MakeRow(_column, "Buttons");
            _buttonsRow.GetComponent<LayoutElement>().preferredHeight = 140;
            var column = new GameObject("Buttons Column", typeof(RectTransform), typeof(LayoutElement));
            column.transform.SetParent(_side, false);
            _buttonsColumn = AddVertical(column, 20, new RectOffset());

            _revealButton = CounterButton("Scratch it all", RevealAll);
            _anotherButton = CounterButton("Another", Another);
            _rackButton = CounterButton("Back to the rack", ShowRack);

            OnTurn(canvas, Rearrange);
        }

        /// <summary>
        /// Portrait stacks the title, the stage, the status and the buttons. Landscape is too
        /// short for that, so the title, status and buttons stand to the right of the stage.
        /// </summary>
        void Rearrange(bool landscape)
        {
            _landscape = landscape;
            _column.GetComponent<VerticalLayoutGroup>().padding =
                landscape ? new RectOffset(48, 48, 16, 16) : new RectOffset(36, 36, 28, 28);
            var header = _column.GetChild(0);
            if (landscape)
            {
                Arrange(_column, header, _body);
                Arrange(_side, _title, _statusText, _buttonsColumn);
                _statusText.GetComponent<LayoutElement>().preferredHeight = 160;
            }
            else
            {
                Arrange(_column, header, _title, _body, _statusText, _buttonsRow);
                _statusText.GetComponent<LayoutElement>().preferredHeight = 70;
            }
            _side.gameObject.SetActive(landscape);
            var buttons = landscape ? _buttonsColumn : _buttonsRow;
            Arrange(buttons, _revealButton, _anotherButton, _rackButton);
            foreach (var button in new[] { _revealButton, _anotherButton, _rackButton })
                button.GetComponent<LayoutElement>().preferredHeight = landscape ? 130 : 140;
            if (_ticket != null && _ticket.Wide != WantsWide(_ticket.Card.Ticket)) Turn();
            if (_state != null) Refresh();
        }

        bool WantsWide(ScratchTicket ticket) => _landscape && LookFor(ticket).WideArt != null;

        /// <summary>
        /// The phone turned with a ticket in hand: the same ticket on the other card, with the
        /// spots you'd read still showing. Nothing about the ticket changes; it's settled.
        /// </summary>
        void Turn()
        {
            var old = _ticket;
            _ticket = new TicketView(_holder, old.Card, LookFor(old.Card.Ticket), old.Serial, _landscape);
            _ticket.RevealAsOn(old);
            if (Finished) _ticket.ShowWin();
            old.Destroy();
            FitTicket();
        }

        void BuildHeader(Transform column)
        {
            var header = MakeRow(column, "Header");
            header.GetComponent<HorizontalLayoutGroup>().childForceExpandWidth = false;
            header.GetComponent<LayoutElement>().preferredHeight = 120;
            var info = new GameObject("Info", typeof(RectTransform), typeof(LayoutElement));
            info.transform.SetParent(header, false);
            info.GetComponent<LayoutElement>().flexibleWidth = 1;
            AddVertical(info, 4, new RectOffset());
            _bankrollText = MakeText(info.transform, "", 46, TextAnchor.MiddleLeft, 60);
            _bankrollText.name = "Cash";
            _bankrollText.fontStyle = FontStyles.Bold;
            _clockText = MakeText(info.transform, "", 32, TextAnchor.MiddleLeft, 44);
            _clockText.color = new Color(1, 1, 1, 0.8f);
            _getUpButton = MakeButton(header, "Leave game", GetUp);
            var getUp = _getUpButton.GetComponent<LayoutElement>();
            getUp.preferredHeight = 100;
            getUp.minWidth = getUp.preferredWidth = 340;
            getUp.flexibleWidth = 0;
            NoWrap(_getUpButton.GetComponentInChildren<TMP_Text>());
        }

        /// <summary>The roll, one tile a ticket in its own colours: its name, its price and its top prize.</summary>
        void BuildRack()
        {
            var rack = new GameObject("Rack", typeof(RectTransform), typeof(GridLayoutGroup));
            rack.transform.SetParent(_stage, false);
            _rack = (RectTransform)rack.transform;
            Stretch(_rack);
            _rackGrid = rack.GetComponent<GridLayoutGroup>();
            _rackGrid.spacing = new Vector2(Gap, Gap);
            _rackGrid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            _rackGrid.childAlignment = TextAnchor.MiddleCenter;

            foreach (var ticket in Roll)
            {
                var look = LookFor(ticket);
                bool hasArt = look.Art != null || look.WideArt != null;
                var tile = MakeImage(_rack, $"{ticket.Name} Ticket", look.Bottom);
                tile.sprite = Rounded;
                tile.type = Image.Type.Sliced;
                var entry = new Tile
                {
                    Ticket = ticket,
                    Look = look,
                    Group = tile.gameObject.AddComponent<CanvasGroup>(),
                    Button = tile.gameObject.AddComponent<Button>(),
                };
                var picked = ticket;
                entry.Button.onClick.AddListener(() => Buy(picked));

                if (hasArt)
                {
                    // The ticket's own picture across the top half, its name on its own colour below.
                    var picture = new GameObject("Art", typeof(RectTransform), typeof(RawImage));
                    picture.transform.SetParent(tile.transform, false);
                    entry.Picture = picture.GetComponent<RawImage>();
                    entry.Picture.raycastTarget = false;
                    Pin(entry.Picture.rectTransform, new Vector2(0, 0.46f), Vector2.one, Vector2.zero, Vector2.zero);
                }
                else
                {
                    var sweep = MakeImage(tile.transform, "Sweep", look.Top);
                    sweep.sprite = Fade;
                    sweep.raycastTarget = false;
                    Stretch(sweep.rectTransform);
                }
                var border = MakeImage(tile.transform, "Border", look.Accent);
                border.sprite = RoundedFrame;
                border.type = Image.Type.Sliced;
                border.raycastTarget = false;
                Pin(border.rectTransform, Vector2.zero, Vector2.one, new Vector2(8, 8), new Vector2(-8, -8));

                var name = TileText(tile.transform, "Name", ticket.Name, 60, look);
                name.fontStyle = FontStyles.Bold | FontStyles.Italic;
                Pin(name.rectTransform, new Vector2(0.06f, hasArt ? 0.22f : 0.3f), new Vector2(0.94f, hasArt ? 0.45f : 0.74f), Vector2.zero, Vector2.zero);
                name.textWrappingMode = TextWrappingModes.Normal;

                var top = TileText(tile.transform, "Top Prize", $"TOP PRIZE {Money(ticket.TopPrize)}", 30, look);
                Pin(top.rectTransform, new Vector2(0.06f, 0.05f), new Vector2(0.94f, hasArt ? 0.21f : 0.28f), Vector2.zero, Vector2.zero);
                top.color = look.FootInk;
                top.outlineWidth = 0;

                var badge = MakeImage(tile.transform, "Price Badge", look.Accent);
                badge.sprite = Circle;
                badge.raycastTarget = false;
                Pin(badge.rectTransform, Vector2.one, Vector2.one, new Vector2(-104, -104), new Vector2(-14, -14));
                var price = MakeText(badge.transform, Money(ticket.Price), 40, TextAnchor.MiddleCenter, 0);
                price.name = "Price";
                price.color = look.BadgeInk;
                price.fontStyle = FontStyles.Bold;
                Stretch(price.rectTransform);

                _tiles.Add(entry);
            }
        }

        static TMP_Text TileText(Transform tile, string name, string text, int size, TicketLook look)
        {
            var label = MakeText(tile, text, size, TextAnchor.MiddleCenter, 0);
            label.name = name;
            label.color = look.TitleInk;
            label.enableAutoSizing = true;
            label.fontSizeMin = 18;
            label.fontSizeMax = size;
            label.outlineWidth = 0.2f;
            label.outlineColor = look.TitleOutline;
            NoWrap(label);
            return label;
        }

        Button CounterButton(string label, UnityEngine.Events.UnityAction onClick)
        {
            var button = MakeButton(_buttonsRow, label, onClick);
            button.GetComponent<Image>().color = new Color(0.93f, 0.91f, 0.86f);
            var text = button.GetComponentInChildren<TMP_Text>();
            text.color = new Color(0.1f, 0.08f, 0.08f);
            text.fontStyle = FontStyles.Bold;
            NoWrap(text);
            return button;
        }
    }
}
