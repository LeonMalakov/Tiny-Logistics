using System.Collections.Generic;
using Code.Simulation.Features.Stockpile.Model;

namespace Code.Simulation.Features.ProductionUi.Model {
    public readonly struct ResourceUiInfo {
        public readonly ItemId Id;
        public readonly string Name;
        public readonly long Amount;

        public ResourceUiInfo(ItemId id, string name, long amount) {
            Id = id;
            Name = name;
            Amount = amount;
        }
    }

    public readonly struct ProductionUiInfo {
        public readonly IReadOnlyList<ResourceUiInfo> Resources;
        public readonly float CraftDuration;
        public readonly float TrucksPerSecond;
        public readonly long UpgradeCost;
        public readonly float UpgradeSpeedMultiplier;
        public readonly bool CanUpgrade;
        public readonly bool IsFactoryAlive;

        public ProductionUiInfo(IReadOnlyList<ResourceUiInfo> resources, float craftDuration, float trucksPerSecond,
            long upgradeCost, float upgradeSpeedMultiplier, bool canUpgrade, bool isFactoryAlive) {
            Resources = resources;
            CraftDuration = craftDuration;
            TrucksPerSecond = trucksPerSecond;
            UpgradeCost = upgradeCost;
            UpgradeSpeedMultiplier = upgradeSpeedMultiplier;
            CanUpgrade = canUpgrade;
            IsFactoryAlive = isFactoryAlive;
        }
    }
}
