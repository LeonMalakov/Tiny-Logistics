namespace Code.Infrastructure.SaveLoad.Storage {
    public interface IProgressSaveStorage {
        string SavePath { get; }
        bool HasProgress();
        string ReadProgress();
        void WriteProgress(string json);
        void ClearProgress();
    }
}
