using System.IO;
using Code.Infrastructure.SaveLoad.Storage;
using NUnit.Framework;
using Tests.Editor.Support;

namespace Tests.Editor.Integration.Persistence {
    [Category("Integration")]
    [Category("Persistence")]
    public sealed class SaveFileStoreTests {
        private TemporarySaveDirectory _directory;
        private ProgressSaveFileStore _files;

        [SetUp]
        public void SetUp() {
            _directory = new TemporarySaveDirectory();
            _files = new ProgressSaveFileStore();
        }

        [TearDown]
        public void TearDown() => _directory.Dispose();

        [Test]
        public void WriteAllText_ExistingSave_ReplacesContentsAndRemovesTemporaryFile() {
            _files.WriteAllText(_directory.SavePath, "first");

            _files.WriteAllText(_directory.SavePath, "second");

            Assert.That(_files.ReadAllText(_directory.SavePath), Is.EqualTo("second"));
            Assert.That(File.Exists(_directory.SavePath + ".tmp"), Is.False);
        }

        [Test]
        public void WriteAllText_TemporaryFileLocked_PreservesPreviousSave() {
            _files.WriteAllText(_directory.SavePath, "original");

            using (FileStream locked = new(_directory.SavePath + ".tmp", FileMode.Create, FileAccess.ReadWrite, FileShare.None)) {
                Assert.Throws<IOException>(() => _files.WriteAllText(_directory.SavePath, "replacement"));
            }

            Assert.That(_files.ReadAllText(_directory.SavePath), Is.EqualTo("original"));
        }
    }
}
