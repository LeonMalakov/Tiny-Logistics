using System.Collections;
using Code.Infrastructure.SaveLoad.Storage;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Editor.Support {
    [Category("Startup")]
    public abstract class IsolatedSaveStartupTest {
        private const string TimeScaleKey = "TinyLogistics.Tests.TimeScale";

        protected string SaveDirectory => SessionState.GetString(StartupSavePathOverride.DirectoryKey, string.Empty);
        protected string SavePath => System.IO.Path.Combine(SaveDirectory, ProgressSavePaths.FileName);

        [SetUp]
        public void IsolateSaveFiles() {
            // Unity reruns SetUp after the EnterPlayMode domain reload.
            if (!string.IsNullOrEmpty(SaveDirectory))
                return;

            SessionState.SetFloat(TimeScaleKey, Time.timeScale);
            SessionState.SetString(StartupSavePathOverride.DirectoryKey, new TemporarySaveDirectory().DirectoryPath);
        }

        [UnityTearDown]
        public IEnumerator RestoreEditorState() {
            if (Application.isPlaying) {
                Time.timeScale = 1f;
                yield return new ExitPlayMode();
            }

            Time.timeScale = SessionState.GetFloat(TimeScaleKey, 1f);
            string directory = SaveDirectory;
            SessionState.EraseString(StartupSavePathOverride.DirectoryKey);
            SessionState.EraseFloat(TimeScaleKey);

            if (!string.IsNullOrEmpty(directory)) {
                TemporarySaveDirectory.Delete(directory);
            }
        }
    }
}
