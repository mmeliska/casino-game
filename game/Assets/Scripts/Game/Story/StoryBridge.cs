using System;
using Ink.Runtime;
using Ink.UnityIntegration;
using Starflare.State;
using UnityEngine;

namespace Starflare.Game
{
    /// <summary>
    /// The one place the Ink story and the player state meet. It loads the story, restores its
    /// saved state, binds the story's external functions to the state (so the story spends,
    /// borrows, sleeps, works and leans through the rules in <see cref="PlayerState"/>), and pushes the
    /// shared variables before a scene. Everything goes by the names in <see cref="StoryContract"/>.
    /// </summary>
    public sealed class StoryBridge
    {
        const string StoryResource = "Story/northlight";

        /// <summary>The Ink runtime story, for the player that shows it.</summary>
        public Story Ink { get; }

        /// <summary>Builds the story from its compiled JSON and binds it to <paramref name="state"/>.</summary>
        public StoryBridge(string storyJson, string savedState, PlayerState state)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));
            Ink = new Story(storyJson);
            if (savedState != null)
            {
                try { Ink.state.LoadJson(savedState); }
                catch (Exception e) { Debug.LogWarning($"Couldn't read the story save, starting it fresh: {e.Message}"); }
            }

            // These change state, so Ink must never call them during lookahead (the false).
            Ink.BindExternalFunction(StoryContract.Externals.Spend, (int amount) => state.Spend(amount), false);
            Ink.BindExternalFunction(StoryContract.Externals.Earn, (int amount) => state.Earn(amount), false);
            Ink.BindExternalFunction(StoryContract.Externals.Borrow, (int amount) => state.Borrow(amount), false);
            Ink.BindExternalFunction(StoryContract.Externals.Repay, (int amount) => state.Repay(amount), false);
            Ink.BindExternalFunction(StoryContract.Externals.Sleep, () => state.Sleep(), false);
            Ink.BindExternalFunction(StoryContract.Externals.Work, (int pay) => state.WorkShift(pay), false);
            Ink.BindExternalFunction(StoryContract.Externals.Lean, (string pole) => state.Lean(pole), false);
            // Fails now, not in the middle of a scene, if the story declares an EXTERNAL we don't bind.
            Ink.ValidateExternalBindings();
        }

        /// <summary>Loads the game's story (<c>northlight.ink</c> and what it includes) from Resources.</summary>
        public static StoryBridge Load(string savedState, PlayerState state)
        {
            var file = Resources.Load<InkFile>(StoryResource);
            if (file == null || !file.isCompiled)
                throw new InvalidOperationException($"Ink story Resources/{StoryResource}.ink is missing or didn't compile.");
            return new StoryBridge(file.storyJson, savedState, state);
        }

        /// <summary>A story flag, by its <see cref="StoryContract"/> name.</summary>
        public bool Flag(string name) => (bool)Ink.variablesState[name];

        public void Set(string name, object value) => Ink.variablesState[name] = value;

        /// <summary>Brings the story's copy of the shared numbers up to date before a scene.</summary>
        public void Sync(PlayerState state)
        {
            Set(StoryContract.PlayerName, state.Name);
            Set(StoryContract.Bankroll, state.BankrollNow);
            Set(StoryContract.Book, state.Debts);
        }

        /// <summary>The story's state (flags, visit counts, where it is) for the save file.</summary>
        public string SaveJson() => Ink.state.ToJson();
    }
}
