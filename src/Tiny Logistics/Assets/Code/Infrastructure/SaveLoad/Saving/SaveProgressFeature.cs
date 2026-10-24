using Code.Infrastructure.Ecs.Systems;
using Code.Infrastructure.SaveLoad.Saving.Systems;
using Code.Simulation.Features.Production.Systems;
using Code.Simulation.Features.Stockpile.Systems;

namespace Code.Infrastructure.SaveLoad.Saving {
    public sealed class SaveProgressFeature : Feature {
        public SaveProgressFeature(ISystemFactory factory) {
            Add(factory.Create<ClearLostProgressSystem>());
            Add(factory.Create<BeginSaveProgressSystem>());
            Add(factory.Create<SaveStockpileSystem>());
            Add(factory.Create<SaveProductionSystem>());
            Add(factory.Create<SaveEntitiesSystem>());
            Add(factory.Create<CompleteSaveProgressSystem>());
        }
    }
}
