using UnityEngine;

namespace Starflare.Game
{
    /// <summary>
    /// Every place built so far, in one list, so adding a place is a line here and the city
    /// doesn't grow. A place that needs components of its own (a machine, Dot's register) gets
    /// them on <paramref name="root"/>, the playthrough's object.
    /// </summary>
    public static class PlaceList
    {
        public static IPlace[] Build(IPlaceHost host, GameObject root)
        {
            var tap = root.AddComponent<KasiasTap>();
            tap.Init(host);
            var bridgehead = root.AddComponent<Bridgehead>();
            bridgehead.Init(host);
            return new IPlace[]
            {
                new SlackWaterMotel(host, root.AddComponent<RegisterCard>()),
                tap,
                new MillstoneLanes(host),
                bridgehead,
            };
        }
    }
}
