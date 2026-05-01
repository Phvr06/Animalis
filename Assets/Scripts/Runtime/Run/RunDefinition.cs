using UnityEngine;

namespace Animalis.Run
{
    [CreateAssetMenu(menuName = "Animalis/Run/Run Definition", fileName = "RunDefinition")]
    public sealed class RunDefinition : ScriptableObject
    {
        [Header("Progression")]
        [Min(1)]
        [Tooltip("XP needed to go from level 1 to level 2.")]
        [SerializeField] private int baseExperienceForLevel = 5;
        [Min(1f)]
        [Tooltip("Multiplier applied to XP requirement every level.")]
        [SerializeField] private float experienceGrowthFactor = 1.3f;

        [Header("Flow")]
        [Tooltip("Freeze gameplay time when the player dies.")]
        [SerializeField] private bool pauseOnDefeat = true;

        [Header("Enemy Spawning")]
        [Min(0.2f)]
        [Tooltip("Initial delay between spawns in seconds.")]
        [SerializeField] private float startingSpawnInterval = 2.5f;
        [Min(0.2f)]
        [Tooltip("Lowest allowed spawn interval after scaling.")]
        [SerializeField] private float minimumSpawnInterval = 0.8f;
        [Min(0f)]
        [Tooltip("How much spawn interval is reduced per minute.")]
        [SerializeField] private float intervalRampPerMinute = 0.55f;
        [Min(1)]
        [Tooltip("Maximum enemies alive at time zero.")]
        [SerializeField] private int startingMaxAlive = 10;
        [Min(0)]
        [Tooltip("Additional enemies allowed alive every minute.")]
        [SerializeField] private int extraAlivePerMinute = 6;
        [Min(1f)]
        [Tooltip("Distance from the player where enemies are spawned.")]
        [SerializeField] private float spawnRadius = 9f;
        [Min(0f)]
        [Tooltip("Extra random distance added to each spawn position.")]
        [SerializeField] private float spawnJitter = 2f;

        public int BaseExperienceForLevel => baseExperienceForLevel;
        public float ExperienceGrowthFactor => experienceGrowthFactor;
        public bool PauseOnDefeat => pauseOnDefeat;
        public float StartingSpawnInterval => startingSpawnInterval;
        public float MinimumSpawnInterval => minimumSpawnInterval;
        public float IntervalRampPerMinute => intervalRampPerMinute;
        public int StartingMaxAlive => startingMaxAlive;
        public int ExtraAlivePerMinute => extraAlivePerMinute;
        public float SpawnRadius => spawnRadius;
        public float SpawnJitter => spawnJitter;

        public int GetExperienceToNextLevel(int currentLevel)
        {
            int level = Mathf.Max(1, currentLevel);
            return Mathf.Max(1, Mathf.RoundToInt(baseExperienceForLevel * Mathf.Pow(experienceGrowthFactor, level - 1)));
        }

        private void OnValidate()
        {
            baseExperienceForLevel = Mathf.Max(1, baseExperienceForLevel);
            experienceGrowthFactor = Mathf.Max(1f, experienceGrowthFactor);
            startingSpawnInterval = Mathf.Max(0.2f, startingSpawnInterval);
            minimumSpawnInterval = Mathf.Max(0.2f, minimumSpawnInterval);
            intervalRampPerMinute = Mathf.Max(0f, intervalRampPerMinute);
            startingMaxAlive = Mathf.Max(1, startingMaxAlive);
            extraAlivePerMinute = Mathf.Max(0, extraAlivePerMinute);
            spawnRadius = Mathf.Max(1f, spawnRadius);
            spawnJitter = Mathf.Max(0f, spawnJitter);
        }
    }
}
