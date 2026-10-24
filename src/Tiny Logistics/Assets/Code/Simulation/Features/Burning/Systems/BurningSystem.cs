using Code.Common.Time;
using Code.Infrastructure.Ecs;
using Entitas;
using UnityEngine;

namespace Code.Simulation.Features.Burning.Systems {
    public sealed class BurningSystem : IExecuteSystem {
        private readonly ITimeService _time;
        private readonly IGroup<GameEntity> _burning;

        public BurningSystem(GameContext game, ITimeService time) {
            _time = time;

            _burning = game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.Burning,
                    GameMatcher.BurningTimeLeft,
                    GameMatcher.BurnDamagePerSecond,
                    GameMatcher.Health)
                .NoneOf(
                    GameMatcher.Dead,
                    GameMatcher.Destructed));
        }

        public void Execute() {
            foreach (GameEntity entity in _burning.GetEntities(GameEntityBuffer.A)) {
                float burningDelta = Mathf.Min(_time.DeltaTime, Mathf.Max(0f, entity.BurningTimeLeft));

                entity.ReplaceHealth(Mathf.Max(0f, entity.Health - entity.BurnDamagePerSecond * burningDelta));

                float timeLeft = entity.BurningTimeLeft - burningDelta;

                if (timeLeft <= 0f) {
                    entity.StopBurning();
                } else {
                    entity.ReplaceBurningTimeLeft(timeLeft);
                }
            }
        }
        
    }
}
