using Code.Common;
using Code.Infrastructure.AssetManagement;
using Code.Infrastructure.Configs.Model;
using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.Configs {
    public sealed class ConfigService : IConfigService {
        private readonly IAssetRepository _assetRepository;
        private BalanceConfig _balance;
        private WorldConfig _world;
        private ViewConfig _view;
        
        public BalanceConfig Balance => _balance != null 
            ? _balance 
            : ThrowException.InvalidOperation<BalanceConfig>("Configs have not been loaded.");
        
        public WorldConfig World => _world != null 
            ? _world 
            : ThrowException.InvalidOperation<WorldConfig>("Configs have not been loaded.");
        
        public ViewConfig View => _view != null 
            ? _view 
            : ThrowException.InvalidOperation<ViewConfig>("Configs have not been loaded.");
        
        public ConfigService(IAssetRepository assetRepository) {
            _assetRepository = assetRepository;
        }
        
        public UniTask LoadAll() {
            ConfigRepository repository = _assetRepository.Configs;
            
            if (repository == null || repository.Balance == null || repository.World == null || repository.View == null) {
                ThrowException.InvalidOperation("AssetRepository must reference ConfigRepository with BalanceConfig, WorldConfig and ViewConfig.");
            }
            
            repository.Balance.Validate();
            repository.World.Validate();
            repository.View.Validate();
            
            _balance = repository.Balance;
            _world = repository.World;
            _view = repository.View;
            
            return UniTask.CompletedTask;
        }
        
    }
}
