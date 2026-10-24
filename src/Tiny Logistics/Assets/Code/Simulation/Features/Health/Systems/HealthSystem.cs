using Code.Infrastructure.Configs;
using Code.Infrastructure.Ecs;
using Code.Infrastructure.UiDispatcher;
using Code.Simulation.Features.Burning;
using Code.Simulation.Features.Health.Model;
using Entitas;

namespace Code.Simulation.Features.Health.Systems {
    public sealed class HealthSystem : IExecuteSystem {
        private readonly IConfigService _configs;
        private readonly IUiDispatcher _uiDispatcher;
        private readonly IGroup<GameEntity> _living;

        public HealthSystem(GameContext game, IConfigService configs, IUiDispatcher uiDispatcher) {
            _configs = configs;
            _uiDispatcher = uiDispatcher;

            _living = game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.Health)
                .NoneOf(
                    GameMatcher.Dead,
                    GameMatcher.Destructed));
        }

        public void Execute() {
            foreach (GameEntity entity in _living.GetEntities(GameEntityBuffer.A)) {
                if (entity.Health > 0f) {
                    continue;
                }

                entity.ReplaceHealth(0f);
                entity.isDead = true;
                entity.StopBurning();

                if (!entity.hasSelfDestructTimer) {
                    entity.AddSelfDestructTimer(_configs.Balance.DeathDestructDelay);
                }

                _uiDispatcher.Publish(new HealthUiEvents.EntityDied(entity));
            }
        }
        
    }
}
