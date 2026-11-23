using System.Collections.Generic;
using UnityEngine;

namespace Starflare.Scratchers
{
    /// <summary>
    /// A ticket's colours and its pictures, kept apart from its rules the way a slot machine's
    /// look is: the ticket says what's printed, the look says how it's printed. With art, the
    /// name, the play area and the odds are printed on the card's flat colour below its
    /// picture; without, the counter prints the whole ticket itself, a two-colour sweep with
    /// its name on top.
    /// </summary>
    public sealed class TicketLook
    {
        /// <summary>
        /// The ticket's paper at the top and at the bottom, swept between when there's no art.
        /// With art, <see cref="Bottom"/> is the card's flat colour, what the foot is printed on.
        /// </summary>
        public Color Top { get; set; } = new Color(0.3f, 0.3f, 0.36f);
        public Color Bottom { get; set; } = new Color(0.1f, 0.1f, 0.14f);

        /// <summary>The border, the price badge and the play area's edge.</summary>
        public Color Accent { get; set; } = new Color(0.95f, 0.76f, 0.25f);

        public Color TitleInk { get; set; } = Color.white;
        public Color TitleOutline { get; set; } = new Color(0.05f, 0.04f, 0.04f);

        /// <summary>The play area behind the spots, and what's printed in it.</summary>
        public Color Panel { get; set; } = new Color(0.96f, 0.95f, 0.9f);
        public Color PanelInk { get; set; } = new Color(0.1f, 0.08f, 0.08f);

        /// <summary>The latex you scratch, silver unless the ticket's is tinted.</summary>
        public Color Coat { get; set; } = new Color(0.76f, 0.77f, 0.8f);

        /// <summary>
        /// The portrait card (Resources/Scratchers/{id}): a picture across the top and flat
        /// colour below it, where the counter prints the rest. Cut from the drop folder by
        /// tools/import_tickets.py, which also measures <see cref="Header"/>.
        /// </summary>
        public Sprite Art { get; set; }

        /// <summary>The landscape card (Resources/Scratchers/{id}_wide), for a phone on its side.</summary>
        public Sprite WideArt { get; set; }

        /// <summary>How far down each card its picture runs, as a share of its height: nothing is printed above it.</summary>
        public float Header { get; set; } = 0.36f;
        public float WideHeader { get; set; } = 0.46f;

        /// <summary>
        /// Pictures for the symbols under the latex, by symbol as the ticket prints it (Gold
        /// Rush's GOLD and TIN, Lucky 7s' 7 and CHERRY): the picture shows with its word small
        /// beneath, as real tickets caption theirs. A symbol without one is printed as its word.
        /// </summary>
        public Dictionary<string, Sprite> Symbols { get; } = new Dictionary<string, Sprite>();

        /// <summary>
        /// Pictures over your numbers, one to a spot in turn (Ice Fishing's perch, walleye, pike
        /// and bass), by name: the art is Scratchers/Symbols/{id}_{name}. Their loaded sprites
        /// are <see cref="Marks"/>, a missing one null.
        /// </summary>
        public string[] MarkNames { get; set; } = new string[0];
        public List<Sprite> Marks { get; } = new List<Sprite>();

        /// <summary>The picture over their number (Ice Fishing's lunker, Photo Finish's favorite).</summary>
        public string KeyMarkName { get; set; } = "";
        public Sprite KeyMark { get; set; }

        /// <summary>A picture printed faintly behind every amount or number (Penny Jar's penny), for tickets whose spots are only those.</summary>
        public string BackingName { get; set; } = "";
        public Sprite Backing { get; set; }

        /// <summary>The picture over your spot at this place on the ticket, or null.</summary>
        public Sprite MarkFor(int spot) => Marks.Count == 0 ? null : Marks[spot % Marks.Count];

        /// <summary>Print on the price badge: dark on a light badge, white on a dark one.</summary>
        public Color BadgeInk => InkOn(Accent);

        /// <summary>Print along the ticket's foot (the odds, the top prize), over the bottom of the sweep.</summary>
        public Color FootInk => InkOn(Bottom);

        /// <summary>Near-black or white, whichever reads on this colour.</summary>
        public static Color InkOn(Color background) =>
            0.299f * background.r + 0.587f * background.g + 0.114f * background.b > 0.55f
                ? new Color(0.06f, 0.05f, 0.05f)
                : Color.white;

        static Color C(float r, float g, float b) => new Color(r, g, b);

