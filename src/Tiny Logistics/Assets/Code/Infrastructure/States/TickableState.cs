using System.Threading;
using Cysharp.Threading.Tasks;
using VContainer.Unity;

namespace Code.Infrastructure.States {
    public abstract class TickableState : IState, ITickable {
        protected bool Running { get; private set; }
        
        public UniTask Enter(CancellationToken ct) {
            Running = true;
            return OnEnter(ct);
        }
        
        public async UniTask Exit(CancellationToken ct) {
            Running = false;
            await UniTask.Yield(cancellationToken: ct);
            await OnExit(ct);
        }

        public void Tick() {
            if (Running) {
                OnTick();
            }
        }

        protected abstract UniTask OnEnter(CancellationToken ct);
        
        protected abstract UniTask OnExit(CancellationToken ct);
        
        protected abstract void OnTick();
    }
}