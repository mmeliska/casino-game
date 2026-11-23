using NUnit.Framework;
using Starflare.State;
using Starflare.Ui;
using UnityEngine;

namespace Starflare.Tests
{
    /// <summary>
    /// The top bar's phone glow (Dot sending you to the map on night one): it shows with the
    /// phone, steps away with it during a scene, and stays on until it's turned off.
    /// </summary>
    public class HudTests
    {
        GameObject _go;

        [TearDown]
        public void CleanUp()
        {
            if (_go != null) Object.Destroy(_go);
        }

        [Test]
        public void PointingAtThePhone_GlowsWhileThePhoneShows()
        {
            var hud = Make();
            hud.Show("Slack Water Motel", buttons: true);
            Assert.That(Glow(hud).activeInHierarchy, Is.False);

            hud.PointAtPhone(true);
            Assert.That(Glow(hud).activeInHierarchy, Is.True);

            hud.Show("Slack Water Motel", buttons: false);
            Assert.That(Glow(hud).activeInHierarchy, Is.False, "a scene hides the phone, and its glow");

            hud.Show("Slack Water Motel", buttons: true);
            Assert.That(Glow(hud).activeInHierarchy, Is.True, "still pointing on the next place screen");

            hud.PointAtPhone(false);
            Assert.That(hud.PointingAtPhone, Is.False);
            Assert.That(Glow(hud).activeInHierarchy, Is.False);
        }

        Hud Make()
        {
            _go = new GameObject("Hud");
            var hud = _go.AddComponent<Hud>();
            hud.Init(new PlayerState(), () => { }, () => { }, () => { });
            return hud;
        }

        static GameObject Glow(Hud hud)
        {
            foreach (var t in hud.GetComponentsInChildren<Transform>(includeInactive: true))
                if (t.name == "Phone Glow") return t.gameObject;
            Assert.Fail("The phone button has no glow.");
            return null;
        }
    }
}
