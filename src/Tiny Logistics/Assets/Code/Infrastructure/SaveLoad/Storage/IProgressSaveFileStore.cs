namespace Code.Infrastructure.SaveLoad.Storage {
    public interface IProgressSaveFileStore {
        bool FileExists(string path);
        string ReadAllText(string path);
        void WriteAllText(string path, string text);
        void DeleteIfExists(string path);
    }
}
