using Code.Infrastructure.Ecs.Systems;
using Code.Simulation.Features.ProductionUi.Systems;

namespace Code.Simulation.Features.ProductionUi {
    public sealed class ProductionUiFeature : Feature {
        public ProductionUiFeature(ISystemFactory factory) {
            Add(factory.Create<UpgradeProductionSystem>());
        }
    }
}
