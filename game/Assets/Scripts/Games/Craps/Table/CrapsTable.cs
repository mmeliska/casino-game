using System;
using System.Collections.Generic;
using System.Text;
using Starflare.State;
using Starflare.Ui;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Starflare.Ui.UiKit;

namespace Starflare.Craps
{
    /// <summary>
    /// Juan's late game in Millstone Lanes' back lot (GDD, "Street craps"): a chalk circle on the
    /// asphalt over the venue's plate, you and three regulars around it, cash and no felt. The
    /// dice go around; you throw on your turn and tap to call each regular's throw, so nothing
    /// happens while you think. Each throw that settles a bet of yours is a round, reported to
    /// the player state with the story minutes played, and every win shows Juan's cut on its own
    /// line. You get up between throws when nothing of yours is riding; that's the break point.
    ///
    /// It never reads story state, counts heat or saves: the venue does those. A venue can hand
    /// it a <see cref="PracticeTurn"/> to shoot first, which doesn't count, and a teacher can
    /// coach over it (<see cref="ICoachable"/>, by the ids in <see cref="CrapsControls"/>).
    /// </summary>
    public sealed class CrapsTable : MonoBehaviour, ICoachable
    {
        static readonly Color Chalk = new Color(0.94f, 0.94f, 0.9f, 0.8f);
        static readonly Color DieFace = new Color(0.96f, 0.95f, 0.9f);
        static readonly Color Seat = new Color(0.32f, 0.32f, 0.34f);
        static readonly BetKind[] Kinds = { BetKind.Right, BetKind.Wrong, BetKind.PointFor, BetKind.PointAgainst };

        /// <summary>Pips on a die face, as cells of a 3x3 grid (x, y from -1 to 1), by face value.</summary>
        static readonly Vector2Int[][] PipCells =
        {
            new Vector2Int[0],
            new[] { new Vector2Int(0, 0) },
            new[] { new Vector2Int(-1, 1), new Vector2Int(1, -1) },
            new[] { new Vector2Int(-1, 1), new Vector2Int(0, 0), new Vector2Int(1, -1) },
            new[] { new Vector2Int(-1, 1), new Vector2Int(1, 1), new Vector2Int(-1, -1), new Vector2Int(1, -1) },
            new[] { new Vector2Int(-1, 1), new Vector2Int(1, 1), new Vector2Int(0, 0), new Vector2Int(-1, -1), new Vector2Int(1, -1) },
            new[] { new Vector2Int(-1, 1), new Vector2Int(1, 1), new Vector2Int(-1, 0), new Vector2Int(1, 0), new Vector2Int(-1, -1), new Vector2Int(1, -1) },
        };

        /// <summary>Raised at a break point, where story beats and seed reveals may fire.</summary>
        public event Action<BreakPoint> BreakPointReached;

        public event Action<string> ActionTaken;

        public CrapsRules Rules => _rules;

        /// <summary>This sitting so far; still readable at the break point after you get up.</summary>
        public int SittingNet => _state == null ? 0 : _state.BankrollNow - _sittingStartBankroll;

        /// <summary>A practice turn is being played: none of it counts.</summary>
        public bool Practicing => _practiceGame != null;

        /// <summary>The seat holding the dice: 0 is you.</summary>
        public int Shooter => Current.Shooter;

        CrapsRules _rules;
        Regular[] _regulars;
        Sprite _plate;
        float _plateFocus;

        /// <summary>The circle's game, on fair dice; it carries on between sittings, the dice wherever they'd got to.</summary>
        CrapsGame _game;
        Dice _fairDice;
        CrapsGame _practiceGame;
        Dice _practiceDice;
        int _practiceStake;

        readonly Dictionary<string, Button> _controls = new Dictionary<string, Button>();
        string _lock;
        PlayerState _state;
        ClockPace _pace;
        Sitting _sitting;
        int _sittingStartBankroll;
        Roll? _lastRoll;
        int _lastShooter;
        string _message = "";

        /// <summary>The dice in the air: how long they've been, and when they next show a new face.</summary>
        bool _rolling;
        float _rollTime, _nextTumble;

        /// <summary>The payout popup, and whether it's waiting for the dice to land.</summary>
        bool _payoutPending;
        GameObject _payout;
        TMP_Text _payoutTitle, _payoutLines;
        Image _payoutEdge;

        GameObject _root;
        Image _plateImage;
        CoverFit _plateFit;

        /// <summary>The painted circles for each way up, if the venue has them, and the one showing.</summary>
        CircleArt _portraitArt, _landscapeArt, _art;
        bool _landscape;

        /// <summary>Lies exactly over the plate as it's fitted, so the dice can sit on its painted circle.</summary>
        RectTransform _ground;
        CoverFit _groundFit;
        Image _ring;
        RectTransform _diceRect;
        TMP_Text _bankrollText, _clockText, _messageText, _pointText, _rollText, _totalText;
        Image _pointBadge, _pointEdge;
        RectTransform _portraitColumn, _landscapeRow, _circleSide, _controlSide, _header, _seats, _circle, _resultBar, _bets, _actions;
        Button _throwButton, _callButton, _passButton, _getUpButton;
        readonly Image[][] _pips = new Image[2][];
        readonly RectTransform[] _dice = new RectTransform[2];
        CanvasGroup[] _reveal;
        Image[] _seatFrames;
        GameObject[] _shooterTags;
        TMP_Text[] _seatNames, _seatInitials;
        Image[] _seatPortraits, _seatDiscs;
        readonly TMP_Text[] _betLabels = new TMP_Text[4], _betHints = new TMP_Text[4], _betAmounts = new TMP_Text[4];
        readonly Button[] _upButtons = new Button[4], _downButtons = new Button[4];

        CrapsGame Current => _practiceGame ?? _game;
        int Cash => Practicing ? _practiceStake : _state.BankrollNow;

        /// <summary>
        /// Sets the circle up with its rules, over the venue's plate framed at this focus, with
        /// the regulars standing in it (one fewer than the rules' seats). It stays hidden until
        /// you sit down; the venue shows its own screens around it.
        /// </summary>
        public void Init(CrapsRules rules, Sprite plate, float plateFocus, params Regular[] regulars)
        {
            _rules = rules ?? throw new ArgumentNullException(nameof(rules));
            CheckRegulars(regulars);
            _regulars = regulars;
            _plate = plate;
            _plateFocus = plateFocus;
            _fairDice = new Dice(new System.Random());
            _game = new CrapsGame(_rules, _fairDice);
            BuildUi();
            _root.SetActive(false);
        }

