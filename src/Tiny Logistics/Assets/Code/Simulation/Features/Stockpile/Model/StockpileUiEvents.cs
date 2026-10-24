using Code.Infrastructure.UiDispatcher;

namespace Code.Simulation.Features.Stockpile.Model {
    public static class StockpileUiEvents {
        public readonly struct Changed : IUiSignalEvent { }
    }
}
