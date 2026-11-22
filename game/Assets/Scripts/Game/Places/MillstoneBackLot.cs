using System;
using System.Collections.Generic;
using Starflare.Craps;
using Starflare.State;
using UnityEngine;

namespace Starflare.Game
{
    /// <summary>
    /// Millstone Lanes' back lot: a chalk circle under the lean-to by the steel door, where Juan
    /// runs the late game after the last frame, 10 p.m. to 3 a.m. (Mike, 2026-11-20). You can step
    /// out the steel door from the lanes, or walk around back from the lot out front, any time;
    /// outside those hours it's the empty lot. Your first time in the circle, Juan
    /// coaches a practice roll on set dice, his money and nothing counting; from then on it's
    /// real money and fair dice. The table never sees the story or counts heat; this lot does.
    /// </summary>
    public sealed class MillstoneBackLot : MonoBehaviour
    {
        /// <summary>The late game opens at 10 p.m. ...</summary>
        public const int OpensAt = 22 * 60;

        /// <summary>... and Juan picks up the chalk at 3 a.m.</summary>
        public const int ClosesAt = 3 * 60;

        /// <summary>
        /// Hanging out by the back door: a few minutes at a time, so turning up at 9:55 gets you
        /// to the game at ten, and nobody waits out a whole day in a parking lot (Mike, 2026-11-21).
        /// </summary>
        static readonly int[] HangOutMinutes = { 5, 15, 30 };

        /// <summary>The back lot is an illegal game: a heat point the nights you step into the circle (Mike, 2026-11-20).</summary>
        const int HeatPerNight = 1;

        /// <summary>
        /// Chuy the league bowler, Nick the cab driver and Teddy the line cook: at least one of
        /// them is in the circle every night (Mike, 2026-11-21).
        /// </summary>
        public static readonly string[] Regulars = { "avatars-7_r1c1", "avatars-7_r1c2", "avatars-7_r2c1" };

        /// <summary>
        /// Juan's practice roll: a come-out 6 sets the point, a 6 makes it, a come-out 10 sets a
        /// new one, and a 7 ends the turn and passes the dice. $20 of his, pretend.
        /// </summary>
        static PracticeTurn JuansPracticeRoll() =>
            new PracticeTurn(20, new Roll(3, 3), new Roll(5, 1), new Roll(5, 5), new Roll(4, 3));

        IPlaceHost _host;
        CrapsTable _table;
        Action _backInside, _backOutFront;

        /// <summary>You walked around from the front, so you leave the same way; else back through the steel door.</summary>
        bool _cameAround;
        int _crowdNight = -1;

        /// <summary>The late game is on: from 10 p.m. to 3 a.m.</summary>
        public static bool GameOn(GameClock clock) => clock.MinuteOfDay >= OpensAt || clock.MinuteOfDay < ClosesAt;

        /// <summary>Tonight's three in the circle (with you), as face ids, from the roster.</summary>
        public static string[] CrowdFor(int night, IEnumerable<CrowdFace> roster, int seats)
        {
            var patrons = new List<string>();
            foreach (var face in FaceRoster.PatronsAt(roster, MillstoneLanes.PlaceId)) patrons.Add(face.id);
            return Crowd.Pick(night, MillstoneLanes.PlaceId, Regulars, patrons, seats);
        }

        public void Init(IPlaceHost host, Action backInside, Action backOutFront)
        {
            _host = host;
            _backInside = backInside;
            _backOutFront = backOutFront;
            _table = gameObject.AddComponent<CrapsTable>();
            var rules = new CrapsRules();
            // Tonight's crowd stands in when you step into the circle, once the clock is known.
            var waiting = new Regular[rules.Seats - 1];
            for (int i = 0; i < waiting.Length; i++) waiting[i] = new Regular("Regular");
            _table.Init(rules, null, Art.Focus("lanes_backlot"), waiting);
            _table.SetCircleArt(
                Circle("lanes_circle_night", new Rect(0.064f, 0.526f, 0.869f, 0.124f), wallFoot: 0.719f),
                Circle("lanes_circle_night_wide", new Rect(0.001f, 0.071f, 0.434f, 0.369f), wallFoot: 0.655f));
            _table.BreakPointReached += OnBreakPoint;
        }

