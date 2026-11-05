using System;
using Starflare.State;

namespace Starflare.Game
{
    /// <summary>
    /// A named point in the story to test from: a fresh game brought there through the same
    /// <see cref="PlayerState"/> methods play uses, with the story's flags set as the beat leaves
    /// them. Each builds on the one before it, so a beat holds only its own changes. Recipes
    /// rather than saved files, so they keep working as the Ink changes; <c>CheckpointTests</c>
    /// plays every one against the compiled story.
    /// </summary>
    public sealed class Checkpoint
    {
        readonly Checkpoint _after;
        readonly Action<PlayerState, StoryBridge> _beat;

        /// <param name="after">The checkpoint this one starts from, or null for a new game.</param>
        /// <param name="place">Where you stand when it loads (a place id).</param>
        /// <param name="beat">This beat's own changes to the state and the story.</param>
        public Checkpoint(string name, Checkpoint after, string place, Action<PlayerState, StoryBridge> beat)
        {
            Name = name;
            _after = after;
            Place = place;
            _beat = beat;
        }

        public string Name { get; }
        public string Place { get; }

        /// <summary>Plays every beat up to this one onto a new game's state and story.</summary>
        public void Apply(PlayerState state, StoryBridge story)
        {
            _after?.Apply(state, story);
            _beat(state, story);
            state.MoveTo(Place);
        }

        public override string ToString() => Name;
    }
}
