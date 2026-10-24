using Code.Common;
using Code.Infrastructure.Configs;
using Code.Infrastructure.Ecs;
using Code.Infrastructure.UiDispatcher;
using Code.Simulation.Features.Production.Model;
using Code.Simulation.Features.Stockpile.Model;
using Code.Simulation.Features.Stockpile.Services;
using Entitas;
using UnityEngine;

namespace Code.Simulation.Features.Production.Services {
    public sealed class ProductionService : IProductionService {
        private readonly IStockpileService _stockpile;
        private readonly IUiDispatcher _uiDispatcher;
        private readonly IConfigService _configs;
        private readonly IGroup<GameEntity> _factories;
        private float _craftDuration;

        public int UpgradeLevel { get; private set; }

        public float CraftDuration => _craftDuration > 0f ? _craftDuration : _configs.Balance.InitialCraftDuration;
        public float TrucksPerSecond => 1f / CraftDuration;
        public float UpgradeSpeedMultiplier => _configs.Balance.CraftSpeedMultiplierPerUpgrade;
        public long UpgradeCost => _configs.Balance.ProductionUpgradeCost;

        public bool IsFactoryAlive {
            get {
                foreach (GameEntity factory in _factories) {
                    if (factory.Health > 0f) {
                        return true;
                    }
                }

                return false;
            }
        }

        public bool CanUpgrade => IsFactoryAlive && CraftDuration > _configs.Balance.MinCraftDuration &&
                                  _stockpile.GetAmount(ItemId.Gold) >= UpgradeCost;

        public ProductionService(GameContext game, IStockpileService stockpile,
            IUiDispatcher uiDispatcher, IConfigService configs) {
            _stockpile = stockpile;
            _uiDispatcher = uiDispatcher;
            _configs = configs;

            _factories = game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.Factory,
                    GameMatcher.Health,
                    GameMatcher.CraftDuration)
                .NoneOf(
                    GameMatcher.Dead,
                    GameMatcher.Destructed));
        }

        public bool TryUpgrade() {
            if (!CanUpgrade || !_stockpile.TryConsume(ItemId.Gold, UpgradeCost)) {
                return false;
            }

            RestoreUpgradeLevel(UpgradeLevel + 1);

            return true;
        }

        public void Reset() => RestoreUpgradeLevel(0);

        public void RestoreUpgradeLevel(int level) {
            if (level < 0) {
                ThrowException.ArgumentOutOfRange(nameof(level));
            }

            UpgradeLevel = level;
            SetDuration(Mathf.Max(_configs.Balance.MinCraftDuration,
                _configs.Balance.InitialCraftDuration / Mathf.Pow(UpgradeSpeedMultiplier, level)));
        }

        private void SetDuration(float duration) {
            _craftDuration = duration;

            foreach (GameEntity factory in _factories.GetEntities(GameEntityBuffer.A)) {
                if (factory.Health > 0f) {
                    factory.ReplaceCraftDuration(duration);
                }
            }

            _uiDispatcher.Publish<ProductionUiEvents.Changed>();
        }
    }
}
