using Code.Infrastructure.UiDispatcher;
using NSubstitute;
using NUnit.Framework;

namespace Tests.Editor.Unit.Infrastructure {
    [Category("Unit")]
    [Category("Infrastructure")]
    public sealed class UiDispatcherTests {
        [Test]
        public void Flush_SingleBytePayloads_DeliversAllEventsInOrder() {
            UiDispatcher dispatcher = new();
            IUiEventListener<BytePayload> listener = Substitute.For<IUiEventListener<BytePayload>>();
            dispatcher.AddListener<BytePayload>(listener);
            dispatcher.Publish(new BytePayload(1));
            dispatcher.Publish(new BytePayload(2));

            listener.DidNotReceive().OnUiEvent(Arg.Any<BytePayload>());

            dispatcher.Flush();

            Received.InOrder(() => {
                listener.OnUiEvent(Arg.Is<BytePayload>(payload => payload.Value == 1));
                listener.OnUiEvent(Arg.Is<BytePayload>(payload => payload.Value == 2));
            });
            listener.Received(2).OnUiEvent(Arg.Any<BytePayload>());

            dispatcher.Flush();

            listener.Received(2).OnUiEvent(Arg.Any<BytePayload>());

            dispatcher.RemoveListener<BytePayload>(listener);
            dispatcher.Publish(new BytePayload(3));
            dispatcher.Flush();

            listener.Received(2).OnUiEvent(Arg.Any<BytePayload>());
            listener.DidNotReceive().OnUiEvent(Arg.Is<BytePayload>(payload => payload.Value == 3));
        }

        public readonly struct BytePayload : IUiPayloadEvent {
            public readonly byte Value;

            public BytePayload(byte value) => Value = value;
        }
    }
}
