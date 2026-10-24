namespace Code.Infrastructure.States.App {
    public class AppStateMachine : StateMachine {
        private readonly AppStateFactory _factory;

        public AppStateMachine(AppStateFactory factory) {
            _factory = factory;
        }

        public void RegisterStates() {
            Register(_factory.Create<BootState>());
            Register(_factory.Create<GameLoadingState>());
            Register(_factory.Create<GameInitializeState>());
            Register(_factory.Create<GameLoopState>());
        }
    }

    public class AppStateFactory : StateFactory { }
}