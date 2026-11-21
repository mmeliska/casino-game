using Starflare.Phone;

namespace Starflare.Game
{
    /// <summary>
    /// Millstone Lanes, Juan's bowling alley. Kasia's coupons put it on the map and are how you
    /// first walk in (A1.3); after that Juan's at the counter, and the steel door opens on the
    /// back lot and his late game (<see cref="MillstoneBackLot"/>). The licensed machines inside
    /// pay real money, like Kasia's (Mike, 2026-11-20), and aren't built yet.
    /// </summary>
    public sealed class MillstoneLanes : IPlace
    {
        public const string PlaceId = "millstone-lanes";

        readonly IPlaceHost _host;
        readonly MillstoneBackLot _backLot;

        public MillstoneLanes(IPlaceHost host, MillstoneBackLot backLot)
        {
            _host = host;
            _backLot = backLot;
            _backLot.Init(host, ShowInside, ShowOutside);
        }

        public string Id => PlaceId;
        public string Name => "Millstone Lanes";
        public PhoneMap.Place Pin => new PhoneMap.Place(PlaceId, Name, 0.22f, 0.44f);

        /// <summary>Opens once Kasia's given you the box to carry.</summary>
        public bool IsOpen => _host.Story.Flag(StoryContract.CarryingCoupons) || _host.Story.Flag(StoryContract.MetJuan);

        /// <summary>You arrive in the lot, under the sign.</summary>
        public void Arrive() => ShowOutside();

        public void Leave() { }

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

        void ShowInside()
        {
            _host.ShowPlace("lanes_lanes",
                ("Find Juan at the counter", () => _host.PlayScene(StoryContract.Knots.Juan, ShowInside)),
                ("Watch the lanes", () => _host.ShowWait("lanes_lanes", "Watch the lanes", 0, ShowInside)),
                ("Step out back", _backLot.StepOut),
                ("Step back outside", ShowOutside));
        }
    }
}
