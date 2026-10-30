using System;
using System.Text;
using Starflare.Blackjack;
using Starflare.Cards;
using Starflare.State;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Starflare.Game
{
    public enum BreakPoint { GotUpFromTable }

    /// <summary>
    /// The blackjack table: portrait UI built in code, one round at a time. It reports each
    /// settled round to the player state and raises a break point when you get up. It never
    /// reads story state.
    /// </summary>
    public sealed class BlackjackTable : MonoBehaviour
    {
        static readonly int[] ChipValues = { 5, 25, 100 };

        static readonly Color Felt = new Color(0.05f, 0.33f, 0.2f);
        static readonly Color Gold = new Color(0.95f, 0.76f, 0.25f);
        static readonly Color ButtonColor = new Color(0.12f, 0.12f, 0.16f);
        static readonly Color CardBack = new Color(0.16f, 0.2f, 0.55f);
        static readonly Color CardRed = new Color(0.8f, 0.1f, 0.12f);
        static readonly Color CardBlack = new Color(0.08f, 0.08f, 0.08f);

        /// <summary>Raised at a break point, where story beats and seed reveals may fire.</summary>
        public event Action<BreakPoint> BreakPointReached;

        readonly BlackjackRules _rules = new BlackjackRules();
        PlayerState _state;
        Shoe _shoe;
        BlackjackRound _round;
        int _bet;
        int _sittingStartBankroll;
        int _sittingRounds;
        string _message = "";

        Font _font;
        Text _bankrollText, _clockText, _dealerLabel, _messageText, _betText, _breakText;
        RectTransform _dealerCards, _playerHands;
        GameObject _bettingControls, _actionRow, _breakPanel;
        Button _dealButton, _hitButton, _standButton, _doubleButton, _splitButton, _getUpButton;
        Button[] _chipButtons;

        void Awake()
        {
            _font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            _state = SaveStore.Load();
            _shoe = new Shoe(_rules.Decks, _rules.Penetration, new System.Random());
            BuildUi();
            SitDown();
        }

        void OnApplicationPause(bool paused)
        {
            if (paused) SaveStore.Save(_state);
        }

        void OnApplicationQuit() => SaveStore.Save(_state);

        void SitDown()
        {
            _round = null;
            _sittingStartBankroll = _state.BankrollNow;
            _sittingRounds = 0;
            _bet = Mathf.Min(_rules.MinBet, _state.BankrollNow);
            _message = "Place your bet.";
            _breakPanel.SetActive(false);
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
            if (!Betting || Broke) return;
            _bet = Mathf.Clamp(_bet, _rules.MinBet, MaxAffordableBet);

            bool shuffled = false;
            if (_shoe.PastCutCard)
            {
                _shoe.Shuffle();
                shuffled = true;
            }

            _round = new BlackjackRound(_rules, _shoe, _bet, _state.BankrollNow);
            _message = shuffled ? "The dealer shuffles a fresh shoe." : "";
            AfterAction();
        }

        void Act(Action action)
        {
            action();
            AfterAction();
        }

        void AfterAction()
        {
            if (_round.Phase == RoundPhase.Settled)
            {
                _state.ApplyRoundResult(_round.Net, _rules.MinutesPerRound);
                _sittingRounds++;
                SaveStore.Save(_state);
                _message = DescribeResult(_round);
                _bet = Mathf.Min(_bet, MaxAffordableBet);
            }
            Refresh();
        }

        void GetUp()
        {
            if (!Betting) return;
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
            _state.Clock.Sleep();
            SaveStore.Save(_state);
            SitDown();
        }

        void StartOver()
        {
            _state = new PlayerState();
            SaveStore.Save(_state);
            SitDown();
        }

        // ---- Display ----

        void Refresh()
        {
            var clock = _state.Clock;
            _bankrollText.text = $"Bankroll {Money(_state.BankrollNow)}     Best {Money(_state.BestBankroll)}";
            _clockText.text = $"Day {clock.Day} · {clock.Season} {clock.DayOfSeason} · {clock.Part} {clock.TimeLabel}"
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

            if (Broke && Betting)
                _message = "You're broke. Get up and find another way back in.";
            _messageText.text = _message;
            _betText.text = Betting ? $"Bet {Money(_bet)}" : $"In play {Money(_round.TotalStake)}";

            bool betting = Betting;
            _bettingControls.SetActive(betting);
            _actionRow.SetActive(!betting);
            _dealButton.interactable = betting && !Broke;
            foreach (var chip in _chipButtons) chip.interactable = betting && !Broke && _bet < MaxAffordableBet;
            _getUpButton.interactable = betting;

            if (!betting)
            {
                _hitButton.interactable = _round.CanHit;
                _standButton.interactable = _round.CanStand;
                _doubleButton.interactable = _round.CanDouble;
                _splitButton.interactable = _round.CanSplit;
            }
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
            return sb.ToString().TrimEnd();
        }

        static string Money(int amount, bool signed = false)
        {
            string body = "$" + Math.Abs(amount).ToString("N0");
            if (amount < 0) return "-" + body;
            return signed && amount > 0 ? "+" + body : body;
        }

        // ---- UI construction ----

        void BuildUi()
        {
            if (FindAnyObjectByType<EventSystem>() == null)
                new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));

            var canvasGo = new GameObject("Table Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasGo.transform.SetParent(transform, false);
            canvasGo.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0f;

            var background = MakeImage(canvasGo.transform, "Felt", Felt);
            Stretch(background.rectTransform);

            var root = new GameObject("Safe Area", typeof(RectTransform), typeof(SafeAreaFitter));
            root.transform.SetParent(canvasGo.transform, false);
            Stretch((RectTransform)root.transform);
            var column = AddVertical(root, 24, new RectOffset(40, 40, 40, 40));

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
            _chipButtons = new Button[ChipValues.Length];
            for (int i = 0; i < ChipValues.Length; i++)
            {
                int value = ChipValues[i];
                _chipButtons[i] = MakeButton(chips, "+" + Money(value), () => AddChip(value));
            }
            MakeButton(chips, "Clear", ClearBet);
            _dealButton = MakeButton(betting, "Deal", Deal);
            _dealButton.GetComponent<Image>().color = Gold;
            _dealButton.GetComponentInChildren<Text>().color = CardBlack;

            var actions = MakeRow(column, "Actions");
            _actionRow = actions.gameObject;
            _hitButton = MakeButton(actions, "Hit", () => Act(_round.Hit));
            _standButton = MakeButton(actions, "Stand", () => Act(_round.Stand));
            _doubleButton = MakeButton(actions, "Double", () => Act(_round.Double));
            _splitButton = MakeButton(actions, "Split", () => Act(_round.Split));

            _getUpButton = MakeButton(column, "Get up", GetUp);

            BuildBreakPanel(canvasGo.transform);
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

        RectTransform MakeRow(Transform parent, string name)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(LayoutElement));
            go.transform.SetParent(parent, false);
            var group = go.GetComponent<HorizontalLayoutGroup>();
            group.spacing = 16;
            group.childControlWidth = true;
            group.childControlHeight = true;
            group.childForceExpandWidth = true;
            group.childForceExpandHeight = false;
            return (RectTransform)go.transform;
        }

        static RectTransform AddVertical(GameObject go, float spacing, RectOffset padding)
        {
            var group = go.AddComponent<VerticalLayoutGroup>();
            group.spacing = spacing;
            group.padding = padding;
            group.childControlWidth = true;
            group.childControlHeight = true;
            group.childForceExpandWidth = true;
            group.childForceExpandHeight = false;
            return (RectTransform)go.transform;
        }

        Button MakeButton(Transform parent, string label, UnityAction onClick)
        {
            var image = MakeImage(parent, label + " Button", ButtonColor);
            var layout = image.gameObject.AddComponent<LayoutElement>();
            layout.preferredHeight = 140;
            layout.flexibleWidth = 1;
            var button = image.gameObject.AddComponent<Button>();
            button.onClick.AddListener(onClick);
            var text = MakeText(image.transform, label, 44, TextAnchor.MiddleCenter, 0);
            Stretch(text.rectTransform);
            return button;
        }

        Text MakeText(Transform parent, string content, int size, TextAnchor anchor, float height)
        {
            var go = new GameObject("Text", typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);
            var text = go.GetComponent<Text>();
            text.font = _font;
            text.fontSize = size;
            text.alignment = anchor;
            text.color = Color.white;
            text.text = content;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.raycastTarget = false;
            if (height > 0) go.AddComponent<LayoutElement>().preferredHeight = height;
            return text;
        }

        static Image MakeImage(Transform parent, string name, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            var image = go.GetComponent<Image>();
            image.color = color;
            return image;
        }

        static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        static void ClearChildren(Transform parent)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                var child = parent.GetChild(i).gameObject;
                child.SetActive(false);
                Destroy(child);
            }
        }
    }
}