        /// <summary>
        /// Stands other regulars in the circle, as a new night brings a different crowd. Only
        /// while you're out of the circle; the dice stay with whichever seat had them.
        /// </summary>
        public void SetRegulars(params Regular[] regulars)
        {
            CheckRegulars(regulars);
            if (_root.activeSelf) throw new InvalidOperationException("The crowd changes only while you're out of the circle.");
            _regulars = regulars;
            for (int seat = 1; seat < _rules.Seats; seat++) ShowRegular(seat);
        }

        void CheckRegulars(Regular[] regulars)
        {
            if (regulars == null || regulars.Length != _rules.Seats - 1 || Array.IndexOf(regulars, null) >= 0)
                throw new ArgumentException($"The circle needs {_rules.Seats - 1} regulars.", nameof(regulars));
        }

        /// <summary>Puts the circle over another plate, as the night moves on.</summary>
        public void SetBackdrop(Sprite plate, float plateFocus)
        {
            _plate = plate;
            _plateFocus = plateFocus;
            ShowBackdrop();
        }

        /// <summary>
        /// Plays the circle on painted grounds instead, one for each way up the screen turns (either
        /// may be null, and the other stands in). They take the place of the venue's plate and the
        /// drawn ring; the dice land in the painted circle and go off the painted wall.
        /// </summary>
        public void SetCircleArt(CircleArt portrait, CircleArt landscape)
        {
            _portraitArt = portrait;
            _landscapeArt = landscape;
            ShowBackdrop();
        }

        /// <summary>The painted ground for this way up if there is one, else the venue's plate with the ring drawn over it.</summary>
        void ShowBackdrop()
        {
            _art = _landscape ? _landscapeArt ?? _portraitArt : _portraitArt ?? _landscapeArt;
            _ring.enabled = _art == null;
            if (_art == null)
            {
                _plateFit.Show(_plateImage, _plate, _plateFocus);
                _diceRect.SetParent(_circle, false);
                _diceRect.anchorMin = _diceRect.anchorMax = new Vector2(0.5f, 0.37f);
            }
            else
            {
                float focus = _art.Circle.center.x;
                _plateFit.Show(_plateImage, _art.Plate, focus);
                _groundFit.Aspect = _art.Plate.rect.width / _art.Plate.rect.height;
                _groundFit.Focus = focus;
                _diceRect.SetParent(_ground, false);
                _diceRect.anchorMin = _diceRect.anchorMax = _art.Circle.center;
            }
            _diceRect.anchoredPosition = Vector2.zero;
        }

        /// <summary>
        /// Steps you into the circle with this state, the clock moving at this pace. A practice
        /// turn, if given, is played first: you shoot it, and the real game picks up with the
        /// dice where it leaves them.
        /// </summary>
        public void SitDown(PlayerState state, ClockPace pace, PracticeTurn practice = null)
        {
            _state = state ?? throw new ArgumentNullException(nameof(state));
            _pace = pace ?? throw new ArgumentNullException(nameof(pace));
            _sitting = new Sitting(_pace);
            _sittingStartBankroll = _state.BankrollNow;
            _lastRoll = null;
            FinishRoll();
            if (practice != null)
            {
                _practiceDice = Dice.Set(practice.Rolls);
                _practiceGame = new CrapsGame(_rules, _practiceDice);
                _practiceStake = practice.Stake;
                _message = "Practice turn, on Juan. Nothing counts.";
            }
            else
            {
                _message = Prompt();
            }
            _root.SetActive(true);
            Refresh();
        }

        public RectTransform Control(string id) => _controls.TryGetValue(id, out var button) ? (RectTransform)button.transform : null;

        public void Lock(string id)
        {
            _lock = id;
            if (_state != null) Refresh();
        }

        /// <summary>Counts real time played. Not through a practice turn, while a teacher talks (it's locked) or the app is away.</summary>
        /// <summary>
        /// On painted art, the dice rest at the painted circle's height but in line with the
        /// middle of the circle's column, under the faces and the popup (the art is drawn to suit).
        /// </summary>
        void CentreDice()
        {
            if (_art == null) return;
            var at = _diceRect.position;
            at.x = _circle.TransformPoint(_circle.rect.center).x;
            _diceRect.position = at;
        }

        /// <summary>In landscape, the popup's top lines up with the top of the bet spots beside it.</summary>
        void AlignPayout()
        {
            if (!_landscape || !_payout.activeSelf) return;
            var corners = new Vector3[4];
            _bets.GetWorldCorners(corners);
            var at = _payout.transform.position;
            at.y = corners[1].y;
            _payout.transform.position = at;
        }

        /// <summary>The width the seat cards were last sized for.</summary>
        float _seatSlot;

        /// <summary>
        /// Sizes the seat cards to the width each slot has: full size in portrait, smaller where
        /// the landscape circle side is narrow, so the four never overlap. The row is as tall as a
        /// card and its name.
        /// </summary>
        void LateUpdate()
        {
            if (_root == null || !_root.activeSelf) return;
            CentreDice();
            AlignPayout();
            float slot = ((RectTransform)_seatFrames[0].transform.parent).rect.width;
            if (slot <= 0 || Mathf.Approximately(slot, _seatSlot)) return;
            _seatSlot = slot;
            float card = Mathf.Clamp(slot - 8, 80, 196);
            float height = card + 54;
            foreach (var frame in _seatFrames)
            {
                frame.rectTransform.sizeDelta = new Vector2(card, card);
                frame.transform.parent.GetComponent<LayoutElement>().preferredHeight = height;
            }
            _seats.GetComponent<LayoutElement>().preferredHeight = height;
        }

        void Update()
        {
            if (_root == null || !_root.activeSelf) return;
            if (_rolling) Tumble(Time.unscaledDeltaTime);
            if (_sitting == null || _lock != null || Practicing) return;
            _sitting.Play(Math.Min(Time.unscaledDeltaTime, Sitting.MaxFrameSeconds));
        }

        // ---- Play ----

        /// <summary>Moves a bet to the next amount the circle takes that your cash covers, up or down; down from the smallest picks it up.</summary>
        void Step(BetKind kind, bool up)
        {
            var game = Current;
            if (!game.CanBet(kind)) return;
            int now = game.Bet(kind);
            int next = NextAmount(game, kind, up);
            if (next > 0) game.Place(kind, next, Cash);
            else if (!up && now > 0) game.TakeBack(kind);
            Refresh();
        }

