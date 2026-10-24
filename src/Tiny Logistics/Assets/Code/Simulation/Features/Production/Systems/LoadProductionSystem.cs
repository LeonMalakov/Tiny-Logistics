using Code.Infrastructure.SaveLoad.Services;
using Code.Simulation.Features.Production.Services;
using Entitas;

namespace Code.Simulation.Features.Production.Systems {
    public sealed class LoadProductionSystem : IInitializeSystem {
        private readonly IProgressProvider _progress;
        private readonly IProductionService _production;

        public LoadProductionSystem(IProgressProvider progress, IProductionService production) {
            _progress = progress;
            _production = production;
        }

        public void Initialize() => _production.RestoreUpgradeLevel(_progress.ProgressData.ProductionUpgradeLevel);
    }
}
