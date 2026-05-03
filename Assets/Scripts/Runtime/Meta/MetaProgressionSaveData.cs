using System;
using System.Collections.Generic;

namespace Animalis.Meta
{
    [Serializable]
    public sealed class MetaProgressionSaveData
    {
        public const int CurrentVersion = 1;

        public int version = CurrentVersion;
        public List<StageProgressRecord> stages = new();

        public StageProgressRecord GetOrCreateStage(string stageId)
        {
            if (string.IsNullOrWhiteSpace(stageId))
            {
                return null;
            }

            for (int i = 0; i < stages.Count; i++)
            {
                StageProgressRecord record = stages[i];
                if (record != null && string.Equals(record.stageId, stageId, StringComparison.OrdinalIgnoreCase))
                {
                    return record;
                }
            }

            StageProgressRecord created = new()
            {
                stageId = stageId
            };
            stages.Add(created);
            return created;
        }

        public StageProgressRecord FindStage(string stageId)
        {
            if (string.IsNullOrWhiteSpace(stageId))
            {
                return null;
            }

            for (int i = 0; i < stages.Count; i++)
            {
                StageProgressRecord record = stages[i];
                if (record != null && string.Equals(record.stageId, stageId, StringComparison.OrdinalIgnoreCase))
                {
                    return record;
                }
            }

            return null;
        }
    }

    [Serializable]
    public sealed class StageProgressRecord
    {
        public string stageId;
        public bool unlocked;
        public bool completed;
    }
}
