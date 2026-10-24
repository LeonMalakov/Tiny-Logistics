using Code.Common.Time;
using Code.Infrastructure.Ecs;
using Entitas;

namespace Code.Common.ProcessDestructed.Systems {
    public class SelfDestructTimerSystem : IExecuteSystem {
        private readonly ITimeService _time;
        private readonly IGroup<GameEntity> _entities;

        public SelfDestructTimerSystem(GameContext game, ITimeService time) {
            _time = time;
            _entities = game.GetGroup(GameMatcher.SelfDestructTimer);
        }


        public void Execute() {
            foreach (GameEntity entity in _entities.GetEntities(GameEntityBuffer.A)) {
                float timeLeft = entity.SelfDestructTimer - _time.DeltaTime;
                if (timeLeft > 0f) {
                    entity.ReplaceSelfDestructTimer(timeLeft);
                } else {
                    entity.RemoveSelfDestructTimer();
                    entity.isDestructed = true;
                }
            }
        }
    }
}
