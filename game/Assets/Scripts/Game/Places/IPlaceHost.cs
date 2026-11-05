using System;
using Starflare.State;
using UnityEngine.Events;

namespace Starflare.Game
{
    /// <summary>What a place sees of the city: the state and story, and the shared screens.</summary>
    public interface IPlaceHost
    {
        PlayerState State { get; }
        StoryBridge Story { get; }

        /// <summary>Plays one knot with the shared variables up to date, saves, then calls back.</summary>
        void PlayScene(string knot, Action then);

        /// <summary>
        /// Plays one knot over a game the player can still see, so a teacher can coach it
        /// ("# point:" lines), then saves and calls back.
        /// </summary>
        void PlaySceneOver(string knot, ICoachable game, Action then);

        /// <summary>The place screen: the view's plate for the time of day, with these actions under the top bar.</summary>
        void ShowPlace(string view, params (string label, UnityAction action)[] actions);

        /// <summary>Hides the place screen and top bar, for a module that fills the screen.</summary>
        void LeavePlaceScreen();

        /// <summary>Opens the map on your phone.</summary>
        void OpenMap();

        /// <summary>Wipes the save and starts the game again.</summary>
        void StartOver();
    }
}
