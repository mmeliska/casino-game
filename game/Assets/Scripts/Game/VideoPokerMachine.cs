using System;
using Starflare.Cards;
using Starflare.State;
using Starflare.VideoPoker;
using UnityEngine;
using UnityEngine.UI;
using static Starflare.Game.UiKit;

namespace Starflare.Game
{
    /// <summary>
    /// A Jacks or Better machine: UI built in code, portrait or landscape, over the venue's plate, one hand
    /// at a time. Like the blackjack table it reports each settled hand to the player state,
    /// raises a break point when you get up, and never reads story state.
    /// </summary>
    public sealed class VideoPokerMachine : MonoBehaviour
    {
        static readonly PokerHandRank[] PaytableRows =
        {
            PokerHandRank.RoyalFlush, PokerHandRank.StraightFlush, PokerHandRank.FourOfAKind,
            PokerHandRank.FullHouse, PokerHandRank.Flush, PokerHandRank.Straight,
            PokerHandRank.ThreeOfAKind, PokerHandRank.TwoPair, PokerHandRank.JacksOrBetter,
        };

        static readonly Color Cabinet = new Color(0.13f, 0.09f, 0.07f, 0.97f);
        static readonly Color ScreenBlue = new Color(0.04f, 0.07f, 0.3f);
        static readonly Color ScreenLit = new Color(0.12f, 0.2f, 0.6f);
        static readonly Color Amber = new Color(1f, 0.78f, 0.35f);
        static readonly Color CardBack = new Color(0.45f, 0.08f, 0.1f);

        /// <summary>Raised at a break point, where story beats and seed reveals may fire.</summary>
        public event Action<BreakPoint> BreakPointReached;

        public VideoPokerRules Rules { get; } = new VideoPokerRules();

        /// <summary>This sitting so far; still readable at the break point after you get up.</summary>
        public int SittingNet => _state == null ? 0 : _state.BankrollNow - _sittingStartBankroll;
        public int SittingRounds { get; private set; }

        PlayerState _state;
        Shoe _deck;
        VideoPokerRound _round;
        int _credits = 1;
        int _sittingStartBankroll;
        string _message = "";

        GameObject _root;
        Text _bankrollText, _clockText, _statusText, _betText;
        Text[,] _paytableCells;
        Image[] _paytableRows, _paytableColumns;
        Image[] _cardFaces;
        Text[] _cardRanks, _cardSuits, _heldLabels;
        GameObject[] _cardBacks;
        Button[] _cardButtons;
        Button _betOneButton, _betMaxButton, _dealButton, _getUpButton;
        Text _dealLabel;
        RectTransform _paytable, _paytablePane, _playPane;

        void Awake()
        {
            _deck = new Shoe(1, 1.0, new System.Random());
            BuildUi();
            _root.SetActive(false);
        }

        public void SitDown(PlayerState state)
        {
            _state = state;
            _round = null;
            _sittingStartBankroll = state.BankrollNow;
            SittingRounds = 0;
            _credits = Mathf.Clamp(_credits, 1, Math.Max(1, MaxAffordableCredits));
            _message = "";
            _root.SetActive(true);
            Refresh();
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
            _state.ApplyRoundResult(_round.Net, Rules.MinutesPerRound);
            SittingRounds++;
            SaveStore.Save(_state);

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
            _root.SetActive(false);
            BreakPointReached?.Invoke(BreakPoint.GotUpFromTable);
        }

        // ---- Display ----

