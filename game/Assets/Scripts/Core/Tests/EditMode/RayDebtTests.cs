using System;
using NUnit.Framework;
using Starflare.State;

namespace Starflare.Tests
{
    /// <summary>Ray's part of what you owe (A1.5): in the total owed, out of Kasia's book.</summary>
    public class RayDebtTests
    {
        [Test]
        public void OweRay_AddsToWhatYouOwe_WithoutMovingCash()
        {
            var state = new PlayerState();
            state.Earn(2000);
            int changes = 0;
            state.Changed += () => changes++;

            state.OweRay(2200);

            Assert.That(state.BankrollNow, Is.EqualTo(2200));
            Assert.That(state.RayOwed, Is.EqualTo(2200));
            Assert.That(state.Debts, Is.EqualTo(2200));
            Assert.That(state.OwnMoney, Is.EqualTo(0));
            Assert.That(state.KasiasBook, Is.EqualTo(0));
            Assert.That(changes, Is.EqualTo(1));
        }

        [Test]
        public void KasiasBook_IsEverythingOwedButRays_AndRepayingItCantTouchRays()
        {
            var state = new PlayerState();
            state.Borrow(50);
            state.OweRay(2200);

            Assert.That(state.Debts, Is.EqualTo(2250));
            Assert.That(state.KasiasBook, Is.EqualTo(50));
            Assert.Throws<ArgumentOutOfRangeException>(() => state.Repay(51));

            state.Repay(50);
            Assert.That(state.KasiasBook, Is.EqualTo(0));
            Assert.That(state.RayOwed, Is.EqualTo(2200));
        }

        [Test]
        public void OweRay_RefusesNothing()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new PlayerState().OweRay(0));
        }
    }
}
