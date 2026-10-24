using Code.Common.Time;
using Code.Simulation.Features.SimulationView.Views;
using Entitas;

namespace Code.Simulation.Features.SimulationView.Systems {
    public sealed class SyncEntityViewSystem : IExecuteSystem {
        private readonly IGroup<GameEntity> _entities;
        private readonly ITimeService _time;
        private float _elapsed;

        public SyncEntityViewSystem(GameContext game, ITimeService time) {
            _time = time;

            _entities = game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.View,
                    GameMatcher.WorldPosition)
                .NoneOf(
                    GameMatcher.Destructed));
        }

        public void Execute() {
            _elapsed += _time.DeltaTime;

            foreach (GameEntity entity in _entities) {
                if (entity.View is SimulationEntityView view) {
                    view.Refresh(_elapsed);
                } else if (entity.View != null) {
                    entity.View.transform.position = entity.WorldPosition;
                }
            }
        }
        
    }
}
