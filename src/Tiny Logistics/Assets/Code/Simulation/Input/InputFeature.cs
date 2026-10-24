using Code.Infrastructure.Ecs.Systems;
using Code.Simulation.Input.Systems;

namespace Code.Simulation.Input {
    public sealed class InputFeature : Feature {
        public InputFeature(ISystemFactory factory) {
            Add(factory.Create<ReadExplosionInputSystem>());
        }
    }
}
