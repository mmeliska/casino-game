namespace Starflare.Game
{
    /// <summary>
    /// The Slack Water Motel, where you live: the lot, with Dot's office and your room off it.
    /// The arrival and check-in (A1.1) are the city's, since they happen before you have a place.
    /// </summary>
    public sealed class SlackWaterMotel : IPlace
    {
        /// <summary>Also the default <c>PlayerState.Place</c>, since you start here.</summary>
        public const string PlaceId = "slack-water";

        readonly IPlaceHost _host;

        public SlackWaterMotel(IPlaceHost host) => _host = host;

        public string Id => PlaceId;
        public string Name => "Slack Water Motel";
        public PhoneMap.Place Pin => new PhoneMap.Place(PlaceId, Name, 0.45f, 0.32f);
        public bool IsOpen => true;

        public void Arrive() => ShowLot();

        public void Leave() { }

        void ShowLot()
        {
            _host.ShowPlace("motel_lot",
                ("Step into the office", () => _host.PlayScene(StoryContract.Knots.Office, ShowLot)),
                ("Go to your room", () => _host.PlayScene(StoryContract.Knots.TurnIn, ShowLot)),
                ("Start over (wipe save)", _host.StartOver));
        }
    }
}
