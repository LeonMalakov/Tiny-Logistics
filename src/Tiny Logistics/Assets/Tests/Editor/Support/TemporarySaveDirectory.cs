using System;
using System.IO;
using Code.Infrastructure.SaveLoad.Storage;

namespace Tests.Editor.Support {
    internal sealed class TemporarySaveDirectory : IDisposable {
        public string DirectoryPath { get; } = Path.Combine(Path.GetTempPath(), "TinyLogisticsTests-" + Guid.NewGuid().ToString("N"));
        public string SavePath => Path.Combine(DirectoryPath, ProgressSavePaths.FileName);

        public void Dispose() => Delete(DirectoryPath);

        public static void Delete(string directory) {
            if (!Directory.Exists(directory))
                return;

            foreach (string file in Directory.GetFiles(directory)) {
                File.Delete(file);
            }

            Directory.Delete(directory);
        }
    }
}
