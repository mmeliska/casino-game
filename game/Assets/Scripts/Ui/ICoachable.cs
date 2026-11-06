using System;
using UnityEngine;

namespace Starflare.Ui
{
    /// <summary>
    /// A game a teacher can coach over (writing.md's "# point:" tag): it names its controls, can
    /// let only one of them work, and says when you've used one. The story player points at a
    /// control and waits for it; the game never sees the story.
    /// </summary>
    public interface ICoachable
    {
        /// <summary>The control with this id, to highlight, or null if the game has none by that name.</summary>
        RectTransform Control(string id);

        /// <summary>
        /// Lets only the control with this id work: null for none locked (ordinary play), or ""
        /// for all locked while the teacher talks.
        /// </summary>
        void Lock(string id);

        /// <summary>Raised with a control's id after you've used it and the game has moved on.</summary>
        event Action<string> ActionTaken;
    }
}
