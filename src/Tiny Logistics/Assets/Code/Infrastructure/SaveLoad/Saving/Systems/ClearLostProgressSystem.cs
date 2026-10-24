using Code.Infrastructure.SaveLoad.Services;
using Code.Simulation.Features.Production.Services;
using Entitas;

namespace Code.Infrastructure.SaveLoad.Saving.Systems {
    public sealed class ClearLostProgressSystem : IExecuteSystem {
        private readonly ISaveLoadService _saveLoadService;
        private readonly IProductionService _production;

        public ClearLostProgressSystem(ISaveLoadService saveLoadService, IProductionService production) {
            _saveLoadService = saveLoadService;
            _production = production;
        }

        public void Execute() {
            if (_saveLoadService.IsSessionReady && !_production.IsFactoryAlive)
                _saveLoadService.ClearProgress();
        }
    }
}
