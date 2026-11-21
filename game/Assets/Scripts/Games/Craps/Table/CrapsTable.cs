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
        static readonly Color Spots = new Color(0.05f, 0.05f, 0.06f, 0.78f);
        static readonly BetKind[] Kinds = { BetKind.Right, BetKind.Wrong, BetKind.PointFor, BetKind.PointAgainst };

        /// <summary>Where each seat stands on the circle, in the order the dice go: you at the bottom, then clockwise.</summary>
        static readonly Vector2[] SeatSpots = { new Vector2(0.5f, 0.1f), new Vector2(0.11f, 0.5f), new Vector2(0.5f, 0.9f), new Vector2(0.89f, 0.5f) };

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
        string _message = "";

        GameObject _root;
        Image _plateImage;
        CoverFit _plateFit;
        TMP_Text _bankrollText, _clockText, _messageText, _pointText;
        Image _pointPuck;
        RectTransform _portraitColumn, _landscapeRow, _circleSide, _controlSide, _header, _circle, _bets, _actions;
        Button _throwButton, _callButton, _passButton, _getUpButton;
        readonly Image[][] _pips = new Image[2][];
        Image[] _shooterRings;
        TMP_Text[] _seatNames, _seatInitials;
        Image[] _seatPortraits;
        readonly TMP_Text[] _betLabels = new TMP_Text[4];
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
            _plateFit.Show(_plateImage, plate, plateFocus);
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
        void Update()
        {
            if (_sitting == null || !_root.activeSelf || _lock != null || Practicing) return;
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

            if (Practicing)
            {
                _message = "Practice turn, so it doesn't count.\n" + Describe(result, point);
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
            _message = "That's the practice. Real money from here.\n" + _message.Substring(_message.IndexOf('\n') + 1);
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

        /// <summary>The throw, what it did, and a line for each of your bets it settled.</summary>
        string Describe(ThrowResult result, int pointBefore)
        {
            var sb = new StringBuilder();
            var roll = result.Roll;
            sb.Append(result.Shooter == CrapsGame.You ? "You roll" : NameOf(result.Shooter) + " rolls")
                .Append($" {roll.Die1} and {roll.Die2}: {roll.Total}.");
            if (result.PointSet > 0) sb.Append($" The point is {result.PointSet}.");
            else if (result.PointMade) sb.Append(" Point made.");
            else if (result.SevenOut) sb.Append($" Seven out. The dice go to {NameOf(Current.Shooter)}.");
            else if (pointBefore == 0) sb.Append(roll.Total == 7 || roll.Total == 11 ? " A natural." : " Craps.");
            foreach (var bet in result.Settled) sb.Append('\n').Append(PayoutLine(bet, pointBefore));
            return sb.ToString();
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

        // ---- Display ----

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

            for (int seat = 0; seat < _rules.Seats; seat++)
            {
                bool shooting = seat == game.Shooter;
                _shooterRings[seat].enabled = shooting;
                _seatNames[seat].color = shooting ? Gold : Color.white;
            }

            ShowDie(0, _lastRoll?.Die1 ?? 0);
            ShowDie(1, _lastRoll?.Die2 ?? 0);
            bool on = game.Phase == CrapsPhase.Point;
            _pointPuck.color = on ? Gold : new Color(0.1f, 0.1f, 0.1f, 0.9f);
            _pointText.text = on ? $"ON\n{game.Point}" : "OFF";
            _pointText.color = on ? CardBlack : Color.white;

            // Every control starts each redraw usable, then the rules and any coach's lock narrow it.
            foreach (var control in _controls.Values) control.interactable = true;
            for (int i = 0; i < Kinds.Length; i++)
            {
                var kind = Kinds[i];
                int amount = game.Bet(kind);
                string state = game.IsRiding(kind) ? " · riding" : amount > 0 ? " · down" : "";
                _betLabels[i].text = $"{BetName(kind, game.Point)}  {(amount > 0 ? Money(amount) : "-")}{state}";
                bool open = game.CanBet(kind);
                _betLabels[i].color = open || amount > 0 ? Color.white : new Color(1, 1, 1, 0.4f);
                _upButtons[i].interactable = open && NextAmount(game, kind, up: true) > 0;
                _downButtons[i].interactable = open && amount > 0;
            }

            _throwButton.gameObject.SetActive(game.YouShoot);
            _callButton.gameObject.SetActive(!game.YouShoot);
            _passButton.gameObject.SetActive(game.CanPassDice);
            _throwButton.interactable = game.CanThrow;
            _callButton.interactable = game.CanThrow;
            _getUpButton.interactable = CanGetUp;

            // While a teacher coaches, only the control they point at works.
            if (_lock != null)
                foreach (var control in _controls)
                    if (control.Key != _lock) control.Value.interactable = false;
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
            var shade = MakeImage(backdrop.transform, "Shade", new Color(0, 0, 0, 0.4f));
            Stretch(shade.rectTransform);

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
            BuildCircle();
            _messageText = MakeText(_portraitColumn, "", 38, TextAnchor.MiddleCenter, 210);
            _messageText.name = "Message";
            _messageText.color = Gold;
            Shadowed(_messageText);
            BuildBets();
            BuildActions();

            OnTurn(canvas, Rearrange);
        }

        /// <summary>
        /// Portrait is one column: your cash and the way out, the circle, what the dice did, the
        /// bet spots, the throw. Landscape puts the circle on the left and the rest on the right.
        /// </summary>
        void Rearrange(bool landscape)
        {
            _portraitColumn.gameObject.SetActive(!landscape);
            _landscapeRow.gameObject.SetActive(landscape);
            var circleLayout = _circle.GetComponent<LayoutElement>();
            circleLayout.preferredHeight = landscape ? 0 : 720;
            circleLayout.flexibleHeight = landscape ? 1 : 0;
            if (landscape)
            {
                Arrange(_circleSide, _circle);
                Arrange(_controlSide, _header, _messageText, _bets, _actions);
            }
            else
            {
                Arrange(_portraitColumn, _header, _circle, _messageText, _bets, _actions);
            }
        }

        void BuildHeader()
        {
            _header = MakeRow(_portraitColumn, "Header");
            _header.GetComponent<LayoutElement>().preferredHeight = 110;
            var info = new GameObject("Info", typeof(RectTransform), typeof(LayoutElement));
            info.transform.SetParent(_header, false);
            info.GetComponent<LayoutElement>().flexibleWidth = 3;
            AddVertical(info, 4, new RectOffset());
            _bankrollText = MakeText(info.transform, "", 40, TextAnchor.MiddleLeft, 56);
            _bankrollText.fontStyle = FontStyles.Bold;
            Shadowed(_bankrollText);
            _clockText = MakeText(info.transform, "", 32, TextAnchor.MiddleLeft, 44);
            _clockText.color = new Color(1, 1, 1, 0.8f);
            Shadowed(_clockText);
            _getUpButton = Control(_header, CrapsControls.GetUp, "Get up", GetUp);
            _getUpButton.GetComponent<LayoutElement>().preferredHeight = 104;
        }

        /// <summary>The chalk circle, a square that fits its space, with the seats round it and the dice and point puck inside.</summary>
        void BuildCircle()
        {
            var holder = new GameObject("Circle", typeof(RectTransform), typeof(LayoutElement));
            holder.transform.SetParent(_portraitColumn, false);
            _circle = (RectTransform)holder.transform;

            var square = new GameObject("Square", typeof(RectTransform), typeof(AspectRatioFitter));
            square.transform.SetParent(_circle, false);
            Stretch((RectTransform)square.transform);
            var fitter = square.GetComponent<AspectRatioFitter>();
            fitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
            fitter.aspectRatio = 1;

            var ring = MakeImage(square.transform, "Chalk", Chalk);
            ring.sprite = MakeRing();
            ring.raycastTarget = false;
            Pin(ring.rectTransform, new Vector2(0.1f, 0.1f), new Vector2(0.9f, 0.9f), Vector2.zero, Vector2.zero);

            var dice = new GameObject("Dice", typeof(RectTransform));
            dice.transform.SetParent(square.transform, false);
            var diceRect = (RectTransform)dice.transform;
            diceRect.anchorMin = diceRect.anchorMax = new Vector2(0.5f, 0.45f);
            for (int die = 0; die < 2; die++) _pips[die] = MakeDie(diceRect, new Vector2(die == 0 ? -70 : 70, 0));

            _pointPuck = MakeImage(square.transform, "Point", Color.white);
            _pointPuck.sprite = Circle;
            _pointPuck.raycastTarget = false;
            var puck = _pointPuck.rectTransform;
            puck.anchorMin = puck.anchorMax = new Vector2(0.5f, 0.65f);
            puck.sizeDelta = new Vector2(110, 110);
            _pointText = MakeText(puck, "", 30, TextAnchor.MiddleCenter, 0);
            _pointText.fontStyle = FontStyles.Bold;
            _pointText.lineSpacing = -20;
            Stretch(_pointText.rectTransform);

            _shooterRings = new Image[_rules.Seats];
            _seatNames = new TMP_Text[_rules.Seats];
            _seatInitials = new TMP_Text[_rules.Seats];
            _seatPortraits = new Image[_rules.Seats];
            for (int seat = 0; seat < _rules.Seats; seat++) MakeSeat(square.transform, seat);
        }

        /// <summary>A face (or the placeholder disc) with a gold ring for the shooter and a name under it.</summary>
        void MakeSeat(Transform square, int seat)
        {
            var spot = new GameObject("Seat " + seat, typeof(RectTransform));
            spot.transform.SetParent(square, false);
            var rect = (RectTransform)spot.transform;
            rect.anchorMin = rect.anchorMax = SeatSpots[seat % SeatSpots.Length];
            rect.sizeDelta = new Vector2(220, 220);

            var ring = MakeImage(rect, "Shooter", Gold);
            ring.sprite = Circle;
            ring.raycastTarget = false;
            ring.rectTransform.sizeDelta = new Vector2(176, 176);
            ring.rectTransform.anchoredPosition = new Vector2(0, 24);
            _shooterRings[seat] = ring;

            var disc = MakeImage(rect, "Face", Seat);
            disc.sprite = Circle;
            disc.raycastTarget = false;
            disc.rectTransform.sizeDelta = new Vector2(160, 160);
            disc.rectTransform.anchoredPosition = new Vector2(0, 24);
            disc.gameObject.AddComponent<Mask>().showMaskGraphic = true;

            var portrait = MakeImage(disc.transform, "Portrait", Color.white);
            portrait.preserveAspect = true;
            portrait.raycastTarget = false;
            Stretch(portrait.rectTransform);
            _seatPortraits[seat] = portrait;

            var initial = MakeText(disc.transform, "", 72, TextAnchor.MiddleCenter, 0);
            initial.fontStyle = FontStyles.Bold;
            Stretch(initial.rectTransform);
            _seatInitials[seat] = initial;

            var label = MakeText(rect, "", 32, TextAnchor.MiddleCenter, 0);
            NoWrap(label);
            Shadowed(label);
            label.rectTransform.anchorMin = label.rectTransform.anchorMax = new Vector2(0.5f, 0);
            label.rectTransform.sizeDelta = new Vector2(260, 44);
            label.rectTransform.anchoredPosition = new Vector2(0, 8);
            _seatNames[seat] = label;
            ShowRegular(seat);
        }

        /// <summary>A seat's face and name, or for a regular with no face yet, the disc with their initial.</summary>
        void ShowRegular(int seat)
        {
            string name = seat == CrapsGame.You ? "You" : _regulars[seat - 1].Name;
            var face = seat == CrapsGame.You ? null : _regulars[seat - 1].Face;
            _seatPortraits[seat].sprite = face;
            _seatPortraits[seat].enabled = face != null;
            _seatInitials[seat].text = face != null ? "" : name.Substring(0, 1).ToUpperInvariant();
            _seatNames[seat].text = name;
        }

        /// <summary>A die drawn in code: an off-white rounded square and its seven possible pips, lit by face.</summary>
        static Image[] MakeDie(RectTransform parent, Vector2 at)
        {
            var body = MakeImage(parent, "Die", DieFace);
            body.sprite = Rounded;
            body.type = Image.Type.Sliced;
            body.raycastTarget = false;
            body.rectTransform.sizeDelta = new Vector2(110, 110);
            body.rectTransform.anchoredPosition = at;
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

        /// <summary>The four bet spots, each its name, what's on it, and a step down and up through the amounts it takes.</summary>
        void BuildBets()
        {
            var panel = MakeImage(_portraitColumn, "Bets", Spots);
            panel.sprite = Rounded;
            panel.type = Image.Type.Sliced;
            _bets = AddVertical(panel.gameObject, 8, new RectOffset(24, 16, 12, 12));
            for (int i = 0; i < Kinds.Length; i++)
            {
                var kind = Kinds[i];
                var row = MakeRow(_bets, kind + " Spot");
                row.GetComponent<LayoutElement>().preferredHeight = 88;
                _betLabels[i] = MakeText(row, "", 38, TextAnchor.MiddleLeft, 0);
                _betLabels[i].gameObject.AddComponent<LayoutElement>().flexibleWidth = 3;
                _downButtons[i] = StepButton(row, CrapsControls.Down(kind), "-", () => Step(kind, up: false));
                _upButtons[i] = StepButton(row, CrapsControls.Up(kind), "+", () => Step(kind, up: true));
            }
        }

        Button StepButton(Transform row, string id, string label, Action onClick)
        {
            var button = Control(row, id, label, onClick);
            button.name = id + " Button";
            var layout = button.GetComponent<LayoutElement>();
            layout.flexibleWidth = 0;
            layout.preferredWidth = 120;
            layout.preferredHeight = 88;
            button.GetComponentInChildren<TMP_Text>().fontSize = 56;
            return button;
        }

        /// <summary>Throw on your turn or call a regular's, in the same place; pass the dice beside it on your come-out.</summary>
        void BuildActions()
        {
            _actions = MakeRow(_portraitColumn, "Actions");
            _actions.GetComponent<LayoutElement>().preferredHeight = 140;
            _passButton = Control(_actions, CrapsControls.PassDice, "Pass the dice", PassDice);
            _throwButton = Control(_actions, CrapsControls.Throw, "Throw", Throw);
            _callButton = Control(_actions, CrapsControls.CallIt, "Call it", Throw);
            foreach (var main in new[] { _throwButton, _callButton })
            {
                main.GetComponent<Image>().color = Gold;
                main.GetComponentInChildren<TMP_Text>().color = CardBlack;
                main.GetComponent<LayoutElement>().flexibleWidth = 1.4f;
            }
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
            return AddVertical(go, 20, new RectOffset(padding, padding, padding, padding));
        }
    }
}
