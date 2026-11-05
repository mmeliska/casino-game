using System.Collections.Generic;
using Starflare.State;

namespace Starflare.Game
{
    /// <summary>
    /// What the debug panel sees of the city. The panel edits the live state and story through
    /// their own methods; anything that writes the save files whole (a checkpoint, a snapshot, a
    /// reload) goes through the city, which owns the save, and boots the game again from it.
    /// </summary>
    public interface IDebugHost
    {
        PlayerState State { get; }
        StoryBridge Story { get; }

        /// <summary>Every place built so far, open on the map or not.</summary>
        IReadOnlyList<IPlace> Places { get; }

        /// <summary>Saves, then boots the game again from the save, so every screen reads the edited state.</summary>
        void Reload();

        /// <summary>Starts a new game brought to <paramref name="checkpoint"/>.</summary>
        void Restart(Checkpoint checkpoint);

        IReadOnlyList<string> Snapshots { get; }
        void TakeSnapshot();
        void LoadSnapshot(string name);
        void DeleteSnapshot(string name);

        /// <summary>Wipes the save and starts the game again.</summary>
        void StartOver();
    }
}
