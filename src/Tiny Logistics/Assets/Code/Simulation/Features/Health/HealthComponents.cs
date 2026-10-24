using Entitas;

namespace Code.Simulation.Features.Health {
    [Game] public class Health : IComponent { public float Value; }

    [Game] public class MaxHealth : IComponent { public float Value; }

    [Game] public class Dead : IComponent { }
}
