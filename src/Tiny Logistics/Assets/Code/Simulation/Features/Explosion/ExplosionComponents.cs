using Entitas;
using Code.Infrastructure.Ecs;

namespace Code.Simulation.Features.Explosion {
    [Game, EntityName] public class Explosion : IComponent { }

    [Game] public class ExplosionRadius : IComponent { public float Value; }

    [Game] public class ExplosionDamage : IComponent { public float Value; }

    [Game] public class ExplosionBurnDuration : IComponent { public float Value; }

    [Game] public class ExplosionBurnDamagePerSecond : IComponent { public float Value; }
}
