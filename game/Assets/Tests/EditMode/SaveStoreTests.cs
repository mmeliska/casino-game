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
            _store.Save(state);

            var loaded = _store.Load();
            Assert.That(loaded.Name, Is.EqualTo("Lou"));
            Assert.That(loaded.Box, Is.EqualTo(RegisterBox.Ms));
            Assert.That(loaded.BankrollNow, Is.EqualTo(220));
            Assert.That(loaded.Debts, Is.EqualTo(50));
            Assert.That(loaded.RoundsPlayed, Is.EqualTo(1));
            Assert.That(loaded.Place, Is.EqualTo("kasias-tap"));
            Assert.That(loaded.Clock.MinutesIntoPart, Is.EqualTo(10));
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
    }
}
