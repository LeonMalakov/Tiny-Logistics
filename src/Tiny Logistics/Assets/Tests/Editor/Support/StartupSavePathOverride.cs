using System;
using Code.Infrastructure.SaveLoad.Storage;
using UnityEditor;
using VContainer;
using VContainer.Unity;

namespace Tests.Editor.Support {
    // SessionState survives the domain reload performed by EnterPlayMode.
    [InitializeOnLoad]
    internal static class StartupSavePathOverride {
        public const string DirectoryKey = "TinyLogistics.Tests.SaveDirectory";
        private static IDisposable _installation;

        static StartupSavePathOverride() {
            Install();
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        private static void OnPlayModeChanged(PlayModeStateChange state) {
            if (state == PlayModeStateChange.ExitingEditMode) {
                Install();
            } else if (state == PlayModeStateChange.EnteredEditMode) {
                _installation?.Dispose();
                _installation = null;
            }
        }

        private static void Install() {
            string directory = SessionState.GetString(DirectoryKey, string.Empty);

            if (_installation != null || string.IsNullOrEmpty(directory))
                return;

            _installation = LifetimeScope.Enqueue(builder => ReplaceSavePaths(builder, directory));
        }

        private static void ReplaceSavePaths(IContainerBuilder builder, string directory) {
            if (!builder.Exists(typeof(ProgressSavePaths)))
                return;

            ContainerBuilder replacement = new();
            RegistrationBuilder paths = replacement.RegisterInstance(new ProgressSavePaths(directory));

            // VContainer rejects duplicate singleton implementations, so replace the existing registration.
            for (int i = 0; i < builder.Count; i++) {
                if (builder[i].Build().ImplementationType == typeof(ProgressSavePaths)) {
                    builder[i] = paths;

                    return;
                }
            }
        }
    }
}
