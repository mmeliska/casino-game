using System;
using System.Collections.Generic;
using System.Linq;
using Starflare.State;
using Starflare.Ui;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Starflare.Ui.UiKit;

namespace Starflare.Bowling
{
    /// <summary>
    /// A lane at Millstone Lanes (bowling spec): open bowling for the lane fee, a regular's
    /// challenge between games when one comes over, or a sub night's three games for a short
    /// team. Each throw is four steps: drag the ball to a board, then stop the aim, hook and speed
    /// sweeps with a tap; the lane, the ball and the pins do the rest, and the trace shows what
    /// the ball did. Every finished game is a settled round, reported to the player state with the
    /// story minutes played. You get up between games, never mid-game; that's the break point.
    ///
    /// It never reads story state, counts heat or saves: the venue hands it tonight's oil, your
    /// ball and shoes, who's on the lanes and a sub night's match (<see cref="LaneSetup"/>), and
    /// turns what it reports at the break point into story. A teacher can coach over it
    /// (<see cref="ICoachable"/>, by the ids in <see cref="BowlingControls"/>) with a
    /// <see cref="PracticeFrame"/> that counts for nothing.
    /// </summary>
    public sealed class BowlingLane : MonoBehaviour, ICoachable
    {
        /// <summary>Where the throw is: lining up, a sweep running, the ball out, or between balls and games.</summary>
        /// <summary>
        /// Where the lane is. Standing, the aim's arrow is already swinging: the throw is three taps
        /// anywhere, aim, hook, speed (Mike, 2026-11-24), and you can drag the ball before the first.
        /// </summary>
        public enum Step { Between, Stand, Hook, Speed, Rolling, Pins, Others, Over }

        /// <summary>Two taps on the lane this close together skip ahead.</summary>
        const float DoubleTap = 0.45f;

        /// <summary>How long the struck pins stay on screen after they settle, seconds.</summary>
        const float PinsLinger = 0.6f;

        /// <summary>The meters' block and the between-turns block are the same height, so the lane holds still.</summary>
        const float ThrowAreaHeight = 430;

        static readonly Color Outlined = new Color(0.06f, 0.08f, 0.1f, 0.55f);

        public event Action<BreakPoint> BreakPointReached;
        public event Action<string> ActionTaken;

        public BowlingRules Rules => _rules;

        /// <summary>This sitting so far; still readable at the break point after you get up.</summary>
        public int SittingNet => _state == null ? 0 : _state.BankrollNow - _sittingStartBankroll;

        /// <summary>Games finished this sitting (open, challenge or league).</summary>
        public int GamesBowled { get; private set; }

        /// <summary>Your best game this sitting.</summary>
        public int BestGame { get; private set; }

        public int ChallengesWon { get; private set; }
        public int ChallengesLost { get; private set; }

        /// <summary>A sub night: games your team took.</summary>
        public int GamesWon => _match?.GamesWon ?? 0;

        /// <summary>A sub night: all three games bowled.</summary>
        public bool FinishedNight => _match != null && _match.FinishedNight;

        public bool TeamWonNight => _match != null && _match.TeamWonNight;

        /// <summary>A coached frame is being bowled: none of it counts.</summary>
        public bool Practicing => _practice != null;

        public Step Now => _step;

        /// <summary>The game being bowled, or the last one finished.</summary>
        public BowlingGame Game => _game;

        /// <summary>A regular at your lane between games, waiting on your answer.</summary>
        public Offer Offer => _offer;

        /// <summary>The board the ball sits on, your own count.</summary>
        public int StandBoard => _ball.Board;

        /// <summary>The ball you're bowling with this sitting: the house ball in a coached frame.</summary>
        public BowlingBall Ball => _practice != null || _setup == null ? BowlingBall.House : _setup.Ball;

        /// <summary>The last ball rolled, by anyone.</summary>
        public Delivery LastDelivery => _delivery;

        BowlingRules _rules;
        Sprite _plate;
        LaneArt _art;

        /// <summary>Stand-in textures made for art that isn't in yet; destroyed with the lane.</summary>
        Texture2D[] _madeTextures = new Texture2D[0];

        PlayerState _state;
        Sitting _sitting;
        int _sittingStartBankroll;
        LaneSetup _setup;
        LaneState _lane;
        System.Random _rng;
        Challenges _challenges;
        LeagueMatch _match;
        BowlingGame _game;
        Offer _offer;
        PracticeFrame _practice;
        int _practiceBall;
        bool _shoesPaid, _chaining;
        Step _step = Step.Between;
        string _message = "", _lock;
        double _aim, _hook, _speed;

        Delivery _delivery;

        /// <summary>How the ball in play is turned along its roll, for the drawn ball.</summary>
        BallSpin _spin;
        float _animTime, _playSpeed = 1, _lastLaneTap = -10;

        readonly Dictionary<string, RectTransform> _controls = new Dictionary<string, RectTransform>();
        readonly Dictionary<string, Button> _buttons = new Dictionary<string, Button>();

