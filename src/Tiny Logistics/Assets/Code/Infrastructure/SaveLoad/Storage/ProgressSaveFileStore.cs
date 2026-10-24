using System.IO;
using System.Text;

namespace Code.Infrastructure.SaveLoad.Storage {
    public sealed class ProgressSaveFileStore : IProgressSaveFileStore {
        public bool FileExists(string path) => File.Exists(path);

        public string ReadAllText(string path) => File.ReadAllText(path, Encoding.UTF8);

        public void WriteAllText(string path, string text) {
            string directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(directory))
                Directory.CreateDirectory(directory);

            string temporaryPath = path + ".tmp";
            File.WriteAllText(temporaryPath, text, Encoding.UTF8);

            if (File.Exists(path))
                File.Replace(temporaryPath, path, null);
            else
                File.Move(temporaryPath, path);
        }

        public void DeleteIfExists(string path) {
            if (File.Exists(path))
                File.Delete(path);
        }
    }
}
