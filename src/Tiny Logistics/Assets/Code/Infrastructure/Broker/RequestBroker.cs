using System.Collections.Generic;
using System.Threading;

namespace Code.Infrastructure.Broker {
    public class RequestBroker : IRequestBroker {
        private readonly Dictionary<int, BrokerEventQueueBase> _events = new();
        private readonly Dictionary<int, bool> _hasEvents = new();
        
        private readonly HashSet<int> _flags = new();

        public void PublishFlag<T>() {
            int index = EventType<T>.Index;
            _flags.Add(index);
        }

        public bool TryConsumeFlag<T>() {
            int index = EventType<T>.Index;
            return _flags.Remove(index);
        }

        public void Publish<T>(T eventData) {
            int index = EventType<T>.Index;
            var queue = GetEventQueue<T>(index);
            queue.Enqueue(eventData);
        }
        
        public bool TryGetEventQueue<T>(out BrokerEventQueue<T> queue) {
            int index = EventType<T>.Index;

            if (!_hasEvents.GetValueOrDefault(index, false)) {
                queue = null;
                return false;
            }
            
            queue = GetEventQueue<T>(index);
            return true;
        }

        private BrokerEventQueue<T> GetEventQueue<T>(int index) {
            if (!_events.TryGetValue(index, out BrokerEventQueueBase list)) {
                list = new BrokerEventQueue<T>(this);
                _events[index] = list;
            }

            return (BrokerEventQueue<T>)list;
        }

        public void OnQueueCountChanged<T>(int queueCount) {
            _hasEvents[EventType<T>.Index] = queueCount > 0;
        }

        private static class EventType<T> {
            // ReSharper disable StaticMemberInGenericType
            internal static readonly int Index;
            // ReSharper restore StaticMemberInGenericType

            static EventType() {
                Index = Interlocked.Increment(ref EventTypeTotal.Total);
            }
        }

        private static class EventTypeTotal {
            internal static int Total = -1;
        }
    }
}