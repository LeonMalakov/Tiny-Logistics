using Code.Common.Time;
using Code.Infrastructure.Configs;
using Code.Simulation.Features.Trucks.Factories;
using Entitas;
using UnityEngine;

namespace Code.Simulation.Features.Crafting.Systems {
    public sealed class CraftSystem : IExecuteSystem {
        private readonly ITimeService _time;
        private readonly ITruckFactory _trucks;
        private readonly IConfigService _configs;
        private readonly IGroup<GameEntity> _factories;

        public CraftSystem(GameContext game, ITimeService time, ITruckFactory trucks, IConfigService configs) {
            _time = time;
            _trucks = trucks;
            _configs = configs;

            _factories = game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.Factory,
                    GameMatcher.CraftProgress,
                    GameMatcher.CraftDuration,
                    GameMatcher.WorldPosition,
                    GameMatcher.Health)
                .NoneOf(
                    GameMatcher.Dead,
                    GameMatcher.Destructed));
        }

        public void Execute() {
            if (_time.DeltaTime <= 0f)
                return;

            foreach (GameEntity factory in _factories) {
                if (factory.Health <= 0f)
                    continue;

                float progress = factory.CraftProgress + _time.DeltaTime / factory.CraftDuration;

                while (progress >= 1f) {
                    _trucks.Create(factory.WorldPosition + _configs.World.TruckSpawnOffset);
                    progress -= 1f;
                }

                factory.ReplaceCraftProgress(Mathf.Max(0f, progress));
            }
        }
    }
}
