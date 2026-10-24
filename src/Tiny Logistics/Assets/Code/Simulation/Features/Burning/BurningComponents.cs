using Entitas;

namespace Code.Simulation.Features.Burning {
    [Game] public class Burning : IComponent { }

    [Game] public class BurningTimeLeft : IComponent { public float Value; }

    [Game] public class BurnDamagePerSecond : IComponent { public float Value; }
}
