using System.Collections.Generic;
using System.Threading;
using Code.Common;

namespace Code.Infrastructure.UiDispatcher {
    public interface IUiEventListener<in T> {
        void OnUiEvent(T eventData);
    }

    public interface IUiDispatcherQueue {
        public void Flush();
    }
    
    public interface IUiDispatcherQueue<out T> : IUiDispatcherQueue {
        void AddListener(IUiEventListener<T> listener);
        void RemoveListener(IUiEventListener<T> listener);
    }

    public interface IUiEvent { }

    public interface IUiSignalEvent : IUiEvent { }

    public interface IUiPayloadEvent : IUiEvent { }

    public enum UiEventType {
        None = 0,
        Signal,
        Payload
    }
    
    public class UiDispatcherPayloadQueue<T> : IUiDispatcherQueue<T> {
        private readonly List<T> _events = new(32);
        private readonly List<T> _eventsBuffer = new(32);
        private readonly List<IUiEventListener<T>> _listeners = new();
        private readonly List<IUiEventListener<T>> _listenersBuffer = new();
        
        public void EnqueueEvent(T eventData) => _events.Add(eventData);

        public void AddListener(IUiEventListener<T> listener) => _listeners.Add(listener);

        public void RemoveListener(IUiEventListener<T> listener) => _listeners.Remove(listener);

        public void Flush() {
            if (_events.Count <= 0)
                return;

            _eventsBuffer.Clear();
            _eventsBuffer.AddRange(_events);
            _events.Clear();

            _listenersBuffer.Clear();
            _listenersBuffer.AddRange(_listeners);

            for (int i = 0; i < _eventsBuffer.Count; i++) {
                T evt = _eventsBuffer[i];

                for (int j = 0; j < _listenersBuffer.Count; j++) {
                    _listenersBuffer[j].OnUiEvent(evt);
                }
            }

            _listenersBuffer.Clear();
            _eventsBuffer.Clear();
        }
    }
    
    public class UiDispatcherSignalQueue<T> : IUiDispatcherQueue<T> {
        private readonly List<IUiEventListener<T>> _listeners = new();
        private readonly List<IUiEventListener<T>> _listenersBuffer = new();
        private bool _raised;

        public void Raise() => _raised = true;

        public void AddListener(IUiEventListener<T> listener) => _listeners.Add(listener);

        public void RemoveListener(IUiEventListener<T> listener) => _listeners.Remove(listener);

        public void Flush() {
            if(!_raised) {
                return;
            }

            _raised = false;
            
            T eventData = default(T);

            _listenersBuffer.Clear();
            _listenersBuffer.AddRange(_listeners);
            
            for (int j = 0; j < _listenersBuffer.Count; j++) {
                _listenersBuffer[j].OnUiEvent(eventData);
            }

            _listenersBuffer.Clear();
        }
    }

    public interface IUiDispatcher {
        void Flush();
        void AddListener<T>(IUiEventListener<T> listener) where T : struct, IUiEvent;
        void RemoveListener<T>(IUiEventListener<T> listener) where T : struct, IUiEvent;
        void Publish<T>(T eventData) where T : struct, IUiPayloadEvent;
        void Publish<T>() where T : struct, IUiSignalEvent;
    }

    public class UiDispatcher : IUiDispatcher {
        private readonly Dictionary<int, IUiDispatcherQueue> _queues = new();
        private readonly List<IUiDispatcherQueue> _flushQueuesBuffer = new(64);

        public void Flush() {
            _flushQueuesBuffer.Clear();

            foreach (IUiDispatcherQueue queue in _queues.Values)
                _flushQueuesBuffer.Add(queue);

            for (int i = 0; i < _flushQueuesBuffer.Count; i++)
                _flushQueuesBuffer[i].Flush();

            _flushQueuesBuffer.Clear();
        }

        public void AddListener<T>(IUiEventListener<T> listener) where T : struct, IUiEvent  {
            int index = EventType<T>.Index;
            UiEventType eventType = EventType<T>.UiEventType;
            var queue = GetQueue<T>(index, eventType);
            queue.AddListener(listener);
        }

        public void RemoveListener<T>(IUiEventListener<T> listener) where T : struct, IUiEvent  {
            int index = EventType<T>.Index;
            UiEventType eventType = EventType<T>.UiEventType;
            var queue = GetQueue<T>(index, eventType);
            queue.RemoveListener(listener);
        }

        public void Publish<T>(T eventData) where T : struct, IUiPayloadEvent {
            int index = EventType<T>.Index;
            UiEventType eventType = EventType<T>.UiEventType;

            if (eventType != UiEventType.Payload) {
                ThrowException.InvalidOperation();
                return;
            }
            
            GetPayloadQueue<T>(index).EnqueueEvent(eventData);
        }
        
        public void Publish<T>() where T : struct, IUiSignalEvent {
            int index = EventType<T>.Index;
            UiEventType eventType = EventType<T>.UiEventType;

            if (eventType != UiEventType.Signal) {
                ThrowException.InvalidOperation();
                return;
            }
            
            GetSignalQueue<T>(index).Raise();
        }
        
        private IUiDispatcherQueue<T> GetQueue<T>(int index, UiEventType eventType) where T : struct, IUiEvent {
            return eventType switch {
                UiEventType.Payload => GetPayloadQueue<T>(index),
                UiEventType.Signal => GetSignalQueue<T>(index),
                _ => ThrowException.NotImplemented<IUiDispatcherQueue<T>>()
            };
        }
        
        private UiDispatcherPayloadQueue<T> GetPayloadQueue<T>(int index) {
            if (!_queues.TryGetValue(index, out IUiDispatcherQueue list)) {
                list = new UiDispatcherPayloadQueue<T>();
                _queues[index] = list;
            }

            return (UiDispatcherPayloadQueue<T>)list;
        }
        
        private UiDispatcherSignalQueue<T> GetSignalQueue<T>(int index) {
            if (!_queues.TryGetValue(index, out IUiDispatcherQueue list)) {
                list = new UiDispatcherSignalQueue<T>();
                _queues[index] = list;
            }

            return (UiDispatcherSignalQueue<T>)list;
        }

        private static class EventType<T> where T : struct, IUiEvent {
            // ReSharper disable StaticMemberInGenericType
            internal static readonly int Index;
            internal static readonly UiEventType UiEventType;
            // ReSharper restore StaticMemberInGenericType

            static EventType() {
                Index = Interlocked.Increment(ref EventTypeTotal.Total);
                
                if (typeof(IUiSignalEvent).IsAssignableFrom(typeof(T))) {
                    UiEventType = UiEventType.Signal;
                } else if (typeof(IUiPayloadEvent).IsAssignableFrom(typeof(T))) {
                    UiEventType = UiEventType.Payload;
                } else {
                    ThrowException.Default();
                }
            }
        }

        private static class EventTypeTotal {
            internal static int Total = -1;
        }
    }
}
