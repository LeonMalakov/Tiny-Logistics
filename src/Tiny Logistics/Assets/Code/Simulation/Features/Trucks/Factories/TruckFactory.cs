using Code.Common.Identity;
using Code.Infrastructure.Configs;
using UnityEngine;

namespace Code.Simulation.Features.Trucks.Factories {
    public sealed class TruckFactory : ITruckFactory {
        private readonly GameContext _game;
        private readonly IIdentityService _identity;
        private readonly IConfigService _configs;

        public TruckFactory(GameContext game, IIdentityService identity, IConfigService configs) {
            _game = game;
            _identity = identity;
            _configs = configs;
        }

        public GameEntity Create(Vector3 position) {
            GameEntity truck = _game.CreateEntity();

            truck.isTruck = true;
            truck.AddRuntimeId(_identity.Next());
            truck.AddWorldPosition(position);
            truck.AddVelocity(Vector3.right * _configs.Balance.TruckSpeed);
            truck.AddHealth(_configs.Balance.TruckHealth);
            truck.AddMaxHealth(_configs.Balance.TruckHealth);

            return truck;
        }
    }
}
