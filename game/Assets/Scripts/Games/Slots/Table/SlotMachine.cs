using System;
using System.Linq;
using Starflare.State;
using Starflare.Ui;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Starflare.Ui.UiKit;

namespace Starflare.Slots
{
    /// <summary>
    /// A slot machine: any reel set, any number of reels and rows, any paylines, in the venue's
    /// look. UI built in code, portrait or landscape, filling the screen. Each pull settles into
    /// the player state the moment it's made (<see cref="SlotSpin"/>); the reels, the bulbs and
    /// the win meter only show it. Like the poker machine it raises a break point when you get
    /// up, and never reads story state or saves: the state's owner does that on its Changed event.
    /// </summary>
    public sealed class SlotMachine : MonoBehaviour
    {
        /// <summary>When the first reel lands, and how long each after it waits (seconds of spin).</summary>
        public const float FirstStop = 0.55f, NextStop = 0.38f;

        /// <summary>How far apart the reels land when you tap to stop them.</summary>
        const float QuickStop = 0.12f;

        /// <summary>How long the bulbs celebrate a win before going back to the slow chase.</summary>
        const float WinLights = 3f, JackpotLights = 8f;

        /// <summary>The win meter's slowest count, in dollars a second, and its longest.</summary>
        const float CountRate = 12f, LongestCount = 1.6f;

        const int BulbsPerRow = 11;

        /// <summary>Raised at a break point, where story beats and seed reveals may fire.</summary>
        public event Action<BreakPoint> BreakPointReached;

        public SlotRules Rules { get; private set; }
        public SlotLook Look { get; private set; }

        /// <summary>This sitting so far; still readable at the break point after you get up.</summary>
        public int SittingNet => _state == null ? 0 : _state.BankrollNow - _sittingStartBankroll;
        public int SittingRounds { get; private set; }

        /// <summary>The last pull, settled; its reels may still be turning.</summary>
        public SlotSpin LastSpin => _spin;

        /// <summary>Any reel still turning.</summary>
        public bool Spinning => _reels != null && _reels.Any(r => r.Turning);

        /// <summary>The bulbs' pattern right now.</summary>
        public LightPattern Lights => _pattern;

        PlayerState _state;
        Sitting _sitting;
        System.Random _random;
        SlotSpin _spin;
        int _credits = 1;
        int _sittingStartBankroll;
        float _shownWin;
        bool _counting;

        ReelMotion[] _reels;
        SymbolView[,] _cells;
        Image[,] _winFrames;
        Image[] _bulbs, _glows;
        LightPattern _pattern = LightPattern.Idle;
        float _patternTime;

        GameObject _root;
        GameHeader _gameHeader;
        TMP_Text _statusText, _betText;
        TMP_Text[,] _paytableCells;
        Image[] _paytableRows, _paytableColumns;
        Button _betOneButton, _betMaxButton, _spinButton, _getUpButton;
        TMP_Text _spinLabel;
        RectTransform _paytable, _paytablePane, _playPane;

        Sprite _plate;
        float _plateFocus;
        Image _plateImage;
        CoverFit _plateFit;

        /// <summary>
        /// Sets the machine up over the venue's plate, framed at this focus on a portrait screen,
        /// with the venue's reels and look. It stays hidden until you sit down. A seeded random
        /// is for tests; play uses a fresh one.
        /// </summary>
        public void Init(SlotRules rules, Sprite plate, float plateFocus, SlotLook look = null, System.Random random = null)
        {
            Rules = rules ?? throw new ArgumentNullException(nameof(rules));
            Look = look ?? new SlotLook();
            _plate = plate;
            _plateFocus = plateFocus;
            _random = random ?? new System.Random();
            _reels = Rules.Reels.Select((reel, c) => new ReelMotion(reel.Count, _random.Next(reel.Count))).ToArray();
            BuildUi();
            DrawReels();
            _root.SetActive(false);
        }

