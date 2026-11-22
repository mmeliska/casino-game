using System;
using Starflare.Cards;
using Starflare.State;
using Starflare.Ui;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Starflare.Ui.UiKit;

namespace Starflare.VideoPoker
{
    /// <summary>
    /// A Jacks or Better machine: UI built in code, portrait or landscape, filling the screen, one hand
    /// at a time. Like the blackjack table it reports each settled hand to the player state,
    /// raises a break point when you get up, and never reads story state or saves: the state's
    /// owner does that on its Changed event.
    /// </summary>
    public sealed class VideoPokerMachine : MonoBehaviour
    {
        static readonly PokerHandRank[] PaytableRows =
        {
            PokerHandRank.RoyalFlush, PokerHandRank.StraightFlush, PokerHandRank.FourOfAKind,
            PokerHandRank.FullHouse, PokerHandRank.Flush, PokerHandRank.Straight,
            PokerHandRank.ThreeOfAKind, PokerHandRank.TwoPair, PokerHandRank.JacksOrBetter,
        };


        /// <summary>Raised at a break point, where story beats and seed reveals may fire.</summary>
        public event Action<BreakPoint> BreakPointReached;

        public VideoPokerRules Rules { get; private set; } = new VideoPokerRules();
        public VideoPokerLook Look { get; private set; } = VideoPokerLook.Classic;

        /// <summary>This sitting so far; still readable at the break point after you get up.</summary>
        public int SittingNet => _state == null ? 0 : _state.BankrollNow - _sittingStartBankroll;
        public int SittingRounds { get; private set; }

        PlayerState _state;
        Sitting _sitting;
        Shoe _deck;
        VideoPokerRound _round;
        int _credits = 1;
        int _sittingStartBankroll;
        string _message = "";

        GameObject _root;
        TMP_Text _bankrollText, _clockText, _statusText, _betText;
        TMP_Text[,] _paytableCells;
        Image[] _paytableRows, _paytableColumns;
        Image[] _cardFaces;
        TMP_Text[] _cardRanks, _cardSuits, _heldLabels;
        GameObject[] _cardBacks;
        Button[] _cardButtons;
        Button _betOneButton, _betMaxButton, _dealButton, _getUpButton;
        TMP_Text _dealLabel;
        RectTransform _paytable, _paytablePane, _playPane;
        LayoutElement _cardRow;
        GameObject _portraitGap;

        Sprite _plate;
        float _plateFocus;
        Image _plateImage;
        CoverFit _plateFit;

        /// <summary>
        /// Sets the machine up over the venue's plate, framed at this focus on a portrait screen.
        /// It stays hidden until you sit down. The venue's paytable comes with it (Kasia's 9/6 if
        /// none is given).
        /// </summary>
        public void Init(Sprite plate, float plateFocus, VideoPokerRules rules = null, VideoPokerLook look = null)
        {
            if (look != null) Look = look;
            _plate = plate;
            _plateFocus = plateFocus;
            if (rules != null) Rules = rules;
            _deck = new Shoe(1, 1.0, new System.Random());
            BuildUi();
            _root.SetActive(false);
        }

        /// <summary>
        /// Puts the machine over another plate, for a venue whose machine you play by day and by
        /// night (Bridgehead's store).
        /// </summary>
        public void SetBackdrop(Sprite plate, float plateFocus)
        {
            _plate = plate;
            _plateFocus = plateFocus;
            _plateFit.Show(_plateImage, plate, plateFocus);
        }

        /// <summary>Sits you down with this state, the clock moving at this pace while you play.</summary>
        public void SitDown(PlayerState state, ClockPace pace)
        {
            _state = state;
            _sitting = new Sitting(pace);
            _round = null;
            _sittingStartBankroll = state.BankrollNow;
            SittingRounds = 0;
            _credits = Mathf.Clamp(_credits, 1, Math.Max(1, MaxAffordableCredits));
            _message = "";
            _root.SetActive(true);
            Refresh();
        }

        /// <summary>Counts real time played, while the machine is up and the app is in front.</summary>
        void Update()
        {
            if (_sitting == null || !_root.activeSelf) return;
            _sitting.Play(Math.Min(Time.unscaledDeltaTime, Sitting.MaxFrameSeconds));
        }

        bool Betting => _round == null || _round.Phase == VideoPokerPhase.Settled;
        int MaxAffordableCredits => Mathf.Min(Rules.MaxCredits, _state.BankrollNow / Rules.CreditValue);
        bool Broke => MaxAffordableCredits < 1;

