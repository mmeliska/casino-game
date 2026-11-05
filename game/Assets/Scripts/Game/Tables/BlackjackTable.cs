using System;
using System.Collections.Generic;
using System.Text;
using Starflare.Blackjack;
using Starflare.Cards;
using Starflare.State;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Starflare.Game.UiKit;

namespace Starflare.Game
{
    /// <summary>
    /// The blackjack table: UI built in code, portrait or landscape, one round at a time. It reports each
    /// settled round to the player state and raises a break point when you get up. It never
    /// reads story state or saves; whoever owns the state (see <see cref="BlackjackScene"/>)
    /// hands it in through <see cref="SitDown"/> and saves on its Changed event. A venue can
    /// hand it one <see cref="PracticeHand"/> to deal first, which doesn't count, and a teacher
    /// can coach over it (<see cref="ICoachable"/>, by the ids in <see cref="BlackjackControls"/>).
    /// </summary>
    public sealed class BlackjackTable : MonoBehaviour, ICoachable
    {
        static readonly Color Felt = new Color(0.05f, 0.33f, 0.2f);
        static readonly Color CardBack = new Color(0.16f, 0.2f, 0.55f);

        /// <summary>Raised at a break point, where story beats and seed reveals may fire.</summary>
        public event Action<BreakPoint> BreakPointReached;

        /// <summary>The break panel's "start over": the owner wipes the save and sits you back down.</summary>
        public event Action StartOverRequested;

        public event Action<string> ActionTaken;

        public BlackjackRules Rules => _rules;

        /// <summary>This sitting so far; still readable at the break point after you get up.</summary>
        public int SittingNet => _state == null ? 0 : _state.BankrollNow - _sittingStartBankroll;

        BlackjackRules _rules;
        int[] _chipValues;
        bool _standalone;
        PracticeHand _practice;
        bool _practicing;
        readonly Dictionary<string, Button> _controls = new Dictionary<string, Button>();
        string _lock;
        PlayerState _state;
        Shoe _shoe;
        BlackjackRound _round;
        int _bet;
        int _sittingStartBankroll;
        int _sittingRounds;
        string _message = "";

        GameObject _root;
        TMP_Text _bankrollText, _clockText, _dealerLabel, _messageText, _betText, _breakText;
        RectTransform _dealerCards, _playerHands;
        RectTransform _portraitColumn, _landscapeRow, _tableSide, _controlSide, _controlSpacer;
        GameObject _bettingControls, _actionRow, _insuranceRow, _breakPanel;
        Button _dealButton, _hitButton, _standButton, _doubleButton, _splitButton, _getUpButton;
        TMP_Text _insureLabel;
        Button[] _chipButtons;

        /// <summary>
        /// Sets the table up: its rules and chips, and whether it stands alone (the test scene,
        /// with its own break panel) or sits in a venue that shows its own screens around it.
        /// </summary>
        public void Init(BlackjackRules rules, int[] chips, bool standalone)
        {
            _rules = rules ?? throw new ArgumentNullException(nameof(rules));
            _chipValues = chips;
            _standalone = standalone;
            _shoe = new Shoe(_rules.Decks, _rules.Penetration, new System.Random());
            BuildUi();
            _root.SetActive(standalone);
        }

        /// <summary>
        /// Sits you down with this state; the same shoe carries on between sittings. A practice
        /// hand, if given, is the next hand dealt.
        /// </summary>
        public void SitDown(PlayerState state, PracticeHand practice = null)
        {
            _state = state;
            _practice = practice;
            SitDown();
        }

        public RectTransform Control(string id) => _controls.TryGetValue(id, out var button) ? (RectTransform)button.transform : null;

        public void Lock(string id)
        {
            _lock = id;
            if (_state != null) Refresh();
        }

        void SitDown()
        {
            _root.SetActive(true);
            _round = null;
            _sittingStartBankroll = _state.BankrollNow;
            _sittingRounds = 0;
            _bet = Mathf.Min(_rules.MinBet, _state.BankrollNow);
            _message = "Place your bet.";
            if (_breakPanel != null) _breakPanel.SetActive(false);
            Refresh();
        }

