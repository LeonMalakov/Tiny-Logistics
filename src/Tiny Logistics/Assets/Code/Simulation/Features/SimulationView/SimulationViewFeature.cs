using Code.Infrastructure.Ecs.Systems;
using Code.Simulation.Features.SimulationView.Systems;

namespace Code.Simulation.Features.SimulationView {
    public sealed class SimulationViewFeature : Feature {
        public SimulationViewFeature(ISystemFactory factory) {
            Add(factory.Create<WorldViewSystem>());
            Add(factory.Create<CreateEntityViewSystem>());
            Add(factory.Create<SyncEntityViewSystem>());
            Add(factory.Create<ExplosionViewSystem>());
        }
    }
}
