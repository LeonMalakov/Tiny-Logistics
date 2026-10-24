using Entitas;
using Code.Infrastructure.Ecs;

namespace Code.Simulation.Features.Crafting {
    [Game, EntityName] public class Factory : IComponent { }

    [Game] public class CraftProgress : IComponent { public float Value; }

    [Game] public class CraftDuration : IComponent { public float Value; }
}
