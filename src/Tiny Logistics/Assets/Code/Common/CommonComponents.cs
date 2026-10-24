using Code.Infrastructure.Ecs;
using Entitas;
using UnityEngine;

namespace Code.Common {
    [Game] public class WorldPosition : IComponent { public Vector3 Value; }
    [Game] public class View : IComponent { public EntityView Value; }
    [Game] public class Destructed : IComponent { }
    [Game] public class SelfDestructTimer : IComponent { public float Value; }
}