        void BetOne()
        {
            if (!Betting || Broke) return;
            _credits = _credits >= MaxAffordableCredits ? 1 : _credits + 1;
            Refresh();
        }

        void BetMax()
        {
            if (!Betting || Broke) return;
            _credits = MaxAffordableCredits;
            Deal();
        }

        void DealOrDraw()
        {
            if (Betting) Deal();
            else Draw();
        }

        void Deal()
        {
            if (!Betting || Broke) return;
            _credits = Mathf.Clamp(_credits, 1, MaxAffordableCredits);
            _deck.Shuffle();
            _round = new VideoPokerRound(Rules, _deck, _credits, _state.BankrollNow);
            _message = "";
            Refresh();
        }

        void Draw()
        {
            _round.Draw();
            _state.ApplyRoundResult(_round.Net, _sitting.TakeMinutes());
            SittingRounds++;

            _message = _round.Rank == PokerHandRank.Nothing
                ? "Game over"
                : $"{PokerHand.Label(_round.Rank)}   Win {Money(_round.Payout)}";
            _credits = Mathf.Clamp(_credits, 1, Math.Max(1, MaxAffordableCredits));
            Refresh();
        }

        void ToggleHold(int index)
        {
            if (Betting) return;
            _round.ToggleHold(index);
            Refresh();
        }

        void GetUp()
        {
            if (!Betting) return;
            _state.PassTime(_sitting.TakeMinutes());
            _root.SetActive(false);
            BreakPointReached?.Invoke(BreakPoint.GotUpFromTable);
        }

        // ---- Display ----

        void Refresh()
        {
            var clock = _state.Clock;
            _bankrollText.text = $"Cash {Money(_state.BankrollNow)}" + (_state.Debts > 0 ? $"    Owed {Money(_state.Debts)}" : "");
            _clockText.text = $"{clock.DateLabel} · {clock.Part} {clock.TimeLabel}"
                + (_sitting.ClockStopped ? " (late)" : "");

            bool betting = Betting;
            int column = (_round?.Credits ?? _credits) - 1;
            var showing = _round?.Rank;
            bool won = betting && showing.HasValue && showing != PokerHandRank.Nothing;

            for (int r = 0; r < PaytableRows.Length; r++)
            {
                bool lit = showing == PaytableRows[r];
                _paytableRows[r].color = lit ? new Color(1, 1, 1, 0.16f) : Color.clear;
                for (int c = 0; c <= Rules.MaxCredits; c++)
                    _paytableCells[r, c].color = lit ? Color.white : Look.PaytableText;
            }
            for (int c = 0; c < Rules.MaxCredits; c++)
                _paytableColumns[c].color = c == column ? Look.LitColumn : Color.clear;

            for (int i = 0; i < 5; i++)
            {
                bool faceUp = _round != null;
                _cardBacks[i].SetActive(!faceUp);
                _cardRanks[i].enabled = _cardSuits[i].enabled = faceUp;
                _heldLabels[i].enabled = faceUp && !betting && _round.IsHeld(i);
                _cardButtons[i].interactable = !betting;
                if (!faceUp) continue;

                var card = _round.Cards[i];
                var ink = card.IsRed ? CardRed : CardBlack;
                _cardRanks[i].text = card.RankLabel;
                _cardRanks[i].color = ink;
                _cardSuits[i].text = card.SuitSymbol;
                _cardSuits[i].color = ink;
                _cardFaces[i].color = !betting && !_round.IsHeld(i) ? new Color(0.86f, 0.86f, 0.86f) : Color.white;
            }

            if (Broke && betting)
                _message = "You're broke. Get up and find another way back in.";
            else if (!betting)
                _message = _round.Rank == PokerHandRank.Nothing ? "Tap cards to hold" : PokerHand.Label(_round.Rank);
            else if (_round == null)
                _message = "Pick a bet and deal";
            _statusText.text = _message;
            _statusText.color = won ? Gold : Look.PaytableText;
            _betText.text = $"Bet {Money(_credits * Rules.CreditValue)}";

            _dealLabel.text = betting ? "Deal" : "Draw";
            _dealButton.interactable = !betting || !Broke;
            _betOneButton.interactable = betting && !Broke;
            _betMaxButton.interactable = betting && !Broke;
            _getUpButton.interactable = betting;
        }

        // ---- UI construction ----

