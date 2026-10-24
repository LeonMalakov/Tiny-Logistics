using System.Collections.Generic;

namespace Code.Infrastructure.Broker {
    public class BrokerEventQueue<T> : BrokerEventQueueBase {
        private readonly Queue<T> _queue = new();
        private readonly RequestBroker _eventBroker;

        public BrokerEventQueue(RequestBroker eventBroker) {
            _eventBroker = eventBroker;
        }
        
        public void Enqueue(T eventData) {
            _queue.Enqueue(eventData);
            RefreshCount();
        }

        public bool TryDequeue(out T eventData) {
            if (_queue.TryDequeue(out eventData)) {
                RefreshCount();
                return true;
            }
            return false;
        }

        private void RefreshCount() {
            _eventBroker.OnQueueCountChanged<T>(_queue.Count);
        }
    }
    
    public class BrokerEventQueueBase { }
}