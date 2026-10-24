using Code.Infrastructure.Ecs;
using Code.Simulation.Features.SimulationView.Factories;
using Entitas;

namespace Code.Simulation.Features.SimulationView.Systems {
    public sealed class CreateEntityViewSystem : IInitializeSystem, IExecuteSystem {
        private readonly IGroup<GameEntity> _entities;
        private readonly ISimulationViewFactory _views;

        public CreateEntityViewSystem(GameContext game, ISimulationViewFactory views) {
            _views = views;

            _entities = game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.WorldPosition)
                .AnyOf(
                    GameMatcher.Factory,
                    GameMatcher.Truck,
                    GameMatcher.Enemy)
                .NoneOf(
                    GameMatcher.View,
                    GameMatcher.Destructed));
        }

        public void Initialize() => Execute();

        public void Execute() {
            foreach (GameEntity entity in _entities.GetEntities(GameEntityBuffer.A)) {
                _views.CreateEntityView(entity);
            }
        }
        
    }
}