        void BuildUi()
        {
            var canvas = MakeCanvas(transform, "Machine Canvas", 0);
            _root = canvas.gameObject;

            var backdrop = MakeImage(canvas.transform, "Backdrop", Color.black);
            Stretch(backdrop.rectTransform);
            _plateImage = MakeImage(backdrop.transform, "Plate", Color.white);
            _plateImage.raycastTarget = false;
            _plateFit = _plateImage.gameObject.AddComponent<CoverFit>();
            _plateFit.Show(_plateImage, _plate, _plateFocus);
            var shade = MakeImage(backdrop.transform, "Shade", new Color(0, 0, 0, 0.35f));
            Stretch(shade.rectTransform);

            // The machine's screen fills yours, the way the blackjack felt does: you're at the
            // glass, not looking at the cabinet from across the bar.
            var cabinet = MakeImage(canvas.transform, "Screen Fill", Look.Screen);
            Stretch(cabinet.rectTransform);

            var safe = new GameObject("Safe Area", typeof(RectTransform), typeof(SafeAreaFitter));
            safe.transform.SetParent(canvas.transform, false);
            Stretch((RectTransform)safe.transform);
            var column = AddVertical(safe, 16, new RectOffset(36, 36, 28, 28));

            var header = MakeRow(column, "Header");
            header.GetComponent<HorizontalLayoutGroup>().childForceExpandWidth = false;
            header.GetComponent<LayoutElement>().preferredHeight = 120;
            var info = new GameObject("Info", typeof(RectTransform), typeof(LayoutElement));
            info.transform.SetParent(header, false);
            info.GetComponent<LayoutElement>().flexibleWidth = 1;
            AddVertical(info, 4, new RectOffset());
            _bankrollText = MakeText(info.transform, "", 46, TextAnchor.MiddleLeft, 60);
            _bankrollText.fontStyle = FontStyles.Bold;
            _clockText = MakeText(info.transform, "", 32, TextAnchor.MiddleLeft, 44);
            _clockText.color = new Color(1, 1, 1, 0.8f);
            _getUpButton = MachineButton(header, "Leave game", GetUp);
            var getUp = _getUpButton.GetComponent<LayoutElement>();
            getUp.preferredHeight = 100;
            getUp.minWidth = getUp.preferredWidth = 340;
            getUp.flexibleWidth = 0;
            NoWrap(_getUpButton.GetComponentInChildren<TMP_Text>());

            BuildCabinet(column);

            OnTurn(canvas, landscape =>
            {
                column.GetComponent<VerticalLayoutGroup>().padding =
                    landscape ? new RectOffset(48, 48, 20, 20) : new RectOffset(36, 36, 28, 28);
                Rearrange(landscape);
            });
        }

        /// <summary>
        /// Portrait stacks the paytable over the cards. Landscape is too short for that, so the
        /// paytable gets its own pane to the left of the cards.
        /// </summary>
        void Rearrange(bool landscape)
        {
            _paytablePane.gameObject.SetActive(landscape);
            _paytable.SetParent(landscape ? _paytablePane : _playPane, false);
            if (!landscape) _paytable.SetAsFirstSibling();
            _cardRow.preferredHeight = landscape ? 280 : 270;
            // Portrait has height to spare: the paytable reads bigger, at the top of the glass,
            // and the cards sit low, over the buttons, as on a real machine.
            _portraitGap.SetActive(!landscape);
            foreach (var row in _paytableRows)
                row.GetComponent<LayoutElement>().preferredHeight = landscape ? 50 : 80;
            foreach (var cell in _paytableCells)
                cell.fontSize = landscape ? 30 : 38;
        }