        /// <summary>Puts the machine over another plate, for a venue you play by day and by night.</summary>
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
            _sittingStartBankroll = state.BankrollNow;
            SittingRounds = 0;
            _counting = false;
            _credits = Mathf.Clamp(_credits, 1, Math.Max(1, MaxAffordableCredits));
            SetPattern(LightPattern.Idle);
            _root.SetActive(true);
            Refresh();
        }

        /// <summary>Counts real time played and turns the reels, while the machine is up and the app is in front.</summary>
        void Update()
        {
            if (_sitting == null || !_root.activeSelf) return;
            float seconds = Mathf.Min(Time.unscaledDeltaTime, Sitting.MaxFrameSeconds);
            _sitting.Play(seconds);
            Animate(seconds);
        }

        int MaxAffordableCredits
        {
            get
            {
                int credits = Rules.MaxCredits;
                while (credits > 0 && Rules.BetFor(credits) > _state.BankrollNow) credits--;
                return credits;
            }
        }

        bool Broke => MaxAffordableCredits < 1;

        // ---- Play ----

        void BetOne()
        {
            if (Spinning || Broke) return;
            _credits = _credits >= MaxAffordableCredits ? 1 : _credits + 1;
            Refresh();
        }

        void BetMax()
        {
            if (Spinning || Broke) return;
            _credits = MaxAffordableCredits;
            Pull();
        }

        /// <summary>The spin button: a pull, or while the reels turn, a tap that lands them now.</summary>
        void SpinOrStop()
        {
            if (Spinning) StopReels();
            else Pull();
        }

        void Pull()
        {
            if (Spinning || Broke) return;
            FinishCount();
            _credits = Mathf.Clamp(_credits, 1, MaxAffordableCredits);
            _spin = SlotSpin.Pull(Rules, _credits, _state.BankrollNow, _random);
            _state.ApplyRoundResult(_spin.Net, _sitting.TakeMinutes());
            SittingRounds++;
            _shownWin = 0;
            for (int c = 0; c < _reels.Length; c++)
                _reels[c].Spin(_spin.Stops[c], FirstStop + NextStop * c);
            SetPattern(LightPattern.Spinning);
            Refresh();
        }

        void StopReels()
        {
            for (int c = 0; c < _reels.Length; c++) _reels[c].StopWithin(QuickStop * c);
        }

        void GetUp()
        {
            if (Spinning) return;
            FinishCount();
            _state.PassTime(_sitting.TakeMinutes());
            _root.SetActive(false);
            BreakPointReached?.Invoke(BreakPoint.GotUpFromTable);
        }

        // ---- Animation ----

        void Animate(float seconds)
        {
            bool wasSpinning = Spinning;
            foreach (var reel in _reels) reel.Advance(seconds);
            DrawReels();
            if (wasSpinning && !Spinning) Landed();

            if (_counting)
            {
                float rate = Mathf.Max(CountRate, _spin.Payout / LongestCount);
                _shownWin = Mathf.MoveTowards(_shownWin, _spin.Payout, rate * seconds);
                if (_shownWin >= _spin.Payout) _counting = false;
                Refresh();
            }

            _patternTime += seconds;
            if ((_pattern == LightPattern.Win && _patternTime > WinLights)
                || (_pattern == LightPattern.Jackpot && _patternTime > JackpotLights))
                SetPattern(LightPattern.Idle);
            DrawLights();
        }

        /// <summary>The last reel's down: the win, if any, lights up and counts in.</summary>
        void Landed()
        {
            if (_spin.Payout > 0)
            {
                bool top = _spin.Wins.Any(w => w.Pay == Rules.Pays[0]);
                SetPattern(top ? LightPattern.Jackpot : LightPattern.Win);
                _counting = true;
            }
            else SetPattern(LightPattern.Idle);
            Refresh();
        }

        void FinishCount()
        {
            if (!_counting) return;
            _shownWin = _spin.Payout;
            _counting = false;
        }

        void SetPattern(LightPattern pattern)
        {
            _pattern = pattern;
            _patternTime = 0;
            if (_bulbs != null) DrawLights();
        }

