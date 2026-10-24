using System.Threading;
using Code.Common.Identity;
using Code.Infrastructure.Configs;
using Code.Infrastructure.Ecs.Systems;
using Code.Infrastructure.SaveLoad.Loading;
using Code.Infrastructure.SaveLoad.Services;
using Code.Simulation.Features.Production.Services;
using Code.Simulation.Features.Stockpile.Services;
using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.States.App {
    public class GameInitializeState : IState {
        private readonly AppStateMachine _stateMachine;
        private readonly ISaveLoadService _saveLoadService;
        private readonly IProgressProvider _progressProvider;
        private readonly ISystemFactory _systemFactory;
        private readonly GameContext _game;
        private readonly IIdentityService _identity;
        private readonly IStockpileService _stockpile;
        private readonly IConfigService _configs;
        private readonly IProductionService _production;

        public GameInitializeState(AppStateMachine stateMachine, ISaveLoadService saveLoadService,
            GameContext game, IIdentityService identity, IStockpileService stockpile,
            IConfigService configs, IProductionService production, IProgressProvider progressProvider,
            ISystemFactory systemFactory) {
            
            _stateMachine = stateMachine;
            _saveLoadService = saveLoadService;
            _progressProvider = progressProvider;
            _systemFactory = systemFactory;
            _game = game;
            _identity = identity;
            _stockpile = stockpile;
            _configs = configs;
            _production = production;
        }
        
        public UniTask Enter(CancellationToken ct) {
            ct.ThrowIfCancellationRequested();
            _game.DestroyAllEntities();
            _stockpile.Clear();
            _production.Reset();

            if (_progressProvider.ProgressData != null) {
                _systemFactory.Create<LoadProgressFeature>().Initialize();
                _stateMachine.Enter<GameLoopState>();
                return UniTask.CompletedTask;
            }

            GameEntity factory = _game.CreateEntity();
            factory.isFactory = true;
            factory.AddRuntimeId(_identity.Next());
            factory.AddWorldPosition(_configs.World.FactoryPosition);
            factory.AddCraftProgress(0f);
            factory.AddCraftDuration(_production.CraftDuration);
            factory.AddHealth(_configs.Balance.FactoryHealth);
            factory.AddMaxHealth(_configs.Balance.FactoryHealth);

            _saveLoadService.CreateNewProgress();

            _stateMachine.Enter<GameLoopState>();
            return UniTask.CompletedTask;
        }
        
        public UniTask Exit(CancellationToken ct) => UniTask.CompletedTask;
    }
}
