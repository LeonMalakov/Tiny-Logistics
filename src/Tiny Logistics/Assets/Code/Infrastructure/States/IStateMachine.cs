namespace Code.Infrastructure.States {
    public interface IStateMachine {
        void Enter<TState>() where TState : IState;
        void Enter<TState, TPayload>(TPayload payload) where TState : IState<TPayload>;
    }
}