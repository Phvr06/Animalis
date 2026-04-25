using System;
using UnityEngine;

namespace Animalis.Run
{
    public sealed class PlayerExperience : MonoBehaviour
    {
        [Min(1)]
        [Tooltip("XP needed to go from level 1 to level 2.")]
        [SerializeField] private int baseExperienceForLevel = 5;
        [Min(1f)]
        [Tooltip("Multiplier applied to XP requirement every level.")]
        [SerializeField] private float growthFactor = 1.3f;

        public event Action<int> LevelChanged;
        public event Action<int, int> ExperienceChanged;

        public int CurrentLevel { get; private set; } = 1;
        public int CurrentExperience { get; private set; }
        public int ExperienceToNextLevel => Mathf.Max(1, Mathf.RoundToInt(baseExperienceForLevel * Mathf.Pow(growthFactor, CurrentLevel - 1)));

        public void AddExperience(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            CurrentExperience += amount;

            while (CurrentExperience >= ExperienceToNextLevel)
            {
                CurrentExperience -= ExperienceToNextLevel;
                CurrentLevel++;
                LevelChanged?.Invoke(CurrentLevel);
            }

            ExperienceChanged?.Invoke(CurrentExperience, ExperienceToNextLevel);
        }

        private void Start()
        {
            ExperienceChanged?.Invoke(CurrentExperience, ExperienceToNextLevel);
            LevelChanged?.Invoke(CurrentLevel);
        }

        private void OnValidate()
        {
            baseExperienceForLevel = Mathf.Max(1, baseExperienceForLevel);
            growthFactor = Mathf.Max(1f, growthFactor);
        }
    }
}
