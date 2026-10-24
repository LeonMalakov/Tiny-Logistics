using Entitas;

namespace Code.Infrastructure.UiDispatcher {
    public class UiDispatcherFlushSystem : IExecuteSystem {
        private readonly IUiDispatcher _uiDispatcher;
        
        public UiDispatcherFlushSystem(IUiDispatcher uiDispatcher) {
            _uiDispatcher = uiDispatcher;
        }
        
        public void Execute() {
            _uiDispatcher.Flush();
        }
    }
}