        /// <summary>
        /// Bridgehead's roll, by ticket id (<see cref="ScratchTicket.Bridgehead"/>). The headers
        /// and the flat colours (<see cref="Bottom"/>) are measured off Mike's art
        /// (Images/LotteryTickets, 2026-11-23) by tools/import_tickets.py.
        /// </summary>
        public static Dictionary<string, TicketLook> Bridgehead() => new Dictionary<string, TicketLook>
        {
            ["lucky7s"] = new TicketLook
            {
                Top = C(0.9f, 0.16f, 0.1f), Bottom = C(0.73f, 0.01f, 0.02f), Accent = C(0.98f, 0.8f, 0.2f),
                TitleInk = C(1f, 0.86f, 0.3f), Panel = C(0.99f, 0.96f, 0.86f), PanelInk = C(0.45f, 0.03f, 0.06f),
                Header = 0.331f, WideHeader = 0.446f,
            },
            ["pennyjar"] = new TicketLook
            {
                Top = C(0.86f, 0.52f, 0.26f), Bottom = C(0.45f, 0.12f, 0.03f), Accent = C(1f, 0.84f, 0.56f),
                TitleInk = C(1f, 0.95f, 0.84f), Panel = C(0.98f, 0.93f, 0.85f), PanelInk = C(0.36f, 0.17f, 0.06f),
                Coat = C(0.78f, 0.6f, 0.46f), Header = 0.336f, WideHeader = 0.42f, BackingName = "penny",
            },
            ["icefishing"] = new TicketLook
            {
                Top = C(0.62f, 0.86f, 0.98f), Bottom = C(0.26f, 0.75f, 0.96f), Accent = C(0.08f, 0.28f, 0.6f),
                TitleInk = Color.white, TitleOutline = C(0.04f, 0.14f, 0.34f), Panel = C(0.93f, 0.97f, 1f), PanelInk = C(0.05f, 0.18f, 0.4f),
                Coat = C(0.74f, 0.8f, 0.88f), Header = 0.295f, WideHeader = 0.492f,
                MarkNames = new[] { "perch", "walleye", "pike", "bass" }, KeyMarkName = "lunker",
            },
            ["lakeeffect"] = new TicketLook
            {
                Top = C(0.86f, 0.93f, 1f), Bottom = C(0.55f, 0.83f, 0.88f), Accent = C(0.16f, 0.58f, 0.3f),
                TitleInk = C(0.06f, 0.2f, 0.46f), TitleOutline = Color.white, Panel = Color.white, PanelInk = C(0.06f, 0.2f, 0.46f),
                Header = 0.381f, WideHeader = 0.457f, BackingName = "snowflake",
            },
            ["northernlights"] = new TicketLook
            {
                Top = C(0.2f, 0.12f, 0.4f), Bottom = C(0.05f, 0.66f, 0.18f), Accent = C(0.62f, 1f, 0.82f),
                TitleInk = Color.white, TitleOutline = C(0.03f, 0.2f, 0.06f), Panel = C(0.95f, 0.95f, 1f), PanelInk = C(0.22f, 0.06f, 0.36f),
                Header = 0.355f, WideHeader = 0.463f, BackingName = "star",
            },
            ["photofinish"] = new TicketLook
            {
                Top = C(0.22f, 0.56f, 0.26f), Bottom = C(0.01f, 0.4f, 0.11f), Accent = C(0.96f, 0.84f, 0.3f),
                TitleInk = Color.white, Panel = C(0.99f, 0.97f, 0.9f), PanelInk = C(0.08f, 0.24f, 0.1f),
                Header = 0.381f, WideHeader = 0.489f,
                MarkNames = new[] { "red", "blue", "green", "yellow", "purple", "orange" }, KeyMarkName = "favorite",
            },
            ["goldrush"] = new TicketLook
            {
                Top = C(0.5f, 0.3f, 0.1f), Bottom = C(0.77f, 0.42f, 0.01f), Accent = C(1f, 0.9f, 0.5f),
                TitleInk = C(0.3f, 0.15f, 0.03f), TitleOutline = C(1f, 0.93f, 0.66f), Panel = C(0.99f, 0.95f, 0.84f), PanelInk = C(0.32f, 0.17f, 0.04f),
                Coat = C(0.86f, 0.72f, 0.36f), Header = 0.349f, WideHeader = 0.465f,
            },
            ["oreboat"] = new TicketLook
            {
                Top = C(0.5f, 0.25f, 0.45f), Bottom = C(0.61f, 0.09f, 0.06f), Accent = C(0.96f, 0.6f, 0.24f),
                TitleInk = Color.white, TitleOutline = C(0.07f, 0.11f, 0.26f), Panel = C(0.95f, 0.93f, 0.9f), PanelInk = C(0.07f, 0.11f, 0.26f),
                Header = 0.391f, WideHeader = 0.471f, BackingName = "wheel",
            },
            ["diamonddeluxe"] = new TicketLook
            {
                Top = C(0.06f, 0.08f, 0.12f), Bottom = C(0f, 0.84f, 0.93f), Accent = C(0.04f, 0.05f, 0.08f),
                TitleInk = C(0.03f, 0.06f, 0.1f), TitleOutline = Color.white, Panel = C(0.94f, 0.97f, 1f), PanelInk = C(0.04f, 0.05f, 0.08f),
                Coat = C(0.82f, 0.84f, 0.9f), Header = 0.352f, WideHeader = 0.468f, BackingName = "diamond",
            },
            ["cashblizzard"] = new TicketLook
            {
                Top = C(0.9f, 0.96f, 1f), Bottom = C(0.04f, 0.6f, 0.06f), Accent = C(0.95f, 0.97f, 1f),
                TitleInk = Color.white, TitleOutline = C(0.02f, 0.2f, 0.05f), Panel = Color.white, PanelInk = C(0.04f, 0.26f, 0.14f),
                Header = 0.328f, WideHeader = 0.403f, BackingName = "banknote",
            },
        };
    }
}
