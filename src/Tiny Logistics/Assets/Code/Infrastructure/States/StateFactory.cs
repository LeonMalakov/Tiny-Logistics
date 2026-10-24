using VContainer;

namespace Code.Infrastructure.States {
    public class StateFactory {
        [Inject]
        private IObjectResolver _resolver;

        public TState Create<TState>() where TState : IState {
            return _resolver.Resolve<TState>();
        }
    }
}