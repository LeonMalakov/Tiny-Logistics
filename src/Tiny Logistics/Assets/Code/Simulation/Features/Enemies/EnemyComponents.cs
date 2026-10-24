using Code.Infrastructure.Ecs;
using Entitas;

namespace Code.Simulation.Features.Enemies {
    [Game, EntityName] public class Enemy : IComponent { }

    [Game] public class EnemySpeed : IComponent { public float Value; }
}
