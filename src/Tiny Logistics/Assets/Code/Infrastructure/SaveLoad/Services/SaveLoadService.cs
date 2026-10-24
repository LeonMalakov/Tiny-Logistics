using System;
using System.IO;
using Code.Infrastructure.SaveLoad.Model;
using Code.Infrastructure.SaveLoad.Storage;
using Code.Infrastructure.SaveLoad.Utility;
using Code.Infrastructure.Serialization;
using Newtonsoft.Json;
using UnityEngine;

namespace Code.Infrastructure.SaveLoad.Services {
    public sealed class SaveLoadService : ISaveLoadService {
        private readonly IProgressProvider _progressProvider;
        private readonly IProgressSaveStorage _storage;

        public bool IsSessionReady { get; private set; }

        public bool HasSavedProgress => _storage.HasProgress();

        public SaveLoadService(IProgressProvider progressProvider, IProgressSaveStorage storage) {
            _progressProvider = progressProvider;
            _storage = storage;
        }

        public void CreateNewProgress() {
            _progressProvider.SetProgressData(new ProgressData { Version = ProgressData.CurrentVersion });
            IsSessionReady = true;
        }

        public void LoadProgress() {
            IsSessionReady = false;
            _progressProvider.SetProgressData(null);

            try {
                if (!HasSavedProgress)
                    return;

                ProgressData data = JsonSerialization.Deserialize<ProgressData>(_storage.ReadProgress());
                SaveLoadUtility.Validate(data);
                _progressProvider.SetProgressData(data);
            } catch (Exception exception) when (exception is IOException || exception is InvalidDataException ||
                                                exception is UnauthorizedAccessException || exception is JsonException) {
                Debug.LogWarning($"[Save] Cannot load session; starting a new game. {exception.Message}");
            }
        }

        public void CompleteLoading() => IsSessionReady = true;

        public void SaveProgress() {
            if (!IsSessionReady)
                return;

            try {
                ProgressData data = _progressProvider.ProgressData;
                _storage.WriteProgress(JsonSerialization.Serialize(data));
            } catch (Exception exception) when (exception is IOException || exception is UnauthorizedAccessException) {
                Debug.LogWarning($"[Save] Cannot save session. {exception.Message}");
            }
        }

        public void ClearProgress() {
            IsSessionReady = false;
            _progressProvider.SetProgressData(null);

            try {
                _storage.ClearProgress();
            } catch (Exception exception) when (exception is IOException || exception is UnauthorizedAccessException) {
                Debug.LogWarning($"[Save] Cannot clear session. {exception.Message}");
            }
        }
    }
}
