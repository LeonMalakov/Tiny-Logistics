using Code.Common.ProcessDestructed;
using Code.Infrastructure.Ecs.Systems;
using Code.Infrastructure.SaveLoad.Saving.Systems;
using Code.Infrastructure.UiDispatcher;
using Code.Simulation.Features.ProductionUi;
using Code.Simulation.Features.SimulationView;
using Code.Simulation.Input;

namespace Code.Simulation {
    public class EveryFrameFeature : Feature {
        public EveryFrameFeature(ISystemFactory factory) {
            Add(factory.Create<InputFeature>());
            Add(factory.Create<ProductionUiFeature>());
            Add(factory.Create<SimulationFeature>());
            Add(factory.Create<SimulationViewFeature>());
            Add(factory.Create<ProcessDestructedFeature>());
            Add(factory.Create<PeriodicallySaveSystem>());
            Add(factory.Create<UiDispatcherFlushSystem>());
        }
    }
}
