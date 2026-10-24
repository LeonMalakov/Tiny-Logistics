using Code.Infrastructure.UiDispatcher;

namespace Code.Simulation.Features.Production.Model {
    public static class ProductionUiEvents {
        public readonly struct Changed : IUiSignalEvent { }
    }
}
