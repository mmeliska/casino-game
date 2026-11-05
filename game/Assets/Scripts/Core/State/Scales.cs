using System;

namespace Starflare.State
{
    /// <summary>
    /// The four hidden scales: generous/tight, reliable/flaky, ruthless/soft and
    /// straight/crooked. Each is one number, up toward its first pole and down toward its
    /// second. They are never shown; the player feels them through how people react, which
    /// events open and how much credit Kasia gives. Straight/crooked is who you are, not heat.
    /// </summary>
    [Serializable]
    public sealed class Scales
    {
        public int Generous;
        public int Reliable;
        public int Ruthless;
        public int Straight;

        /// <summary>The pole names a scene leans by, as the story writes them.</summary>
        public static readonly string[] Poles =
        {
            "generous", "tight", "reliable", "flaky", "ruthless", "soft", "straight", "crooked",
        };

        /// <summary>One step toward <paramref name="pole"/>. Only <see cref="PlayerState.Lean"/> calls it.</summary>
        internal void Lean(string pole)
        {
            switch (pole)
            {
                case "generous": Generous++; break;
                case "tight": Generous--; break;
                case "reliable": Reliable++; break;
                case "flaky": Reliable--; break;
                case "ruthless": Ruthless++; break;
                case "soft": Ruthless--; break;
                case "straight": Straight++; break;
                case "crooked": Straight--; break;
                default: throw new ArgumentException($"'{pole}' isn't a pole of any scale.", nameof(pole));
            }
        }
    }
}