        /// <summary>The amount a step would move a bet to, or 0 for none (down from the smallest means picking it up).</summary>
        int NextAmount(CrapsGame game, BetKind kind, bool up)
        {
            int now = game.Bet(kind);
            var amounts = _rules.Amounts(kind, game.Point);
            if (up)
            {
                foreach (int amount in amounts)
                    if (amount > now && game.CanPlace(kind, amount, Cash)) return amount;
                return 0;
            }
            for (int i = amounts.Count - 1; i >= 0; i--)
                if (amounts[i] < now) return amounts[i];
            return 0;
        }

        void Throw()
        {
            var game = Current;
            if (!game.CanThrow) return;
            int point = game.Point;
            var result = game.Throw();
            _lastRoll = result.Roll;
            _lastShooter = result.Shooter;
            StartRoll();
            SetPayout(result, point);

            if (Practicing)
            {
                _message = Describe(result, point);
                if (!_practiceDice.HasMore) EndPractice();
            }
            else
            {
                if (result.SettledAny) _state.ApplyRoundResult(result.Net, _sitting.TakeMinutes());
                _message = Describe(result, point);
            }
            Refresh();
        }

        /// <summary>The set dice are done: the real game starts with the dice where the practice left them, and fair from here.</summary>
        void EndPractice()
        {
            _game = new CrapsGame(_rules, _fairDice, _practiceGame.Shooter);
            _practiceGame = null;
            _practiceDice = null;
            _message = "That's the practice. Real money from here.\n" + _message;
        }

        void PassDice()
        {
            var game = Current;
            if (!game.CanPassDice) return;
            game.PassDice();
            _message = $"You pass the dice to {NameOf(game.Shooter)}.";
            Refresh();
        }

        /// <summary>A control's press: ignored while the coach has another one, reported once done.</summary>
        void Press(string id, Action action)
        {
            if (_lock != null && _lock != id) return;
            HidePayout();
            action();
            ActionTaken?.Invoke(id);
        }

        bool CanGetUp => !Practicing && !_game.AnyRiding;

        void GetUp()
        {
            if (!CanGetUp) return;
            // Money placed but not yet thrown on goes back in your pocket.
            foreach (var kind in Kinds)
                if (_game.Bet(kind) > 0) _game.TakeBack(kind);
            _state.PassTime(_sitting.TakeMinutes());
            FinishRoll();
            _root.SetActive(false);
            BreakPointReached?.Invoke(BreakPoint.GotUpFromTable);
        }

        // ---- Words ----

        string NameOf(int seat) => seat == CrapsGame.You ? "you" : _regulars[seat - 1].Name;

        /// <summary>What the circle is waiting on, between throws.</summary>
        string Prompt()
        {
            var game = Current;
            if (game.YouShoot && game.Phase == CrapsPhase.ComeOut)
                return game.Bet(BetKind.Right) > 0 ? "Your dice. Throw when you're ready." : "Your dice. Bet right to shoot, or pass the dice.";
            if (game.YouShoot) return $"Your point is {game.Point}. Throw again.";
            return game.Phase == CrapsPhase.ComeOut
                ? $"{NameOf(game.Shooter)} has the dice. Bet, then call it."
                : $"{NameOf(game.Shooter)}'s point is {game.Point}. Call it.";
        }

        /// <summary>
        /// What the throw did, in one line. The dice are on the result bar above ("Nick rolled
        /// 5 + 5 | 10"); what your bets did is the payout popup's.
        /// </summary>
        string Describe(ThrowResult result, int pointBefore)
        {
            var roll = result.Roll;
            if (result.PointSet > 0) return $"The point is {result.PointSet}.";
            if (result.PointMade) return "Point made.";
            if (result.SevenOut) return $"Seven out. The dice go to {NameOf(Current.Shooter)}.";
            if (pointBefore == 0) return roll.Total == 7 || roll.Total == 11 ? "A natural." : "Craps.";
            return $"No decision. The point's still {pointBefore}.";
        }

        /// <summary>
        /// The popup's words for a throw that settled any of your bets: the total won or lost, big,
        /// then a line for each bet with Juan's cut on its own (GDD, "Juan's cut").
        /// </summary>
        void SetPayout(ThrowResult result, int pointBefore)
        {
            _payoutPending = result.SettledAny;
            if (!result.SettledAny) return;
            int net = result.Net;
            bool allPushed = result.Settled.TrueForAll(bet => bet.Outcome == BetOutcome.Push);
            _payoutTitle.text = net > 0 ? $"You won {Money(net)}"
                : net < 0 ? $"You lost {Money(-net)}"
                : allPushed ? "Push. Money back." : "You broke even";
            var mood = net > 0 ? Gold : net < 0 ? Loss : Color.white;
            _payoutTitle.color = mood;
            _payoutEdge.color = mood;
            var sb = new StringBuilder();
            foreach (var bet in result.Settled)
            {
                if (sb.Length > 0) sb.Append('\n');
                sb.Append(PayoutLine(bet, pointBefore));
            }
            if (Practicing) sb.Append("\n<size=26><alpha=#99>Practice, so it doesn't count</size>");
            _payoutLines.text = sb.ToString();
        }

        /// <summary>
        /// One settled bet, Juan's cut on its own line and never netted out quietly (GDD, "Juan's
        /// cut"): "Right +$12 · Juan's dollar -$1".
        /// </summary>
        public static string PayoutLine(SettledBet bet, int point)
        {
            string name = BetName(bet.Kind, point);
            switch (bet.Outcome)
            {
                case BetOutcome.Won:
                    string line = $"{name} {Money(bet.Net + bet.Cut, true)}";
                    if (bet.Cut > 0) line += $" · Juan's {(bet.Cut == 1 ? "dollar" : "dollars")} {Money(-bet.Cut)}";
                    return line;
                case BetOutcome.Lost:
                    return $"{name} {Money(bet.Net)}";
                default:
                    return $"{name} pushes · {Money(bet.Amount)} back";
            }
        }

        static string BetName(BetKind kind, int point) => kind switch
        {
            BetKind.Right => "Right",
            BetKind.Wrong => "Wrong",
            BetKind.PointFor => point > 0 ? $"For the {point}" : "For the point",
            _ => point > 0 ? $"Against the {point}" : "Against the point",
        };

