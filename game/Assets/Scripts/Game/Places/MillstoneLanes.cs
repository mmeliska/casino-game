using System;
using System.Collections.Generic;
using Starflare.Bowling;
using Starflare.Phone;
using Starflare.State;
using Starflare.Ui;
using UnityEngine;
using UnityEngine.Events;

namespace Starflare.Game
{
    /// <summary>
    /// Millstone Lanes, Juan's bowling alley. Kasia's coupons put it on the map and are how you
    /// first walk in (A1.3); after that Juan's at the counter, the lanes are open for bowling
    /// (bowling spec), and the steel door opens on the back lot and his late game
    /// (<see cref="MillstoneBackLot"/>). The licensed machines inside pay real money, like
    /// Kasia's (Mike, 2026-11-20), and aren't built yet.
    ///
    /// The lanes: open bowling any time but league night (Tuesdays 7 to 10, when you watch),
    /// unless a short team has asked you to sub. The venue does everything the lane can't know:
    /// tonight's oil from the clock, who's on the lanes, the ball and shoes you own, the sub ask,
    /// Juan's counter, and what the night means to Juan.
    /// </summary>
    public sealed class MillstoneLanes : MonoBehaviour, IPlace
    {
        public const string PlaceId = "millstone-lanes";

        /// <summary>Chuy, the league bowler from the back-lot circle: the lanes' main opponent.</summary>
        public const string Chuy = "avatars-7_r1c1";

        /// <summary>League night: Tuesdays, 7 to 10 (Mike, 2026-11-23).</summary>
        public const int LeagueFrom = 19 * 60, LeagueTo = 22 * 60;

        /// <summary>A short team asks between 6:30 and 7:30, at a break point.</summary>
        public const int SubAskFrom = 18 * 60 + 30, SubAskTo = 19 * 60 + 30;

        /// <summary>About one Tuesday in three a team's short (proposed).</summary>
        public const int SubAskOdds = 3;

        /// <summary>Juan's warmer greeting opens at this standing, and he's easy with you at the next.</summary>
        public const int Warmer = 2, Easy = 6;

        /// <summary>The averages of tonight's two other faces on the lanes, besides Chuy's 185.</summary>
        static readonly int[] OtherAverages = { 160, 135 };

        const int ChuysAverage = 185;
        const string Lanes = "lanes_lanes";
        const string Counter = "lanes_counter";
        const string YourTeam = "Millstone Parts", TheirTeam = "Lane Six";

        IPlaceHost _host;
        MillstoneBackLot _backLot;
        BowlingLane _lane;
        readonly BowlingRules _rules = new BowlingRules();

        /// <summary>You sat down to sub tonight: the get-up plays the league's word, not Juan's.</summary>
        bool _subbing;

        public string Id => PlaceId;
        public string Name => "Millstone Lanes";
        public PhoneMap.Place Pin => new PhoneMap.Place(PlaceId, Name, 0.22f, 0.44f);

        /// <summary>Opens once Kasia's given you the box to carry.</summary>
        public bool IsOpen => _host.Story.Flag(StoryContract.CarryingCoupons) || _host.Story.Flag(StoryContract.MetJuan);

        public BowlingLane Lane => _lane;

        public void Init(IPlaceHost host, MillstoneBackLot backLot, BowlingLane lane)
        {
            _host = host;
            _backLot = backLot;
            _backLot.Init(host, ShowInside, ShowOutside);
            _lane = lane;
            _lane.Init(_rules, Art.Plate(Art.PlateAt(Lanes, host.State.Clock)), LaneTextures());
            _lane.BreakPointReached += OnGetUp;
        }

        /// <summary>You arrive in the lot, under the sign.</summary>
        public void Arrive() => ShowOutside();

        public void Leave() { }

        // ---- The calendar ----

        /// <summary>The league has every lane: Tuesday, 7 to 10.</summary>
        public static bool LeagueOn(GameClock clock) =>
            clock.Weekday == DayOfWeek.Tuesday && clock.MinuteOfDay >= LeagueFrom && clock.MinuteOfDay < LeagueTo;

        /// <summary>
        /// A team's short this Tuesday: one in three, by the date, so a reload asks the same.
        /// The ask comes between 6:30 and 7:30.
        /// </summary>
        public static bool ShortTonight(GameClock clock) =>
            clock.Weekday == DayOfWeek.Tuesday && clock.MinuteOfDay >= SubAskFrom && clock.MinuteOfDay < SubAskTo &&
            ShortTuesday(clock.Day);

        public static bool ShortTuesday(int day) => unchecked((uint)(day * 2654435761u) >> 7) % SubAskOdds == 0;

        /// <summary>The lane Juan puts you on tonight, 1 to 6: by the night, so it's the same all evening.</summary>
        public static int LaneTonight(int night) => (int)((unchecked((uint)night * 2654435761u) >> 13) % 6) + 1;

        /// <summary>How warm Juan is: a sub night counts two, a challenge won one, open bowling nothing (Mike, 2026-11-23).</summary>
        public static int Standing(int subNights, int challengesWon) => 2 * subNights + challengesWon;

