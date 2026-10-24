using Code.Infrastructure.Ecs.Systems;
using Code.Simulation.Features.Burning.Systems;

namespace Code.Simulation.Features.Burning {
    public sealed class BurningFeature : Feature {
        public BurningFeature(ISystemFactory factory) {
            Add(factory.Create<BurningSystem>());
        }
    }
}