        /// <summary>
        /// The line under a bet spot's name: what it's a bet on and what it pays, or when it opens
        /// if it can't take money now. Pays are said Juan's way, "6 for 5".
        /// </summary>
        static string BetHint(BetKind kind, int point, bool open)
        {
            bool line = kind == BetKind.Right || kind == BetKind.Wrong;
            if (!open) return line ? "Opens on the next come-out" : "Opens once a point is set";
            if (line) return kind == BetKind.Right ? "With the shooter · pays even" : "Against the shooter · pays even";
            var (pays, per) = CrapsRules.Odds(kind, point);
            return kind == BetKind.PointFor
                ? $"A {point} before a 7 · pays {pays} for {per}"
                : $"A 7 before the {point} · pays {pays} for {per}";
        }

        // ---- Display ----

        static readonly Color Faded = new Color(1, 1, 1, 0.35f);
        static readonly Color Line = new Color(1, 1, 1, 0.22f);
        static readonly Color SeatEdge = new Color(1, 1, 1, 0.3f);
        static readonly Color Outlined = new Color(0.05f, 0.06f, 0.07f, 0.75f);
        static readonly Color Loss = new Color(1f, 0.45f, 0.4f);

        void Refresh()
        {
            var game = Current;
            var clock = _state.Clock;
            _bankrollText.text = Practicing ? $"Practice · {Money(_practiceStake)} of Juan's, pretend"
                : $"Cash {Money(_state.BankrollNow)}     Down {Money(game.Staked)}";
            _bankrollText.color = Practicing ? Gold : Color.white;
            _clockText.text = $"{clock.DateLabel} · {clock.Part} {clock.TimeLabel}" + (_sitting.ClockStopped ? " (late)" : "");

            // Between throws, the message says what the circle's waiting on; after one, what it did.
            bool broke = !Practicing && game.Staked == 0 && _state.BankrollNow < _rules.MinBet;
            _messageText.text = broke ? "You're broke. Step out and find another way back in." : _message;

            // The result bar: the last throw, who threw it, and its total, big.
            if (_lastRoll is Roll roll)
            {
                string who = _lastShooter == CrapsGame.You ? "You" : NameOf(_lastShooter);
                _rollText.text = $"{who} rolled {roll.Die1} + {roll.Die2}";
                _totalText.text = roll.Total.ToString();
            }
            else
            {
                _rollText.text = "No dice thrown yet";
                _totalText.text = "-";
            }

            for (int seat = 0; seat < _rules.Seats; seat++)
            {
                bool shooting = seat == game.Shooter;
                _seatFrames[seat].color = shooting ? Gold : SeatEdge;
                _shooterTags[seat].SetActive(shooting);
            }

            if (!_rolling)
            {
                ShowDie(0, _lastRoll?.Die1 ?? 0);
                ShowDie(1, _lastRoll?.Die2 ?? 0);
            }
            bool on = game.Phase == CrapsPhase.Point;
            _pointText.text = on ? $"POINT  <size=64><b>{game.Point}</b></size>" : "POINT  <size=40><b>OFF</b></size>";
            _pointText.color = on ? Color.white : Faded;
            _pointEdge.color = on ? Gold : SeatEdge;

            // Every control starts each redraw usable, then the rules and any coach's lock narrow it.
            foreach (var control in _controls.Values) control.interactable = true;
            for (int i = 0; i < Kinds.Length; i++)
            {
                var kind = Kinds[i];
                int amount = game.Bet(kind);
                bool riding = game.IsRiding(kind);
                bool open = game.CanBet(kind);
                bool live = open || amount > 0;
                _betLabels[i].text = BetName(kind, game.Point);
                _betLabels[i].color = live ? Color.white : Faded;
                _betHints[i].text = BetHint(kind, game.Point, open || riding);
                _betHints[i].color = live ? new Color(1, 1, 1, 0.7f) : Faded;
                // The money on the spot, and once it's thrown on, that it can't come back.
                _betAmounts[i].text = riding ? $"{Money(amount)}\n<size=24>RIDING</size>" : Money(amount);
                _betAmounts[i].color = amount > 0 ? Gold : Faded;
                _upButtons[i].interactable = open && NextAmount(game, kind, up: true) > 0;
                _downButtons[i].interactable = open && amount > 0;
            }

            _throwButton.gameObject.SetActive(game.YouShoot);
            _callButton.gameObject.SetActive(!game.YouShoot);
            if (!game.YouShoot) _callButton.GetComponentInChildren<TMP_Text>().text = $"Call {NameOf(game.Shooter)}'s throw";
            _passButton.gameObject.SetActive(game.CanPassDice);
            _throwButton.interactable = game.CanThrow;
            _callButton.interactable = game.CanThrow;
            _getUpButton.interactable = CanGetUp;

            // While a teacher coaches, only the control they point at works.
            if (_lock != null)
                foreach (var control in _controls)
                    if (control.Key != _lock) control.Value.interactable = false;

            // A control that can't be pressed fades whole, edge and label with it.
            foreach (var control in _controls.Values)
                control.GetComponent<CanvasGroup>().alpha = control.interactable ? 1 : 0.4f;
        }

        /// <summary>Lights a die's pips for this face; 0 is a blank die, before the first throw.</summary>
        void ShowDie(int die, int face)
        {
            var pips = _pips[die];
            var cells = PipCells[face];
            for (int i = 0; i < pips.Length; i++)
            {
                pips[i].enabled = i < cells.Length;
                if (i < cells.Length) pips[i].rectTransform.anchoredPosition = (Vector2)cells[i] * 30;
            }
        }

        // ---- The throw, animated ----

        /// <summary>How long the dice are in the air, off the brick and back to rest.</summary>
        const float RollSeconds = 0.8f;

        /// <summary>Where each die comes to rest, either side of the circle's middle.</summary>
        static readonly Vector2[] DiceRest = { new Vector2(-70, 0), new Vector2(70, 0) };

        /// <summary>
        /// Sends the dice up off the wall and back. Only for show: the throw is already settled,
        /// so the result bar and message stay hidden until the dice land, and any press carries
        /// on as normal (a new throw just starts the dice again).
        /// </summary>
        void StartRoll()
        {
            _rolling = true;
            _rollTime = 0;
            _nextTumble = 0;
            foreach (var group in _reveal) group.alpha = 0;
            HidePayout();
        }