        // ---- Screens ----

        /// <summary>The lot under the sign. Around back opens once you've met Juan, so the coupons come first.</summary>
        void ShowOutside()
        {
            if (!_host.Story.Flag(StoryContract.MetJuan))
            {
                _host.ShowPlace("lanes_front", ("Go inside", () => _host.PlayScene(StoryContract.Knots.Coupons, ShowInside)));
                return;
            }
            _host.ShowPlace("lanes_front",
                ("Go inside", ShowInside),
                ("Go around back", _backLot.WalkAround));
        }

        /// <summary>
        /// Inside, at the lanes. On a short Tuesday between 6:30 and 7:30 the ask comes first,
        /// once. Then bowling, unless the league has the lanes and you're not subbing.
        /// </summary>
        /// <summary>
        /// The textures Juan's lanes are drawn with (Bowling, "Art"): the maple, the approach,
        /// the gutter and the masking panel. Any not in yet, the lane stands in for itself.
        /// </summary>
        static LaneArt LaneTextures() => new LaneArt
        {
            Boards = Art.Texture("lane_boards"),
            Approach = Art.Texture("lane_approach"),
            Gutter = Art.Texture("lane_gutter"),
            Masking = Art.Texture("lane_masking"),
        };

        void ShowInside()
        {
            var clock = _host.State.Clock;
            if (_host.Story.Flag(StoryContract.MetJuan) && ShortTonight(clock) && _host.Story.Int(StoryContract.SubAskedDay) != clock.Day)
            {
                _host.PlayScene(StoryContract.Knots.LeagueSubAsk, ShowInside);
                return;
            }
            var actions = new List<(string, UnityAction)>();
            if (SubbingTonight()) actions.Add(("Bowl for Chuy's team", Bowl));
            else if (!LeagueOn(clock)) actions.Add(("Bowl", Bowl));
            actions.Add(("Go to the counter", ShowCounter));
            actions.Add(("Watch the lanes", () => _host.ShowWait(Lanes, "Watch the lanes", 0, ShowInside)));
            actions.Add(("Step out back", _backLot.StepOut));
            actions.Add(("Step back outside", ShowOutside));
            _host.ShowPlace(Lanes, actions.ToArray());
        }

        bool SubbingTonight() => _host.Story.Int(StoryContract.SubTonightDay) == _host.State.Clock.Day;

        /// <summary>
        /// Juan's counter: him, and the balls and shoes for sale, each gone once bought (Mike,
        /// 2026-11-23). Only what you can pay for is on offer. Until Pockets is built, what you buy
        /// is a flag on the save that Pockets adopts when it lands.
        /// </summary>
        void ShowCounter()
        {
            var actions = new List<(string, UnityAction)>
            {
                ("Talk to Juan", () => _host.PlayScene(StoryContract.Knots.Juan, ShowCounter)),
            };
            int cash = _host.State.BankrollNow;
            foreach (var ball in _rules.BallsForSale(OwnedBall))
            {
                int price = _rules.PriceOf(ball);
                if (price <= cash) actions.Add(($"Buy the {ball.Name.ToLowerInvariant()}, {UiKit.Money(price)}", () => BuyBall(ball, price)));
            }
            if (!_host.Story.Flag(StoryContract.OwnsShoes) && _rules.ShoesPrice <= cash)
                actions.Add(($"Buy a pair of shoes, {UiKit.Money(_rules.ShoesPrice)}", BuyShoes));
            actions.Add(("Back to the lanes", ShowInside));
            _host.ShowPlace(Counter, actions.ToArray());
        }

        BowlingBall OwnedBall => BowlingBall.ById(_host.Story.Text(StoryContract.OwnsBall));

        void BuyBall(BowlingBall ball, int price)
        {
            _host.State.Spend(price);
            _host.Story.Set(StoryContract.OwnsBall, ball.Id);
            ShowCounter();
        }

        void BuyShoes()
        {
            _host.State.Spend(_rules.ShoesPrice);
            _host.Story.Set(StoryContract.OwnsShoes, true);
            ShowCounter();
        }

        // ---- The lane ----

        void Bowl() => _host.LeavePlaceScreen(SitDownNow);

        /// <summary>
        /// Onto the lane. Your first time, Juan bowls a frame with you, coached over the lane; a
        /// sub night goes straight to the team's first game.
        /// </summary>
        void SitDownNow()
        {
            _rules.Hand = BowlingHand.Current;
            var setup = Tonight(_host.State.Clock, SubbingTonight(), _rules);
            _subbing = setup.Match != null;
            setup.Ball = OwnedBall;
            setup.OwnShoes = _host.Story.Flag(StoryContract.OwnsShoes);
            bool first = !_subbing && !_host.Story.Flag(StoryContract.BowledAtLanes);
            if (first) setup.Practice = PracticeFrame.Juans();
            _lane.SitDown(_host.State, _host.Pace, setup);
            if (first) _host.PlaySceneOver(StoryContract.Knots.BowlFirst, _lane, () => { });
        }

