using Code.Common.ProcessDestructed.Systems;
using Code.Infrastructure.Ecs.Systems;

namespace Code.Common.ProcessDestructed {
    public class ProcessDestructedFeature : Feature {
        public ProcessDestructedFeature(ISystemFactory factory) {
            Add(factory.Create<SelfDestructTimerSystem>());
            Add(factory.Create<CleanupEntityViewSystem>());
            Add(factory.Create<CleanupDestructedSystem>());
        }
    }
}
