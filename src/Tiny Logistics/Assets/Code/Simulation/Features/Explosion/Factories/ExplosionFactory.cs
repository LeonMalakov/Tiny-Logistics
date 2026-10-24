using Code.Common.Identity;
using Code.Infrastructure.Configs;
using UnityEngine;

namespace Code.Simulation.Features.Explosion.Factories {
    public sealed class ExplosionFactory : IExplosionFactory {
        private readonly GameContext _game;
        private readonly IIdentityService _identity;
        private readonly IConfigService _configs;

        public ExplosionFactory(GameContext game, IIdentityService identity, IConfigService configs) {
            _game = game;
            _identity = identity;
            _configs = configs;
        }

        public GameEntity Create(Vector3 position) {
            GameEntity explosion = _game.CreateEntity();

            explosion.isExplosion = true;
            explosion.AddRuntimeId(_identity.Next());
            explosion.AddWorldPosition(position);
            explosion.AddExplosionRadius(_configs.Balance.ExplosionRadius);
            explosion.AddExplosionDamage(_configs.Balance.ExplosionDamage);
            explosion.AddExplosionBurnDuration(_configs.Balance.BurnDuration);
            explosion.AddExplosionBurnDamagePerSecond(_configs.Balance.BurnDamagePerSecond);

            return explosion;
        }
    }
}
