using Code.Common.Identity;
using Code.Infrastructure.Configs;
using UnityEngine;

namespace Code.Simulation.Features.Enemies.Factories {
    public sealed class EnemyFactory : IEnemyFactory {
        private readonly GameContext _game;
        private readonly IIdentityService _identity;
        private readonly IConfigService _configs;

        public EnemyFactory(GameContext game, IIdentityService identity, IConfigService configs) {
            _game = game;
            _identity = identity;
            _configs = configs;
        }

        public GameEntity Create(Vector3 position) {
            GameEntity enemy = _game.CreateEntity();

            enemy.isEnemy = true;
            enemy.AddRuntimeId(_identity.Next());
            enemy.AddWorldPosition(position);
            enemy.AddVelocity(Vector3.zero);
            enemy.AddEnemySpeed(_configs.Balance.EnemySpeed);
            enemy.AddHealth(_configs.Balance.EnemyHealth);
            enemy.AddMaxHealth(_configs.Balance.EnemyHealth);

            return enemy;
        }
    }
}
