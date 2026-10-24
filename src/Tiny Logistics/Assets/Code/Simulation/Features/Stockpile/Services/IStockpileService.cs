using System.Collections.Generic;
using Code.Simulation.Features.Stockpile.Model;

namespace Code.Simulation.Features.Stockpile.Services {
    public interface IStockpileService {
        IReadOnlyDictionary<ItemId, long> Items { get; }

        long GetAmount(ItemId itemId);

        void Add(ItemId itemId, long amount);

        bool TryConsume(ItemId itemId, long amount);

        void Clear();
    }
}
