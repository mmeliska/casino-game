using NUnit.Framework;
using Starflare.Phone;
using UnityEngine;

namespace Starflare.Tests
{
    /// <summary>Where the map opens: on your pin, with every open place still in view.</summary>
    public sealed class PhoneMapFramingTests
    {
        static readonly Vector2 Motel = new(0.45f, 0.32f), Lanes = new(0.22f, 0.44f), Flare = new(0.475f, 0.41f);

        [Test]
        public void WithYourPin_ItCentresOnYou_AndReachesTheFarthestPlace()
        {
            var min = Vector2.Min(Vector2.Min(Motel, Lanes), Flare);
            var max = Vector2.Max(Vector2.Max(Motel, Lanes), Flare);

            var (centre, span) = PhoneMap.Framing(min, max, Flare);

            Assert.That(centre, Is.EqualTo(Flare));
            // Millstone Lanes is the farthest across, the motel the farthest up.
            Assert.That(span.x, Is.EqualTo(2 * (Flare.x - Lanes.x)).Within(1e-5f));
            Assert.That(span.y, Is.EqualTo(2 * (Flare.y - Motel.y)).Within(1e-5f));
        }

        [Test]
        public void WithoutYourPin_ItCentresOnTheOpenPlaces()
        {
            Vector2 min = Vector2.Min(Lanes, Motel), max = Vector2.Max(Lanes, Motel);

            var (centre, span) = PhoneMap.Framing(min, max, null);

            Assert.That(Vector2.Distance(centre, (min + max) / 2), Is.LessThan(1e-5f));
            Assert.That(Vector2.Distance(span, max - min), Is.LessThan(1e-5f));
        }

        [Test]
        public void AloneOnTheMap_ItStillHasASpanToZoomTo()
        {
            var (centre, span) = PhoneMap.Framing(Flare, Flare, Flare);

            Assert.That(centre, Is.EqualTo(Flare));
            Assert.That(span.x, Is.GreaterThan(0));
            Assert.That(span.y, Is.GreaterThan(0));
        }
    }
}