        GameObject _root;
        Image _plateImage;
        CoverFit _plateFit;
        RectTransform _portraitColumn, _landscapeRow, _laneSide, _infoSide, _header, _laneArea, _laneFrame, _messageBar, _sheets, _controlsArea, _actions, _offerRow, _standRow;
        LaneView _laneView;
        DeckView _deckView;
        PathTrace _trace;
        ApproachBall _ball;
        Meter _aimMeter, _hookMeter, _speedMeter;
        ScoreSheetView _yourSheet, _theirSheet;
        TMP_Text _bankrollText, _clockText, _messageText, _teamText, _offerText, _standText, _bowlText;

        // ---- Setting up ----

        /// <summary>
        /// Sets the lane up with its rules over the venue's plate, drawn with the venue's lane
        /// textures (any not in yet are stood in for). It stays hidden until you sit down.
        /// </summary>
        public void Init(BowlingRules rules, Sprite plate, LaneArt art = null)
        {
            _rules = rules ?? throw new ArgumentNullException(nameof(rules));
            _plate = plate;
            _art = LaneArt.Complete(art, out _madeTextures);
            BuildUi();
            _root.SetActive(false);
        }

        /// <summary>
        /// Sits you at the lane with this state, the clock moving at this pace, and what the venue
        /// hands over for tonight. A coached frame, if there is one, comes first; then a sub night's
        /// first game, or an open game.
        /// </summary>
        public void SitDown(PlayerState state, ClockPace pace, LaneSetup setup)
        {
            _state = state ?? throw new ArgumentNullException(nameof(state));
            if (pace == null) throw new ArgumentNullException(nameof(pace));
            _setup = setup ?? new LaneSetup();
            _sitting = new Sitting(pace);
            _sittingStartBankroll = _state.BankrollNow;
            GamesBowled = BestGame = ChallengesWon = ChallengesLost = 0;
            _lane = new LaneState(_setup.Oil ?? new LaneOil(LanePattern.House), _rules.Hand, _setup.Seed);
            _rng = new System.Random(unchecked(_setup.Seed * 7919 + 1));
            _challenges = new Challenges(_rules, _setup.Seed);
            _match = _setup.Match;
            _practice = _setup.Practice;
            _practiceBall = 0;
            _shoesPaid = false;
            _offer = null;
            _game = null;
            _delivery = null;
            _trace.Path = null;
            _trace.Redraw();
            _laneView.Hand = _ball.Hand = _rules.Hand;
            ShowBall(_practice != null ? BowlingBall.House : _setup.Ball);
            _root.SetActive(true);

            if (_practice != null) BeginPracticeBall();
            else if (_match != null) NextLeagueGame();
            else StartOpenGame();
        }

        /// <summary>The ball on the approach: the one you're bowling with.</summary>
        void ShowBall(BowlingBall ball) => _ball.Show(ball, _art.BallSprite(ball));

        public RectTransform Control(string id) => _controls.TryGetValue(id, out var rect) ? rect : null;

        public void Lock(string id)
        {
            _lock = id;
            if (_state != null) Refresh();
        }

        bool Allowed(string id) => _lock == null || _lock == id;

        void OnDestroy()
        {
            foreach (var texture in _madeTextures)
                if (texture != null) Destroy(texture);
        }

        void Update()
        {
            if (_root == null || !_root.activeSelf || _state == null) return;
            float dt = Mathf.Min(Time.unscaledDeltaTime, Sitting.MaxFrameSeconds);
            if (_lock == null && !Practicing) _sitting.Play(dt);
            if (_step == Step.Rolling) Roll(dt * _playSpeed);
            else if (_step == Step.Pins) Hit(dt * _playSpeed);
            else if (_step == Step.Stand && _aimMeter.Running)
            {
                // The arrow swings with the sweep, every frame.
                _laneView.AimBoard = AimFrom(_aimMeter.Value);
                _laneView.RedrawAim();
            }
        }

        // ---- Games ----

        void StartOpenGame()
        {
            int cost = _rules.OpenGameCost(!_setup.OwnShoes, _shoesPaid);
            if (cost > _state.BankrollNow)
            {
                _game = null;
                _step = Step.Between;
                _message = "You can't cover the lane.";
                Refresh();
                return;
            }
            _game = BowlingGame.Open(cost);
            _message = cost > _rules.LaneFee ? $"{Money(_rules.LaneFee)} a game, {Money(_rules.ShoeRental)} for the shoes." : $"{Money(cost)} a game.";
            _lane.Rack();
            Continue();
        }

        void StartChallenge(Offer offer)
        {
            _game = BowlingGame.Challenge(offer.Bowler, offer.Stake);
            _message = $"{offer.Bowler.Name} for {Money(offer.Stake)}. You're up first.";
            _lane.Rack();
            Continue();
        }

        void NextLeagueGame()
        {
            _game = _match.Next();
            _message = $"Game {_match.Played.Count} of {_match.Games}. You lead off.";
            _lane.Rack();
            Continue();
        }

        /// <summary>After a ball or a new game: the next rack if it's due, and whose turn it is.</summary>
        void Continue()
        {
            if (_game.IsComplete)
            {
                EndGame();
                return;
            }
            if (_game.Up.Sheet.FreshRack && _lane.Deck.StandingCount < PinDeck.Pins) _lane.Rack();
            if (_game.YourTurn)
            {
                _chaining = false;
                BeginStand();
            }
            else
            {
                _step = Step.Others;
                Refresh();
                if (_chaining) BowlRegular();
            }
        }

