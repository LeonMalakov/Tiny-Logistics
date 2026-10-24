using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace Code.Common.Identity {
    /// <summary>
    /// Временный идентификатор сущности.
    /// </summary>
    [Game] public class RuntimeId : IComponent { [PrimaryEntityIndex] public int Value; }
}
