using System;
using System.Collections;
using System.IO;
using NUnit.Framework;
using Starflare.Game;
using Starflare.State;
using UnityEngine;
using UnityEngine.TestTools;

namespace Starflare.Tests
{
    /// <summary>
    /// Smoke tests for the city booting: into the arrival on a new game, and back to the place
    /// screen on a saved one. They use their own save folder so a real save is never touched.
    /// </summary>
    public class CityBootTests
    {
        string _folder;
        GameObject _city;

        [SetUp]
        public void UseATempSaveFolder()
        {
            _folder = Path.Combine(Path.GetTempPath(), "starflare-playmode-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_folder);
            City.SaveFolder = _folder;
        }

        [TearDown]
        public void CleanUp()
        {
            if (_city != null) UnityEngine.Object.Destroy(_city);
            City.SaveFolder = null;
            Directory.Delete(_folder, recursive: true);
        }

        [UnityTest]
        public IEnumerator ANewGame_OpensOnTheArrival()
        {
            var city = Boot();
            yield return null;

            Assert.That(city.State.HasSigned, Is.False);
            Assert.That(city.GetComponent<StoryPlayer>().IsPlaying, Is.True, "the arrival scene should be playing");
            Assert.That(PlaceScreen(city).activeSelf, Is.False);
        }

        [UnityTest]
        public IEnumerator ACheckedInSave_OpensOnTheMotelLot()
        {
            // A saved night: signed in, and the story knows you've checked in.
            var saves = new SaveStore(_folder);
            var state = new PlayerState();
            state.SignRegister("Lou", RegisterBox.Ms);
            saves.Save(state);
            var story = StoryBridge.Load(null, state);
            story.Set(StoryContract.CheckedIn, true);
            saves.SaveStory(story.SaveJson());

            var city = Boot();
            yield return null;

            Assert.That(city.State.Name, Is.EqualTo("Lou"));
            Assert.That(city.State.Place, Is.EqualTo(SlackWaterMotel.PlaceId));
            Assert.That(city.GetComponent<StoryPlayer>().IsPlaying, Is.False);
            Assert.That(PlaceScreen(city).activeSelf, Is.True, "the motel lot should be showing");
        }

        [UnityTest]
        public IEnumerator ASettledChange_IsSavedAtOnce()
        {
            var city = Boot();
            yield return null;

            city.State.Earn(25);
            var reloaded = new SaveStore(_folder).Load();
            Assert.That(reloaded.BankrollNow, Is.EqualTo(PlayerState.StartingBankroll + 25));
        }

        [UnityTest]
        public IEnumerator TheDebugPanel_OpensAndDrawsEverySection()
        {
            var city = Boot();
            yield return null;

            var panel = city.GetComponent<DebugPanel>();
            Assert.That(panel, Is.Not.Null, "the editor build should carry the debug panel");
            panel.Open();
            city.State.Earn(10);
            yield return null;
            Assert.That(panel.IsOpen, Is.True);
        }

        City Boot()
        {
            _city = new GameObject("City", typeof(City));
            return _city.GetComponent<City>();
        }

        static GameObject PlaceScreen(City city) => city.transform.Find("Place Canvas").gameObject;
    }
}
