using NUnit.Framework;
using Starflare.Ui;
using UnityEngine;

namespace Starflare.Tests
{
    public class OrientationLayoutTests
    {
        GameObject _root;

        [TearDown]
        public void CleanUp()
        {
            if (_root != null) Object.Destroy(_root);
        }

        [Test]
        public void OnTurn_RearrangesOnceNow_ForTheCurrentOrientation()
        {
            _root = new GameObject("Root");
            var canvas = UiKit.MakeCanvas(_root.transform, "Canvas", 0);

            int calls = 0;
            bool? landscape = null;
            UiKit.OnTurn(canvas, isLandscape => { calls++; landscape = isLandscape; });

            Assert.That(calls, Is.EqualTo(1));
            Assert.That(landscape, Is.EqualTo(OrientationLayout.IsLandscape));
        }
    }
}
