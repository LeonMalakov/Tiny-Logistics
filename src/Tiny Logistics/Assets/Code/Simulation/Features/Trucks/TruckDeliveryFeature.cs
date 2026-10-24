using Code.Infrastructure.Ecs.Systems;
using Code.Simulation.Features.Trucks.Systems;

namespace Code.Simulation.Features.Trucks {
    public sealed class TruckDeliveryFeature : Feature {
        public TruckDeliveryFeature(ISystemFactory factory) {
            Add(factory.Create<TruckDeliverySystem>());
        }
    }
}
