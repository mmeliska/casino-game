using System;
using System.Collections.Generic;

namespace Starflare.Game
{
    /// <summary>
    /// Who's at a venue's game tonight, picked from the crowd roster by the night, so the same
    /// night always shows the same crowd and a reload doesn't reshuffle it. A venue can name its
    /// regulars: at least one of them is always there (Mike, 2026-11-21, for Chuy, Nick and Teddy
    /// in the Millstone back lot), and the other seats go to the rest of them and the patrons who
    /// turn up there.
    /// </summary>
    public static class Crowd
    {
        /// <summary>
        /// <paramref name="seats"/> different face ids for <paramref name="night"/> at
        /// <paramref name="placeId"/>, in seat order: one of <paramref name="regulars"/>, and the
        /// rest drawn from the other regulars and <paramref name="patrons"/>.
        /// </summary>
        public static string[] Pick(int night, string placeId, IReadOnlyList<string> regulars, IReadOnlyList<string> patrons, int seats)
        {
            if (seats < 1) throw new ArgumentOutOfRangeException(nameof(seats));
            if (regulars == null || regulars.Count == 0) throw new ArgumentException("A crowd needs at least one regular.", nameof(regulars));

            var random = new Random(Seed(night, placeId));
            string anchor = regulars[random.Next(regulars.Count)];

            var others = new List<string>();
            foreach (string id in regulars) Add(others, id, anchor);
            if (patrons != null) foreach (string id in patrons) Add(others, id, anchor);
            if (others.Count < seats - 1) throw new ArgumentException($"Not enough faces for {seats} seats.", nameof(patrons));

            // Shuffle the others, take what's needed, and stand the regular in one of the seats.
            for (int i = others.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (others[i], others[j]) = (others[j], others[i]);
            }
            var crowd = others.GetRange(0, seats - 1);
            crowd.Insert(random.Next(seats), anchor);
            return crowd.ToArray();
        }

        static void Add(List<string> into, string id, string anchor)
        {
            if (!string.IsNullOrEmpty(id) && id != anchor && !into.Contains(id)) into.Add(id);
        }

        /// <summary>A seed from the night and place that doesn't depend on the runtime's string hashing.</summary>
        static int Seed(int night, string placeId)
        {
            unchecked
            {
                int hash = 17;
                foreach (char c in placeId ?? "") hash = hash * 31 + c;
                return hash * 397 ^ night;
            }
        }
    }
}
