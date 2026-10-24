using Code.Infrastructure.SaveLoad.Services;
using Code.Simulation.Features.Production.Services;
using Entitas;

namespace Code.Simulation.Features.Production.Systems {
    public sealed class SaveProductionSystem : IExecuteSystem {
        private readonly IProgressProvider _progress;
        private readonly IProductionService _production;
        private readonly IGroup<GameEntity> _requests;

        public SaveProductionSystem(GameContext game, IProgressProvider progress, IProductionService production) {
            _progress = progress;
            _production = production;
            _requests = game.GetGroup(GameMatcher.SaveProgressRequest);
        }

        public void Execute() {
            if (_requests.count == 0)
                return;

            _progress.ProgressData.ProductionUpgradeLevel = _production.UpgradeLevel;
        }
    }
}
