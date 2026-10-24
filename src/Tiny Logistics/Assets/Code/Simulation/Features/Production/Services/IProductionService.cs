namespace Code.Simulation.Features.Production.Services {
    public interface IProductionService {
        int UpgradeLevel { get; }
        float CraftDuration { get; }
        float TrucksPerSecond { get; }
        float UpgradeSpeedMultiplier { get; }
        long UpgradeCost { get; }
        bool IsFactoryAlive { get; }
        bool CanUpgrade { get; }

        bool TryUpgrade();

        void Reset();

        void RestoreUpgradeLevel(int level);
    }
}