        void Refresh()
        {
            var clock = _state.Clock;
            _bankrollText.text = $"Cash {Money(_state.BankrollNow)}" + (_state.Debts > 0 ? $"    Owed {Money(_state.Debts)}" : "");
            _clockText.text = $"{clock.DateLabel} · {clock.Part} {clock.TimeLabel}"
                + (clock.PartIsOver ? " (late)" : "");

            bool betting = Betting;
            int column = (_round?.Credits ?? _credits) - 1;
            var showing = _round?.Rank;
            bool won = betting && showing.HasValue && showing != PokerHandRank.Nothing;

            for (int r = 0; r < PaytableRows.Length; r++)
            {
                bool lit = showing == PaytableRows[r];
                _paytableRows[r].color = lit ? new Color(1, 1, 1, 0.16f) : Color.clear;
                for (int c = 0; c <= Rules.MaxCredits; c++)
                    _paytableCells[r, c].color = lit ? Color.white : Amber;
            }
            for (int c = 0; c < Rules.MaxCredits; c++)
                _paytableColumns[c].color = c == column ? ScreenLit : Color.clear;

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
            _statusText.color = won ? Gold : Amber;
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
            var plate = MakeImage(backdrop.transform, "Plate", Color.white);
            plate.raycastTarget = false;
            plate.gameObject.AddComponent<CoverFit>().Show(plate, Art.Plate("kasia_bar_night"), Art.Focus("kasia_bar_night"));
            var shade = MakeImage(backdrop.transform, "Shade", new Color(0, 0, 0, 0.35f));
            Stretch(shade.rectTransform);

            var safe = new GameObject("Safe Area", typeof(RectTransform), typeof(SafeAreaFitter));
            safe.transform.SetParent(canvas.transform, false);
            Stretch((RectTransform)safe.transform);
            var column = AddVertical(safe, 20, new RectOffset(36, 36, 36, 36));

            var header = MakeRow(column, "Header");
            header.GetComponent<LayoutElement>().preferredHeight = 120;
            var info = new GameObject("Info", typeof(RectTransform), typeof(LayoutElement));
            info.transform.SetParent(header, false);
            info.GetComponent<LayoutElement>().flexibleWidth = 3;
            AddVertical(info, 4, new RectOffset());
            _bankrollText = MakeText(info.transform, "", 46, TextAnchor.MiddleLeft, 60);
            _bankrollText.fontStyle = FontStyle.Bold;
            _clockText = MakeText(info.transform, "", 32, TextAnchor.MiddleLeft, 44);
            _clockText.color = new Color(1, 1, 1, 0.8f);
            _getUpButton = MakeButton(header, "Get up", GetUp);
            _getUpButton.GetComponent<LayoutElement>().preferredHeight = 110;

            var spacer = new GameObject("Spacer", typeof(RectTransform), typeof(LayoutElement));
            spacer.transform.SetParent(column, false);
            spacer.GetComponent<LayoutElement>().flexibleHeight = 1;

            BuildCabinet(column);

            OnTurn(canvas, landscape =>
            {
                column.GetComponent<VerticalLayoutGroup>().padding =
                    landscape ? new RectOffset(120, 120, 36, 36) : new RectOffset(36, 36, 36, 36);
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
        }

        void BuildCabinet(Transform parent)
        {
            var cabinet = MakeImage(parent, "Cabinet", Cabinet);
            cabinet.sprite = Rounded;
            cabinet.type = Image.Type.Sliced;
            var body = AddVertical(cabinet.gameObject, 18, new RectOffset(28, 28, 24, 28));

            var glass = MakeText(body, "FOR AMUSEMENT ONLY", 30, TextAnchor.MiddleCenter, 40);
            glass.color = new Color(Amber.r, Amber.g, Amber.b, 0.6f);

            var screen = MakeImage(body, "Screen", ScreenBlue);
            screen.sprite = Rounded;
            screen.type = Image.Type.Sliced;
            var panes = screen.gameObject.AddComponent<HorizontalLayoutGroup>();
            panes.spacing = 28;
            panes.padding = new RectOffset(20, 20, 20, 24);
            panes.childControlWidth = panes.childControlHeight = true;
            panes.childForceExpandWidth = false;
            panes.childForceExpandHeight = true;
            _paytablePane = MakePane(screen.transform, "Paytable Pane", 1.1f);
            _playPane = MakePane(screen.transform, "Play Pane", 1f);
            _paytable = BuildPaytable(_playPane);

            var holds = MakeRow(_playPane, "Held");
            holds.GetComponent<LayoutElement>().preferredHeight = 44;
            var cards = MakeRow(_playPane, "Cards");
            cards.GetComponent<LayoutElement>().preferredHeight = 250;
            foreach (var group in new[] { cards.GetComponent<HorizontalLayoutGroup>(), holds.GetComponent<HorizontalLayoutGroup>() })
            {
                group.spacing = 12;
                group.childForceExpandHeight = true;
            }

            _cardFaces = new Image[5];
            _cardRanks = new Text[5];
            _cardSuits = new Text[5];
            _cardBacks = new GameObject[5];
            _cardButtons = new Button[5];
            _heldLabels = new Text[5];
            for (int i = 0; i < 5; i++)
            {
                _heldLabels[i] = MakeText(holds, "HELD", 34, TextAnchor.MiddleCenter, 0);
                _heldLabels[i].color = Gold;
                _heldLabels[i].fontStyle = FontStyle.Bold;
                _heldLabels[i].gameObject.AddComponent<LayoutElement>().flexibleWidth = 1;
                BuildCard(cards, i);
            }

            _statusText = MakeText(_playPane, "", 44, TextAnchor.MiddleCenter, 64);
            _statusText.fontStyle = FontStyle.Bold;

            var buttons = MakeRow(body, "Buttons");
            buttons.GetComponent<LayoutElement>().preferredHeight = 140;
            _betOneButton = MakeButton(buttons, "Bet one", BetOne);
            _betMaxButton = MakeButton(buttons, "Bet max", BetMax);
            _dealButton = MakeButton(buttons, "Deal", DealOrDraw);
            _dealButton.GetComponent<Image>().color = Gold;
            _dealLabel = _dealButton.GetComponentInChildren<Text>();
            _dealLabel.color = CardBlack;
            _dealLabel.fontStyle = FontStyle.Bold;

            _betText = MakeText(body, "", 34, TextAnchor.MiddleCenter, 44);
            _betText.color = new Color(1, 1, 1, 0.75f);
        }

        static RectTransform MakePane(Transform parent, string name, float width)
        {
            var pane = new GameObject(name, typeof(RectTransform), typeof(LayoutElement));
            pane.transform.SetParent(parent, false);
            pane.GetComponent<LayoutElement>().flexibleWidth = width;
            return AddVertical(pane, 14, new RectOffset());
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
            labelGap.gameObject.AddComponent<LayoutElement>().flexibleWidth = 2.6f;
            _paytableColumns = new Image[Rules.MaxCredits];
            for (int c = 0; c < Rules.MaxCredits; c++)
            {
                _paytableColumns[c] = MakeImage(stripes, $"Column {c + 1}", Color.clear);
                _paytableColumns[c].gameObject.AddComponent<LayoutElement>().flexibleWidth = 1;
                _paytableColumns[c].raycastTarget = false;
            }

            _paytableRows = new Image[PaytableRows.Length];
            _paytableCells = new Text[PaytableRows.Length, Rules.MaxCredits + 1];
            for (int r = 0; r < PaytableRows.Length; r++)
            {
                var rank = PaytableRows[r];
                _paytableRows[r] = MakeImage(table.transform, PokerHand.Label(rank), Color.clear);
                _paytableRows[r].raycastTarget = false;
                _paytableRows[r].gameObject.AddComponent<LayoutElement>().preferredHeight = 42;
                var row = _paytableRows[r].gameObject.AddComponent<HorizontalLayoutGroup>();
                row.childControlWidth = row.childControlHeight = true;
                row.childForceExpandWidth = false;
                row.childForceExpandHeight = true;

                var label = MakeText(row.transform, PokerHand.Label(rank).ToUpperInvariant(), 28, TextAnchor.MiddleLeft, 0);
                label.gameObject.AddComponent<LayoutElement>().flexibleWidth = 2.6f;
                _paytableCells[r, 0] = label;
                for (int c = 1; c <= Rules.MaxCredits; c++)
                {
                    var cell = MakeText(row.transform, Rules.PayoutCredits(rank, c).ToString(), 28, TextAnchor.MiddleRight, 0);
                    cell.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1;
                    _paytableCells[r, c] = cell;
                }
            }
            return (RectTransform)table.transform;
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
            rank.fontStyle = FontStyle.Bold;
            Stretch(rank.rectTransform);
            rank.rectTransform.offsetMin = new Vector2(14, 0);
            rank.rectTransform.offsetMax = new Vector2(0, -8);

            var suit = MakeText(face.transform, "", 96, TextAnchor.MiddleCenter, 0);
            Stretch(suit.rectTransform);
            suit.rectTransform.offsetMin = new Vector2(0, 0);
            suit.rectTransform.offsetMax = new Vector2(0, -36);

            var back = MakeImage(face.transform, "Back", CardBack);
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
            var star = MakeText(back.transform, "★", 64, TextAnchor.MiddleCenter, 0);
            star.color = new Color(Gold.r, Gold.g, Gold.b, 0.7f);
            Stretch(star.rectTransform);

            _cardFaces[index] = face;
            _cardRanks[index] = rank;
            _cardSuits[index] = suit;
            _cardBacks[index] = back.gameObject;
            _cardButtons[index] = button;
        }
    }
}
