using Code.Infrastructure.Ecs.Systems;
using Code.Simulation.Features.Enemies.Systems;

namespace Code.Simulation.Features.Enemies {
    public sealed class EnemyFeature : Feature {
        public EnemyFeature(ISystemFactory factory) {
            Add(factory.Create<EnemySpawnSystem>());
            Add(factory.Create<EnemyChaseSystem>());
        }
    }
}
