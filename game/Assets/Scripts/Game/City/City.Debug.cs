using System;
using System.Collections.Generic;
using Starflare.State;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Starflare.Game
{
    /// <summary>
    /// The city's side of the debug panel, kept out of <c>City.cs</c>. Only the Editor and
    /// Development builds open it (<c>Debug.isDebugBuild</c>); a release build carries the code unused.
    /// The city owns the save, so everything that writes it whole is here: each writes the save
    /// files, then boots the scene again from them, the way a player's next launch would.
    /// </summary>
    public sealed partial class City : IDebugHost
    {
        IReadOnlyList<IPlace> IDebugHost.Places => _places;

        IReadOnlyList<string> IDebugHost.Snapshots => _saves.Snapshots();

        partial void AddDebugPanel()
        {
            if (Debug.isDebugBuild) gameObject.AddComponent<DebugPanel>().Init(this);
        }

        void IDebugHost.Reload()
        {
            SaveAll();
            Boot();
        }

        void IDebugHost.Restart(Checkpoint checkpoint)
        {
            var state = new PlayerState();
            var story = StoryBridge.Load(null, state);
            checkpoint.Apply(state, story);
            _saves.Save(state);
            _saves.SaveStory(story.SaveJson());
            Boot();
        }

        /// <summary>Named for when it was taken and where you were, so the list reads last first.</summary>
        void IDebugHost.TakeSnapshot()
        {
            SaveAll();
            _saves.TakeSnapshot($"{DateTime.Now:yyyy-MM-dd HH.mm.ss}  {Find(State.Place).Name}, day {State.Clock.Day}, {UiKit.Money(State.BankrollNow)}");
        }

        void IDebugHost.LoadSnapshot(string name)
        {
            _saves.RestoreSnapshot(name);
            Boot();
        }

        void IDebugHost.DeleteSnapshot(string name) => _saves.DeleteSnapshot(name);

        /// <summary>Stops saving first, so nothing writes the old state over the files just written.</summary>
        void Boot()
        {
            State.Changed -= SaveAll;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
