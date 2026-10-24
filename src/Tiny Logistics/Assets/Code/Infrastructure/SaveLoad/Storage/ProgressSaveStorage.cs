namespace Code.Infrastructure.SaveLoad.Storage {
    public sealed class ProgressSaveStorage : IProgressSaveStorage {
        private readonly ProgressSavePaths _paths;
        private readonly IProgressSaveFileStore _fileStore;

        public string SavePath => _paths.SavePath;

        public ProgressSaveStorage(ProgressSavePaths paths, IProgressSaveFileStore fileStore) {
            _paths = paths;
            _fileStore = fileStore;
        }

        public bool HasProgress() => _fileStore.FileExists(SavePath);

        public string ReadProgress() => _fileStore.ReadAllText(SavePath);

        public void WriteProgress(string json) => _fileStore.WriteAllText(SavePath, json);

        public void ClearProgress() {
            _fileStore.DeleteIfExists(SavePath);
            _fileStore.DeleteIfExists(SavePath + ".tmp");
        }
    }
}
