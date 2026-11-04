using System.Collections.Generic;
using NUnit.Framework;
using Starflare.Game;
using Starflare.State;

namespace Starflare.Tests
{
    public class ArtTests
    {
        static GameClock At(PartOfDay part, int day = 1) => new GameClock { Day = day, Part = part };

        // Day 1 is May 4 (spring); day 212 is Dec 1 (winter).
        const int WinterDay = 212;

        static System.Func<string, bool> Have(params string[] plates)
        {
            var set = new HashSet<string>(plates);
            return set.Contains;
        }

        [Test]
        public void DayAndNight_PickTheirOwnPlates()
        {
            var have = Have("kasia_bar_day", "kasia_bar_night");
            Assert.That(Art.PlateAt("kasia_bar", At(PartOfDay.Morning), have), Is.EqualTo("kasia_bar_day"));
            Assert.That(Art.PlateAt("kasia_bar", At(PartOfDay.Afternoon), have), Is.EqualTo("kasia_bar_day"));
            Assert.That(Art.PlateAt("kasia_bar", At(PartOfDay.Night), have), Is.EqualTo("kasia_bar_night"));
        }

        [Test]
        public void AViewWithOnlyANightPlate_ShowsItAllDay()
        {
            var have = Have("lanes_front_night");
            Assert.That(Art.PlateAt("lanes_front", At(PartOfDay.Morning), have), Is.EqualTo("lanes_front_night"));
        }

        [Test]
        public void Winter_UsesOneSnowPlate_ForAnyTimeOfDay()
        {
            var have = Have("kasia_bar_day", "kasia_bar_night", "kasia_bar_winter");
            Assert.That(Art.PlateAt("kasia_bar", At(PartOfDay.Morning, WinterDay), have), Is.EqualTo("kasia_bar_winter"));
            Assert.That(Art.PlateAt("kasia_bar", At(PartOfDay.Night, WinterDay), have), Is.EqualTo("kasia_bar_winter"));
        }

        [Test]
        public void Winter_PrefersASnowPlateForThisTimeOfDay()
        {
            var have = Have("kasia_front_day", "kasia_front_night", "kasia_front_day_winter", "kasia_front_night_winter", "kasia_front_winter");
            Assert.That(Art.PlateAt("kasia_front", At(PartOfDay.Night, WinterDay), have), Is.EqualTo("kasia_front_night_winter"));
        }

        [Test]
        public void Winter_WithoutASnowPlate_KeepsTheUsualOne()
        {
            var have = Have("motel_room_day", "motel_room_night");
            Assert.That(Art.PlateAt("motel_room", At(PartOfDay.Night, WinterDay), have), Is.EqualTo("motel_room_night"));
        }

        [Test]
        public void TheSnowPlate_IsOnlyForWinter()
        {
            var have = Have("kasia_bar_day", "kasia_bar_winter");
            Assert.That(Art.PlateAt("kasia_bar", At(PartOfDay.Morning), have), Is.EqualTo("kasia_bar_day"));
        }

        [TestCase("kasia_bar_night", "kasia_bar")]
        [TestCase("kasia_bar_winter", "kasia_bar")]
        [TestCase("kasia_bar_rain", "kasia_bar")]
        [TestCase("kasia_front_night_winter", "kasia_front")]
        [TestCase("motel_lot_day_rain", "motel_lot")]
        [TestCase("motel_river_night", "motel_river")]
        public void ViewOf_DropsWeatherSeasonAndTimeOfDay(string plate, string view) =>
            Assert.That(Art.ViewOf(plate), Is.EqualTo(view));
    }
}
