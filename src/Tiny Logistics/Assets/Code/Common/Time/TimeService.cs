namespace Code.Common.Time {
    public class TimeService : ITimeService {
        public float DeltaTime => UnityEngine.Time.deltaTime;
    }
}