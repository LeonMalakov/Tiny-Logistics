using Code.Common.Time;
using Code.Infrastructure.Ecs.Systems;
using Entitas;

namespace Code.Infrastructure.SaveLoad.Saving.Systems {
    public sealed class PeriodicallySaveSystem : TimerExecuteSystem {
        public const float AUTOSAVE_INTERVAL = 5f;

        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _requests;

        public PeriodicallySaveSystem(GameContext game, ITimeService time) : base(time, AUTOSAVE_INTERVAL) {
            _game = game;
            _requests = game.GetGroup(GameMatcher.SaveProgressRequest);
        }

        protected override void Execute() {
            if (_requests.count == 0) {
                _game.CreateEntity().isSaveProgressRequest = true;
            }
        }
        
    }
}
