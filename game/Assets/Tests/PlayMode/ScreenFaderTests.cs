using System.Collections;
using NUnit.Framework;
using Starflare.Game;
using UnityEngine;
using UnityEngine.TestTools;

namespace Starflare.Tests
{
    /// <summary>
    /// The dip to black between screens: the swap happens only once the screen is dark, so it's
    /// never seen, and a swap asked for from inside another runs in the same dark.
    /// </summary>
    public class ScreenFaderTests
    {
        GameObject _go;

        [TearDown]
        public void CleanUp()
        {
            if (_go != null) Object.Destroy(_go);
        }

        [Test]
        public void ItStartsDark_SoTheFirstScreenSwapsInAtOnce()
        {
            var fader = Make();
            bool swapped = false;
            fader.Cover(() => swapped = true);
            Assert.That(swapped, Is.True);
        }

        [UnityTest]
        public IEnumerator OnceClear_TheSwapWaitsForTheDark()
        {
            var fader = Make();
            yield return UntilIdle(fader);

            bool swapped = false, inner = false;
            fader.Cover(() =>
            {
                swapped = true;
                fader.Cover(() => inner = true, away: true);
                Assert.That(inner, Is.True, "a swap from inside a swap runs in the same dark");
            });
            Assert.That(swapped, Is.False, "the screen isn't dark yet");
            Assert.That(fader.IsBusy, Is.True);

            for (int frame = 0; frame < 300 && !swapped; frame++) yield return null;
            Assert.That(swapped, Is.True);
            yield return UntilIdle(fader);
        }

        ScreenFader Make()
        {
            _go = new GameObject("Fader", typeof(ScreenFader));
            return _go.GetComponent<ScreenFader>();
        }

        static IEnumerator UntilIdle(ScreenFader fader)
        {
            for (int frame = 0; frame < 300 && fader.IsBusy; frame++) yield return null;
            Assert.That(fader.IsBusy, Is.False, "the fade should finish");
        }
    }
}