        /// <summary>
        /// One frame of the dice in the air: thrown from your side of the circle up to the brick,
        /// bouncing back down to rest, spinning and showing random faces until they settle.
        /// </summary>
        void Tumble(float seconds)
        {
            _rollTime += seconds;
            float t = Mathf.Clamp01(_rollTime / RollSeconds);
            // From your side of the circle up to the foot of the wall, measured from where they rest.
            float bottom, wall;
            if (_art != null)
            {
                float height = _ground.rect.height, middle = _art.Circle.center.y;
                bottom = (_art.Circle.yMin - middle) * height - 60;
                wall = (_art.WallFoot - middle) * height + 30;
            }
            else
            {
                float height = _circle.rect.height;
                bottom = -height * 0.37f + 70;
                wall = height * 0.63f - 90;
            }
            for (int die = 0; die < 2; die++)
            {
                var rest = DiceRest[die];
                var start = new Vector2(rest.x * 0.3f, bottom);
                var atWall = new Vector2(rest.x * 1.8f, wall);
                Vector2 at;
                if (t < 0.4f)
                {
                    at = Vector2.Lerp(start, atWall, EaseOut(t / 0.4f));
                }
                else
                {
                    float u = (t - 0.4f) / 0.6f;
                    at = Vector2.Lerp(atWall, rest, EaseOut(u));
                    at.y += Mathf.Abs(Mathf.Sin(u * Mathf.PI * 2)) * (1 - u) * 50;
                }
                _dice[die].anchoredPosition = at;
                _dice[die].localRotation = Quaternion.Euler(0, 0, (1 - t) * (1 - t) * 720 * (die == 0 ? 1 : -1));
            }

            // Show-only faces while they tumble; the real ones as they settle.
            if (t < 0.85f && _rollTime >= _nextTumble)
            {
                ShowDie(0, UnityEngine.Random.Range(1, 7));
                ShowDie(1, UnityEngine.Random.Range(1, 7));
                _nextTumble = _rollTime + 0.07f;
            }
            if (t >= 1) FinishRoll();
        }

        static float EaseOut(float x) => 1 - (1 - x) * (1 - x);

        /// <summary>Lands the dice where they rest, shows what was thrown and brings the result back.</summary>
        void FinishRoll()
        {
            _rolling = false;
            if (_dice[0] == null) return;
            for (int die = 0; die < 2; die++)
            {
                _dice[die].anchoredPosition = DiceRest[die];
                _dice[die].localRotation = Quaternion.identity;
            }
            ShowDie(0, _lastRoll?.Die1 ?? 0);
            ShowDie(1, _lastRoll?.Die2 ?? 0);
            foreach (var group in _reveal) group.alpha = 1;
            if (_payoutPending && _root.activeSelf) _payout.SetActive(true);
            _payoutPending = false;
        }

        /// <summary>
        /// Portrait: a set width in the middle of the circle. Landscape: the width of the circle's
        /// side, less a margin, so it sits centred between the screen's edge and the controls.
        /// </summary>
        void FitPayout()
        {
            if (_payout == null) return;
            var rect = (RectTransform)_payout.transform;
            if (_landscape)
            {
                rect.anchorMin = new Vector2(0, 1);
                rect.anchorMax = new Vector2(1, 1);
                rect.sizeDelta = new Vector2(-48, rect.sizeDelta.y);
            }
            else
            {
                rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 1);
                rect.sizeDelta = new Vector2(760, rect.sizeDelta.y);
            }
        }

        void HidePayout()
        {
            if (_payout != null) _payout.SetActive(false);
        }

        /// <summary>
        /// What your bets did on the last throw, over the middle of the circle once the dice land:
        /// "You won $11", big, and a line for each bet under it. It stays until you press
        /// Continue, or any of the table's own buttons.
        /// </summary>
        void BuildPayout()
        {
            var card = MakePanel(_circle, "Payout", Panel);
            _payout = card.gameObject;
            _payoutEdge = EdgeOf(card);
            var rect = card.rectTransform;
            // Its top just under the point badge, so it sits high over the circle.
            rect.pivot = new Vector2(0.5f, 1);
            rect.anchoredPosition = new Vector2(0, -160);
            FitPayout();
            var group = AddVertical(card.gameObject, 8, new RectOffset(40, 40, 28, 32)).GetComponent<VerticalLayoutGroup>();
            group.childAlignment = TextAnchor.MiddleCenter;
            var fitter = card.gameObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            _payoutTitle = MakeText(card.transform, "", 64, TextAnchor.MiddleCenter, 0);
            _payoutTitle.name = "Payout Title";
            _payoutTitle.fontStyle = FontStyles.Bold;
            _payoutTitle.enableAutoSizing = true;
            _payoutTitle.fontSizeMin = 44;
            _payoutTitle.fontSizeMax = 64;
            _payoutLines = MakeText(card.transform, "", 38, TextAnchor.MiddleCenter, 0);
            _payoutLines.name = "Payout";
            _payoutLines.lineSpacing = 10;

            var gap = new GameObject("Gap", typeof(RectTransform), typeof(LayoutElement));
            gap.transform.SetParent(card.transform, false);
            gap.GetComponent<LayoutElement>().preferredHeight = 8;
            var go = MakeButton(card.transform, "Continue", HidePayout);
            go.GetComponent<Image>().color = Gold;
            go.GetComponent<LayoutElement>().preferredHeight = 96;
            var label = go.GetComponentInChildren<TMP_Text>();
            label.color = CardBlack;
            label.fontStyle = FontStyles.Bold;

            _payout.SetActive(false);
        }

        /// <summary>A panel's thin edge, kept out of the panel's own layout so it frames the whole panel.</summary>
        static Image EdgeOf(Image panel)
        {
            var edge = panel.transform.Find("Edge").GetComponent<Image>();
            edge.gameObject.AddComponent<LayoutElement>().ignoreLayout = true;
            return edge;
        }

        // ---- UI construction ----

