using System.Threading;
using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.States {
    public interface IBaseState {
        UniTask Exit(CancellationToken ct);
    }

    public interface IState : IBaseState {
        UniTask Enter(CancellationToken ct);
    }

    public interface IState<in T> : IBaseState {
        UniTask Enter(T payload, CancellationToken ct);
    }
}