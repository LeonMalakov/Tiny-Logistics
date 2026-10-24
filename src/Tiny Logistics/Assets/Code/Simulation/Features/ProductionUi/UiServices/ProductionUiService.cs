using System;
using System.Collections.Generic;
using Code.Infrastructure.Broker;
using Code.Infrastructure.UiDispatcher;
using Code.Simulation.Features.Health.Model;
using Code.Simulation.Features.Production.Model;
using Code.Simulation.Features.Production.Services;
using Code.Simulation.Features.ProductionUi.Model;
using Code.Simulation.Features.ProductionUi.Services;
using Code.Simulation.Features.Stockpile.Model;
using Code.Simulation.Features.Stockpile.Services;

namespace Code.Simulation.Features.ProductionUi.UiServices {
    public sealed class ProductionUiService : IProductionUiService, IDisposable,
        IUiEventListener<StockpileUiEvents.Changed>,
        IUiEventListener<ProductionUiEvents.Changed>,
        IUiEventListener<HealthUiEvents.EntityDied> {
        
        public event Action onDataChanged;

        private static readonly ItemId[] ResourceIds = (ItemId[])Enum.GetValues(typeof(ItemId));
        private readonly IStockpileService _stockpile;
        private readonly IProductionService _production;
        private readonly IRequestBroker _broker;
        private readonly IUiDispatcher _uiDispatcher;

        public ProductionUiService(IStockpileService stockpile, IProductionService production,
            IRequestBroker broker, IUiDispatcher uiDispatcher) {
            _stockpile = stockpile;
            _production = production;
            _broker = broker;
            _uiDispatcher = uiDispatcher;
            
            _uiDispatcher.AddListener<StockpileUiEvents.Changed>(this);
            _uiDispatcher.AddListener<ProductionUiEvents.Changed>(this);
            _uiDispatcher.AddListener<HealthUiEvents.EntityDied>(this);
        }

        public void Dispose() {
            _uiDispatcher.RemoveListener<StockpileUiEvents.Changed>(this);
            _uiDispatcher.RemoveListener<ProductionUiEvents.Changed>(this);
            _uiDispatcher.RemoveListener<HealthUiEvents.EntityDied>(this);
        }

        public ProductionUiInfo GetUiInfo() {
            List<ResourceUiInfo> resources = new(ResourceIds.Length - 1);

            foreach (ItemId id in ResourceIds) {
                if (id != ItemId.None) {
                    resources.Add(new ResourceUiInfo(id, id.ToString(), _stockpile.GetAmount(id)));
                }
            }

            return new ProductionUiInfo(resources, _production.CraftDuration, _production.TrucksPerSecond,
                _production.UpgradeCost, _production.UpgradeSpeedMultiplier, _production.CanUpgrade, _production.IsFactoryAlive);
        }

        public void RequestUpgrade() {
            _broker.Publish(new UpgradeProductionRequest());
        }

        void IUiEventListener<StockpileUiEvents.Changed>.OnUiEvent(StockpileUiEvents.Changed eventData) {
            onDataChanged?.Invoke();
        }

        void IUiEventListener<ProductionUiEvents.Changed>.OnUiEvent(ProductionUiEvents.Changed eventData) {
            onDataChanged?.Invoke();
        }

        void IUiEventListener<HealthUiEvents.EntityDied>.OnUiEvent(HealthUiEvents.EntityDied eventData) {
            if (eventData.Entity.isFactory) {
                onDataChanged?.Invoke();
            }
        }
    }
}
