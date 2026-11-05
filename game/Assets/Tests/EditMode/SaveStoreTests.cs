using System;
using System.IO;
using NUnit.Framework;
using Starflare.Game;
using Starflare.State;

namespace Starflare.Tests
{
    public class SaveStoreTests
    {
        string _folder;
        SaveStore _store;

        [SetUp]
        public void MakeFolder()
        {
            _folder = Path.Combine(Path.GetTempPath(), "starflare-tests-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_folder);
            _store = new SaveStore(_folder);
        }

        [TearDown]
        public void RemoveFolder() => Directory.Delete(_folder, recursive: true);

        [Test]
        public void NoFile_StartsANewGame()
        {
            var state = _store.Load();
            Assert.That(state.BankrollNow, Is.EqualTo(PlayerState.StartingBankroll));
            Assert.That(state.Version, Is.EqualTo(PlayerState.CurrentVersion));
        }

        [Test]
        public void SaveThenLoad_KeepsTheState()
        {
            var state = new PlayerState();
            state.SignRegister("Lou", RegisterBox.Ms);
            state.Borrow(50);
            state.ApplyRoundResult(-30, 10);
            state.MoveTo("kasias-tap");
            state.Lean("crooked");
            _store.Save(state);

            var loaded = _store.Load();
            Assert.That(loaded.Name, Is.EqualTo("Lou"));
            Assert.That(loaded.Box, Is.EqualTo(RegisterBox.Ms));
            Assert.That(loaded.BankrollNow, Is.EqualTo(220));
            Assert.That(loaded.Debts, Is.EqualTo(50));
            Assert.That(loaded.RoundsPlayed, Is.EqualTo(1));
            Assert.That(loaded.Place, Is.EqualTo("kasias-tap"));
            Assert.That(loaded.Clock.TimeLabel, Is.EqualTo("18:10"));
            Assert.That(loaded.Scales.Straight, Is.EqualTo(-1));
            Assert.That(loaded.Version, Is.EqualTo(PlayerState.CurrentVersion));
        }

        [Test]
        public void StorySavesBesideTheState()
        {
            Assert.That(_store.LoadStory(), Is.Null);
            _store.SaveStory("{\"flags\":1}");
            Assert.That(_store.LoadStory(), Is.EqualTo("{\"flags\":1}"));
            _store.Wipe();
            Assert.That(_store.LoadStory(), Is.Null);
            Assert.That(File.Exists(Path.Combine(_folder, "save.json")), Is.False);
        }

        /// <summary>A save from before the Version field (the first prototype builds) reads as version 1.</summary>
        [Test]
        public void VersionZero_MigratesForward()
        {
            const string versionZero = @"{
                ""BankrollNow"": 175,
                ""BestBankroll"": 260,
                ""Debts"": 50,
                ""RoundsPlayed"": 12,
                ""Name"": ""Pat"",
                ""Box"": 1,
                ""Place"": ""millstone-lanes"",
                ""Clock"": { ""Day"": 3, ""Part"": 2, ""MinutesIntoPart"": 40 }
            }";
            var state = SaveStore.Parse(versionZero);
            Assert.That(state.Version, Is.EqualTo(PlayerState.CurrentVersion));
            Assert.That(state.BankrollNow, Is.EqualTo(175));
            Assert.That(state.BestBankroll, Is.EqualTo(260));
            Assert.That(state.Debts, Is.EqualTo(50));
            Assert.That(state.Name, Is.EqualTo("Pat"));
            Assert.That(state.Box, Is.EqualTo(RegisterBox.Mr));
            Assert.That(state.Place, Is.EqualTo("millstone-lanes"));
            Assert.That(state.Clock.Day, Is.EqualTo(3));
            Assert.That(state.Clock.Part, Is.EqualTo(PartOfDay.Night));
            Assert.That(state.Clock.TimeLabel, Is.EqualTo("18:40"));
        }

        /// <summary>A version 1 save had no hidden scales; it reads them as level.</summary>
        [Test]
        public void VersionOne_MigratesForward_WithLevelScales()
        {
            const string versionOne = @"{
                ""Version"": 1,
                ""BankrollNow"": 240,
                ""BestBankroll"": 260,
                ""Debts"": 0,
                ""RoundsPlayed"": 30,
                ""Name"": ""Pat"",
                ""Box"": 2,
                ""Place"": ""kasias-tap"",
                ""Clock"": { ""Day"": 4, ""Part"": 0, ""MinutesIntoPart"": 90 }
            }";
            var state = SaveStore.Parse(versionOne);
            Assert.That(state.Version, Is.EqualTo(PlayerState.CurrentVersion));
            Assert.That(state.BankrollNow, Is.EqualTo(240));
            Assert.That(state.Box, Is.EqualTo(RegisterBox.Ms));
            Assert.That(state.Clock.Day, Is.EqualTo(4));
            Assert.That(state.Clock.TimeLabel, Is.EqualTo("7:30"));
            Assert.That(state.Scales, Is.Not.Null);
            Assert.That(state.Scales.Straight, Is.EqualTo(0));
            Assert.That(state.Scales.Reliable, Is.EqualTo(0));
        }