        void DrawLights()
        {
            int count = _bulbs.Length;
            for (int i = 0; i < count; i++)
            {
                bool lit = LightShow.IsLit(_pattern, i, count, _patternTime);
                _bulbs[i].color = lit ? Look.BulbOn : Look.BulbOff;
                _glows[i].enabled = lit;
            }

            bool celebrating = _pattern == LightPattern.Win || _pattern == LightPattern.Jackpot;
            bool flashOn = LightShow.IsLit(LightPattern.Win, 0, 1, _patternTime);
            for (int c = 0; c < Rules.Columns; c++)
            for (int r = 0; r < Rules.Rows; r++)
                _winFrames[c, r].enabled = celebrating && flashOn && _spin != null && _spin.IsWinningCell(c, r);

            for (int p = 0; p < _paytableRows.Length; p++)
            {
                bool won = _spin != null && !Spinning && _spin.Wins.Any(w => w.Pay == Rules.Pays[p]);
                _paytableRows[p].color = won && (!celebrating || flashOn) ? new Color(1, 1, 1, 0.18f) : Color.clear;
            }
        }

        /// <summary>
        /// Places each reel's cells where its motion has the strip: the cell above the window,
        /// the rows in it, and the one coming in below, by anchors so they fit any size.
        /// </summary>
        void DrawReels()
        {
            int rows = Rules.Rows;
            for (int c = 0; c < _reels.Length; c++)
            {
                var motion = _reels[c];
                var strip = Rules.Reels[c];
                float offset = (float)motion.Offset;
                for (int k = 0; k < rows + 2; k++)
                {
                    var cell = _cells[c, k];
                    int index = ((motion.Top + k - 1) % strip.Count + strip.Count) % strip.Count;
                    cell.Show(Look.For(strip[index]));
                    float top = 1 - (k - 1 - offset) / rows;
                    cell.Rect.anchorMin = new Vector2(0, top - 1f / rows);
                    cell.Rect.anchorMax = new Vector2(1, top);
                    cell.Rect.offsetMin = cell.Rect.offsetMax = Vector2.zero;
                    cell.Alpha = motion.Blurred ? 0.65f : 1f;
                    cell.Rect.localScale = motion.Blurred ? new Vector3(1, 1.25f, 1) : Vector3.one;
                }
            }
        }

        // ---- Display ----

        void Refresh()
        {
            bool spinning = Spinning;
            int column = (spinning && _spin != null ? _spin.Credits : _credits) - 1;
            for (int c = 0; c < Rules.MaxCredits; c++)
                _paytableColumns[c].color = c == column ? Look.LitColumn : Color.clear;

            string message;
            if (spinning) message = "";
            else if (Broke) message = "You're broke. Get up and find another way back in.";
            else if (_spin == null) message = "Pick a bet and spin";
            else if (_spin.Wins.Count == 1) message = $"{_spin.Wins[0].Pay.Label}   Win {Money(ShownWin)}";
            else if (_spin.Wins.Count > 1) message = $"{_spin.Wins.Count} lines   Win {Money(ShownWin)}";
            else message = "";
            _statusText.text = message;
            _statusText.color = !spinning && _spin != null && _spin.Payout > 0 ? Gold : Look.GlassText;

            _spinLabel.text = spinning ? "Stop" : "Spin";
            _spinButton.interactable = spinning || !Broke;
            _betOneButton.interactable = !spinning && !Broke;
            _betMaxButton.interactable = !spinning && !Broke;
            _getUpButton.interactable = !spinning;
            RefreshMeters();
            DrawLights();
        }

        /// <summary>
        /// Cash and the bet. The pull is already settled, so until the reels are down and the
        /// win has counted in, the cash shows what you had less the bet, as a real meter would.
        /// </summary>
        void RefreshMeters()
        {
            int notYetShown = _spin == null ? 0 : _spin.Payout - (Spinning ? 0 : ShownWin);
            int cash = _state.BankrollNow - notYetShown;
            _gameHeader.Show(cash, _state.Debts, _state.Clock, _sitting.ClockStopped ? " (late)" : "");
            string lines = Rules.Lines.Count > 1 ? $" on {Rules.Lines.Count} lines" : "";
            _betText.text = $"Bet {Money(Rules.BetFor(_credits))}{lines}";
        }

