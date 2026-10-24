using Code.Infrastructure.Ecs.Systems;
using Code.Simulation.Features.Movement.Systems;

namespace Code.Simulation.Features.Movement {
    public sealed class MovementFeature : Feature {
        public MovementFeature(ISystemFactory factory) {
            Add(factory.Create<MovementSystem>());
        }
    }
}
