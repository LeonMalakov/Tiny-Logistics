using Code.Common.Time;
using Entitas;
using UnityEngine;

namespace Code.Simulation.Features.Enemies.Systems {
    public sealed class EnemyChaseSystem : IExecuteSystem {
        private readonly ITimeService _time;
        private readonly IGroup<GameEntity> _enemies;
        private readonly IGroup<GameEntity> _trucks;

        public EnemyChaseSystem(GameContext game, ITimeService time) {
            _time = time;

            _enemies = game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.Enemy,
                    GameMatcher.EnemySpeed,
                    GameMatcher.WorldPosition,
                    GameMatcher.Velocity,
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
            foreach (GameEntity enemy in _enemies) {
                if (enemy.Health <= 0f) {
                    enemy.ReplaceVelocity(Vector3.zero);
                    continue;
                }

                var nearest = FindNearestTruck(enemy);

                if (nearest == null) {
                    enemy.ReplaceVelocity(Vector3.zero);
                    continue;
                }

                Vector3 offset = nearest.WorldPosition - enemy.WorldPosition;
                float speed = Mathf.Min(enemy.EnemySpeed, offset.magnitude / _time.DeltaTime);

                enemy.ReplaceVelocity(offset.normalized * speed);
            }
        }

        private GameEntity FindNearestTruck(GameEntity enemy) {
            GameEntity nearest = null;
            float closestSquared = float.PositiveInfinity;

            foreach (GameEntity truck in _trucks) {
                if (truck.Health <= 0f) {
                    continue;
                }

                float squared = (truck.WorldPosition - enemy.WorldPosition).sqrMagnitude;

                if (squared < closestSquared) {
                    closestSquared = squared;
                    nearest = truck;
                }
            }

            return nearest;
        }
    }
}
