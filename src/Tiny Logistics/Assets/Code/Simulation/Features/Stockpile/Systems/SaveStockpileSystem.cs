using Code.Infrastructure.SaveLoad.Services;
using Code.Simulation.Features.Stockpile.Services;
using Entitas;

namespace Code.Simulation.Features.Stockpile.Systems {
    public sealed class SaveStockpileSystem : IExecuteSystem {
        private readonly IProgressProvider _progress;
        private readonly IStockpileService _stockpile;
        private readonly IGroup<GameEntity> _requests;

        public SaveStockpileSystem(GameContext game, IProgressProvider progress, IStockpileService stockpile) {
            _progress = progress;
            _stockpile = stockpile;
            _requests = game.GetGroup(GameMatcher.SaveProgressRequest);
        }

        public void Execute() {
            if (_requests.count == 0)
                return;

            var items = _progress.ProgressData.Stockpile;
            items.Clear();

            foreach (var item in _stockpile.Items)
                items.Add(item.Key, item.Value);
        }
    }
}
