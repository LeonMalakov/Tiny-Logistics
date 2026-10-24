using Code.Infrastructure.SaveLoad.Model;

namespace Code.Infrastructure.SaveLoad.Services {
    public interface IProgressProvider {
        ProgressData ProgressData { get; }
        void SetProgressData(ProgressData data);
    }
}