        bool Betting => _round == null || _round.Phase == RoundPhase.Settled;
        int MaxAffordableBet => Mathf.Min(_rules.MaxBet, _state.BankrollNow);
        bool Broke => _state.BankrollNow < _rules.MinBet;

        void AddChip(int value)
        {
            _bet = Mathf.Min(_bet + value, MaxAffordableBet);
            Refresh();
        }

        void ClearBet()
        {
            _bet = Mathf.Min(_rules.MinBet, _state.BankrollNow);
            Refresh();
        }

        void Deal()
        {
            if (!Betting) return;
            if (_practice != null)
            {
                // Played for show: the bankroll it's checked against is the practice bet's own.
                _round = new BlackjackRound(_rules, Shoe.Stacked(_practice.Cards), _practice.Bet, _practice.Bet * 4);
                _practice = null;
                _practicing = true;
                _message = "";
                AfterAction();
                return;
            }
            if (Broke) return;
            _bet = Mathf.Clamp(_bet, _rules.MinBet, MaxAffordableBet);

            bool shuffled = false;
            if (_shoe.PastCutCard)
            {
                _shoe.Shuffle();
                shuffled = true;
            }

            _round = new BlackjackRound(_rules, _shoe, _bet, _state.BankrollNow);
            _message = _round.InsuranceTooSmall ? $"Dealer shows an ace. Insurance needs a {Money(2)} bet."
                : shuffled ? "The dealer shuffles a fresh shoe." : "";
            AfterAction();
        }

        void Act(Action action)
        {
            action();
            AfterAction();
        }

        void AfterAction()
        {
            if (_round.Phase == RoundPhase.Settled && _practicing)
            {
                _practicing = false;
                _message = "Practice hand, so it doesn't count.\n" + DescribeResult(_round);
            }
            else if (_round.Phase == RoundPhase.Settled)
            {
                _state.ApplyRoundResult(_round.Net, _rules.MinutesPerRound);
                _sittingRounds++;
                _message = DescribeResult(_round);
                _bet = Mathf.Min(_bet, MaxAffordableBet);
            }
            else if (_round.CanInsure)
            {
                _message = "Dealer shows an ace. Insurance?";
            }
            Refresh();
        }

        /// <summary>A control's press: ignored while the coach has another one, reported once done.</summary>
        void Press(string id, Action action)
        {
            if (_lock != null && _lock != id) return;
            action();
            ActionTaken?.Invoke(id);
        }

        void GetUp()
        {
            if (!Betting) return;
            if (!_standalone)
            {
                _root.SetActive(false);
                BreakPointReached?.Invoke(BreakPoint.GotUpFromTable);
                return;
            }
            int net = _state.BankrollNow - _sittingStartBankroll;
            var clock = _state.Clock;
            _breakText.text =
                "You get up from the table.\n\n" +
                $"This sitting: {_sittingRounds} rounds, {Money(net, true)}\n" +
                $"Bankroll {Money(_state.BankrollNow)}   Best {Money(_state.BestBankroll)}\n" +
                $"Day {clock.Day}, {clock.Part} {clock.TimeLabel}\n\n" +
                "(Break point: story beats and reveals will check in here.)";
            _breakPanel.SetActive(true);
            BreakPointReached?.Invoke(BreakPoint.GotUpFromTable);
        }

        void SleepUntilMorning()
        {
            _state.Sleep();
            SitDown();
        }

        void StartOver() => StartOverRequested?.Invoke();

        // ---- Display ----