        void BuildCabinet(Transform body)
        {
            var glass = MakeText(body, "JACKS OR BETTER", 30, TextAnchor.MiddleCenter, 40);
            glass.color = Look.PaytableText;

            var screen = MakeImage(body, "Screen", Look.Screen);
            screen.sprite = Rounded;
            screen.type = Image.Type.Sliced;
            screen.gameObject.AddComponent<LayoutElement>().flexibleHeight = 1;
            var panes = screen.gameObject.AddComponent<HorizontalLayoutGroup>();
            panes.spacing = 80;
            panes.padding = new RectOffset(20, 20, 20, 24);
            panes.childControlWidth = panes.childControlHeight = true;
            panes.childForceExpandWidth = false;
            panes.childForceExpandHeight = true;
            _paytablePane = MakePane(screen.transform, "Paytable Pane", 1.25f);
            _playPane = MakePane(screen.transform, "Play Pane", 1f);
            _paytable = BuildPaytable(_playPane);
            _portraitGap = new GameObject("Gap", typeof(RectTransform), typeof(LayoutElement));
            _portraitGap.transform.SetParent(_playPane, false);
            _portraitGap.GetComponent<LayoutElement>().flexibleHeight = 1;

            var holds = MakeRow(_playPane, "Held");
            holds.GetComponent<LayoutElement>().preferredHeight = 44;
            var cards = MakeRow(_playPane, "Cards");
            _cardRow = cards.GetComponent<LayoutElement>();
            foreach (var group in new[] { cards.GetComponent<HorizontalLayoutGroup>(), holds.GetComponent<HorizontalLayoutGroup>() })
            {
                group.GetComponent<LayoutElement>().flexibleHeight = 0;
                group.spacing = 12;
                group.childForceExpandHeight = true;
            }

            _cardFaces = new Image[5];
            _cardRanks = new TMP_Text[5];
            _cardSuits = new TMP_Text[5];
            _cardBacks = new GameObject[5];
            _cardButtons = new Button[5];
            _heldLabels = new TMP_Text[5];
            for (int i = 0; i < 5; i++)
            {
                _heldLabels[i] = MakeText(holds, "HELD", 34, TextAnchor.MiddleCenter, 0);
                _heldLabels[i].color = Gold;
                _heldLabels[i].fontStyle = FontStyles.Bold;
                _heldLabels[i].gameObject.AddComponent<LayoutElement>().flexibleWidth = 1;
                BuildCard(cards, i);
            }

            _statusText = MakeText(_playPane, "", 44, TextAnchor.MiddleCenter, 64);
            _statusText.fontStyle = FontStyles.Bold;

            var buttons = MakeRow(body, "Buttons");
            buttons.GetComponent<LayoutElement>().preferredHeight = 140;
            _betOneButton = MachineButton(buttons, "Bet one", BetOne);
            _betMaxButton = MachineButton(buttons, "Bet max", BetMax);
            _dealButton = MachineButton(buttons, "Deal", DealOrDraw);
            _dealLabel = _dealButton.GetComponentInChildren<TMP_Text>();

            _betText = MakeText(body, "", 34, TextAnchor.MiddleCenter, 44);
            _betText.color = new Color(1, 1, 1, 0.75f);
        }

        /// <summary>A yellow machine button with dark lettering, in the look's colours.</summary>
        Button MachineButton(Transform parent, string label, UnityEngine.Events.UnityAction onClick)
        {
            var button = MakeButton(parent, label, onClick);
            button.GetComponent<Image>().color = Look.Button;
            var text = button.GetComponentInChildren<TMP_Text>();
            text.color = Look.ButtonText;
            text.fontStyle = FontStyles.Bold;
            return button;
        }

        /// <summary>
        /// A pane sized by its share of the screen alone, never by its contents, so a long
        /// win line under the cards can't squeeze or stretch them.
        /// </summary>
        static RectTransform MakePane(Transform parent, string name, float width)
        {
            var pane = new GameObject(name, typeof(RectTransform), typeof(LayoutElement));
            pane.transform.SetParent(parent, false);
            var layout = pane.GetComponent<LayoutElement>();
            layout.minWidth = 0;
            layout.preferredWidth = 0;
            layout.flexibleWidth = width;
            var column = AddVertical(pane, 14, new RectOffset());
            column.GetComponent<VerticalLayoutGroup>().childAlignment = TextAnchor.MiddleCenter;
            return column;
        }

