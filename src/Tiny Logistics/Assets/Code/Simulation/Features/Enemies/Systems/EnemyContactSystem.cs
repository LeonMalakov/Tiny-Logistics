using Code.Infrastructure.Configs;
using Code.Infrastructure.Ecs;
using Code.Simulation.Features.Explosion.Factories;
using Entitas;

namespace Code.Simulation.Features.Enemies.Systems {
    public sealed class EnemyContactSystem : IExecuteSystem {
        private readonly IConfigService _configs;
        private readonly IExplosionFactory _explosions;
        private readonly IGroup<GameEntity> _enemies;
        private readonly IGroup<GameEntity> _trucks;

        public EnemyContactSystem(GameContext game, IConfigService configs, IExplosionFactory explosions) {
            _configs = configs;
            _explosions = explosions;

            _enemies = game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.Enemy,
                    GameMatcher.WorldPosition,
                    GameMatcher.Health)
                .NoneOf(
                    GameMatcher.Dead,
                    GameMatcher.Destructed));

            _trucks = game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.Truck,
                    GameMatcher.WorldPosition,
                    GameMatcher.Health)
                .NoneOf(
                    GameMatcher.Dead,
                    GameMatcher.Destructed));
        }

        public void Execute() {
            float rangeSquared = _configs.Balance.EnemyAttackRange * _configs.Balance.EnemyAttackRange;

            foreach (GameEntity enemy in _enemies.GetEntities(GameEntityBuffer.A)) {
                if (enemy.Health <= 0f) {
                    continue;
                }

                foreach (GameEntity truck in _trucks) {
                    if (truck.Health <= 0f || (truck.WorldPosition - enemy.WorldPosition).sqrMagnitude > rangeSquared) {
                        continue;
                    }

                    _explosions.Create(enemy.WorldPosition);
                    enemy.isDestructed = true;

                    break;
                }
            }
        }
        
    }
}
