using System.Threading;
using Code.Constants;
using Code.Infrastructure.Configs;
using Code.Infrastructure.Loading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Infrastructure.States.App {
    public class BootState : IState {
        private readonly AppStateMachine _stateMachine;
        private readonly ISceneLoadService _sceneLoadService;
        private readonly IConfigService _configService;

        public BootState(
            AppStateMachine stateMachine,
            ISceneLoadService sceneLoadService,
            IConfigService configService) {

            _stateMachine = stateMachine;
            _sceneLoadService = sceneLoadService;
            _configService = configService;
        }

        public async UniTask Enter(CancellationToken ct) {
            await _sceneLoadService.Load(SceneConstants.BOOT);
            await _configService.LoadAll();

            _stateMachine.Enter<GameLoadingState>();
        }

        public UniTask Exit(CancellationToken ct) => UniTask.CompletedTask;
    }
}