        void BuildUi()
        {
            var canvas = MakeCanvas(transform, "Craps Canvas", 0);
            _root = canvas.gameObject;

            var backdrop = MakeImage(canvas.transform, "Backdrop", Color.black);
            Stretch(backdrop.rectTransform);
            _plateImage = MakeImage(backdrop.transform, "Plate", Color.white);
            _plateImage.raycastTarget = false;
            _plateFit = _plateImage.gameObject.AddComponent<CoverFit>();
            _plateFit.Show(_plateImage, _plate, _plateFocus);
            var shade = MakeImage(backdrop.transform, "Shade", new Color(0, 0, 0, 0.3f));
            Stretch(shade.rectTransform);
            var ground = new GameObject("Ground", typeof(RectTransform));
            ground.transform.SetParent(backdrop.transform, false);
            _ground = (RectTransform)ground.transform;
            _groundFit = ground.AddComponent<CoverFit>();

            var safe = new GameObject("Safe Area", typeof(RectTransform), typeof(SafeAreaFitter));
            safe.transform.SetParent(canvas.transform, false);
            Stretch((RectTransform)safe.transform);

            _portraitColumn = MakeColumn(safe.transform, "Portrait", 36);
            _landscapeRow = MakeRow(safe.transform, "Landscape");
            Stretch(_landscapeRow);
            var sides = _landscapeRow.GetComponent<HorizontalLayoutGroup>();
            sides.spacing = 40;
            sides.padding = new RectOffset(36, 36, 36, 36);
            sides.childForceExpandHeight = true;
            _circleSide = MakeColumn(_landscapeRow, "Circle Side", 0);
            _circleSide.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1.2f;
            _controlSide = MakeColumn(_landscapeRow, "Controls", 0);
            _controlSide.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1;

            BuildHeader();
            BuildSeats();
            BuildCircle();
            BuildResultBar();
            _messageText = MakeText(_portraitColumn, "", 36, TextAnchor.MiddleCenter, 110);
            _messageText.name = "Message";
            _messageText.enableAutoSizing = true;
            _messageText.fontSizeMin = 28;
            _messageText.fontSizeMax = 36;
            _messageText.color = Gold;
            Shadowed(_messageText);
            _reveal = new[] { _resultBar.gameObject.AddComponent<CanvasGroup>(), _messageText.gameObject.AddComponent<CanvasGroup>() };
            BuildBets();
            BuildActions();
            BuildPayout();

            OnTurn(canvas, Rearrange);
        }

        /// <summary>
        /// Portrait is one column: your cash and the way out, the faces round the circle, the
        /// circle and the dice, what they rolled, the bet spots, the throw. Landscape puts the
        /// faces and the circle on the left and the rest on the right.
        /// </summary>
        void Rearrange(bool landscape)
        {
            _landscape = landscape;
            ShowBackdrop();
            FitPayout();
            _portraitColumn.gameObject.SetActive(!landscape);
            _landscapeRow.gameObject.SetActive(landscape);
            if (landscape)
            {
                Arrange(_circleSide, _seats, _circle);
                Arrange(_controlSide, _header, _resultBar, _messageText, _bets, _actions);
            }
            else
            {
                Arrange(_portraitColumn, _header, _seats, _circle, _resultBar, _messageText, _bets, _actions);
            }
        }

        void BuildHeader()
        {
            _header = MakeRow(_portraitColumn, "Header");
            _header.GetComponent<LayoutElement>().preferredHeight = 90;
            var info = new GameObject("Info", typeof(RectTransform), typeof(LayoutElement));
            info.transform.SetParent(_header, false);
            info.GetComponent<LayoutElement>().flexibleWidth = 3;
            AddVertical(info, 2, new RectOffset());
            _bankrollText = MakeText(info.transform, "", 38, TextAnchor.MiddleLeft, 50);
            _bankrollText.fontStyle = FontStyles.Bold;
            Shadowed(_bankrollText);
            _clockText = MakeText(info.transform, "", 30, TextAnchor.MiddleLeft, 38);
            _clockText.color = new Color(1, 1, 1, 0.8f);
            Shadowed(_clockText);
            _getUpButton = Control(_header, CrapsControls.GetUp, "Leave game", GetUp);
            Outline(_getUpButton);
            var getUp = _getUpButton.GetComponent<LayoutElement>();
            getUp.preferredHeight = 88;
            getUp.flexibleWidth = 0;
            getUp.preferredWidth = getUp.minWidth = 220;
        }

        /// <summary>
        /// The faces in the circle as a row of cards, in the order the dice go round and you
        /// last: the gold card with the tag is the shooter.
        /// </summary>
        void BuildSeats()
        {
            _seats = MakeRow(_portraitColumn, "Seats");
            _seats.GetComponent<LayoutElement>().preferredHeight = 250;
            _seats.GetComponent<HorizontalLayoutGroup>().spacing = 12;
            _seatFrames = new Image[_rules.Seats];
            _shooterTags = new GameObject[_rules.Seats];
            _seatNames = new TMP_Text[_rules.Seats];
            _seatInitials = new TMP_Text[_rules.Seats];
            _seatPortraits = new Image[_rules.Seats];
            _seatDiscs = new Image[_rules.Seats];
            for (int i = 1; i <= _rules.Seats; i++) MakeSeat(i % _rules.Seats);
        }

        /// <summary>A square card with the face (or the initial on a disc), the shooter's tag along its foot, and the name under it.</summary>
        void MakeSeat(int seat)
        {
            var spot = new GameObject("Seat " + seat, typeof(RectTransform), typeof(LayoutElement));
            spot.transform.SetParent(_seats, false);
            // Each slot is the row's full height: the card hangs from its top, the name sits on its foot.
            var slot = spot.GetComponent<LayoutElement>();
            slot.flexibleWidth = 1;
            slot.preferredHeight = 250;
            var rect = (RectTransform)spot.transform;

            var frame = MakeImage(rect, "Frame", SeatEdge);
            frame.sprite = Rounded;
            frame.type = Image.Type.Sliced;
            frame.raycastTarget = false;
            var frameRect = frame.rectTransform;
            frameRect.anchorMin = frameRect.anchorMax = frameRect.pivot = new Vector2(0.5f, 1);
            frameRect.sizeDelta = new Vector2(196, 196);
            _seatFrames[seat] = frame;

            var card = MakeImage(frameRect, "Card", Seat);
            card.sprite = Rounded;
            card.type = Image.Type.Sliced;
            card.raycastTarget = false;
            Pin(card.rectTransform, Vector2.zero, Vector2.one, new Vector2(5, 5), new Vector2(-5, -5));
            card.gameObject.AddComponent<Mask>().showMaskGraphic = true;

            var portrait = MakeImage(card.transform, "Portrait", Color.white);
            portrait.raycastTarget = false;
            Stretch(portrait.rectTransform);
            _seatPortraits[seat] = portrait;

            var disc = MakeImage(card.transform, "Disc", new Color(0.2f, 0.2f, 0.22f));
            disc.sprite = Circle;
            disc.raycastTarget = false;
            disc.rectTransform.sizeDelta = new Vector2(120, 120);
            _seatDiscs[seat] = disc;
            var initial = MakeText(disc.transform, "", 72, TextAnchor.MiddleCenter, 0);
            initial.fontStyle = FontStyles.Bold;
            Stretch(initial.rectTransform);
            _seatInitials[seat] = initial;

            var tag = MakeImage(frameRect, "Shooter", Gold);
            tag.sprite = Rounded;
            tag.type = Image.Type.Sliced;
            tag.raycastTarget = false;
            Pin(tag.rectTransform, new Vector2(0.06f, 0), new Vector2(0.94f, 0), new Vector2(0, 8), new Vector2(0, 48));
            var tagText = MakeText(tag.transform, "SHOOTER", 24, TextAnchor.MiddleCenter, 0);
            tagText.enableAutoSizing = true;
            tagText.fontSizeMin = 14;
            tagText.fontSizeMax = 24;
            tagText.fontStyle = FontStyles.Bold;
            tagText.color = CardBlack;
            Stretch(tagText.rectTransform);
            _shooterTags[seat] = tag.gameObject;

            var label = MakeText(frameRect, "", 34, TextAnchor.MiddleCenter, 0);
            NoWrap(label);
            Shadowed(label);
            // Just under the card, whatever size the card ends up.
            label.rectTransform.anchorMin = label.rectTransform.anchorMax = label.rectTransform.pivot = new Vector2(0.5f, 0);
            label.rectTransform.sizeDelta = new Vector2(260, 48);
            label.rectTransform.anchoredPosition = new Vector2(0, -52);
            _seatNames[seat] = label;
            ShowRegular(seat);
        }