        /// <summary>
        /// What the lane gets tonight: your lane's oil, worn by the balls since Juan oiled it at
        /// five; tonight's regulars (Chuy and two faces, by the night); and on a sub night the
        /// match, Chuy on your team.
        /// </summary>
        public static LaneSetup Tonight(GameClock clock, bool subbing, BowlingRules rules)
        {
            var pattern = LaneTonight(clock.Night) == 6 ? LanePattern.LaneSix : LanePattern.House;
            int balls = LaneWear.BallsSinceOiling(clock.Weekday, clock.MinuteOfDay);
            var faces = TonightsFaces(clock.Night, subbing ? 7 : 3);
            var setup = new LaneSetup
            {
                Oil = LaneOil.Worn(pattern, balls, rules.Hand),
                Seed = clock.Day * GameClock.MinutesPerDay + clock.MinuteOfDay,
            };
            if (!subbing)
            {
                setup.Regulars = new[]
                {
                    Bowler.WithAverage(faces[0].name, faces[0].id, ChuysAverage),
                    Bowler.WithAverage(faces[1].name, faces[1].id, OtherAverages[0]),
                    Bowler.WithAverage(faces[2].name, faces[2].id, OtherAverages[1]),
                };
                return setup;
            }
            var rng = new System.Random(clock.Day);
            var bowlers = new Bowler[faces.Length];
            for (int i = 0; i < faces.Length; i++)
                bowlers[i] = Bowler.WithAverage(faces[i].name, faces[i].id, i == 0 ? ChuysAverage : rng.Next(130, 191));
            setup.Match = new LeagueMatch(YourTeam, TheirTeam,
                new[] { bowlers[0], bowlers[1], bowlers[2] },
                new[] { bowlers[3], bowlers[4], bowlers[5], bowlers[6] },
                rules.LeagueGames, rules.SidePot);
            return setup;
        }

        /// <summary>Chuy first, then faces from the Lanes' crowd, by the night: (name, face id).</summary>
        static (string name, string id)[] TonightsFaces(int night, int count)
        {
            var roster = Art.Faces();
            var names = new Dictionary<string, string>();
            var patrons = new List<string>();
            foreach (var face in roster) names[face.id] = face.FirstName;
            foreach (var face in FaceRoster.PatronsAt(roster, PlaceId))
                if (face.id != Chuy) patrons.Add(face.id);
            string[] ids = patrons.Count + 1 >= count ? Crowd.Pick(night, PlaceId + "/lanes", new[] { Chuy }, patrons, count) : null;
            if (ids != null)
            {
                // Chuy's always on the lanes, and first: the main opponent, and the sub night's captain.
                int at = Array.IndexOf(ids, Chuy);
                (ids[0], ids[at]) = (ids[at], ids[0]);
            }
            var picked = new (string, string)[count];
            for (int i = 0; i < count; i++)
            {
                string id = ids != null ? ids[i] : i == 0 ? Chuy : "";
                string name = names.TryGetValue(id, out string n) && !string.IsNullOrEmpty(n) ? n : i == 0 ? "Chuy" : $"Bowler {i}";
                picked[i] = (name, id);
            }
            return picked;
        }

        /// <summary>
        /// Up from the lane: the night goes into the story (games, the best one, challenges won,
        /// a sub night finished), then the league's word or Juan's, then back inside.
        /// </summary>
        void OnGetUp(BreakPoint point)
        {
            var story = _host.Story;
            story.Set(StoryContract.Broke, _host.State.BankrollNow < _rules.LaneFee);
            story.Set(StoryContract.SittingNet, _lane.SittingNet);
            story.Set(StoryContract.GamesBowled, _lane.GamesBowled);
            story.Set(StoryContract.BestGame, _lane.BestGame);
            story.Set(StoryContract.ChallengesWonTonight, _lane.ChallengesWon);
            story.Set(StoryContract.ChallengesWon, story.Int(StoryContract.ChallengesWon) + _lane.ChallengesWon);
            if (_lane.GamesBowled > 0) story.Set(StoryContract.BowledAtLanes, true);

            if (!_subbing)
            {
                _host.PlayScene(StoryContract.Knots.BowlGetUp, ShowInside);
                return;
            }
            _subbing = false;
            story.Set(StoryContract.SubTonightDay, -1);
            story.Set(StoryContract.SubGamesWon, _lane.GamesWon);
            story.Set(StoryContract.SubFinished, _lane.FinishedNight);
            if (_lane.FinishedNight) story.Set(StoryContract.SubNights, story.Int(StoryContract.SubNights) + 1);
            bool shirts = _lane.FinishedNight && !story.Flag(StoryContract.ShirtsAsked);
            _host.PlayScene(StoryContract.Knots.LeagueSubDone,
                shirts ? () => _host.PlayScene(StoryContract.Knots.BowlShirts, ShowInside) : (Action)ShowInside);
        }
    }
}
