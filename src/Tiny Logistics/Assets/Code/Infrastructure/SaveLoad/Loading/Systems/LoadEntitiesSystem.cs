using Code.Common.Identity;
using Code.Infrastructure.SaveLoad.Services;
using Code.Infrastructure.SaveLoad.Utility;
using Code.Simulation.Features.Production.Services;
using Entitas;

namespace Code.Infrastructure.SaveLoad.Loading.Systems {
    public sealed class LoadEntitiesSystem : IInitializeSystem {
        private readonly GameContext _game;
        private readonly IProgressProvider _progress;
        private readonly IIdentityService _identity;
        private readonly IProductionService _production;

        public LoadEntitiesSystem(GameContext game, IProgressProvider progress,
            IIdentityService identity, IProductionService production) {
            _game = game;
            _progress = progress;
            _identity = identity;
            _production = production;
        }

        public void Initialize() {
            SaveLoadUtility.RestoreEntities(
                _progress.ProgressData.Entities, 
                _game, 
                _identity,
                _production.CraftDuration);
        }
        
    }
}
