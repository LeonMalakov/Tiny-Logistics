using System.IO;

namespace Code.Infrastructure.SaveLoad.Storage {
    public sealed class ProgressSavePaths {
        public const string FileName = "progress.json";

        public string PersistentDataPath { get; }
        public string SavePath => Path.Combine(PersistentDataPath, FileName);

        public ProgressSavePaths(string persistentDataPath) {
            PersistentDataPath = persistentDataPath;
        }
    }
}
