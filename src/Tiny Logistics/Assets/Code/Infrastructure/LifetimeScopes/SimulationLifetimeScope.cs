using Code.Simulation.Features.ProductionUi.Windows;
using VContainer;
using VContainer.Unity;

namespace Code.Infrastructure.LifetimeScopes {
    public class SimulationLifetimeScope : LifetimeScope {
        protected override void Configure(IContainerBuilder builder) {
            base.Configure(builder);
            builder.RegisterComponentInHierarchy<ProductionUi>();
        }
    }
}
