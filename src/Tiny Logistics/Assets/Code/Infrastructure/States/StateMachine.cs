using System;
using System.Collections.Generic;
using System.Threading;
using Code.Common;
using Code.Common.Utility;
using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.States {
    public abstract class StateMachine : IStateMachine, IDisposable {
        private readonly Dictionary<int, IBaseState> _states = new();

        private readonly CancellationTokenSource _lifetimeCts = new();
        
        private int _currentStateIndex = -1;
        
        public void Dispose() {
            _lifetimeCts.TryCancelAndDispose();
        }

        public void Enter<TState>() where TState : IState => 
            EnterAsync<TState>().Forget();

        public void Enter<TState, TPayload>(TPayload payload) where TState : IState<TPayload> =>
            EnterAsync<TState, TPayload>(payload).Forget();

        public void Register<TState>(TState state) where TState : IBaseState {
            _states.Add(StateIndexer<TState>.Index, state);
        }

        private async UniTaskVoid EnterAsync<TState>() where TState : IState {
            var state = await ChangeState<TState>();
            state.Enter(_lifetimeCts.Token).Forget();
        }

        private async UniTaskVoid EnterAsync<TState, TPayload>(TPayload payload) where TState : IState<TPayload> {
            var state = await ChangeState<TState>();
            state.Enter(payload, _lifetimeCts.Token).Forget();
        }

        private async UniTask<TState> ChangeState<TState>() where TState : IBaseState {
            if (_states.TryGetValue(_currentStateIndex, out var state)) {
                await state.Exit(_lifetimeCts.Token);
            }

            _currentStateIndex = StateIndexer<TState>.Index;
            
            return GetState<TState>();
        }

        private TState GetState<TState>() where TState : IBaseState {
            if(!_states.TryGetValue(_currentStateIndex, out var newState)) {
                ThrowException.KeyNotFound($"State '{typeof(TState)}' not found");
                return default;
            }
            
            if(newState is not TState typedState) {
                ThrowException.InvalidCast($"State '{typeof(TState)}' not found");
                return default;
            }

            return typedState;
        }
    }
}