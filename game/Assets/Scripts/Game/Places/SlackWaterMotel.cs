using Starflare.Phone;
using Starflare.State;
using UnityEngine.Events;

namespace Starflare.Game
{
    /// <summary>
    /// The Slack Water Motel, where you live: the lot, with Dot's office and your room off it.
    /// A new game opens here on the arrival and check-in (A1.1): the arrival scene, Dot's
    /// register, then her question; a save from after the register picks up at the question.
    /// Your room is where you pass time: sleep, the TV, the internet, each for the hours you pick
    /// (Mike, 2026-11-05). Night one you can't sleep, which is how you end up at Kasia's (A1.2).
    /// </summary>
    public sealed class SlackWaterMotel : IPlace
    {
        /// <summary>Also the default <c>PlayerState.Place</c>, since you start here.</summary>
        public const string PlaceId = "slack-water";

        static readonly int[] SleepHours = { 1, 2, 4, 8 };
        static readonly int[] ScreenHours = { 1, 2, 3 };

        readonly IPlaceHost _host;
        readonly RegisterCard _register;

        public SlackWaterMotel(IPlaceHost host, RegisterCard register)
        {
            _host = host;
            _register = register;
        }

        public string Id => PlaceId;
        public string Name => "Slack Water Motel";
        public PhoneMap.Place Pin => new PhoneMap.Place(PlaceId, Name, 0.45f, 0.32f);
        public bool IsOpen => true;

        public void Arrive()
        {
            if (_host.Story.Flag(StoryContract.CheckedIn)) ShowLot();
            else CheckIn();
        }

        public void Leave() { }

        /// <summary>Arrival, the register, then Dot's question. Picks up after the register if you'd signed it.</summary>
        void CheckIn()
        {
            if (_host.State.HasSigned) _host.PlayScene(StoryContract.Knots.CheckingIn, ShowLot);
            else _host.PlayScene(StoryContract.Knots.Arrival, () => _register.Show(SignRegister));
        }

        void SignRegister(string name, RegisterBox box)
        {
            _host.State.SignRegister(name, box);
            _host.PlayScene(StoryContract.Knots.CheckingIn, ShowLot);
        }

        void ShowLot()
        {
            _host.ShowPlace("motel_lot",
                ("Step into the office", () => _host.PlayScene(StoryContract.Knots.Office, ShowLot)),
                ("Go to your room", GoToRoom),
                ("Start over (wipe save)", _host.StartOver));
        }

        void GoToRoom()
        {
            if (!_host.Story.Flag(StoryContract.MetKasia)) _host.PlayScene(StoryContract.Knots.TurnIn, ShowLot);
            else ShowRoom();
        }

        void ShowRoom()
        {
            _host.ShowPlace("motel_room",
                ("Sleep", () => ChooseHours("Sleep", SleepHours)),
                ("Watch TV", () => ChooseHours("Watch TV", ScreenHours)),
                ("Browse the internet", () => ChooseHours("Browse the internet", ScreenHours)),
                ("Step outside", ShowLot));
        }

        /// <summary>How long: the clock moves on that many hours, and you're back in the room at the new time.</summary>
        void ChooseHours(string what, int[] hours)
        {
            var actions = new (string, UnityAction)[hours.Length + 1];
            for (int i = 0; i < hours.Length; i++)
            {
                int h = hours[i];
                actions[i] = ($"{what}, {h} {(h == 1 ? "hour" : "hours")}", () =>
                {
                    _host.State.PassTime(h * 60);
                    ShowRoom();
                });
            }
            actions[hours.Length] = ("Never mind", ShowRoom);
            _host.ShowPlace("motel_room", actions);
        }
    }
}
