using System;
using Code.Simulation.Features.ProductionUi.Systems;
using Code.Simulation.Features.Stockpile.Model;
using Entitas;
using NUnit.Framework;
using Tests.Editor.Support;

namespace Tests.Editor.Integration.Simulation {
    [Category("Integration")]
    [Category("Simulation")]
    public sealed class ProductionTests {
        private GameTestContext _context;
        private GameEntity _factory;

        [SetUp]
        public void SetUp() {
            _context = new GameTestContext();
            _factory = _context.CreateFactory();
        }

        [TearDown]
        public void TearDown() => _context.Dispose();

        [Test]
        public void TryUpgrade_EnoughGold_SpeedsCurrentCraftWithoutResettingProgress() {
            _factory.ReplaceCraftProgress(0.5f);
            _context.Stockpile.Add(ItemId.Gold, _context.Production.UpgradeCost);

            Assert.That(_context.Production.TryUpgrade(), Is.True);

            float duration = _context.Balance.InitialCraftDuration / _context.Balance.CraftSpeedMultiplierPerUpgrade;

            Assert.That(_factory.CraftDuration, Is.EqualTo(duration).Within(0.0001f));
            Assert.That(_factory.CraftProgress, Is.EqualTo(0.5f));
            Assert.That(_context.Stockpile.GetAmount(ItemId.Gold), Is.Zero);
        }

        [Test]
        public void Execute_TwoUpgradeRequestsAndGoldForOne_UpgradesOnceWithoutOverspending() {
            _context.Stockpile.Add(ItemId.Gold, _context.Production.UpgradeCost);
            _context.Ui.RequestUpgrade();
            _context.Ui.RequestUpgrade();
            ((IExecuteSystem)_context.Systems.Create<UpgradeProductionSystem>()).Execute();

            Assert.That(_context.Stockpile.GetAmount(ItemId.Gold), Is.Zero);
            Assert.That(_context.Production.CraftDuration, Is.EqualTo(_context.Balance.InitialCraftDuration / _context.Balance.CraftSpeedMultiplierPerUpgrade).Within(0.0001f));
        }

        [Test]
        public void TryUpgrade_MaximumCraftSpeed_ClampsDurationAndStopsCharging() {
            int upgradesToLimit = (int)Math.Ceiling(Math.Log(
                _context.Balance.InitialCraftDuration / _context.Balance.MinCraftDuration,
                _context.Balance.CraftSpeedMultiplierPerUpgrade));
            _context.Stockpile.Add(ItemId.Gold, _context.Production.UpgradeCost * (upgradesToLimit + 1));

            for (int i = 0; i < upgradesToLimit; i++) {
                Assert.That(_context.Production.TryUpgrade(), Is.True);
            }

            long remainingGold = _context.Stockpile.GetAmount(ItemId.Gold);

            Assert.That(_context.Production.CraftDuration, Is.EqualTo(_context.Balance.MinCraftDuration));
            Assert.That(_context.Production.CanUpgrade, Is.False);
            Assert.That(_context.Production.TryUpgrade(), Is.False);
            Assert.That(_context.Stockpile.GetAmount(ItemId.Gold), Is.EqualTo(remainingGold));
        }

        [Test]
        public void Reset_UpgradedFactory_RestoresBaseCraftDuration() {
            _context.Stockpile.Add(ItemId.Gold, _context.Production.UpgradeCost);
            _context.Production.TryUpgrade();
            _context.Production.Reset();

            Assert.That(_factory.CraftDuration, Is.EqualTo(_context.Balance.InitialCraftDuration));
        }
    }
}