        /// <summary>
        /// The chalk circle under the bulb, painted close up for the table, one picture for each way
        /// up the screen. The circle and the foot of the brick wall are measured off the pictures,
        /// so the dice land in the chalk and go off the bricks; null if a picture isn't in yet.
        /// </summary>
        static CircleArt Circle(string plate, Rect circle, float wallFoot)
        {
            var sprite = Art.Plate(plate);
            return sprite == null ? null : new CircleArt(sprite, circle, wallFoot);
        }

        /// <summary>Out the steel door from the lanes: Juan and the circle if the late game's on, else the empty lot.</summary>
        public void StepOut()
        {
            _cameAround = false;
            Enter();
        }

        /// <summary>Around the side of the building from the lot out front: the same back lot.</summary>
        public void WalkAround()
        {
            _cameAround = true;
            Enter();
        }

        void Enter()
        {
            if (GameOn(_host.State.Clock)) PlayScene(StoryContract.Knots.BackLot, Show);
            else _host.PlayScene(StoryContract.Knots.BackLotEmpty, Show);
        }

        void Show()
        {
            var actions = new List<(string, UnityEngine.Events.UnityAction)>();
            if (GameOn(_host.State.Clock)) actions.Add(("Step into the circle", SitDown));
            else actions.Add(("Hang out", ShowHangOut));
            if (_cameAround) actions.Add(("Back around front", () => _backOutFront()));
            else actions.Add(("Back inside", () => _backInside()));
            _host.ShowPlace("lanes_backlot", actions.ToArray());
        }

        void ShowHangOut()
        {
            var actions = new List<(string, UnityEngine.Events.UnityAction)>();
            foreach (int minutes in HangOutMinutes)
                actions.Add(($"Hang out {minutes} minutes", () =>
                {
                    _host.State.PassTime(minutes);
                    AfterWaiting();
                }));
            actions.Add(("Never mind", Show));
            _host.ShowPlace("lanes_backlot", actions.ToArray());
        }

        /// <summary>Waited into the late game: Juan's out with the chalk. Otherwise, still the empty lot.</summary>
        void AfterWaiting()
        {
            if (GameOn(_host.State.Clock)) Enter();
            else Show();
        }

        void SitDown() => _host.LeavePlaceScreen(SitDownNow);

        /// <summary>Into the circle. The first time, Juan hands you his practice roll and coaches it.</summary>
        void SitDownNow()
        {
            var clock = _host.State.Clock;
            if (clock.Night != _crowdNight) _table.SetRegulars(TonightsCrowd(_table.Rules.Seats - 1));
            string plate = Art.PlateAt("lanes_backlot", clock);
            _table.SetBackdrop(Art.Plate(plate), Art.Focus(plate));

            _host.State.PlayIllegalGame(MillstoneLanes.PlaceId, HeatPerNight);
            if (_host.Story.Flag(StoryContract.RolledWithJuan))
            {
                _table.SitDown(_host.State, _host.Pace);
                return;
            }
            _table.SitDown(_host.State, _host.Pace, JuansPracticeRoll());
            _host.PlaySceneOver(StoryContract.Knots.BackLotPractice, _table, () => { });
        }

        Regular[] TonightsCrowd(int count)
        {
            _crowdNight = _host.State.Clock.Night;
            var roster = Art.Faces();
            var names = new Dictionary<string, string>();
            foreach (var face in roster) names[face.id] = face.FirstName;

            var ids = CrowdFor(_crowdNight, roster, count);
            var crowd = new Regular[count];
            for (int i = 0; i < count; i++)
                crowd[i] = new Regular(names.TryGetValue(ids[i], out string name) && name.Length > 0 ? name : "Regular", Art.Face(ids[i]));
            return crowd;
        }

        void OnBreakPoint(BreakPoint point) => PlayScene(StoryContract.Knots.BackLotGetUp, Show);

        void PlayScene(string knot, Action then)
        {
            _host.Story.Set(StoryContract.Broke, _host.State.BankrollNow < _table.Rules.MinBet);
            _host.Story.Set(StoryContract.SittingNet, _table.SittingNet);
            _host.PlayScene(knot, then);
        }
    }
}