        /// <summary>The win meter's count so far, the whole win once it's done.</summary>
        int ShownWin => _counting ? Mathf.FloorToInt(_shownWin) : _spin.Payout;

        // ---- UI construction ----

        void BuildUi()
        {
            var canvas = MakeCanvas(transform, "Slot Machine Canvas", 0);
            _root = canvas.gameObject;

            var backdrop = MakeImage(canvas.transform, "Backdrop", Color.black);
            Stretch(backdrop.rectTransform);
            _plateImage = MakeImage(backdrop.transform, "Plate", Color.white);
            _plateImage.raycastTarget = false;
            _plateFit = _plateImage.gameObject.AddComponent<CoverFit>();
            _plateFit.Show(_plateImage, _plate, _plateFocus);

            // The cabinet fills your screen: you're at the glass, as at the poker machine.
            var cabinet = MakeImage(canvas.transform, "Cabinet", Look.Cabinet);
            Stretch(cabinet.rectTransform);

            var safe = new GameObject("Safe Area", typeof(RectTransform), typeof(SafeAreaFitter));
            safe.transform.SetParent(canvas.transform, false);
            Stretch((RectTransform)safe.transform);
            var column = AddVertical(safe, 14, new RectOffset(36, 36, 28, 28));

            BuildHeader(column);

            var title = MakeText(column, Rules.Name, 64, TextAnchor.MiddleCenter, 80);
            title.name = "Title";
            title.color = Look.Accent;
            title.fontStyle = FontStyles.Bold | FontStyles.Italic;
            title.characterSpacing = 6;
            NoWrap(title);

            var panes = MakeRow(column, "Panes");
            panes.GetComponent<LayoutElement>().flexibleHeight = 1;
            var group = panes.GetComponent<HorizontalLayoutGroup>();
            group.spacing = 40;
            group.childForceExpandWidth = false;
            group.childForceExpandHeight = true;
            _paytablePane = MakePane(panes, "Paytable Pane", 1f);
            _playPane = MakePane(panes, "Play Pane", 1.35f);
            _paytable = BuildPaytable(_playPane);
            BuildReels(_playPane);

            _statusText = MakeText(_playPane, "", 46, TextAnchor.MiddleCenter, 64);
            _statusText.name = "Status";
            _statusText.fontStyle = FontStyles.Bold;
            NoWrap(_statusText);

            var buttons = MakeRow(column, "Buttons");
            buttons.GetComponent<LayoutElement>().preferredHeight = 140;
            _betOneButton = MachineButton(buttons, "Bet one", BetOne, Look.Button, Look.ButtonText);
            _betMaxButton = MachineButton(buttons, "Bet max", BetMax, Look.Button, Look.ButtonText);
            _spinButton = MachineButton(buttons, "Spin", SpinOrStop, Look.SpinButton, Look.SpinButtonText);
            _spinLabel = _spinButton.GetComponentInChildren<TMP_Text>();

            _betText = MakeText(column, "", 34, TextAnchor.MiddleCenter, 44);
            _betText.name = "Bet";
            _betText.color = new Color(1, 1, 1, 0.75f);

            OnTurn(canvas, landscape =>
            {
                column.GetComponent<VerticalLayoutGroup>().padding =
                    landscape ? new RectOffset(48, 48, 16, 16) : new RectOffset(36, 36, 28, 28);
                Rearrange(landscape);
            });
        }

        void BuildHeader(Transform column)
        {
            _gameHeader = GameHeader.Make(column, GetUp);
            _getUpButton = _gameHeader.Leave;
        }

        /// <summary>
        /// Portrait stacks the paytable over the reels. Landscape is too short for that, so the
        /// paytable gets its own pane to the left, as on the poker machine.
        /// </summary>
        void Rearrange(bool landscape)
        {
            _paytablePane.gameObject.SetActive(landscape);
            _paytable.SetParent(landscape ? _paytablePane : _playPane, false);
            if (!landscape) _paytable.SetAsFirstSibling();
            float row = landscape ? Mathf.Min(70, 560f / Rules.Pays.Count) : Mathf.Min(64, 520f / Rules.Pays.Count);
            foreach (var paytableRow in _paytableRows)
                paytableRow.GetComponent<LayoutElement>().preferredHeight = row;
            foreach (var cell in _paytableCells)
                cell.fontSize = landscape ? 30 : 32;
        }

