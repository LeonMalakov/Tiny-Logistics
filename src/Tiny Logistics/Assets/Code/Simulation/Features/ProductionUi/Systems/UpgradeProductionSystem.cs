using Code.Infrastructure.Broker;
using Code.Infrastructure.Ecs.Systems;
using Code.Simulation.Features.Production.Services;
using Code.Simulation.Features.ProductionUi.Model;

namespace Code.Simulation.Features.ProductionUi.Systems {
    public sealed class UpgradeProductionSystem : BrokerRequestHandlerSystem<UpgradeProductionRequest> {
        private readonly IProductionService _production;

        public UpgradeProductionSystem(IRequestBroker broker, IProductionService production) : base(broker) {
            _production = production;
        }

        protected override void Handle(UpgradeProductionRequest request) => _production.TryUpgrade();
    }
}
