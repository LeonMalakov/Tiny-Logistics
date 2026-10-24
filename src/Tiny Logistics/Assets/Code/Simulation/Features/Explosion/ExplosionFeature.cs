using Code.Infrastructure.Ecs.Systems;
using Code.Simulation.Features.Explosion.Systems;

namespace Code.Simulation.Features.Explosion {
    public sealed class ExplosionFeature : Feature {
        public ExplosionFeature(ISystemFactory factory) {
            Add(factory.Create<CreateExplosionSystem>());
            Add(factory.Create<ExplosionSystem>());
        }
    }
}
