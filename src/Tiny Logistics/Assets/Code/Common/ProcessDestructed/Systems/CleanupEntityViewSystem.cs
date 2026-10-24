using Code.Infrastructure.Ecs;
using Entitas;
using UnityEngine;

namespace Code.Common.ProcessDestructed.Systems {
    public sealed class CleanupEntityViewSystem : ICleanupSystem {
        private readonly IGroup<GameEntity> _entities;
        
        public CleanupEntityViewSystem(GameContext game) {
            _entities = game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.View, 
                    GameMatcher.Destructed));
        }
        
        public void Cleanup() {
            foreach (GameEntity entity in _entities.GetEntities(GameEntityBuffer.A)) {
                EntityView view = entity.View;
                
                if (view == null) {
                    entity.RemoveView();
                    continue;
                }
                
                view.ReleaseEntity();
                view.gameObject.SetActive(false);

                if (Application.isPlaying) {
                    Object.Destroy(view.gameObject);
                } else {
                    Object.DestroyImmediate(view.gameObject);
                }
            }
        }
        
    }
}
