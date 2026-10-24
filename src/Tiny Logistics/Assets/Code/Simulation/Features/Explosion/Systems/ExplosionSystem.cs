using Code.Infrastructure.Ecs;
using Entitas;
using UnityEngine;

namespace Code.Simulation.Features.Explosion.Systems {
    public sealed class ExplosionSystem : IExecuteSystem {
        private readonly IGroup<GameEntity> _explosions;
        private readonly IGroup<GameEntity> _targets;

        public ExplosionSystem(GameContext game) {
            _explosions = game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.Explosion,
                    GameMatcher.WorldPosition,
                    GameMatcher.ExplosionRadius,
                    GameMatcher.ExplosionDamage,
                    GameMatcher.ExplosionBurnDuration,
                    GameMatcher.ExplosionBurnDamagePerSecond)
                .NoneOf(
                    GameMatcher.Destructed));

            _targets = game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.WorldPosition,
                    GameMatcher.Health)
                .NoneOf(
                    GameMatcher.Dead,
                    GameMatcher.Destructed));
        }

        public void Execute() {
            foreach (GameEntity explosion in _explosions.GetEntities(GameEntityBuffer.A)) {
                float radiusSquared = explosion.ExplosionRadius * explosion.ExplosionRadius;

                foreach (GameEntity target in _targets) {
                    if (target.Health <= 0f || !InRange(target, explosion, radiusSquared)) {
                        continue;
                    }

                    target.ReplaceHealth(Mathf.Max(0f, target.Health - explosion.ExplosionDamage));

                    if (target.Health <= 0f) {
                        continue;
                    }

                    float duration = target.hasBurningTimeLeft ? target.BurningTimeLeft : 0f;
                    float damage = target.hasBurnDamagePerSecond ? target.BurnDamagePerSecond : 0f;

                    target.ReplaceBurningTimeLeft(Mathf.Max(duration, explosion.ExplosionBurnDuration));
                    target.ReplaceBurnDamagePerSecond(Mathf.Max(damage, explosion.ExplosionBurnDamagePerSecond));
                    target.isBurning = true;
                }

                explosion.isDestructed = true;
            }
        }

        private static bool InRange(GameEntity target, GameEntity explosion, float radiusSquared) {
            return (target.WorldPosition - explosion.WorldPosition).sqrMagnitude <= radiusSquared;
        }
    }
}