        void EndGame()
        {
            int net = _game.Settle();
            if (_game.Kind == GameKind.Open && !_setup.OwnShoes) _shoesPaid = true;
            _state.ApplyRoundResult(net, _sitting.TakeMinutes());
            GamesBowled++;
            int score = _game.You.Score;
            BestGame = Math.Max(BestGame, score);
            _chaining = false;

            switch (_game.Kind)
            {
                case GameKind.Open:
                    _message = $"You bowled {score}.";
                    _offer = _challenges.AfterGame(_setup.Regulars, _state.BankrollNow);
                    _step = Step.Between;
                    break;
                case GameKind.Challenge:
                    var him = _game.Seats[1];
                    var result = _game.Result;
                    _challenges.Settled(him.Bowler, result);
                    if (result == Outcome.Won) ChallengesWon++;
                    else if (result == Outcome.Lost) ChallengesLost++;
                    _message = $"You {score}, {him.Name} {him.Sheet.Score}. " + (result == Outcome.Won ? $"He pays {Money(_game.Stake)}."
                        : result == Outcome.Lost ? $"You pay {Money(_game.Stake)}." : "A push.");
                    if (_challenges.IsDone(him.Bowler)) _message += $" {him.Name}'s done for the night.";
                    _offer = _challenges.AfterGame(_setup.Regulars, _state.BankrollNow);
                    _step = Step.Between;
                    break;
                default:
                    int ours = _game.TeamPins(BowlingGame.Yours), theirs = _game.TeamPins(BowlingGame.Theirs);
                    _message = $"{_match.YourTeam} {ours}, {_match.TheirTeam} {theirs}. " + (net > 0 ? $"+{Money(net)} from the pot." : net < 0 ? $"{Money(net)} to the pot." : "Even.");
                    _step = _match.FinishedNight ? Step.Over : Step.Between;
                    break;
            }
            Refresh();
        }

        // ---- Your throw ----

        void BeginStand()
        {
            _step = Step.Stand;
            _aimMeter.Clear();
            _hookMeter.Clear();
            _speedMeter.Clear();
            if (Practicing) _ball.Place(_practice.Balls[_practiceBall].Stand);
            _aimMeter.Begin(_rules.AimSweep / (Throw.MaxAim - Throw.MinAim));
            Refresh();
        }

        void Nudge(int boards)
        {
            if (_step != Step.Stand) return;
            _ball.Place(Practicing ? _practice.Balls[_practiceBall].Stand : _ball.Board + boards);
            Refresh();
        }

        /// <summary>The first tap: stops the aim's arrow where it is and starts the hook.</summary>
        void AimTap()
        {
            if (_step != Step.Stand) return;
            double v = _aimMeter.Stop();
            _aim = Practicing ? _practice.Balls[_practiceBall].Aim : AimFrom(v);
            if (Practicing) _aimMeter.Set(MeterFromAim(_aim));
            _step = Step.Hook;
            _hookMeter.Begin(_rules.HookSweep / (Throw.MaxHook - Throw.MinHook));
            Refresh();
            ActionTaken?.Invoke(BowlingControls.Aim);
        }

        void HookTap()
        {
            if (_step != Step.Hook) return;
            double v = _hookMeter.Stop();
            _hook = Practicing ? _practice.Balls[_practiceBall].Hook : HookFrom(v);
            if (Practicing) _hookMeter.Set(MeterFromHook(_hook));
            _step = Step.Speed;
            _speedMeter.Begin(_rules.SpeedSweep / 100);
            Refresh();
        }

        void SpeedTap()
        {
            if (_step != Step.Speed) return;
            double v = _speedMeter.Stop();
            _speed = Practicing ? _practice.Balls[_practiceBall].Speed : Throw.MinSpeed + v * (Throw.MaxSpeed - Throw.MinSpeed);
            if (Practicing) _speedMeter.Set((_speed - Throw.MinSpeed) / (Throw.MaxSpeed - Throw.MinSpeed));
            var t = new Throw(_ball.Board, _aim, _hook, _speed);
            if (!Practicing) t = _rules.Apply(t, _rng);
            var ball = Practicing ? BowlingBall.House : _setup.Ball;
            StartRoll(_lane.Bowl(t, ball), 1);
        }

        /// <summary>The aim sweep's reading as a board, your count: 35 at the left end, 5 at the right.</summary>
        double AimFrom(double v) => Throw.MaxAim - v * (Throw.MaxAim - Throw.MinAim);

        double MeterFromAim(double board) => (Throw.MaxAim - board) / (Throw.MaxAim - Throw.MinAim);

        /// <summary>
        /// Where the hook's needle is, from the screen's side: left of the middle hooks left, right
        /// of it hooks right, the middle rolls straight (Mike, 2026-11-24). For a right-hander the
        /// left is the usual hook (a positive <see cref="Throw.Hook"/>); for a left-hander, the right.
        /// </summary>
        double ScreenSide(double v) => (v - 0.5) * 2;

        double HookFrom(double v) => (_rules.Hand == Hand.Right ? -ScreenSide(v) : ScreenSide(v)) * Throw.MaxHook;