        void Refresh()
        {
            var clock = _state.Clock;
            _bankrollText.text = $"Bankroll {Money(_state.BankrollNow)}     Best {Money(_state.BestBankroll)}";
            _clockText.text = $"{clock.DateLabel} · {clock.Part} {clock.TimeLabel}"
                + (clock.PartIsOver ? " (late)" : "");

            ClearChildren(_dealerCards);
            ClearChildren(_playerHands);

            if (_round != null)
            {
                var dealer = _round.Dealer;
                for (int i = 0; i < dealer.Cards.Count; i++)
                    MakeCard(_dealerCards, dealer.Cards[i], faceUp: i == 0 || _round.DealerHoleRevealed);
                SetCardSpacing(_dealerCards, dealer.Cards.Count);
                _dealerLabel.text = _round.DealerHoleRevealed
                    ? $"Dealer {dealer.Total}"
                    : $"Dealer shows {dealer.Cards[0].BlackjackValue}";

                for (int i = 0; i < _round.Hands.Count; i++)
                    MakeHandView(_round.Hands[i], i, isActive: _round.Phase == RoundPhase.PlayerTurn && i == _round.ActiveHandIndex);
            }
            else
            {
                _dealerLabel.text = $"Dealer stands on soft 17 · Blackjack pays 3:2 · {Money(_rules.MinBet)}–{Money(_rules.MaxBet)}";
            }

            if (Broke && Betting && _practice == null)
                _message = "You're broke. Get up and find another way back in.";
            _messageText.text = _message;
            _betText.text = _practice != null ? $"Practice hand · {Money(_practice.Bet)} on the house"
                : _practicing ? $"Practice hand · {Money(_round.TotalStake)}, doesn't count"
                : Betting ? $"Bet {Money(_bet)}" : $"In play {Money(_round.TotalStake)}";
            _betText.color = _practice != null || _practicing ? Gold : Color.white;

            bool betting = Betting;
            bool insuring = !betting && _round.CanInsure;
            _bettingControls.SetActive(betting);
            _actionRow.SetActive(!betting && !insuring);
            _insuranceRow.SetActive(insuring);
            if (insuring) _insureLabel.text = $"Insurance {Money(_round.InsuranceCost)}";
            // Every control starts each redraw usable, then the rules and any coach's lock narrow it.
            foreach (var control in _controls.Values) control.interactable = true;
            _dealButton.interactable = betting && (!Broke || _practice != null);
            foreach (var chip in _chipButtons) chip.interactable = betting && !Broke && _practice == null && _bet < MaxAffordableBet;
            _getUpButton.interactable = betting && _practice == null;

            if (!betting && !insuring)
            {
                _hitButton.interactable = _round.CanHit;
                _standButton.interactable = _round.CanStand;
                _doubleButton.interactable = _round.CanDouble;
                _splitButton.interactable = _round.CanSplit;
            }

            // While a teacher coaches, only the control they point at works.
            if (_lock != null)
                foreach (var control in _controls)
                    if (control.Key != _lock) control.Value.interactable = false;
        }