        /// <summary>The reel window between two rows of bulbs, which chase round it.</summary>
        void BuildReels(Transform parent)
        {
            var top = BulbRow(parent, "Bulbs Top");
            var frame = MakeImage(parent, "Window", Look.Trim);
            frame.sprite = Rounded;
            frame.type = Image.Type.Sliced;
            var frameLayout = frame.gameObject.AddComponent<LayoutElement>();
            frameLayout.flexibleHeight = 1;
            frameLayout.minHeight = 120 * Rules.Rows;
            var bottom = BulbRow(parent, "Bulbs Bottom");

            // Round the marquee: along the top left to right, back along the bottom right to left.
            _bulbs = new Image[BulbsPerRow * 2];
            _glows = new Image[BulbsPerRow * 2];
            for (int i = 0; i < BulbsPerRow; i++)
            {
                (_bulbs[i], _glows[i]) = Bulb(top, i);
                int back = BulbsPerRow * 2 - 1 - i;
                (_bulbs[back], _glows[back]) = Bulb(bottom, back);
            }

            var reels = new GameObject("Reels", typeof(RectTransform));
            reels.transform.SetParent(frame.transform, false);
            var reelsRect = (RectTransform)reels.transform;
            Stretch(reelsRect);
            reelsRect.offsetMin = new Vector2(14, 14);
            reelsRect.offsetMax = new Vector2(-14, -14);
            var row = reels.AddComponent<HorizontalLayoutGroup>();
            row.spacing = 10;
            row.childControlWidth = row.childControlHeight = true;
            row.childForceExpandWidth = row.childForceExpandHeight = true;

            int columns = Rules.Columns, rows = Rules.Rows;
            _cells = new SymbolView[columns, rows + 2];
            _winFrames = new Image[columns, rows];
            for (int c = 0; c < columns; c++)
            {
                var window = MakeImage(reels.transform, $"Reel {c + 1}", Look.ReelFace);
                window.raycastTarget = false;
                window.gameObject.AddComponent<RectMask2D>();

                // Straight paylines run behind the symbols, as if painted on the reel glass's
                // backing; a zigzag line shows only when it wins.
                for (int l = 0; l < Rules.Lines.Count; l++)
                {
                    var line = Rules.Lines[l];
                    if (line.Any(r => r != line[0])) continue;
                    var bar = MakeImage(window.transform, $"Payline {l + 1}", Look.Accent);
                    bar.raycastTarget = false;
                    float middle = 1 - (line[0] + 0.5f) / rows;
                    bar.rectTransform.anchorMin = new Vector2(0, middle);
                    bar.rectTransform.anchorMax = new Vector2(1, middle);
                    bar.rectTransform.offsetMin = new Vector2(0, -3);
                    bar.rectTransform.offsetMax = new Vector2(0, 3);
                }

                for (int k = 0; k < rows + 2; k++)
                    _cells[c, k] = new SymbolView(window.transform, $"Cell {k}");

                // A shade at the top and bottom of each reel, so it reads as a drum.
                Shade(window.transform, "Shade Top", true);
                Shade(window.transform, "Shade Bottom", false);

                for (int r = 0; r < rows; r++)
                {
                    var win = MakeImage(window.transform, $"Win {r + 1}", Gold);
                    win.sprite = RoundedFrame;
                    win.type = Image.Type.Sliced;
                    win.pixelsPerUnitMultiplier = 0.5f;
                    win.raycastTarget = false;
                    win.rectTransform.anchorMin = new Vector2(0, 1 - (r + 1f) / rows);
                    win.rectTransform.anchorMax = new Vector2(1, 1 - (float)r / rows);
                    win.rectTransform.offsetMin = new Vector2(4, 4);
                    win.rectTransform.offsetMax = new Vector2(-4, -4);
                    win.enabled = false;
                    _winFrames[c, r] = win;
                }
            }
        }

