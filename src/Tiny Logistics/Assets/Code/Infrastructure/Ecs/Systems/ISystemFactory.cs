using Entitas;

namespace Code.Infrastructure.Ecs.Systems {
    public interface ISystemFactory {
        T Create<T>() where T : ISystem;
    }
}