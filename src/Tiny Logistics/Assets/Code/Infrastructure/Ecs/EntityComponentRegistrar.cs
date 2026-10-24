namespace Code.Infrastructure.Ecs {
    public abstract class EntityComponentRegistrar : EntityDependantBehaviour {
        public abstract void RegisterComponents();
        public abstract void UnregisterComponents();
    }
}