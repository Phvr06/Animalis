using System;
using UnityEngine;

namespace Animalis.Run
{
    public sealed class PlayerExperience : MonoBehaviour
    {
        [SerializeField] private RunDefinition runDefinition;

        public event Action<int> LevelChanged;
        public event Action<int, int> ExperienceChanged;

        public RunDefinition RunDefinition => runDefinition;
        public int CurrentLevel { get; private set; } = 1;
        public int CurrentExperience { get; private set; }
        public int ExperienceToNextLevel => runDefinition != null ? runDefinition.GetExperienceToNextLevel(CurrentLevel) : 1;

        public void Initialize(RunDefinition definition)
        {
            runDefinition = definition;
        }

        public void AddExperience(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            CurrentExperience += amount;

            while (CurrentExperience >= ExperienceToNextLevel)
            {
                int requiredExperience = ExperienceToNextLevel;
                CurrentExperience -= requiredExperience;
                CurrentLevel++;
                LevelChanged?.Invoke(CurrentLevel);
            }

            ExperienceChanged?.Invoke(CurrentExperience, ExperienceToNextLevel);
        }

        private void Start()
        {
            CurrentLevel = Mathf.Max(1, CurrentLevel);
            ExperienceChanged?.Invoke(CurrentExperience, ExperienceToNextLevel);
            LevelChanged?.Invoke(CurrentLevel);
        }

        private void OnValidate()
        {
            CurrentLevel = Mathf.Max(1, CurrentLevel);
        }
    }
}
