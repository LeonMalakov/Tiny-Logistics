using Code.Common.Time;
using Code.Infrastructure.Configs;
using Code.Simulation.Features.Enemies.Factories;
using Entitas;
using UnityEngine;

namespace Code.Simulation.Features.Enemies.Systems {
    public sealed class EnemySpawnSystem : IExecuteSystem {
        private readonly ITimeService _time;
        private readonly IConfigService _configs;
        private readonly IEnemyFactory _enemies;
        private readonly IGroup<GameEntity> _trucks;
        private float _timeToSpawn;

        public EnemySpawnSystem(GameContext game, ITimeService time, IConfigService configs, IEnemyFactory enemies) {
            _time = time;
            _configs = configs;
            _enemies = enemies;

            _trucks = game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.Truck,
                    GameMatcher.Health)
                .NoneOf(
                    GameMatcher.Dead,
                    GameMatcher.Destructed));

            _timeToSpawn = configs.Balance.EnemySpawnInterval;
        }

        public void Execute() {
            bool hasTarget = false;

            foreach (GameEntity truck in _trucks) {
                if (truck.Health > 0f) {
                    hasTarget = true;
                    break;
                }
            }

            if (!hasTarget) {
                _timeToSpawn = _configs.Balance.EnemySpawnInterval;
                return;
            }

            _timeToSpawn -= _time.DeltaTime;

            Rect area = _configs.World.EnemySpawnArea;

            while (_timeToSpawn <= 0f) {
                _enemies.Create(new Vector3(
                    Random.Range(area.xMin, area.xMax),
                    _configs.World.FactoryPosition.y, 
                    Random.Range(area.yMin, area.yMax)));
                
                _timeToSpawn += _configs.Balance.EnemySpawnInterval;
            }
        }
    }
}