        static string DescribeResult(BlackjackRound round)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < round.Hands.Count; i++)
            {
                var hand = round.Hands[i];
                if (round.Hands.Count > 1) sb.Append($"Hand {i + 1}: ");
                sb.Append(hand.Outcome switch
                {
                    HandOutcome.Blackjack => "Blackjack!",
                    HandOutcome.Win => round.Dealer.IsBust ? "Dealer busts. You win." : "You win.",
                    HandOutcome.Push => "Push.",
                    HandOutcome.Bust => "Bust.",
                    _ => round.Dealer.IsBlackjack ? "Dealer blackjack." : "Dealer wins.",
                });
                sb.Append(' ').Append(Money(hand.Net, true)).Append('\n');
            }
            if (round.Insurance > 0)
                sb.Append("Insurance ").Append(Money(round.InsuranceNet, true)).Append('\n');
            return sb.ToString().TrimEnd();
        }

        // ---- UI construction ----

        void BuildUi()
        {
            var canvasGo = MakeCanvas(transform, "Table Canvas", 0).gameObject;
            _root = canvasGo;

            var background = MakeImage(canvasGo.transform, "Felt", Felt);
            Stretch(background.rectTransform);

            var root = new GameObject("Safe Area", typeof(RectTransform), typeof(SafeAreaFitter));
            root.transform.SetParent(canvasGo.transform, false);
            Stretch((RectTransform)root.transform);
            _portraitColumn = MakeColumn(root.transform, "Portrait", 40);
            var column = _portraitColumn;

            _landscapeRow = MakeRow(root.transform, "Landscape");
            Stretch(_landscapeRow);
            var sides = _landscapeRow.GetComponent<HorizontalLayoutGroup>();
            sides.spacing = 48;
            sides.padding = new RectOffset(40, 40, 40, 40);
            sides.childForceExpandHeight = true;
            _tableSide = MakeColumn(_landscapeRow, "Table", 0);
            _tableSide.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1.6f;
            _controlSide = MakeColumn(_landscapeRow, "Controls", 0);
            _controlSide.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1;
            var spacer = new GameObject("Spacer", typeof(RectTransform), typeof(LayoutElement));
            spacer.transform.SetParent(_controlSide, false);
            spacer.GetComponent<LayoutElement>().flexibleHeight = 1;
            _controlSpacer = (RectTransform)spacer.transform;

            _bankrollText = MakeText(column, "", 46, TextAnchor.MiddleLeft, 64);
            _clockText = MakeText(column, "", 36, TextAnchor.MiddleLeft, 52);
            _clockText.color = new Color(1f, 1f, 1f, 0.75f);

            _dealerLabel = MakeText(column, "", 40, TextAnchor.MiddleCenter, 60);
            _dealerCards = MakeCardRow(column);

            _messageText = MakeText(column, "", 48, TextAnchor.MiddleCenter, 120);
            _messageText.color = Gold;
            _messageText.GetComponent<LayoutElement>().flexibleHeight = 1;

            var handsGo = new GameObject("Player Hands", typeof(RectTransform));
            handsGo.transform.SetParent(column, false);
            _playerHands = (RectTransform)handsGo.transform;
            AddVertical(handsGo, 12, new RectOffset());

            _betText = MakeText(column, "", 44, TextAnchor.MiddleCenter, 64);

            _bettingControls = new GameObject("Betting", typeof(RectTransform));
            _bettingControls.transform.SetParent(column, false);
            var betting = AddVertical(_bettingControls, 16, new RectOffset());
            var chips = MakeRow(betting, "Chips");
            _chipButtons = new Button[_chipValues.Length];
            for (int i = 0; i < _chipValues.Length; i++)
            {
                int value = _chipValues[i];
                _chipButtons[i] = Control(chips, BlackjackControls.Chip(value), "+" + Money(value), () => AddChip(value));
            }
            Control(chips, BlackjackControls.Clear, "Clear", ClearBet);
            _dealButton = Control(betting, BlackjackControls.Deal, "Deal", Deal);
            _dealButton.GetComponent<Image>().color = Gold;
            _dealButton.GetComponentInChildren<TMP_Text>().color = CardBlack;

            var actions = MakeRow(column, "Actions");
            _actionRow = actions.gameObject;
            _hitButton = Control(actions, BlackjackControls.Hit, "Hit", () => Act(_round.Hit));
            _standButton = Control(actions, BlackjackControls.Stand, "Stand", () => Act(_round.Stand));
            _doubleButton = Control(actions, BlackjackControls.Double, "Double", () => Act(_round.Double));
            _splitButton = Control(actions, BlackjackControls.Split, "Split", () => Act(_round.Split));

            var insurance = MakeRow(column, "Insurance");
            _insuranceRow = insurance.gameObject;
            _insureLabel = Control(insurance, BlackjackControls.Insurance, "Insurance", () => Act(_round.TakeInsurance))
                .GetComponentInChildren<TMP_Text>();
            Control(insurance, BlackjackControls.NoInsurance, "No insurance", () => Act(_round.DeclineInsurance));

            _getUpButton = Control(column, BlackjackControls.GetUp, "Get up", GetUp);

            if (_standalone) BuildBreakPanel(canvasGo.transform);
            OnTurn(canvasGo.GetComponent<Canvas>(), Rearrange);
        }

        /// <summary>
        /// Portrait is one column, top to bottom. Landscape puts the table (dealer, message, your
        /// hands) on the left and your money and the controls on the right.
        /// </summary>
        void Rearrange(bool landscape)
        {
            _portraitColumn.gameObject.SetActive(!landscape);
            _landscapeRow.gameObject.SetActive(landscape);
            if (landscape)
            {
                Arrange(_tableSide, _dealerLabel, _dealerCards, _messageText, _playerHands);
                Arrange(_controlSide, _bankrollText, _clockText, _controlSpacer, _betText,
                    _bettingControls.transform, _actionRow.transform, _insuranceRow.transform, _getUpButton);
            }
            else
            {
                Arrange(_portraitColumn, _bankrollText, _clockText, _dealerLabel, _dealerCards, _messageText,
                    _playerHands, _betText, _bettingControls.transform, _actionRow.transform,
                    _insuranceRow.transform, _getUpButton);
            }
            if (_breakPanel == null) return;
            _breakPanel.GetComponent<VerticalLayoutGroup>().padding =
                landscape ? new RectOffset(560, 560, 60, 60) : new RectOffset(80, 80, 300, 300);
            _breakText.GetComponent<LayoutElement>().preferredHeight = landscape ? 360 : 600;
        }

        /// <summary>A button a coach can point at by <paramref name="id"/>.</summary>
        Button Control(Transform parent, string id, string label, Action onClick)
        {
            var button = MakeButton(parent, label, () => Press(id, onClick));
            _controls[id] = button;
            return button;
        }

        static RectTransform MakeColumn(Transform parent, string name, int padding)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            Stretch((RectTransform)go.transform);
            return AddVertical(go, 24, new RectOffset(padding, padding, padding, padding));
        }

        void BuildBreakPanel(Transform canvas)
        {
            var shade = MakeImage(canvas, "Break Panel", new Color(0f, 0f, 0f, 0.85f));
            Stretch(shade.rectTransform);
            _breakPanel = shade.gameObject;
            var column = AddVertical(_breakPanel, 24, new RectOffset(80, 80, 300, 300));
            column.GetComponent<VerticalLayoutGroup>().childAlignment = TextAnchor.MiddleCenter;

            _breakText = MakeText(column, "", 44, TextAnchor.MiddleCenter, 600);
            MakeButton(column, "Sit back down", SitDown);
            MakeButton(column, "Sleep until morning", SleepUntilMorning);
            MakeButton(column, "Start over (wipe save)", StartOver);
        }

        void MakeHandView(PlayerHand hand, int index, bool isActive)
        {
            var label = MakeText(_playerHands, "", 40, TextAnchor.MiddleCenter, 56);
            string total = hand.IsSoft && !hand.IsBust && hand.Total < 21 ? $"soft {hand.Total}" : hand.Total.ToString();
            string prefix = _round.Hands.Count > 1 ? $"Hand {index + 1} · " : "You · ";
            label.text = $"{prefix}{total} · {Money(hand.Bet)}{(hand.Doubled ? " doubled" : "")}";
            label.color = isActive ? Gold : Color.white;

            var row = MakeCardRow(_playerHands);
            foreach (var card in hand.Cards) MakeCard(row, card, faceUp: true);
            SetCardSpacing(row, hand.Cards.Count);
        }

        void MakeCard(Transform row, Card card, bool faceUp)
        {
            var image = MakeImage(row, faceUp ? card.ToString() : "Hole card", faceUp ? Color.white : CardBack);
            var layout = image.gameObject.AddComponent<LayoutElement>();
            layout.preferredWidth = 140;
            layout.preferredHeight = 200;
            if (!faceUp) return;

            var text = MakeText(image.transform, card.RankLabel + "\n" + card.SuitSymbol, 56, TextAnchor.MiddleCenter, 0);
            Stretch(text.rectTransform);
            text.color = card.IsRed ? CardRed : CardBlack;
        }

        RectTransform MakeCardRow(Transform parent)
        {
            var row = MakeRow(parent, "Cards");
            row.GetComponent<LayoutElement>().preferredHeight = 200;
            var group = row.GetComponent<HorizontalLayoutGroup>();
            group.childForceExpandWidth = false;
            group.childAlignment = TextAnchor.MiddleCenter;
            return row;
        }

        static void SetCardSpacing(RectTransform row, int count) =>
            row.GetComponent<HorizontalLayoutGroup>().spacing = count > 6 ? -40 : 12;
    }
}
