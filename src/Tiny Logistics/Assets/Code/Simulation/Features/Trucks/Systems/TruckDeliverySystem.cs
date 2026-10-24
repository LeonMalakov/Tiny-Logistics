using Code.Infrastructure.Configs;
using Code.Infrastructure.Configs.Model;
using Code.Infrastructure.Ecs;
using Code.Simulation.Features.Stockpile.Model;
using Code.Simulation.Features.Stockpile.Services;
using Entitas;

namespace Code.Simulation.Features.Trucks.Systems {
    public sealed class TruckDeliverySystem : IExecuteSystem {
        private readonly IStockpileService _stockpile;
        private readonly IConfigService _configs;
        private readonly IGroup<GameEntity> _trucks;

        public TruckDeliverySystem(GameContext game, IStockpileService stockpile, IConfigService configs) {
            _stockpile = stockpile;
            _configs = configs;

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
            foreach (GameEntity truck in _trucks.GetEntities(GameEntityBuffer.A)) {
                if (truck.Health <= 0f || ConfigValidation.Contains(_configs.World.MapBounds, truck.WorldPosition)) {
                    continue;
                }

                _stockpile.Add(ItemId.Gold, _configs.Balance.GoldPerTruck);

                truck.isDestructed = true;
            }
        }
        
    }
}
