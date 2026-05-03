using System;
using System.IO;
using UnityEngine;

namespace Animalis.Meta
{
    public static class MetaProgressionSaveStore
    {
        private const string SaveDirectoryName = "Saves";
        private const string SaveFileName = "meta_progression.json";
        private const string BackupExtension = ".bak";
        private static MetaProgressionSaveData _cachedData;

        public static string SaveFilePath => Path.Combine(Application.persistentDataPath, SaveDirectoryName, SaveFileName);

        public static MetaProgressionSaveData Load()
        {
            if (_cachedData != null)
            {
                return _cachedData;
            }

            _cachedData = LoadFromDisk();
            Sanitize(_cachedData);
            return _cachedData;
        }

        public static void Save(MetaProgressionSaveData data)
        {
            if (data == null)
            {
                data = new MetaProgressionSaveData();
            }

            Sanitize(data);
            _cachedData = data;

            string savePath = SaveFilePath;
            string directory = Path.GetDirectoryName(savePath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string tempPath = $"{savePath}.tmp";
            string backupPath = $"{savePath}{BackupExtension}";
            string json = JsonUtility.ToJson(data, true);

            File.WriteAllText(tempPath, json);

            if (File.Exists(savePath))
            {
                File.Copy(savePath, backupPath, overwrite: true);
                File.Delete(savePath);
            }

            File.Move(tempPath, savePath);
        }

        private static MetaProgressionSaveData LoadFromDisk()
        {
            string savePath = SaveFilePath;
            if (!File.Exists(savePath))
            {
                return new MetaProgressionSaveData();
            }

            try
            {
                string json = File.ReadAllText(savePath);
                if (string.IsNullOrWhiteSpace(json))
                {
                    return new MetaProgressionSaveData();
                }

                MetaProgressionSaveData data = JsonUtility.FromJson<MetaProgressionSaveData>(json);
                return data ?? new MetaProgressionSaveData();
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"Failed to load meta progression save. A new save will be created. Reason: {exception.Message}");
                BackupCorruptedSave(savePath);
                return new MetaProgressionSaveData();
            }
        }

        private static void BackupCorruptedSave(string savePath)
        {
            try
            {
                if (!File.Exists(savePath))
                {
                    return;
                }

                string timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
                string corruptedPath = $"{savePath}.corrupted_{timestamp}";
                File.Copy(savePath, corruptedPath, overwrite: false);
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"Failed to backup corrupted save file. Reason: {exception.Message}");
            }
        }

        private static void Sanitize(MetaProgressionSaveData data)
        {
            data.version = MetaProgressionSaveData.CurrentVersion;

            if (data.stages == null)
            {
                data.stages = new System.Collections.Generic.List<StageProgressRecord>();
                return;
            }

            for (int i = data.stages.Count - 1; i >= 0; i--)
            {
                StageProgressRecord record = data.stages[i];
                if (record == null || string.IsNullOrWhiteSpace(record.stageId))
                {
                    data.stages.RemoveAt(i);
                }
            }
        }
    }
}
