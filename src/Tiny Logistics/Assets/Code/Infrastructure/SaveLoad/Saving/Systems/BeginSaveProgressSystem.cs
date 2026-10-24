using Code.Infrastructure.Ecs;
using Code.Infrastructure.SaveLoad.Model;
using Code.Infrastructure.SaveLoad.Services;
using Entitas;

namespace Code.Infrastructure.SaveLoad.Saving.Systems {
    public sealed class BeginSaveProgressSystem : IExecuteSystem {
        private readonly IProgressProvider _progress;
        private readonly ISaveLoadService _saveLoadService;
        private readonly IGroup<GameEntity> _requests;

        public BeginSaveProgressSystem(GameContext game, IProgressProvider progress, ISaveLoadService saveLoadService) {
            _progress = progress;
            _saveLoadService = saveLoadService;
            _requests = game.GetGroup(GameMatcher.SaveProgressRequest);
        }

        public void Execute() {
            if (_requests.count == 0)
                return;

            if (!_saveLoadService.IsSessionReady) {
                foreach (GameEntity request in _requests.GetEntities(GameEntityBuffer.A))
                    request.Destroy();

                return;
            }

            _progress.SetProgressData(new ProgressData { Version = ProgressData.CurrentVersion });
        }
    }
}