        double MeterFromHook(double hook)
        {
            double side = hook / Throw.MaxHook * (_rules.Hand == Hand.Right ? -1 : 1);
            return side / 2 + 0.5;
        }

        string HookReading(double v)
        {
            double side = ScreenSide(v);
            if (Math.Abs(side) < BallSim.StraightBelow) return "Straight";
            return $"{(side < 0 ? "Left" : "Right")} {Math.Abs(side) * 100:0}";
        }

        // ---- The ball out ----

        void StartRoll(Delivery delivery, float playSpeed)
        {
            _delivery = delivery;
            _playSpeed = playSpeed;
            _animTime = 0;
            _laneView.AimBoard = null;
            _laneView.BallColour = BallLook.ColourOf(delivery.Path.Ball);
            _laneView.BallSurface = _art.SurfaceFor(delivery.Path.Ball);
            _spin = new BallSpin(delivery.Path);
            _deckView.BallColour = _laneView.BallColour;
            _deckView.BallSurface = _art.SurfaceFor(delivery.Path.Ball);
            _laneView.Deck = delivery.Before;
            _step = Step.Rolling;
            Refresh();
            Roll(0);
        }

        void Roll(float dt)
        {
            var path = _delivery.Path;
            _animTime += dt;
            if (_animTime >= path.End.Time)
            {
                ToPins();
                return;
            }
            _laneView.Ball = PositionAt(path, _animTime);
            _laneView.BallTurn = _spin.At(_animTime);
            _laneView.Redraw();
        }

        static Vec2 PositionAt(BallPath path, double time)
        {
            var s = path.Samples;
            int i = Math.Min(s.Count - 1, Math.Max(1, (int)(time / (BallSim.Step * BallSim.SampleEvery)) + 1));
            while (i < s.Count - 1 && s[i].Time < time) i++;
            var a = s[i - 1];
            var b = s[i];
            double t = b.Time > a.Time ? Math.Max(0, Math.Min(1, (time - a.Time) / (b.Time - a.Time))) : 1;
            return a.Position + (b.Position - a.Position) * t;
        }

        /// <summary>The ball reaches the deck: the view cuts to the close-up (a gutter ball has nothing to hit).</summary>
        void ToPins()
        {
            _laneView.Ball = null;
            _laneView.Redraw();
            if (_delivery.Path.Gutter || _delivery.Pins.Frames.Count == 0)
            {
                AfterBall();
                return;
            }
            _step = Step.Pins;
            _animTime = 0;
            _deckView.Before = _delivery.Before;
            _deckView.gameObject.SetActive(true);
            Hit(0);
        }

        void Hit(float dt)
        {
            _animTime += dt;
            var frames = _delivery.Pins.Frames;
            int i = 0;
            while (i < frames.Count - 1 && frames[i + 1].Time <= _animTime) i++;
            _deckView.Frame = frames[i];
            _deckView.BallTurn = _spin.At(_delivery.Path.End.Time + frames[i].Time);
            _deckView.Redraw();
            if (_animTime >= frames[frames.Count - 1].Time + PinsLinger) AfterBall();
        }

        /// <summary>Two taps on the lane: the ball's run skips to the pins, the pins to what's left.</summary>
        void LaneTap()
        {
            float now = Time.unscaledTime;
            bool second = now - _lastLaneTap <= DoubleTap;
            _lastLaneTap = second ? -10 : now;
            // Throwing, a tap anywhere is the next meter's tap.
            if (_step == Step.Stand) { Press(BowlingControls.Aim, AimTap); return; }
            if (_step == Step.Hook) { Press(BowlingControls.Hook, HookTap); return; }
            if (_step == Step.Speed) { Press(BowlingControls.Speed, SpeedTap); return; }
            if (!second) return;
            if (_step == Step.Rolling) ToPins();
            else if (_step == Step.Pins) AfterBall();
        }

        void AfterBall()
        {
            if (_step != Step.Rolling && _step != Step.Pins) return;
            _deckView.gameObject.SetActive(false);
            _laneView.Ball = null;
            _laneView.Deck = _lane.Deck;
            _laneView.Redraw();
            _trace.Path = _delivery.Path;
            _trace.Redraw();

            if (Practicing)
            {
                _message = Describe(_delivery, null);
                _practiceBall++;
                if (_practiceBall < _practice.Balls.Length)
                {
                    _lane.Rack();
                    BeginPracticeBall(keepMessage: true);
                }
                else EndPractice();
                return;
            }

            var seat = _game.Up;
            _message = Describe(_delivery, seat.IsYou ? null : seat.Name);
            _game.Record(_delivery.Count);
            Continue();
        }

        // ---- The regulars ----

        void NextTap()
        {
            if (_step != Step.Others) return;
            _chaining = true;
            BowlRegular();
        }

        void BowlRegular()
        {
            var seat = _game.Up;
            if (seat.IsYou) return;
            StartRoll(seat.Bowler.BowlAt(_lane, _rng), _match != null ? 3 : 1.6f);
        }

        // ---- Between games ----

        void TakeGame()
        {
            if (_step != Step.Between || _offer == null) return;
            var offer = _offer;
            _offer = null;
            StartChallenge(offer);
        }

