using System.Threading;
using Code.Infrastructure.Ecs.Systems;
using Code.Infrastructure.SaveLoad.Saving;
using Code.Simulation;
using Cysharp.Threading.Tasks;
using Entitas;

namespace Code.Infrastructure.States.App {
    public class GameLoopState : TickableState {
        private readonly ISystemFactory _systemFactory;
        private readonly GameContext _game;
        private readonly AppStateMachine _stateMachine;
        private readonly IGroup<GameEntity> _factories;
        private EveryFrameFeature _feature;
        private SaveProgressFeature _saveProgressFeature;

        public GameLoopState(ISystemFactory systemFactory, GameContext game, AppStateMachine stateMachine) {
            _systemFactory = systemFactory;
            _game = game;
            _stateMachine = stateMachine;
            _factories = game.GetGroup(GameMatcher.Factory);
        }

        protected override UniTask OnEnter(CancellationToken ct) {
            _feature = _systemFactory.Create<EveryFrameFeature>();
            _saveProgressFeature = _systemFactory.Create<SaveProgressFeature>();
            _feature.Initialize();
            return UniTask.CompletedTask;
        }

        protected override void OnTick() {
            _feature.Execute();
            _feature.Cleanup();
            _saveProgressFeature.Execute();

            // Transition after the frame is complete and the dead factory has been removed.
            if (_factories.count == 0) {
                _stateMachine.Enter<GameLoadingState>();
            }
        }

        public void SaveProgress() {
            if (!Running || _saveProgressFeature == null) {
                return;
            }

            _game.CreateEntity().isSaveProgressRequest = true;
            _saveProgressFeature.Execute();
        }

        protected override UniTask OnExit(CancellationToken ct) {
            _feature.DeactivateReactiveSystems();
            _feature.ClearReactiveSystems();
            
            foreach (GameEntity entity in _game.GetEntities()) {
                entity.isDestructed = true;
            }
            
            _feature.Cleanup();
            _feature.TearDown();
            _saveProgressFeature.TearDown();
            _feature = null;
            _saveProgressFeature = null;
            
            return UniTask.CompletedTask;
        }
    }
}
