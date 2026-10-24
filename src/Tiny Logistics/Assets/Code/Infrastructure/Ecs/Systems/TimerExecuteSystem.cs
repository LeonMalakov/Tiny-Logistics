using Code.Common.Time;
using Entitas;

namespace Code.Infrastructure.Ecs.Systems {
    public abstract class TimerExecuteSystem : IExecuteSystem {
        private readonly ITimeService _time;
        
        protected readonly float SystemPeriod;
        
        private float _timeToNextExecute;

        protected TimerExecuteSystem(ITimeService time, float period, float startDelay = 0f) {
            _time = time;
            SystemPeriod = period;
            _timeToNextExecute = startDelay;
        }

        protected abstract void Execute();

        void IExecuteSystem.Execute() {
            _timeToNextExecute -= _time.DeltaTime;

            if (_timeToNextExecute > 0) {
                return;
            }

            _timeToNextExecute = SystemPeriod;

            Execute();
        }
    }
}