        void WaveOff()
        {
            if (_step != Step.Between || _offer == null) return;
            _message = $"{_offer.Bowler.Name} shrugs and goes back to his lane.";
            _offer = null;
            Refresh();
        }

        void BowlAgain()
        {
            if (!CanBowlAgain) return;
            if (_match != null) NextLeagueGame();
            else StartOpenGame();
        }

        bool CanBowlAgain => _step == Step.Between && _offer == null && (_match != null || _rules.OpenGameCost(!_setup.OwnShoes, _shoesPaid) <= _state.BankrollNow);

        /// <summary>Between games; or before the first ball of one, when nothing's been bowled yet.</summary>
        bool CanGetUp => !Practicing && _state != null &&
            (_step == Step.Between || _step == Step.Over || (_step == Step.Stand && _game != null && _game.Seats.All(s => s.Sheet.Balls.Count == 0)));

        void GetUp()
        {
            if (!CanGetUp) return;
            _state.PassTime(_sitting.TakeMinutes());
            _root.SetActive(false);
            BreakPointReached?.Invoke(BreakPoint.GotUpFromTable);
        }

        // ---- The coached frame ----

        void BeginPracticeBall(bool keepMessage = false)
        {
            if (!keepMessage) _message = "Juan's frame. Nothing counts.";
            _laneView.Deck = _lane.Deck;
            BeginStand();
        }

        void EndPractice()
        {
            _practice = null;
            ShowBall(_setup.Ball);
            string said = _message;
            if (_match != null) NextLeagueGame();
            else StartOpenGame();
            _message = said + " Your game now.";
            Refresh();
        }

        // ---- Words ----

        /// <summary>What a ball did, in a line: a strike, a spare, or the count and what's left.</summary>
        static string Describe(Delivery d, string who)
        {
            string prefix = who == null ? "" : who + ": ";
            int before = d.Before.StandingCount, count = d.Count;
            if (d.Path.Gutter) return prefix + "Gutter.";
            if (count == before) return prefix + (before == PinDeck.Pins ? "Strike!" : "Spare.");
            var left = d.Before.After(d.Pins).StandingPins();
            string leave = left.Count == 1 ? $"the {left[0]} left" : string.Join("-", left) + " left";
            return prefix + (count == 0 ? $"Nothing. {char.ToUpper(leave[0])}{leave.Substring(1)}." : $"{count} down, {leave}.");
        }

        // ---- The screen ----

        void Refresh()
        {
            if (_state == null) return;
            _bankrollText.text = Money(_state.BankrollNow);
            _clockText.text = _state.Clock.TimeLabel;
            _messageText.text = _message;

            bool throwing = _step == Step.Stand || _step == Step.Hook || _step == Step.Speed;
            _ball.Showing = throwing;
            _ball.Layout();
            _standText.text = $"Board {_ball.Board}";
            _laneView.StandBoard = _ball.Board;
            _laneView.AimBoard = _step == Step.Stand ? AimFrom(_aimMeter.Value) : _step == Step.Hook || _step == Step.Speed ? _aim : (double?)null;
            if (_step != Step.Rolling && _step != Step.Pins) _laneView.Deck = _lane?.Deck;
            _laneView.Redraw();

            ShowSheets();

            // The meters stay up while your ball is out, so the lane doesn't jump; the actions take
            // their place between turns, at the same height.
            bool yours = throwing || ((_step == Step.Rolling || _step == Step.Pins) && (Practicing || _game == null || _game.YourTurn));
            _controlsArea.gameObject.SetActive(yours);
            _actions.gameObject.SetActive(!yours);
            _offerRow.gameObject.SetActive(_step == Step.Between && _offer != null);
            if (_offer != null) _offerText.text = $"{_offer.Bowler.Name}: \"Game for {_offer.Stake}?\"";
            _buttons[BowlingControls.Bowl].gameObject.SetActive(_step == Step.Between && _offer == null && (_match != null || !Practicing));
            _bowlText.text = _match != null ? "Next game" : $"Bowl a game, {Money(_rules.OpenGameCost(!_setup.OwnShoes, _shoesPaid))}";
            _buttons[BowlingControls.Next].gameObject.SetActive(_step == Step.Others);

            Enable(BowlingControls.StandLeft, _step == Step.Stand);
            Enable(BowlingControls.StandRight, _step == Step.Stand);
            Enable(BowlingControls.Aim, _step == Step.Stand);
            Enable(BowlingControls.Hook, _step == Step.Hook);
            Enable(BowlingControls.Speed, _step == Step.Speed);
            Enable(BowlingControls.TakeGame, _step == Step.Between && _offer != null);
            Enable(BowlingControls.WaveOff, _step == Step.Between && _offer != null);
            Enable(BowlingControls.Bowl, CanBowlAgain);
            Enable(BowlingControls.Next, _step == Step.Others && !_chaining);
            Enable(BowlingControls.GetUp, CanGetUp);
        }

        void Enable(string id, bool usable) => _buttons[id].interactable = usable && Allowed(id);

        bool CanDrag() => _step == Step.Stand && Allowed(BowlingControls.Stand);

