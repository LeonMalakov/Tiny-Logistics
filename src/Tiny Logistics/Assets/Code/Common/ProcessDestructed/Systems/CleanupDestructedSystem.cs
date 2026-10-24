using Code.Infrastructure.Ecs;
using Entitas;

namespace Code.Common.ProcessDestructed.Systems {
    public class CleanupDestructedSystem : ICleanupSystem {
        private readonly IGroup<GameEntity> _entities;

        public CleanupDestructedSystem(GameContext game) {
            _entities = game.GetGroup(GameMatcher.Destructed);
        }
        
        public void Cleanup() {
            foreach (GameEntity entity in _entities.GetEntities(GameEntityBuffer.A)) {
                entity.Destroy();
            }
        }
        
    }
}