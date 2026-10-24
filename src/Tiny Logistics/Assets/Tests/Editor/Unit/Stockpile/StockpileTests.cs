using System;
using Code.Infrastructure.UiDispatcher;
using Code.Simulation.Features.Stockpile.Model;
using Code.Simulation.Features.Stockpile.Services;
using NSubstitute;
using NUnit.Framework;

namespace Tests.Editor.Unit.Stockpile {
    [Category("Unit")]
    [Category("Simulation")]
    public sealed class StockpileTests {
        private StockpileService _stockpile;

        [SetUp]
        public void SetUp() => _stockpile = new StockpileService(Substitute.For<IUiDispatcher>());

        [Test]
        public void TryConsume_LongAmounts_RejectsOverdraftAndRemovesEmptyEntry() {
            long amount = (long)int.MaxValue + 10;
            _stockpile.Add(ItemId.Gold, amount);

            Assert.That(_stockpile.TryConsume(ItemId.Gold, amount + 1), Is.False);
            Assert.That(_stockpile.GetAmount(ItemId.Gold), Is.EqualTo(amount));
            Assert.That(_stockpile.TryConsume(ItemId.Gold, amount), Is.True);
            Assert.That(_stockpile.Items, Is.Empty);
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void Add_NonPositiveAmount_ThrowsArgumentOutOfRangeException(long amount) {
            Assert.Throws<ArgumentOutOfRangeException>(() => _stockpile.Add(ItemId.Gold, amount));
        }
    }
}