        /// <summary>A seat's face and name, or for you and a regular with no face yet, their initial on a disc.</summary>
        void ShowRegular(int seat)
        {
            string name = seat == CrapsGame.You ? "You" : _regulars[seat - 1].Name;
            var face = seat == CrapsGame.You ? null : _regulars[seat - 1].Face;
            _seatPortraits[seat].sprite = face;
            _seatPortraits[seat].enabled = face != null;
            _seatDiscs[seat].enabled = face == null;
            _seatInitials[seat].text = face != null ? "" : name.Substring(0, 1).ToUpperInvariant();
            _seatNames[seat].text = name;
        }

        /// <summary>
        /// The chalk circle on the asphalt, seen at a slant, with the dice in it and the point
        /// marker in the top corner. It takes whatever height the screen has left.
        /// </summary>
        void BuildCircle()
        {
            var holder = new GameObject("Circle", typeof(RectTransform), typeof(LayoutElement));
            holder.transform.SetParent(_portraitColumn, false);
            _circle = (RectTransform)holder.transform;
            var layout = holder.GetComponent<LayoutElement>();
            layout.flexibleHeight = 1;
            layout.minHeight = 320;

            var ring = _ring = MakeImage(_circle, "Chalk", Chalk);
            ring.sprite = MakeRing();
            ring.raycastTarget = false;
            Pin(ring.rectTransform, new Vector2(0.04f, 0.12f), new Vector2(0.96f, 0.62f), Vector2.zero, Vector2.zero);

            var dice = new GameObject("Dice", typeof(RectTransform));
            dice.transform.SetParent(_circle, false);
            var diceRect = _diceRect = (RectTransform)dice.transform;
            diceRect.anchorMin = diceRect.anchorMax = new Vector2(0.5f, 0.37f);
            for (int die = 0; die < 2; die++) _pips[die] = MakeDie(diceRect, DiceRest[die], out _dice[die]);

            _pointBadge = MakeImage(_circle, "Point", Panel);
            _pointBadge.sprite = Rounded;
            _pointBadge.type = Image.Type.Sliced;
            _pointBadge.raycastTarget = false;
            var badge = _pointBadge.rectTransform;
            badge.anchorMin = badge.anchorMax = badge.pivot = new Vector2(1, 1);
            badge.sizeDelta = new Vector2(250, 96);
            badge.anchoredPosition = new Vector2(-4, -8);
            _pointEdge = MakeImage(badge, "Edge", Gold);
            _pointEdge.sprite = RoundedFrame;
            _pointEdge.type = Image.Type.Sliced;
            _pointEdge.raycastTarget = false;
            Stretch(_pointEdge.rectTransform);
            _pointText = MakeText(badge, "", 32, TextAnchor.MiddleCenter, 0);
            NoWrap(_pointText);
            Stretch(_pointText.rectTransform);
        }

        /// <summary>The last throw on one bar: who rolled what on the left, the total big on the right.</summary>
        void BuildResultBar()
        {
            var bar = MakePanel(_portraitColumn, "Result", Panel);
            EdgeOf(bar).color = Line;
            bar.gameObject.AddComponent<LayoutElement>().preferredHeight = 100;
            _resultBar = bar.rectTransform;

            _rollText = MakeText(_resultBar, "", 40, TextAnchor.MiddleLeft, 0);
            _rollText.fontStyle = FontStyles.Bold;
            NoWrap(_rollText);
            _rollText.overflowMode = TextOverflowModes.Ellipsis;
            Pin(_rollText.rectTransform, Vector2.zero, new Vector2(0.72f, 1), new Vector2(40, 0), new Vector2(-16, 0));

            var divider = MakeImage(_resultBar, "Divider", Line);
            divider.raycastTarget = false;
            Pin(divider.rectTransform, new Vector2(0.72f, 0.2f), new Vector2(0.72f, 0.8f), new Vector2(-1, 0), new Vector2(2, 0));

            _totalText = MakeText(_resultBar, "", 72, TextAnchor.MiddleCenter, 0);
            _totalText.fontStyle = FontStyles.Bold;
            Pin(_totalText.rectTransform, new Vector2(0.72f, 0), Vector2.one, Vector2.zero, Vector2.zero);
        }

        /// <summary>A die drawn in code: an off-white rounded square and its seven possible pips, lit by face.</summary>
        static Image[] MakeDie(RectTransform parent, Vector2 at, out RectTransform die)
        {
            var body = MakeImage(parent, "Die", DieFace);
            body.sprite = Rounded;
            body.type = Image.Type.Sliced;
            body.raycastTarget = false;
            body.rectTransform.sizeDelta = new Vector2(110, 110);
            body.rectTransform.anchoredPosition = at;
            die = body.rectTransform;
            var pips = new Image[6];
            for (int i = 0; i < pips.Length; i++)
            {
                pips[i] = MakeImage(body.transform, "Pip", CardBlack);
                pips[i].sprite = Circle;
                pips[i].raycastTarget = false;
                pips[i].rectTransform.sizeDelta = new Vector2(22, 22);
            }
            return pips;
        }

