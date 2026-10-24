using Code.Infrastructure.Ecs.Systems;
using Code.Simulation.Features.Health.Systems;

namespace Code.Simulation.Features.Health {
    public sealed class HealthFeature : Feature {
        public HealthFeature(ISystemFactory factory) {
            Add(factory.Create<HealthSystem>());
        }
    }
}
