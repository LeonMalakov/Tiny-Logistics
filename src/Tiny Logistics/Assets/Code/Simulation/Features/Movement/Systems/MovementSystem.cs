using Code.Common.Time;
using Entitas;

namespace Code.Simulation.Features.Movement.Systems {
    public sealed class MovementSystem : IExecuteSystem {
        private readonly ITimeService _time;
        private readonly IGroup<GameEntity> _movers;

        public MovementSystem(GameContext game, ITimeService time) {
            _time = time;

            _movers = game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.WorldPosition,
                    GameMatcher.Velocity)
                .NoneOf(
                    GameMatcher.Dead,
                    GameMatcher.Destructed));
        }

        public void Execute() {
            foreach (GameEntity mover in _movers) {
                mover.ReplaceWorldPosition(mover.WorldPosition + mover.Velocity * _time.DeltaTime);
            }
        }
    }
}
