using System;
using Code.Infrastructure.UiDispatcher;
using Code.Simulation.Features.Health.Model;
using Code.Simulation.Features.Health.Systems;
using Code.Simulation.Features.Production.Model;
using Code.Simulation.Features.ProductionUi.Model;
using Code.Simulation.Features.Stockpile.Model;
using NUnit.Framework;
using Tests.Editor.Support;

namespace Tests.Editor.Integration.Presentation {
    [Category("Integration")]
    [Category("UI")]
    public sealed class ProductionUiTests {
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
        public void GetUiInfo_NewGame_ReturnsGoldAndFactoryCraftSpeed() {
            ProductionUiInfo info = _context.Ui.GetUiInfo();

            Assert.That(info.Resources.Count, Is.EqualTo(1));
            Assert.That(info.Resources[0].Id, Is.EqualTo(ItemId.Gold));
            Assert.That(info.Resources[0].Amount, Is.Zero);
            Assert.That(info.CraftDuration, Is.EqualTo(_context.Balance.InitialCraftDuration));
            Assert.That(info.TrucksPerSecond, Is.EqualTo(1f / info.CraftDuration));
            Assert.That(info.CanUpgrade, Is.False);
            Assert.That(info.IsFactoryAlive, Is.True);
        }

        [Test]
        public void OnUiEvent_FactoryDied_RefreshesUiAndDisablesUpgrades() {
            _context.Stockpile.Add(ItemId.Gold, _context.Production.UpgradeCost);
            int changes = 0;
            _context.Ui.onDataChanged += () => changes++;
            _context.Dispatcher.Flush();
            changes = 0;
            UiEventProbe events = new(_context.Dispatcher);
            _factory.ReplaceHealth(0f);
            _context.Systems.Create<HealthSystem>().Execute();
            _context.Dispatcher.Flush();

            Assert.That(changes, Is.EqualTo(1));
            Assert.That(events.FactoryDeaths, Is.EqualTo(1));
            Assert.That(events.StockpileChanges, Is.Zero);
            Assert.That(events.ProductionChanges, Is.Zero);
            Assert.That(_context.Ui.GetUiInfo().IsFactoryAlive, Is.False);
            Assert.That(_context.Production.TryUpgrade(), Is.False);
            Assert.That(_context.Stockpile.GetAmount(ItemId.Gold), Is.EqualTo(_context.Production.UpgradeCost));
        }

        [Test]
        public void OnUiEvent_MultipleStockpileChanges_RefreshesUiOnceOnFlush() {
            UiEventProbe events = new(_context.Dispatcher);
            int changes = 0;
            _context.Ui.onDataChanged += () => changes++;
            _context.Stockpile.Add(ItemId.Gold, 10);
            _context.Stockpile.TryConsume(ItemId.Gold, 5);

            Assert.That(changes, Is.Zero);

            _context.Dispatcher.Flush();

            Assert.That(changes, Is.EqualTo(1));
            Assert.That(events.StockpileChanges, Is.EqualTo(1));
            Assert.That(events.ProductionChanges, Is.Zero);
            Assert.That(events.FactoryDeaths, Is.Zero);
            Assert.That(_context.Ui.GetUiInfo().Resources[0].Amount, Is.EqualTo(5));
        }

        [Test]
        public void OnUiEvent_ProductionRestored_RefreshesUiWithoutStockpileChange() {
            int changes = 0;
            _context.Ui.onDataChanged += () => changes++;
            UiEventProbe events = new(_context.Dispatcher);

            _context.Production.RestoreUpgradeLevel(2);
            _context.Dispatcher.Flush();

            Assert.That(changes, Is.EqualTo(1));
            Assert.That(events.ProductionChanges, Is.EqualTo(1));
            Assert.That(events.StockpileChanges, Is.Zero);
            Assert.That(events.FactoryDeaths, Is.Zero);
            Assert.That(_context.Ui.GetUiInfo().CraftDuration, Is.EqualTo(_context.Production.CraftDuration));
        }

        [Test]
        public void Dispose_UnsubscribesFromAllGameplayEvents() {
            int changes = 0;
            _context.Ui.onDataChanged += () => changes++;
            ((IDisposable)_context.Ui).Dispose();

            _context.Stockpile.Add(ItemId.Gold, _context.Production.UpgradeCost);
            _context.Production.RestoreUpgradeLevel(1);
            _factory.ReplaceHealth(0f);
            _context.Systems.Create<HealthSystem>().Execute();
            _context.Dispatcher.Flush();

            Assert.That(changes, Is.Zero);
        }

        private sealed class UiEventProbe : IUiEventListener<StockpileUiEvents.Changed>,
            IUiEventListener<ProductionUiEvents.Changed>, IUiEventListener<HealthUiEvents.EntityDied> {
            public int StockpileChanges { get; private set; }
            public int ProductionChanges { get; private set; }
            public int FactoryDeaths { get; private set; }

            public UiEventProbe(IUiDispatcher dispatcher) {
                dispatcher.AddListener<StockpileUiEvents.Changed>(this);
                dispatcher.AddListener<ProductionUiEvents.Changed>(this);
                dispatcher.AddListener<HealthUiEvents.EntityDied>(this);
            }

            public void OnUiEvent(StockpileUiEvents.Changed eventData) => StockpileChanges++;

            public void OnUiEvent(ProductionUiEvents.Changed eventData) => ProductionChanges++;

            public void OnUiEvent(HealthUiEvents.EntityDied eventData) {
                if (eventData.Entity.isFactory) {
                    FactoryDeaths++;
                }
            }
        }
    }
}
