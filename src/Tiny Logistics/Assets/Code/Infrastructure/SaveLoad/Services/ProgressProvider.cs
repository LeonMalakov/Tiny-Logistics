using Code.Infrastructure.SaveLoad.Model;

namespace Code.Infrastructure.SaveLoad.Services {
    public class ProgressProvider : IProgressProvider {
        public ProgressData ProgressData { get; private set; }
        
        public void SetProgressData(ProgressData data) {
            ProgressData = data;
        }
    }
}