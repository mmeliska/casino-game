using Starflare.Bowling;
using UnityEngine;

namespace Starflare.Game
{
    /// <summary>
    /// Which hand you bowl with: a device setting, not a story thing (Mike, 2026-11-23), kept like
    /// the clock pace. Left mirrors the lane, the hook and the worn track. Set from the debug
    /// panel until there's a settings screen.
    /// </summary>
    public static class BowlingHand
    {
        const string Key = "settings.bowling.hand";

        public static Hand Current
        {
            get => PlayerPrefs.GetInt(Key, (int)Hand.Right) == (int)Hand.Left ? Hand.Left : Hand.Right;
            set
            {
                PlayerPrefs.SetInt(Key, (int)value);
                PlayerPrefs.Save();
            }
        }
    }
}
