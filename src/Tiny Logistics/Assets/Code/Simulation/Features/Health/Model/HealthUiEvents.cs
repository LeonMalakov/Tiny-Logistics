using Code.Infrastructure.UiDispatcher;

namespace Code.Simulation.Features.Health.Model {
    public static class HealthUiEvents {
        public readonly struct EntityDied : IUiPayloadEvent {
            // Delivered before entity cleanup; consumers can inspect the entity's components.
            public readonly GameEntity Entity;

            public EntityDied(GameEntity entity) => Entity = entity;
        }
    }
}
