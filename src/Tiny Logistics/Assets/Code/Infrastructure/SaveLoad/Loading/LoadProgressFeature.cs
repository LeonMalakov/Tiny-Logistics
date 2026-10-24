using Code.Infrastructure.Ecs.Systems;
using Code.Infrastructure.SaveLoad.Loading.Systems;
using Code.Simulation.Features.Production.Systems;
using Code.Simulation.Features.Stockpile.Systems;

namespace Code.Infrastructure.SaveLoad.Loading {
    public sealed class LoadProgressFeature : Feature {
        public LoadProgressFeature(ISystemFactory factory) {
            Add(factory.Create<LoadStockpileSystem>());
            Add(factory.Create<LoadProductionSystem>());
            Add(factory.Create<LoadEntitiesSystem>());
            Add(factory.Create<CompleteLoadProgressSystem>());
        }
    }
}
