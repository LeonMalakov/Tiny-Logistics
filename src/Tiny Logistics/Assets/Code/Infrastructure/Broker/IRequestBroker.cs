namespace Code.Infrastructure.Broker {
    public interface IRequestBroker {
        void PublishFlag<T>();
        bool TryConsumeFlag<T>();
        void Publish<T>(T eventData);
        bool TryGetEventQueue<T>(out BrokerEventQueue<T> queue);
    }
}