        RectTransform BuildPaytable(Transform parent)
        {
            var table = new GameObject("Paytable", typeof(RectTransform));
            table.transform.SetParent(parent, false);
            AddVertical(table, 0, new RectOffset());

            // Column highlights sit behind the rows, so the current bet reads as a lit stripe.
            var stripes = MakeRow(table.transform, "Bet Columns");
            stripes.GetComponent<LayoutElement>().ignoreLayout = true;
            Stretch(stripes);
            stripes.GetComponent<HorizontalLayoutGroup>().spacing = 0;
            stripes.GetComponent<HorizontalLayoutGroup>().childForceExpandHeight = true;
            var labelGap = MakeImage(stripes, "Label Column", Color.clear);
            PaytableColumn(labelGap.gameObject, LabelColumnWidth);
            _paytableColumns = new Image[Rules.MaxCredits];
            for (int c = 0; c < Rules.MaxCredits; c++)
            {
                _paytableColumns[c] = MakeImage(stripes, $"Column {c + 1}", Color.clear);
                PaytableColumn(_paytableColumns[c].gameObject, 1);
                _paytableColumns[c].raycastTarget = false;
            }

            _paytableRows = new Image[PaytableRows.Length];
            _paytableCells = new TMP_Text[PaytableRows.Length, Rules.MaxCredits + 1];
            for (int r = 0; r < PaytableRows.Length; r++)
            {
                var rank = PaytableRows[r];
                _paytableRows[r] = MakeImage(table.transform, PokerHand.Label(rank), Color.clear);
                _paytableRows[r].raycastTarget = false;
                // Not flexible: the row group would otherwise stretch to fill a tall screen.
                var rowLayout = _paytableRows[r].gameObject.AddComponent<LayoutElement>();
                rowLayout.preferredHeight = 46;
                rowLayout.flexibleHeight = 0;
                var row = _paytableRows[r].gameObject.AddComponent<HorizontalLayoutGroup>();
                row.spacing = 0;
                row.childControlWidth = row.childControlHeight = true;
                row.childForceExpandWidth = false;
                row.childForceExpandHeight = true;

                var label = MakeText(row.transform, PokerHand.Label(rank).ToUpperInvariant(), 28, TextAnchor.MiddleLeft, 0);
                NoWrap(label);
                label.margin = new Vector4(12, 0, 0, 0);
                PaytableColumn(label.gameObject, LabelColumnWidth);
                _paytableCells[r, 0] = label;
                for (int c = 1; c <= Rules.MaxCredits; c++)
                {
                    var cell = MakeText(row.transform, Rules.PayoutCredits(rank, c).ToString(), 28, TextAnchor.MiddleRight, 0);
                    NoWrap(cell);
                    cell.margin = new Vector4(0, 0, 14, 0);
                    PaytableColumn(cell.gameObject, 1);
                    _paytableCells[r, c] = cell;
                }
            }
            return (RectTransform)table.transform;
        }

        const float LabelColumnWidth = 3.4f;

        /// <summary>
        /// Sizes a paytable cell by its share of the row alone, never by its text, so every
        /// row's numbers sit in the same columns as the lit bet stripe behind them.
        /// </summary>
        static void PaytableColumn(GameObject cell, float share)
        {
            var layout = cell.AddComponent<LayoutElement>();
            layout.minWidth = 0;
            layout.preferredWidth = 0;
            layout.flexibleWidth = share;
        }

        void BuildCard(Transform row, int index)
        {
            var face = MakeImage(row, $"Card {index + 1}", Color.white);
            face.sprite = Rounded;
            face.type = Image.Type.Sliced;
            face.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1;
            var button = face.gameObject.AddComponent<Button>();
            button.transition = Selectable.Transition.None;
            button.onClick.AddListener(() => ToggleHold(index));

            var rank = MakeText(face.transform, "", 52, TextAnchor.UpperLeft, 0);
            rank.fontStyle = FontStyles.Bold;
            Stretch(rank.rectTransform);
            rank.rectTransform.offsetMin = new Vector2(14, 0);
            rank.rectTransform.offsetMax = new Vector2(0, -8);

            var suit = MakeText(face.transform, "", 96, TextAnchor.MiddleCenter, 0);
            Stretch(suit.rectTransform);
            suit.rectTransform.offsetMin = new Vector2(0, 0);
            suit.rectTransform.offsetMax = new Vector2(0, -36);

            var back = MakeImage(face.transform, "Back", Look.CardBack);
            back.sprite = Rounded;
            back.type = Image.Type.Sliced;
            back.raycastTarget = false;
            Stretch(back.rectTransform);
            var trim = MakeImage(back.transform, "Trim", new Color(Gold.r, Gold.g, Gold.b, 0.55f));
            trim.sprite = Rounded;
            trim.type = Image.Type.Sliced;
            trim.fillCenter = false;
            trim.raycastTarget = false;
            Stretch(trim.rectTransform);
            trim.rectTransform.offsetMin = new Vector2(10, 10);
            trim.rectTransform.offsetMax = new Vector2(-10, -10);
            var pip = MakeText(back.transform, "♦", 64, TextAnchor.MiddleCenter, 0);
            pip.color = new Color(Gold.r, Gold.g, Gold.b, 0.7f);
            Stretch(pip.rectTransform);

            _cardFaces[index] = face;
            _cardRanks[index] = rank;
            _cardSuits[index] = suit;
            _cardBacks[index] = back.gameObject;
            _cardButtons[index] = button;
        }
    }
}
