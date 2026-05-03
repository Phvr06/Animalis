using Animalis.Meta;

namespace Animalis.Stage
{
    public static class StageProgressionService
    {
        public static void EnsureStageRegistered(StageDefinition stage)
        {
            if (stage == null)
            {
                return;
            }

            MetaProgressionSaveData saveData = MetaProgressionSaveStore.Load();
            StageProgressRecord record = saveData.GetOrCreateStage(stage.StageId);
            bool shouldSave = false;

            if (stage.StartsUnlocked && !record.unlocked)
            {
                record.unlocked = true;
                shouldSave = true;
            }

            if (shouldSave)
            {
                MetaProgressionSaveStore.Save(saveData);
            }
        }

        public static bool IsStageUnlocked(StageDefinition stage)
        {
            if (stage == null)
            {
                return false;
            }

            MetaProgressionSaveData saveData = MetaProgressionSaveStore.Load();
            StageProgressRecord record = saveData.FindStage(stage.StageId);
            return record != null ? record.unlocked : stage.StartsUnlocked;
        }

        public static bool HasCompletedStage(StageDefinition stage)
        {
            if (stage == null)
            {
                return false;
            }

            MetaProgressionSaveData saveData = MetaProgressionSaveStore.Load();
            StageProgressRecord record = saveData.FindStage(stage.StageId);
            return record != null && record.completed;
        }

        public static void RegisterStageVictory(StageDefinition stage)
        {
            if (stage == null)
            {
                return;
            }

            MetaProgressionSaveData saveData = MetaProgressionSaveStore.Load();
            StageProgressRecord currentStageRecord = saveData.GetOrCreateStage(stage.StageId);
            currentStageRecord.unlocked = true;
            currentStageRecord.completed = true;

            if (stage.NextStage != null)
            {
                StageProgressRecord nextStageRecord = saveData.GetOrCreateStage(stage.NextStage.StageId);
                nextStageRecord.unlocked = true;
            }

            MetaProgressionSaveStore.Save(saveData);
        }
    }
}