        static void Shade(Transform window, string name, bool top)
        {
            var shade = MakeImage(window, name, new Color(0, 0, 0, 0.3f));
            shade.sprite = Fade;
            shade.raycastTarget = false;
            shade.rectTransform.anchorMin = new Vector2(0, top ? 0.78f : 0);
            shade.rectTransform.anchorMax = new Vector2(1, top ? 1 : 0.22f);
            shade.rectTransform.offsetMin = shade.rectTransform.offsetMax = Vector2.zero;
            if (!top) shade.rectTransform.localScale = new Vector3(1, -1, 1);
        }

        static RectTransform BulbRow(Transform parent, string name)
        {
            var row = MakeRow(parent, name);
            row.GetComponent<LayoutElement>().preferredHeight = 40;
            var group = row.GetComponent<HorizontalLayoutGroup>();
            group.spacing = 0;
            group.childForceExpandHeight = true;
            return row;
        }

        (Image bulb, Image glow) Bulb(Transform row, int index)
        {
            var slot = new GameObject($"Bulb {index + 1}", typeof(RectTransform), typeof(LayoutElement));
            slot.transform.SetParent(row, false);
            slot.GetComponent<LayoutElement>().flexibleWidth = 1;
            var glow = MakeImage(slot.transform, "Glow", new Color(Look.BulbOn.r, Look.BulbOn.g, Look.BulbOn.b, 0.55f));
            glow.sprite = SlotGlyphs.Glow;
            glow.raycastTarget = false;
            glow.rectTransform.sizeDelta = new Vector2(76, 76);
            var bulb = MakeImage(slot.transform, "Bulb", Look.BulbOff);
            bulb.sprite = Circle;
            bulb.raycastTarget = false;
            bulb.rectTransform.sizeDelta = new Vector2(24, 24);
            return (bulb, glow);
        }

        RectTransform BuildPaytable(Transform parent)
        {
            var glass = MakeImage(parent, "Paytable", Look.Glass);
            glass.sprite = Rounded;
            glass.type = Image.Type.Sliced;
            glass.raycastTarget = false;
            AddVertical(glass.gameObject, 0, new RectOffset(10, 10, 10, 10));
            var table = (RectTransform)glass.transform;

            // Column stripes sit behind the rows, so the current bet reads as a lit stripe.
            var stripes = MakeRow(table, "Bet Columns");
            stripes.GetComponent<LayoutElement>().ignoreLayout = true;
            Stretch(stripes);
            stripes.offsetMin = new Vector2(10, 10);
            stripes.offsetMax = new Vector2(-10, -10);
            var stripeGroup = stripes.GetComponent<HorizontalLayoutGroup>();
            stripeGroup.spacing = 0;
            stripeGroup.childForceExpandHeight = true;
            Share(MakeImage(stripes, "Label Column", Color.clear).gameObject, LabelShare);
            _paytableColumns = new Image[Rules.MaxCredits];
            for (int c = 0; c < Rules.MaxCredits; c++)
            {
                _paytableColumns[c] = MakeImage(stripes, $"Column {c + 1}", Color.clear);
                _paytableColumns[c].raycastTarget = false;
                Share(_paytableColumns[c].gameObject, 1);
            }

            _paytableRows = new Image[Rules.Pays.Count];
            _paytableCells = new TMP_Text[Rules.Pays.Count, Rules.MaxCredits];
            for (int p = 0; p < Rules.Pays.Count; p++)
            {
                var pay = Rules.Pays[p];
                _paytableRows[p] = MakeImage(table, pay.Label, Color.clear);
                _paytableRows[p].raycastTarget = false;
                var rowLayout = _paytableRows[p].gameObject.AddComponent<LayoutElement>();
                rowLayout.preferredHeight = 56;
                rowLayout.flexibleHeight = 0;
                var row = _paytableRows[p].gameObject.AddComponent<HorizontalLayoutGroup>();
                row.spacing = 0;
                row.childControlWidth = row.childControlHeight = true;
                row.childForceExpandWidth = false;
                row.childForceExpandHeight = true;

                PayPicture(row.transform, pay);
                for (int c = 1; c <= Rules.MaxCredits; c++)
                {
                    var cell = MakeText(row.transform, pay.PayoutCredits(c, Rules.MaxCredits).ToString(), 32, TextAnchor.MiddleRight, 0);
                    cell.color = Look.GlassText;
                    cell.margin = new Vector4(0, 0, 14, 0);
                    NoWrap(cell);
                    Share(cell.gameObject, 1);
                    _paytableCells[p, c - 1] = cell;
                }
            }
            return table;
        }

