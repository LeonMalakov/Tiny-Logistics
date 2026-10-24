using Code.Infrastructure.Ecs.Systems;
using Code.Simulation.Features.Burning;
using Code.Simulation.Features.Crafting;
using Code.Simulation.Features.Enemies;
using Code.Simulation.Features.Enemies.Systems;
using Code.Simulation.Features.Explosion;
using Code.Simulation.Features.Health;
using Code.Simulation.Features.Movement;
using Code.Simulation.Features.Trucks;

namespace Code.Simulation {
    public sealed class SimulationFeature : Feature {
        public SimulationFeature(ISystemFactory factory) {
            Add(factory.Create<CraftFeature>());
            Add(factory.Create<EnemyFeature>());
            Add(factory.Create<MovementFeature>());
            Add(factory.Create<EnemyContactSystem>());
            Add(factory.Create<ExplosionFeature>());
            Add(factory.Create<BurningFeature>());
            Add(factory.Create<HealthFeature>());
            Add(factory.Create<TruckDeliveryFeature>());
        }
    }
}
