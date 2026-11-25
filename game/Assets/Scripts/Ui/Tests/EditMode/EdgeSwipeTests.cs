using NUnit.Framework;
using Starflare.Ui;
using UnityEngine;

namespace Starflare.Tests
{
    /// <summary>The debug panel's swipe up from the bottom edge: only from the edge, only upward, and it follows the finger.</summary>
    public sealed class EdgeSwipeTests
    {
        const float Edge = 100, Screen = 2000, Sheet = 1600;

        static EdgeSwipe.Outcome Step(EdgeSwipe swipe, bool pressed, float x, float y) =>
            swipe.Step(pressed, new Vector2(x, y), Edge, Screen, Sheet);

        [Test]
        public void ASwipeFromTheEdge_PullsTheSheetUpUnderTheFinger()
        {
            var swipe = new EdgeSwipe();
            Assert.That(Step(swipe, true, 500, 40), Is.EqualTo(EdgeSwipe.Outcome.None));
            Assert.That(Step(swipe, true, 500, 440), Is.EqualTo(EdgeSwipe.Outcome.Dragging));
            Assert.That(swipe.Progress, Is.EqualTo(0.25f).Within(0.001f));
            Assert.That(Step(swipe, true, 500, 840), Is.EqualTo(EdgeSwipe.Outcome.Dragging));
            Assert.That(swipe.Progress, Is.EqualTo(0.5f).Within(0.001f));
            Assert.That(Step(swipe, false, 500, 840), Is.EqualTo(EdgeSwipe.Outcome.Open));
        }

        [Test]
        public void AShortFlickUp_OpensTheSheet()
        {
            var swipe = new EdgeSwipe();
            Step(swipe, true, 500, 40);
            Step(swipe, true, 500, 140);
            Step(swipe, true, 500, 240);
            Assert.That(swipe.Progress, Is.LessThan(EdgeSwipe.OpenAt));
            Assert.That(Step(swipe, true, 500, 240), Is.EqualTo(EdgeSwipe.Outcome.Dragging), "a frame holding still");
            Assert.That(Step(swipe, false, 500, 240), Is.EqualTo(EdgeSwipe.Outcome.Open));
        }

        [Test]
        public void PullingBackDownHighUp_StillOpensTheSheet()
        {
            var swipe = new EdgeSwipe();
            Step(swipe, true, 500, 40);
            Step(swipe, true, 500, 1200);
            Step(swipe, true, 500, 1000);
            Assert.That(Step(swipe, false, 500, 1000), Is.EqualTo(EdgeSwipe.Outcome.Open));
        }

        [Test]
        public void LettingGoLow_DropsTheSheetAway()
        {
            var swipe = new EdgeSwipe();
            Step(swipe, true, 500, 40);
            Step(swipe, true, 500, 600);
            Assert.That(Step(swipe, true, 500, 200), Is.EqualTo(EdgeSwipe.Outcome.Dragging), "pulling back down follows the finger");
            Assert.That(Step(swipe, false, 500, 200), Is.EqualTo(EdgeSwipe.Outcome.Cancel));
        }

        [Test]
        public void APressAboveTheEdge_IsLeftToTheScreen()
        {
            var swipe = new EdgeSwipe();
            Step(swipe, true, 500, 300);
            Assert.That(Step(swipe, true, 500, 1200), Is.EqualTo(EdgeSwipe.Outcome.None));
            Assert.That(Step(swipe, false, 500, 1200), Is.EqualTo(EdgeSwipe.Outcome.None));
        }

        [Test]
        public void AFingerSlidingIntoTheEdge_DoesNotStartASwipe()
        {
            var swipe = new EdgeSwipe();
            Step(swipe, true, 500, 800);
            Step(swipe, true, 500, 40);
            Assert.That(Step(swipe, true, 500, 1000), Is.EqualTo(EdgeSwipe.Outcome.None));
        }

        [Test]
        public void ASidewaysDragOrATap_DoesNotStartASwipe()
        {
            var swipe = new EdgeSwipe();
            Step(swipe, true, 500, 40);
            Assert.That(Step(swipe, true, 1100, 140), Is.EqualTo(EdgeSwipe.Outcome.None), "more sideways than up");
            Step(swipe, false, 1100, 140);

            Step(swipe, true, 500, 40);
            Assert.That(Step(swipe, true, 500, 60), Is.EqualTo(EdgeSwipe.Outcome.None), "a wobble under the start rise");
            Assert.That(Step(swipe, false, 500, 60), Is.EqualTo(EdgeSwipe.Outcome.None));
        }

        [Test]
        public void APressOnSomethingTheScreenDrags_IsLeftToIt()
        {
            var swipe = new EdgeSwipe(_ => true);
            Step(swipe, true, 500, 40);
            Assert.That(Step(swipe, true, 500, 1000), Is.EqualTo(EdgeSwipe.Outcome.None));
        }
    }
}