        /// <summary>You've let go of the ball. In a coached frame it goes back to the teacher's board.</summary>
        void Placed()
        {
            if (Practicing) _ball.Place(_practice.Balls[_practiceBall].Stand);
            Refresh();
            ActionTaken?.Invoke(BowlingControls.Stand);
        }

        void ShowSheets()
        {
            if (_game == null || Practicing)
            {
                _yourSheet.Hide();
                _theirSheet.Hide();
                _teamText.gameObject.SetActive(false);
                return;
            }
            var you = _game.Seats.First(s => s.IsYou);
            _yourSheet.Show("You", you.Sheet, _game.Up == you && !_game.IsComplete);
            Seat other = null;
            if (_game.Kind == GameKind.Challenge) other = _game.Seats[1];
            else if (_game.Kind == GameKind.League) other = !_game.Up.IsYou && !_game.IsComplete ? _game.Up : null;
            if (other != null) _theirSheet.Show(other.Name, other.Sheet, _game.Up == other && !_game.IsComplete);
            else _theirSheet.Hide();
            _teamText.gameObject.SetActive(_game.Kind == GameKind.League);
            if (_game.Kind == GameKind.League)
                _teamText.text = $"{_match.YourTeam} {_game.TeamPins(BowlingGame.Yours)}   {_match.TheirTeam} {_game.TeamPins(BowlingGame.Theirs)}";
        }

        void Press(string id, Action action)
        {
            if (!Allowed(id)) return;
            action();
            if (id != BowlingControls.Aim) ActionTaken?.Invoke(id);
        }

        /// <summary>
        /// Lays the screen out for a way up, over the lanes: in portrait the scores at the top,
        /// the lane's frame, the meters at the foot; in landscape the frame at the left and the
        /// rest at the right.
        /// </summary>
        public void Layout(bool landscape)
        {
            _portraitColumn.gameObject.SetActive(!landscape);
            _landscapeRow.gameObject.SetActive(landscape);
            if (landscape)
            {
                Arrange(_laneSide, _laneFrame);
                Arrange(_infoSide, _header, _sheets, _messageBar, _controlsArea, _actions);
            }
            else
            {
                Arrange(_portraitColumn, _header, _sheets, _messageBar, _laneFrame, _controlsArea, _actions);
            }
            Canvas.ForceUpdateCanvases();
            _ball.Layout();
            _laneView.Redraw();
        }

        void LateUpdate()
        {
            if (_root == null || !_root.activeSelf) return;
            _laneView.FollowFrame();
            if (_ball.Showing) _ball.Layout();
        }

        void BuildUi()
        {
            var canvas = MakeCanvas(transform, "Bowling Canvas", 0);
            _root = canvas.gameObject;

            var backdrop = MakeImage(canvas.transform, "Backdrop", Color.black);
            Stretch(backdrop.rectTransform);
            _plateImage = MakeImage(backdrop.transform, "Plate", Color.white);
            _plateImage.raycastTarget = false;
            _plateFit = _plateImage.gameObject.AddComponent<CoverFit>();
            _plateFit.Show(_plateImage, _plate, 0.5f);
            var shade = MakeImage(backdrop.transform, "Shade", new Color(0, 0, 0, 0.55f));
            Stretch(shade.rectTransform);

            // The lanes take the whole screen, under everything (Mike, 2026-11-24): the layout
            // only leaves them a frame that places them.
            var laneArea = new GameObject("Lane Area", typeof(RectTransform), typeof(RectMask2D));
            laneArea.transform.SetParent(canvas.transform, false);
            _laneArea = (RectTransform)laneArea.transform;
            Stretch(_laneArea);

            var safe = new GameObject("Safe Area", typeof(RectTransform), typeof(SafeAreaFitter));
            safe.transform.SetParent(canvas.transform, false);
            Stretch((RectTransform)safe.transform);

            _portraitColumn = MakeColumn(safe.transform, "Portrait", 24);
            _landscapeRow = MakeRow(safe.transform, "Landscape");
            Stretch(_landscapeRow);
            var sides = _landscapeRow.GetComponent<HorizontalLayoutGroup>();
            sides.spacing = 32;
            sides.padding = new RectOffset(28, 28, 24, 24);
            sides.childForceExpandHeight = true;
            _laneSide = MakeColumn(_landscapeRow, "Lane Side", 0);
            _laneSide.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1;
            _infoSide = MakeColumn(_landscapeRow, "Controls Side", 0);
            _infoSide.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1.25f;

            BuildHeader();
            BuildSheets();
            // The message on its own dark bar, so it reads over the masking panel.
            _messageBar = (RectTransform)new GameObject("Message Bar", typeof(RectTransform), typeof(LayoutElement)).transform;
            _messageBar.SetParent(_portraitColumn, false);
            _messageBar.GetComponent<LayoutElement>().preferredHeight = 60;
            Panel(_messageBar);
            _messageText = MakeText(_messageBar, "", 34, TextAnchor.MiddleCenter, 0);
            Stretch(_messageText.rectTransform);
            _messageText.name = "Message";
            _messageText.color = Gold;
            _messageText.enableAutoSizing = true;
            _messageText.fontSizeMin = 24;
            _messageText.fontSizeMax = 34;
            Shadowed(_messageText);
            BuildLane();
            BuildMeters();
            BuildActions();

            foreach (var section in new Component[] { _header, _messageBar, _sheets, _controlsArea, _actions })
            {
                var element = section.GetComponent<LayoutElement>() ?? section.gameObject.AddComponent<LayoutElement>();
                element.flexibleHeight = 0;
            }
            foreach (var area in new[] { _controlsArea, _actions })
            {
                var element = area.GetComponent<LayoutElement>();
                element.minHeight = element.preferredHeight = ThrowAreaHeight;
            }
            OnTurn(canvas, Layout);
        }