        const float LabelShare = 2.6f;

        /// <summary>
        /// A pay's picture on the glass: its symbol as many times as it counts, on little reel
        /// tiles, or its label when it takes more than one symbol ("ANY BAR").
        /// </summary>
        void PayPicture(Transform row, SlotPay pay)
        {
            var holder = new GameObject("Picture", typeof(RectTransform));
            holder.transform.SetParent(row, false);
            Share(holder, LabelShare);
            var look = pay.IsOneSymbol ? Look.For(pay.Symbols[0]) : null;
            if (look == null || look.Drawing == SymbolDrawing.Blank)
            {
                var label = MakeText(holder.transform, pay.Label, 28, TextAnchor.MiddleLeft, 0);
                label.color = Look.GlassText;
                label.fontStyle = FontStyles.Bold;
                label.enableAutoSizing = true;
                label.fontSizeMin = 14;
                label.fontSizeMax = 30;
                Stretch(label.rectTransform);
                label.margin = new Vector4(12, 0, 0, 0);
                return;
            }

            var tiles = holder.AddComponent<HorizontalLayoutGroup>();
            tiles.spacing = 6;
            tiles.padding = new RectOffset(10, 0, 4, 4);
            tiles.childAlignment = TextAnchor.MiddleLeft;
            tiles.childControlWidth = tiles.childControlHeight = true;
            tiles.childForceExpandWidth = false;
            tiles.childForceExpandHeight = true;
            for (int i = 0; i < pay.Count; i++)
            {
                var tile = MakeImage(holder.transform, $"Tile {i + 1}", Look.ReelFace);
                tile.sprite = Rounded;
                tile.type = Image.Type.Sliced;
                tile.pixelsPerUnitMultiplier = 3;
                tile.raycastTarget = false;
                var tileLayout = tile.gameObject.AddComponent<LayoutElement>();
                tileLayout.preferredWidth = 64;
                tileLayout.flexibleWidth = 0;
                new SymbolView(tile.transform, "Symbol").Show(look);
            }
        }

        /// <summary>Sizes a paytable cell by its share of the row alone, never by its contents, so the columns line up with their stripes.</summary>
        static void Share(GameObject cell, float share)
        {
            if (!cell.TryGetComponent(out LayoutElement layout)) layout = cell.AddComponent<LayoutElement>();
            layout.minWidth = 0;
            layout.preferredWidth = 0;
            layout.flexibleWidth = share;
        }

        /// <summary>A pane sized by its share of the screen alone, never by its contents.</summary>
        static RectTransform MakePane(Transform parent, string name, float width)
        {
            var pane = new GameObject(name, typeof(RectTransform), typeof(LayoutElement));
            pane.transform.SetParent(parent, false);
            var layout = pane.GetComponent<LayoutElement>();
            layout.minWidth = 0;
            layout.preferredWidth = 0;
            layout.flexibleWidth = width;
            var column = AddVertical(pane, 12, new RectOffset());
            column.GetComponent<VerticalLayoutGroup>().childAlignment = TextAnchor.MiddleCenter;
            return column;
        }

        /// <summary>A machine button in the look's colours.</summary>
        static Button MachineButton(Transform parent, string label, UnityEngine.Events.UnityAction onClick, Color fill, Color ink)
        {
            var button = MakeButton(parent, label, onClick);
            button.GetComponent<Image>().color = fill;
            var text = button.GetComponentInChildren<TMP_Text>();
            text.color = ink;
            text.fontStyle = FontStyles.Bold;
            return button;
        }
    }
}
