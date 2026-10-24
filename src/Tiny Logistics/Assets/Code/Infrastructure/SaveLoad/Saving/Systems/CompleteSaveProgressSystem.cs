using Code.Infrastructure.Ecs;
using Code.Infrastructure.SaveLoad.Services;
using Entitas;

namespace Code.Infrastructure.SaveLoad.Saving.Systems {
    public sealed class CompleteSaveProgressSystem : IExecuteSystem {
        private readonly ISaveLoadService _saveLoadService;
        private readonly IGroup<GameEntity> _requests;

        public CompleteSaveProgressSystem(GameContext game, ISaveLoadService saveLoadService) {
            _saveLoadService = saveLoadService;
            _requests = game.GetGroup(GameMatcher.SaveProgressRequest);
        }

        public void Execute() {
            if (_requests.count == 0)
                return;

            _saveLoadService.SaveProgress();

            foreach (GameEntity request in _requests.GetEntities(GameEntityBuffer.A))
                request.Destroy();
        }
    }
}