        void BuildHeader()
        {
            _header = MakeRow(_portraitColumn, "Header");
            Panel(_header);
            _header.GetComponent<LayoutElement>().preferredHeight = 88;
            var info = new GameObject("Info", typeof(RectTransform), typeof(LayoutElement));
            info.transform.SetParent(_header, false);
            info.GetComponent<LayoutElement>().flexibleWidth = 3;
            AddVertical(info, 2, new RectOffset());
            _bankrollText = MakeText(info.transform, "", 38, TextAnchor.MiddleLeft, 48);
            _bankrollText.name = "Bankroll";
            _bankrollText.fontStyle = FontStyles.Bold;
            Shadowed(_bankrollText);
            _clockText = MakeText(info.transform, "", 30, TextAnchor.MiddleLeft, 36);
            _clockText.color = new Color(1, 1, 1, 0.8f);
            Shadowed(_clockText);
            var getUp = Control(_header, BowlingControls.GetUp, "Leave game", GetUp);
            Outline(getUp);
            var layout = getUp.GetComponent<LayoutElement>();
            layout.preferredHeight = 84;
            layout.flexibleWidth = 0;
            layout.preferredWidth = layout.minWidth = 220;
        }

        /// <summary>
        /// The lanes over the whole screen, and the frame in the layout that places them: the
        /// deck close-up and the ball's path sit in the frame.
        /// </summary>
        void BuildLane()
        {
            _laneFrame = (RectTransform)new GameObject("Lane Frame", typeof(RectTransform), typeof(LayoutElement)).transform;
            _laneFrame.SetParent(_portraitColumn, false);
            var layout = _laneFrame.GetComponent<LayoutElement>();
            layout.flexibleHeight = 1;
            layout.minHeight = 420;

            var surfaces = new List<LaneSurface>();
            foreach (var kind in new[] { LaneSurfaceKind.Masking, LaneSurfaceKind.Approach, LaneSurfaceKind.Gutters, LaneSurfaceKind.Boards })
            {
                var surfaceGo = new GameObject(kind.ToString(), typeof(RectTransform), typeof(CanvasRenderer), typeof(LaneSurface));
                surfaceGo.transform.SetParent(_laneArea, false);
                var surface = surfaceGo.GetComponent<LaneSurface>();
                surface.raycastTarget = false;
                Stretch(surface.rectTransform);
                surfaces.Add(surface);
            }

            var laneGo = new GameObject("Lane", typeof(RectTransform), typeof(CanvasRenderer), typeof(LaneView));
            laneGo.transform.SetParent(_laneArea, false);
            _laneView = laneGo.GetComponent<LaneView>();
            Stretch(_laneView.rectTransform);
            _laneView.Frame = _laneFrame;
            var tap = laneGo.AddComponent<Button>();
            tap.transition = Selectable.Transition.None;
            tap.onClick.AddListener(LaneTap);
            foreach (var surface in surfaces)
            {
                var kind = (LaneSurfaceKind)Enum.Parse(typeof(LaneSurfaceKind), surface.name);
                surface.Show(kind, TextureFor(kind), _laneView);
                _laneView.AddUnder(surface);
            }

            // The ball's path: an upright strip down the frame's right side.
            var traceGo = new GameObject("Trace", typeof(RectTransform), typeof(CanvasRenderer), typeof(PathTrace));
            traceGo.transform.SetParent(_laneFrame, false);
            _trace = traceGo.GetComponent<PathTrace>();
            _trace.raycastTarget = false;
            Pin(_trace.rectTransform, new Vector2(1, 0.06f), new Vector2(1, 0.94f), new Vector2(-104, 0), new Vector2(-8, 0));

            var deckGo = new GameObject("Deck", typeof(RectTransform), typeof(CanvasRenderer), typeof(DeckView));
            deckGo.transform.SetParent(_laneFrame, false);
            _deckView = deckGo.GetComponent<DeckView>();
            _deckView.raycastTarget = false;
            _deckView.Init(_art.Deck, _art.PinTop, _art.PinDown);
            Pin(_deckView.rectTransform, new Vector2(0.06f, 0.45f), new Vector2(0.86f, 1), Vector2.zero, Vector2.zero);
            deckGo.SetActive(false);

            _laneView.AddLayers(_art.PinUp);
            _ball = ApproachBall.Make(_laneView, CanDrag, _ => Refresh(), Placed);
            _controls[BowlingControls.Stand] = (RectTransform)_ball.transform;
        }

        /// <summary>A near-solid dark panel behind a block of the screen, so it reads over the lane (Mike, 2026-11-24).</summary>
        static void Panel(RectTransform block)
        {
            var glass = block.gameObject.AddComponent<Image>();
            glass.color = new Color(0.04f, 0.035f, 0.05f, 0.94f);
            glass.raycastTarget = false;
        }

