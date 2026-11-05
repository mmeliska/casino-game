namespace Starflare.Game
{
    /// <summary>
    /// Millstone Lanes, Juan's bowling alley. Kasia's coupons put it on the map and are how you
    /// first walk in (A1.3); after that Juan's at the counter. Its free-play machines and the
    /// late game out back aren't built yet.
    /// </summary>
    public sealed class MillstoneLanes : IPlace
    {
        public const string PlaceId = "millstone-lanes";

        readonly IPlaceHost _host;

        public MillstoneLanes(IPlaceHost host) => _host = host;

        public string Id => PlaceId;
        public string Name => "Millstone Lanes";
        public PhoneMap.Place Pin => new PhoneMap.Place(PlaceId, Name, 0.22f, 0.44f);

        /// <summary>Opens once Kasia's given you the box to carry.</summary>
        public bool IsOpen => _host.Story.Flag(StoryContract.CarryingCoupons) || _host.Story.Flag(StoryContract.MetJuan);

        /// <summary>You arrive in the lot, under the sign.</summary>
        public void Arrive() => ShowOutside();

        public void Leave() { }

        void ShowOutside()
        {
            _host.ShowPlace("lanes_front",
                ("Go inside", () =>
                {
                    if (!_host.Story.Flag(StoryContract.MetJuan)) _host.PlayScene(StoryContract.Knots.Coupons, ShowInside);
                    else ShowInside();
                }));
        }

        void ShowInside()
        {
            _host.ShowPlace("lanes_lanes",
                ("Find Juan at the counter", () => _host.PlayScene(StoryContract.Knots.Juan, ShowInside)),
                ("Watch the lanes", () => _host.ShowWait("lanes_lanes", "Watch the lanes", 0, ShowInside)),
                ("Step back outside", ShowOutside));
        }
    }
}
