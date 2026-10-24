using Code.Infrastructure.Ecs.Systems;
using Code.Simulation.Features.Crafting.Systems;

namespace Code.Simulation.Features.Crafting {
    public sealed class CraftFeature : Feature {
        public CraftFeature(ISystemFactory factory) {
            Add(factory.Create<CraftSystem>());
        }
    }
}
