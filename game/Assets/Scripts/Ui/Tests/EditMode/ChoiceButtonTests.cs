using NUnit.Framework;
using Starflare.Ui;
using UnityEngine;

namespace Starflare.Tests
{
    /// <summary>The choice bar glows only while your finger is on it, never left lit after a tap.</summary>
    public sealed class ChoiceButtonTests
    {
        GameObject _root;

        [SetUp]
        public void SetUp() => _root = new GameObject("Choices", typeof(RectTransform));

        [TearDown]
        public void TearDown() => Object.DestroyImmediate(_root);

        [Test]
        public void TheGlow_ShowsOnlyUnderAFinger()
        {
            var button = UiKit.MakeChoiceButton(_root.transform, "Go inside", () => { });

            Assert.That(button.targetGraphic.name, Is.EqualTo("Glow"));
            Assert.That(button.colors.normalColor.a, Is.EqualTo(0));
            Assert.That(button.colors.selectedColor.a, Is.EqualTo(0));
            Assert.That(button.colors.pressedColor.a, Is.EqualTo(1));
        }
    }
}
