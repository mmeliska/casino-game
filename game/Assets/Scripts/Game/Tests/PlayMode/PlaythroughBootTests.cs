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
    /// Smoke tests for a playthrough booting: into the arrival on a new game, and back to the place
    /// screen on a saved one. They use their own save folder so a real save is never touched.
    /// </summary>
    public class PlaythroughBootTests
    {
        string _folder;
        GameObject _root;

        [SetUp]
        public void UseATempSaveFolder()
        {
            _folder = Path.Combine(Path.GetTempPath(), "starflare-playmode-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_folder);
            Playthrough.SaveFolder = _folder;
        }

        [TearDown]
        public void CleanUp()
        {
            if (_root != null) UnityEngine.Object.Destroy(_root);
            Playthrough.SaveFolder = null;
            Directory.Delete(_folder, recursive: true);
        }

        [UnityTest]
        public IEnumerator ANewGame_OpensOnTheArrival()
        {
            var game = Boot();
            yield return null;

            Assert.That(game.State.HasSigned, Is.False);
            Assert.That(game.GetComponent<StoryPlayer>().IsPlaying, Is.True, "the arrival scene should be playing");
            Assert.That(PlaceScreen(game).activeSelf, Is.False);
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

            var game = Boot();
            yield return null;

            Assert.That(game.State.Name, Is.EqualTo("Lou"));
            Assert.That(game.State.Place, Is.EqualTo(SlackWaterMotel.PlaceId));
            Assert.That(game.GetComponent<StoryPlayer>().IsPlaying, Is.False);
            Assert.That(PlaceScreen(game).activeSelf, Is.True, "the motel lot should be showing");
        }

        [UnityTest]
        public IEnumerator ASettledChange_IsSavedAtOnce()
        {
            var game = Boot();
            yield return null;

            game.State.Earn(25);
            var reloaded = new SaveStore(_folder).Load();
            Assert.That(reloaded.BankrollNow, Is.EqualTo(PlayerState.StartingBankroll + 25));
        }

        [UnityTest]
        public IEnumerator TheDebugPanel_OpensAndDrawsEverySection()
        {
            var game = Boot();
            yield return null;

            var panel = game.GetComponent<DebugPanel>();
            Assert.That(panel, Is.Not.Null, "the editor build should carry the debug panel");
            panel.Open();
            game.State.Earn(10);
            yield return null;
            Assert.That(panel.IsOpen, Is.True);
        }

        [UnityTest]
        public IEnumerator PullingIntoBridgeheadTheFirstTime_PlaysTheStopOnTheWayOver()
        {
            SaveAt(Checkpoints.OverTheBridge);
            var game = Boot();
            yield return null;

            Assert.That(game.State.Place, Is.EqualTo(Bridgehead.PlaceId));
            Assert.That(game.GetComponent<StoryPlayer>().IsPlaying, Is.True, "Mitch's first scene should be playing");
            Assert.That(game.GetComponent<Bridgehead>().IsOpen, Is.True);
        }

        [UnityTest]
        public IEnumerator ANightAfterMeetingMitch_PlaysTheTruckerOnce()
        {
            SaveAt(Checkpoints.TruckerAtThePumps);
            var game = Boot();
            yield return null;

            Assert.That(game.GetComponent<StoryPlayer>().IsPlaying, Is.True, "the trucker should be at the pumps");
        }

        /// <summary>A save at this checkpoint, the way the debug panel leaves one.</summary>
        void SaveAt(Checkpoint checkpoint)
        {
            var saves = new SaveStore(_folder);
            var state = new PlayerState();
            var story = StoryBridge.Load(null, state);
            checkpoint.Apply(state, story);
            saves.Save(state);
            saves.SaveStory(story.SaveJson());
        }

        Playthrough Boot()
        {
            _root = new GameObject("Playthrough", typeof(Playthrough));
            return _root.GetComponent<Playthrough>();
        }

        static GameObject PlaceScreen(Playthrough game) => game.transform.Find("Place Canvas").gameObject;
    }
}
