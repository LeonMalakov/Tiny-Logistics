using System.Threading;
using Code.Constants;
using Code.Infrastructure.Loading;
using Code.Infrastructure.SaveLoad.Services;
using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.States.App {
    public class GameLoadingState : IState {
        private readonly AppStateMachine _stateMachine;
        private readonly ISceneLoadService _sceneLoadService;
        private readonly ISaveLoadService _saveLoadService;
        
        public GameLoadingState(AppStateMachine stateMachine, ISceneLoadService sceneLoadService, ISaveLoadService saveLoadService) {
            _stateMachine = stateMachine;
            _sceneLoadService = sceneLoadService;
            _saveLoadService = saveLoadService;
        }

        public async UniTask Enter(CancellationToken ct) {
            ct.ThrowIfCancellationRequested();
            
            _saveLoadService.LoadProgress();
            
            await _sceneLoadService.Load(SceneConstants.SIMULATION, forceReload: true);
            ct.ThrowIfCancellationRequested();
            
            _stateMachine.Enter<GameInitializeState>();
        }

        public UniTask Exit(CancellationToken ct) => UniTask.CompletedTask;
    }
}
