using UnityEngine;

namespace Starflare.VideoPoker
{
    /// <summary>
    /// A machine's colours, kept in one place so the look can change without touching the
    /// layout. <see cref="Classic"/> is the bar-top machine: the whole screen blue, amber
    /// paytable, yellow buttons with dark lettering. A venue can hand its own look to
    /// <see cref="VideoPokerMachine.Init"/>.
    /// </summary>
    public sealed class VideoPokerLook
    {
        /// <summary>Fills the whole screen behind the paytable, cards and buttons.</summary>
        public Color Screen { get; set; } = new Color(0.04f, 0.07f, 0.3f);

        /// <summary>The stripe behind the paytable column for the current bet.</summary>
        public Color LitColumn { get; set; } = new Color(0.12f, 0.2f, 0.6f);

        public Color PaytableText { get; set; } = new Color(1f, 0.78f, 0.35f);
        public Color Button { get; set; } = new Color(1f, 0.85f, 0.2f);
        public Color ButtonText { get; set; } = new Color(0.1f, 0.08f, 0.05f);
        public Color CardBack { get; set; } = new Color(0.45f, 0.08f, 0.1f);

        public static VideoPokerLook Classic => new VideoPokerLook();
    }
}
