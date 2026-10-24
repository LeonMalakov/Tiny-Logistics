namespace Code.Infrastructure.SaveLoad.Services {
    public interface ISaveLoadService {
        bool HasSavedProgress { get; }
        bool IsSessionReady { get; }

        void CreateNewProgress();
        void LoadProgress();
        void CompleteLoading();
        void SaveProgress();
        void ClearProgress();
    }
}
