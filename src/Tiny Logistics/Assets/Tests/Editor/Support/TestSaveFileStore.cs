using System.Collections.Generic;
using Code.Infrastructure.SaveLoad.Storage;

namespace Tests.Editor.Support {
    internal sealed class TestSaveFileStore : IProgressSaveFileStore {
        private readonly Dictionary<string, string> _files = new();

        public int Writes { get; private set; }

        public bool FileExists(string path) => _files.ContainsKey(path);

        public string ReadAllText(string path) => _files[path];

        public void WriteAllText(string path, string text) {
            _files[path] = text;
            Writes++;
        }

        public void DeleteIfExists(string path) => _files.Remove(path);
    }
}