        /// <summary>A white chalk ring with a slightly rough edge, drawn once for this table.</summary>
        static Sprite MakeRing()
        {
            const int size = 256;
            const float radius = 120, line = 7;
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp };
            var pixels = new Color32[size * size];
            for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float dx = x + 0.5f - size / 2f, dy = y + 0.5f - size / 2f;
                float angle = Mathf.Atan2(dy, dx);
                // Chalk on asphalt: the line's width wavers round the circle and thins in places.
                float width = line * (0.8f + 0.2f * Mathf.Sin(angle * 7) + 0.1f * Mathf.Sin(angle * 23));
                float coverage = Mathf.Clamp01(width / 2 - Mathf.Abs(Mathf.Sqrt(dx * dx + dy * dy) - radius) + 0.5f);
                coverage *= 0.75f + 0.25f * Mathf.Sin(angle * 41 + x * 0.3f);
                pixels[y * size + x] = new Color32(255, 255, 255, (byte)(Mathf.Clamp01(coverage) * 255));
            }
            texture.SetPixels32(pixels);
            texture.Apply();
            return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100);
        }

        /// <summary>
        /// The four bet spots, ruled off from each other. Each is its name with a line under it
        /// saying what the bet is and pays, then a stepper: down, the money on the spot, up. The
        /// steppers line up in one column whatever the names, so the amounts read straight down.
        /// </summary>
        void BuildBets()
        {
            var panel = MakePanel(_portraitColumn, "Bets", Panel);
            EdgeOf(panel).color = Line;
            _bets = AddVertical(panel.gameObject, 0, new RectOffset(28, 16, 6, 6));
            for (int i = 0; i < Kinds.Length; i++)
            {
                var kind = Kinds[i];
                if (i > 0)
                {
                    var rule = MakeImage(_bets, "Rule", new Color(1, 1, 1, 0.12f));
                    rule.raycastTarget = false;
                    rule.gameObject.AddComponent<LayoutElement>().preferredHeight = 2;
                }
                var row = MakeRow(_bets, kind + " Spot");
                row.GetComponent<LayoutElement>().preferredHeight = 96;
                var rowLayout = row.GetComponent<HorizontalLayoutGroup>();
                rowLayout.childForceExpandWidth = false;
                rowLayout.childAlignment = TextAnchor.MiddleLeft;
                rowLayout.spacing = 8;

                var info = new GameObject("Info", typeof(RectTransform), typeof(LayoutElement));
                info.transform.SetParent(row, false);
                var infoLayout = info.GetComponent<LayoutElement>();
                infoLayout.flexibleWidth = 1;
                infoLayout.minWidth = 0;
                infoLayout.preferredWidth = 0;
                AddVertical(info, 0, new RectOffset()).GetComponent<VerticalLayoutGroup>().childAlignment = TextAnchor.MiddleLeft;
                _betLabels[i] = MakeText(info.transform, "", 38, TextAnchor.MiddleLeft, 46);
                _betLabels[i].fontStyle = FontStyles.Bold;
                NoWrap(_betLabels[i]);
                _betHints[i] = MakeText(info.transform, "", 26, TextAnchor.MiddleLeft, 32);
                NoWrap(_betHints[i]);
                _betHints[i].overflowMode = TextOverflowModes.Ellipsis;

                _downButtons[i] = StepButton(row, CrapsControls.Down(kind), "-", () => Step(kind, up: false));
                _betAmounts[i] = MakeText(row, "", 40, TextAnchor.MiddleCenter, 0);
                _betAmounts[i].fontStyle = FontStyles.Bold;
                _betAmounts[i].lineSpacing = -10;
                var amountLayout = _betAmounts[i].gameObject.AddComponent<LayoutElement>();
                amountLayout.preferredWidth = amountLayout.minWidth = 130;
                _upButtons[i] = StepButton(row, CrapsControls.Up(kind), "+", () => Step(kind, up: true));
            }
        }

        /// <summary>A square, outlined step button.</summary>
        Button StepButton(Transform row, string id, string label, Action onClick)
        {
            var button = Control(row, id, label, onClick);
            button.name = id + " Button";
            Outline(button);
            var layout = button.GetComponent<LayoutElement>();
            layout.flexibleWidth = 0;
            layout.preferredWidth = layout.minWidth = 88;
            layout.preferredHeight = 76;
            var text = button.GetComponentInChildren<TMP_Text>();
            text.fontSize = 52;
            text.rectTransform.offsetMin = text.rectTransform.offsetMax = Vector2.zero;
            return button;
        }

        /// <summary>Throw on your turn or call a regular's, in the same place; pass the dice beside it on your come-out.</summary>
        void BuildActions()
        {
            _actions = MakeRow(_portraitColumn, "Actions");
            _actions.GetComponent<LayoutElement>().preferredHeight = 136;
            _passButton = Control(_actions, CrapsControls.PassDice, "Pass the dice", PassDice);
            Outline(_passButton);
            _throwButton = Control(_actions, CrapsControls.Throw, "Throw", Throw);
            _callButton = Control(_actions, CrapsControls.CallIt, "Call it", Throw);
            foreach (var main in new[] { _throwButton, _callButton })
            {
                main.GetComponent<Image>().color = Gold;
                var text = main.GetComponentInChildren<TMP_Text>();
                text.color = CardBlack;
                text.fontSize = 52;
                text.fontStyle = FontStyles.Bold;
                main.GetComponent<LayoutElement>().flexibleWidth = 1.6f;
            }
        }

        /// <summary>
        /// A button a coach can point at by <paramref name="id"/>. It fades whole when it can't be
        /// pressed (see <see cref="Refresh"/>), rather than by the button's own tint.
        /// </summary>
        Button Control(Transform parent, string id, string label, Action onClick)
        {
            var button = MakeButton(parent, label, () => Press(id, onClick));
            button.gameObject.AddComponent<CanvasGroup>();
            var colors = button.colors;
            colors.disabledColor = Color.white;
            button.colors = colors;
            _controls[id] = button;
            return button;
        }

        /// <summary>A dark see-through button with a light edge, for everything but the throw.</summary>
        static void Outline(Button button)
        {
            button.GetComponent<Image>().color = Outlined;
            var edge = MakeImage(button.transform, "Edge", new Color(1, 1, 1, 0.6f));
            edge.sprite = RoundedFrame;
            edge.type = Image.Type.Sliced;
            edge.raycastTarget = false;
            Stretch(edge.rectTransform);
            edge.transform.SetAsFirstSibling();
        }

        static RectTransform MakeColumn(Transform parent, string name, int padding)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            Stretch((RectTransform)go.transform);
            return AddVertical(go, 20, new RectOffset(padding, padding, padding, padding));
        }
    }
}
