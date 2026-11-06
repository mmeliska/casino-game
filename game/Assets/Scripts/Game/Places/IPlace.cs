using Starflare.Phone;

namespace Starflare.Game
{
    /// <summary>
    /// A place on Northlight's map: its pin, whether it's open to you yet, and what happens when
    /// you arrive. Each place owns its own screens and scene triggers, through the
    /// <see cref="IPlaceHost"/> it was built with (in <see cref="PlaceList"/>); the city only
    /// moves you between them.
    /// </summary>
    public interface IPlace
    {
        /// <summary>The place id, as on the docs' place pages and in <c>PlayerState.Place</c>.</summary>
        string Id { get; }

        string Name { get; }

        /// <summary>Its pin on the phone's map (from the pin table on the Northlight page).</summary>
        PhoneMap.Place Pin { get; }

        /// <summary>Whether the map shows it yet.</summary>
        bool IsOpen { get; }

        /// <summary>You've just arrived: show the first screen.</summary>
        void Arrive();

        /// <summary>You're heading somewhere else: put away anything of your own that's up.</summary>
        void Leave();
    }
}
