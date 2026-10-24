using Code.Infrastructure.Broker;
using Entitas;
using UnityEngine;

namespace Code.Infrastructure.Ecs.Systems {
    public abstract class BrokerRequestHandlerSystem<T> : IExecuteSystem {
        private readonly IRequestBroker _requestBroker;

        protected BrokerRequestHandlerSystem(IRequestBroker requestBroker) {
            _requestBroker = requestBroker;
        }

        protected abstract void Handle(T data);

        void IExecuteSystem.Execute() {
            if (!_requestBroker.TryGetEventQueue<T>(out BrokerEventQueue<T> queue)) {
                return;
            }
            
            while(queue.TryDequeue(out T data)) {
                Handle(data);
            }
        }
    }
}