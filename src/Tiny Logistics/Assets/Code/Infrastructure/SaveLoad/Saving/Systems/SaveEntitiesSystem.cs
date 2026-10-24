using Code.Infrastructure.SaveLoad.Services;
using Code.Infrastructure.SaveLoad.Utility;
using Entitas;

namespace Code.Infrastructure.SaveLoad.Saving.Systems {
    public sealed class SaveEntitiesSystem : IExecuteSystem {
        private readonly GameContext _game;
        private readonly IProgressProvider _progress;
        private readonly IGroup<GameEntity> _requests;

        public SaveEntitiesSystem(GameContext game, IProgressProvider progress) {
            _game = game;
            _progress = progress;
            _requests = game.GetGroup(GameMatcher.SaveProgressRequest);
        }

        public void Execute() {
            if (_requests.count == 0)
                return;

            _progress.ProgressData.Entities = SaveLoadUtility.CaptureEntities(_game);
        }
    }
}
