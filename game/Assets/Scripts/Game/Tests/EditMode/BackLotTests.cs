using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Starflare.Game;
using Starflare.State;

namespace Starflare.Tests
{
    /// <summary>
    /// Millstone Lanes' back lot: the late game's hours (10 p.m. to 3 a.m.), and the crowd in the
    /// circle, picked by the night with at least one of Chuy, Nick and Teddy always there.
    /// </summary>
    public class BackLotTests
    {
        static readonly string[] Patrons = { "p1", "p2", "p3", "p4", "p5" };

        [TestCase(21, 59, false)]
        [TestCase(22, 0, true)]
        [TestCase(23, 30, true)]
        [TestCase(0, 0, true)]
        [TestCase(2, 59, true)]
        [TestCase(3, 0, false)]
        [TestCase(12, 0, false)]
        public void TheLateGame_RunsFromTenToThree(int hour, int minute, bool on)
        {
            var clock = new GameClock { MinuteOfDay = hour * 60 + minute };
            Assert.That(MillstoneBackLot.GameOn(clock), Is.EqualTo(on));
        }

        [Test]
        public void TheSameNight_ShowsTheSameCrowd()
        {
            var first = Crowd.Pick(12, MillstoneLanes.PlaceId, MillstoneBackLot.Regulars, Patrons, 3);
            var again = Crowd.Pick(12, MillstoneLanes.PlaceId, MillstoneBackLot.Regulars, Patrons, 3);
            Assert.That(again, Is.EqualTo(first));
        }

        [Test]
        public void EveryNight_HasARegular_AndThreeDifferentFaces()
        {
            var seen = new HashSet<string>();
            for (int night = 1; night <= 300; night++)
            {
                var crowd = Crowd.Pick(night, MillstoneLanes.PlaceId, MillstoneBackLot.Regulars, Patrons, 3);
                Assert.That(crowd, Has.Length.EqualTo(3));
                Assert.That(crowd, Is.Unique, $"night {night}");
                Assert.That(crowd.Intersect(MillstoneBackLot.Regulars), Is.Not.Empty, $"night {night} has none of the regulars");
                Assert.That(crowd.All(id => MillstoneBackLot.Regulars.Contains(id) || Patrons.Contains(id)), Is.True);
                seen.UnionWith(crowd);
            }
            Assert.That(seen, Is.EquivalentTo(MillstoneBackLot.Regulars.Concat(Patrons)), "over enough nights, everyone turns up");
        }

        [Test]
        public void TooFewFaces_ForTheSeats_IsAnError() =>
            Assert.That(() => Crowd.Pick(1, MillstoneLanes.PlaceId, new[] { "r" }, new[] { "p" }, 3), Throws.ArgumentException);

        [Test]
        public void TheRoster_ReadsItsFaces_AndWhereTheyTurnUp()
        {
            var faces = FaceRoster.Parse(
                "{\"about\": \"x\", \"avatars\": [" +
                "{\"id\": \"a\", \"name\": \"Marcus Bell\", \"type\": \"patron\", \"venues\": [\"millstone-lanes\"]}," +
                "{\"id\": \"b\", \"name\": \"Priya Desai\", \"type\": \"patron\", \"venues\": [\"the-flare\"]}," +
                "{\"id\": \"c\", \"name\": \"Pat\", \"type\": \"worker\", \"venues\": [\"millstone-lanes\"]}," +
                "{\"id\": \"d\", \"name\": null, \"type\": \"patron\", \"venues\": [\"millstone-lanes\"]}]}");
            Assert.That(faces, Has.Length.EqualTo(4));
            Assert.That(faces[0].FirstName, Is.EqualTo("Marcus"));
            Assert.That(FaceRoster.PatronsAt(faces, MillstoneLanes.PlaceId).Select(f => f.id), Is.EqualTo(new[] { "a" }),
                "patrons only, named, and only where they turn up");
        }

        /// <summary>The game's own roster: Chuy, Nick and Teddy are in it, and the back lot draws only from faces tagged for it.</summary>
        [Test]
        public void TheBackLotCrowd_ComesFromTheRoster()
        {
            var roster = FaceRoster.Parse(File.ReadAllText(Path.Combine(UnityEngine.Application.dataPath, "Resources", "Faces", "faces.json")));
            var byId = roster.ToDictionary(f => f.id);
            Assert.That(MillstoneBackLot.Regulars.Select(id => byId[id].name), Is.EqualTo(new[] { "Chuy", "Nick", "Teddy" }));

            for (int night = 1; night <= 50; night++)
                foreach (string id in MillstoneBackLot.CrowdFor(night, roster, 3))
                    Assert.That(byId[id].TurnsUpAt(MillstoneLanes.PlaceId), Is.True, $"{byId[id].name} doesn't turn up at the Lanes");
        }
    }
}
