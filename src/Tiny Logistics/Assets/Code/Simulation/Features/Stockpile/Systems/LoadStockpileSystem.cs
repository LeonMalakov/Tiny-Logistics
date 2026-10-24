using Code.Infrastructure.SaveLoad.Services;
using Code.Simulation.Features.Stockpile.Services;
using Entitas;

namespace Code.Simulation.Features.Stockpile.Systems {
    public sealed class LoadStockpileSystem : IInitializeSystem {
        private readonly IProgressProvider _progress;
        private readonly IStockpileService _stockpile;

        public LoadStockpileSystem(IProgressProvider progress, IStockpileService stockpile) {
            _progress = progress;
            _stockpile = stockpile;
        }

        public void Initialize() {
            _stockpile.Clear();

            foreach (var item in _progress.ProgressData.Stockpile) {
                if (item.Value > 0) {
                    _stockpile.Add(item.Key, item.Value);
                }
            }
        }
        
    }
}
