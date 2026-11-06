using System;
using Starflare.State;
using Starflare.Ui;
using UnityEngine.Events;

namespace Starflare.Game
{
    /// <summary>What a place sees of the city: the state and story, and the shared screens and scenes.</summary>
    public interface IPlaceHost
    {
        PlayerState State { get; }
        StoryBridge Story { get; }

        /// <summary>How fast play moves the clock; the debug panel tunes it.</summary>
        ClockPace Pace { get; }

        /// <summary>Plays one knot with the shared variables up to date, then calls back; the playthrough saves as it ends.</summary>
        void PlayScene(string knot, Action then);

        /// <summary>
        /// Plays one knot over a game the player can still see, so a teacher can coach it
        /// ("# point:" lines), then calls back; the playthrough saves as it ends.
        /// </summary>
        void PlaySceneOver(string knot, ICoachable game, Action then);

        /// <summary>The place screen: the view's plate for the time of day, with these actions under the top bar.</summary>
        void ShowPlace(string view, params (string label, UnityAction action)[] actions);

        /// <summary>
        /// Dips to black, hides the place screen and top bar, and opens the module that fills the
        /// screen (<paramref name="then"/>) while it's dark.
        /// </summary>
        void LeavePlaceScreen(Action then);

        /// <summary>
        /// Passing time without playing, the place's own way (a nap, a beer): an hour, or until the
        /// next part of the day, for <paramref name="price"/> each time. Then <paramref name="then"/>,
        /// at the new time, which is a break point; or straight back if you don't.
        /// </summary>
        void ShowWait(string view, string how, int price, Action then);

        /// <summary>Opens the map on your phone.</summary>
        void OpenMap();

        /// <summary>Asks the playthrough to wipe the save and start the game again.</summary>
        void StartOver();
    }
}