        Texture TextureFor(LaneSurfaceKind kind) =>
            kind == LaneSurfaceKind.Masking ? _art.Masking : kind == LaneSurfaceKind.Approach ? _art.Approach
            : kind == LaneSurfaceKind.Gutters ? _art.Gutter : _art.Boards;

        void BuildSheets()
        {
            _sheets = (RectTransform)new GameObject("Sheets", typeof(RectTransform), typeof(LayoutElement)).transform;
            _sheets.SetParent(_portraitColumn, false);
            AddVertical(_sheets.gameObject, 4, new RectOffset(12, 12, 8, 8));
            Panel(_sheets);
            _yourSheet = ScoreSheetView.Make(_sheets, "You Sheet");
            _theirSheet = ScoreSheetView.Make(_sheets, "Their Sheet");
            _teamText = MakeText(_sheets, "", 28, TextAnchor.MiddleCenter, 36);
            _teamText.name = "Teams";
            _yourSheet.Hide();
            _theirSheet.Hide();
            _teamText.gameObject.SetActive(false);
        }

        void BuildMeters()
        {
            _controlsArea = (RectTransform)new GameObject("Throw", typeof(RectTransform), typeof(LayoutElement)).transform;
            _controlsArea.SetParent(_portraitColumn, false);
            AddVertical(_controlsArea.gameObject, 10, new RectOffset(16, 16, 12, 12));
            Panel(_controlsArea);

            _standRow = MakeRow(_controlsArea, "Stand");
            _standRow.GetComponent<LayoutElement>().preferredHeight = 92;
            var left = Control(_standRow, BowlingControls.StandLeft, "<", () => Nudge(_rules.Hand == Hand.Right ? 1 : -1));
            left.name = "Stand Left Button";
            Outline(left);
            _standText = MakeText(_standRow, "", 38, TextAnchor.MiddleCenter, 0);
            _standText.name = "Stand Board";
            _standText.gameObject.AddComponent<LayoutElement>().flexibleWidth = 2;
            var right = Control(_standRow, BowlingControls.StandRight, ">", () => Nudge(_rules.Hand == Hand.Right ? -1 : 1));
            right.name = "Stand Right Button";
            Outline(right);

            _aimMeter = Meter.Make(_controlsArea, "Aim", MeterStyle.OnTheLane, v => $"Board {AimFrom(v):0.#}", 72);
            Register(BowlingControls.Aim, _aimMeter.Button, AimTap);
            _hookMeter = Meter.Make(_controlsArea, "Hook", MeterStyle.Dial, HookReading, 112);
            Register(BowlingControls.Hook, _hookMeter.Button, HookTap);
            _speedMeter = Meter.Make(_controlsArea, "Speed", MeterStyle.Power, v => $"{Throw.MinSpeed + v * (Throw.MaxSpeed - Throw.MinSpeed):0.0} mph");
            Register(BowlingControls.Speed, _speedMeter.Button, SpeedTap);
        }

        void BuildActions()
        {
            _actions = (RectTransform)new GameObject("Actions", typeof(RectTransform), typeof(LayoutElement)).transform;
            _actions.SetParent(_portraitColumn, false);
            AddVertical(_actions.gameObject, 12, new RectOffset(16, 16, 12, 12));
            Panel(_actions);

            _offerRow = (RectTransform)new GameObject("Offer", typeof(RectTransform), typeof(LayoutElement)).transform;
            _offerRow.SetParent(_actions, false);
            AddVertical(_offerRow.gameObject, 10, new RectOffset());
            _offerText = MakeText(_offerRow, "", 36, TextAnchor.MiddleCenter, 56);
            _offerText.name = "Offer Text";
            Shadowed(_offerText);
            var answers = MakeRow(_offerRow, "Answers");
            answers.GetComponent<LayoutElement>().preferredHeight = 120;
            Control(answers, BowlingControls.TakeGame, "Take the game", TakeGame);
            Outline(Control(answers, BowlingControls.WaveOff, "Wave him off", WaveOff));

            var bowl = Control(_actions, BowlingControls.Bowl, "Bowl", BowlAgain);
            bowl.name = "Bowl Button";
            _bowlText = bowl.GetComponentInChildren<TMP_Text>();
            bowl.GetComponent<LayoutElement>().preferredHeight = 120;
            var next = Control(_actions, BowlingControls.Next, "Next", NextTap);
            next.GetComponent<LayoutElement>().preferredHeight = 120;
        }

        Button Control(Transform parent, string id, string label, Action onClick)
        {
            var button = MakeButton(parent, label, () => Press(id, onClick));
            Register(id, button, null);
            return button;
        }

        void Register(string id, Button button, Action onClick)
        {
            if (onClick != null) button.onClick.AddListener(() => Press(id, onClick));
            var colors = button.colors;
            colors.disabledColor = new Color(1, 1, 1, 0.45f);
            button.colors = colors;
            _controls[id] = (RectTransform)button.transform;
            _buttons[id] = button;
        }

        /// <summary>A dark see-through button with a light edge.</summary>
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
            return AddVertical(go, 14, new RectOffset(padding, padding, padding, padding));
        }
    }
}
