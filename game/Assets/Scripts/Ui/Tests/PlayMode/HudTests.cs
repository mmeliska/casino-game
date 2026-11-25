using System.Collections;
using NUnit.Framework;
using Starflare.State;
using Starflare.Ui;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;

namespace Starflare.Tests
{
    /// <summary>
    /// The top bar: the phone glow (Dot sending you to the map on night one), and the backing
    /// behind the place's name and date, which runs up to the cash pill however wide that is.
    /// </summary>
    public class HudTests
    {
        GameObject _go;
        PlayerState _state;

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

        [UnityTest]
        public IEnumerator TheBacking_FollowsThePill_WhenItNarrows()
        {
            var hud = Make();
            hud.Show("Millstone Lanes", buttons: true);
            _state.Borrow(9800); // "$10,000" and "Owed $9,800": a wide pill
            for (int i = 0; i < 3; i++) yield return null;

            _state.Repay(9800); // back to "$200", and the pill shrinks to the right
            for (int i = 0; i < 3; i++) yield return null;

            var pill = Named(hud, "Cash").GetComponent<RectTransform>();
            var backing = Named(hud, "Backing").GetComponent<RectTransform>();
            Assert.That(Left(pill) - Right(backing), Is.EqualTo(16).Within(1), "the backing stops just short of the pill");

            var date = Named(hud, "Backing").transform.parent.GetComponentsInChildren<TMP_Text>(true);
            foreach (var text in date)
                Assert.That(Right(text.rectTransform), Is.LessThanOrEqualTo(Right(backing)), $"'{text.text}' sits on the backing");
        }

        static float Left(RectTransform rect) => rect.TransformPoint(new Vector3(rect.rect.xMin, 0, 0)).x;

        static float Right(RectTransform rect) => rect.TransformPoint(new Vector3(rect.rect.xMax, 0, 0)).x;

        Hud Make()
        {
            _go = new GameObject("Hud");
            var hud = _go.AddComponent<Hud>();
            _state = new PlayerState();
            hud.Init(_state, () => { }, () => { }, () => { });
            return hud;
        }

        static GameObject Named(Hud hud, string name)
        {
            foreach (var t in hud.GetComponentsInChildren<Transform>(includeInactive: true))
                if (t.name == name) return t.gameObject;
            Assert.Fail($"The bar has no {name}.");
            return null;
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
