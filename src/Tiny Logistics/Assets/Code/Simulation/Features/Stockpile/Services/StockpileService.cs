using System.Collections.Generic;
using System.Collections.ObjectModel;
using Code.Common;
using Code.Infrastructure.UiDispatcher;
using Code.Simulation.Features.Stockpile.Model;

namespace Code.Simulation.Features.Stockpile.Services {
    public sealed class StockpileService : IStockpileService {
        private readonly Dictionary<ItemId, long> _items = new();
        private readonly IUiDispatcher _uiDispatcher;

        public IReadOnlyDictionary<ItemId, long> Items { get; }

        public StockpileService(IUiDispatcher uiDispatcher) {
            _uiDispatcher = uiDispatcher;
            Items = new ReadOnlyDictionary<ItemId, long>(_items);
        }

        public long GetAmount(ItemId itemId) => _items.GetValueOrDefault(itemId, 0L);

        public void Add(ItemId itemId, long amount) {
            ValidateAmount(itemId, amount);
            _items[itemId] = checked(GetAmount(itemId) + amount);
            _uiDispatcher.Publish<StockpileUiEvents.Changed>();
        }

        public bool TryConsume(ItemId itemId, long amount) {
            ValidateAmount(itemId, amount);

            long available = GetAmount(itemId);

            if (available < amount) {
                return false;
            }

            long remaining = available - amount;

            if (remaining == 0) {
                _items.Remove(itemId);
            } else {
                _items[itemId] = remaining;
            }

            _uiDispatcher.Publish<StockpileUiEvents.Changed>();

            return true;
        }

        public void Clear() {
            _items.Clear();
            _uiDispatcher.Publish<StockpileUiEvents.Changed>();
        }

        private static void ValidateAmount(ItemId itemId, long amount) {
            if (itemId == ItemId.None) {
                ThrowException.ArgumentOutOfRange(nameof(itemId));
            }

            if (amount <= 0) {
                ThrowException.ArgumentOutOfRange(nameof(amount));
            }
        }
        
    }
}