        /// <summary>
        /// A version 2 save kept a part of the day and minutes into it; version 3 keeps minutes
        /// since midnight. A night played to its end was "late": midnight, the next day.
        /// </summary>
        [TestCase(0, 90, 5, "7:30", PartOfDay.Morning)]
        [TestCase(1, 0, 5, "12:00", PartOfDay.Afternoon)]
        [TestCase(2, 200, 5, "21:20", PartOfDay.Night)]
        [TestCase(2, 360, 6, "0:00", PartOfDay.BeforeDawn)]
        public void VersionTwo_MigratesTheClock(int part, int minutesIntoPart, int day, string time, PartOfDay newPart)
        {
            string versionTwo = @"{
                ""Version"": 2,
                ""BankrollNow"": 310,
                ""BestBankroll"": 320,
                ""Debts"": 20,
                ""RoundsPlayed"": 44,
                ""Name"": ""Pat"",
                ""Box"": 3,
                ""Place"": ""kasias-tap"",
                ""Clock"": { ""Day"": 5, ""Part"": " + part + @", ""MinutesIntoPart"": " + minutesIntoPart + @" },
                ""Scales"": { ""Generous"": 1, ""Reliable"": -1, ""Ruthless"": 0, ""Straight"": 2 }
            }";
            var state = SaveStore.Parse(versionTwo);
            Assert.That(state.Version, Is.EqualTo(PlayerState.CurrentVersion));
            Assert.That(state.Clock.Day, Is.EqualTo(day));
            Assert.That(state.Clock.TimeLabel, Is.EqualTo(time));
            Assert.That(state.Clock.Part, Is.EqualTo(newPart));
            Assert.That(state.BankrollNow, Is.EqualTo(310));
            Assert.That(state.Box, Is.EqualTo(RegisterBox.PreferNotToAnswer));
            Assert.That(state.Place, Is.EqualTo("kasias-tap"));
            Assert.That(state.Scales.Straight, Is.EqualTo(2));
            Assert.That(state.Scales.Reliable, Is.EqualTo(-1));
        }

        [Test]
        public void ANewerSave_IsRefused()
        {
            string future = "{\"Version\": " + (PlayerState.CurrentVersion + 1) + ", \"BankrollNow\": 5}";
            Assert.Throws<FormatException>(() => SaveStore.Parse(future));
        }

        [Test]
        public void AnUnreadableFile_StartsFresh()
        {
            File.WriteAllText(Path.Combine(_folder, "save.json"), "not json at all");
            UnityEngine.TestTools.LogAssert.Expect(UnityEngine.LogType.Warning, new System.Text.RegularExpressions.Regex("Couldn't read the save"));
            var state = _store.Load();
            Assert.That(state.BankrollNow, Is.EqualTo(PlayerState.StartingBankroll));
        }

        [Test]
        public void LoadedState_RaisesChanged()
        {
            _store.Save(new PlayerState());
            var state = _store.Load();
            int changes = 0;
            state.Changed += () => changes++;
            state.Earn(5);
            Assert.That(changes, Is.EqualTo(1));
        }

        [Test]
        public void ASnapshot_ComesBackAfterTheGameMovesOn()
        {
            var state = new PlayerState();
            state.SignRegister("Lou", RegisterBox.Ms);
            _store.Save(state);
            _store.SaveStory("{\"flags\":1}");
            _store.TakeSnapshot("before");

            state.Earn(500);
            _store.Save(state);
            _store.SaveStory("{\"flags\":2}");
            _store.RestoreSnapshot("before");

            Assert.That(_store.Load().BankrollNow, Is.EqualTo(PlayerState.StartingBankroll));
            Assert.That(_store.LoadStory(), Is.EqualTo("{\"flags\":1}"));
        }

        [Test]
        public void Snapshots_OutliveStartingOver_AndListLastFirst()
        {
            _store.Save(new PlayerState());
            _store.TakeSnapshot("2026-11-05 09.00.00 first");
            _store.TakeSnapshot("2026-11-05 10.00.00 second");
            _store.Wipe();

            Assert.That(_store.Snapshots(), Is.EqualTo(new[] { "2026-11-05 10.00.00 second", "2026-11-05 09.00.00 first" }));
            _store.DeleteSnapshot("2026-11-05 10.00.00 second");
            Assert.That(_store.Snapshots(), Is.EqualTo(new[] { "2026-11-05 09.00.00 first" }));
        }

        [Test]
        public void ASnapshotTakenBeforeTheStory_RestoresWithoutOne()
        {
            _store.Save(new PlayerState());
            _store.TakeSnapshot("no story");
            _store.SaveStory("{}");
            _store.RestoreSnapshot("no story");
            Assert.That(_store.LoadStory(), Is.Null);
        }

        [Test]
        public void ASnapshotName_CantLeaveTheSnapshotsFolder()
        {
            Assert.Throws<ArgumentException>(() => _store.TakeSnapshot("../save"));
            Assert.Throws<ArgumentException>(() => _store.TakeSnapshot(""));
        }
    }
}
