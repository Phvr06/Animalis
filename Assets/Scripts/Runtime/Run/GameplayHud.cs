using Animalis.Player;
using UnityEngine;
using TMPro;

namespace Animalis.Run
{
    public sealed class GameplayHud : MonoBehaviour
    {
        [SerializeField] private TMP_Text characterNameText;
        [SerializeField] private TMP_Text healthText;
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private TMP_Text experienceText;

        private PlayerStatsRuntime _boundStats;
        private PlayerExperience _boundExperience;

        public void Bind(PlayerStatsRuntime stats, PlayerExperience experience)
        {
            UnbindCurrent();

            if (stats != null)
            {
                SetCharacterName(stats.Definition != null ? stats.Definition.DisplayName : "Player");
                UpdateHealth(stats.CurrentHealth, stats.MaxHealth);
                stats.HealthChanged += UpdateHealth;
                _boundStats = stats;
            }

            if (experience != null)
            {
                UpdateLevel(experience.CurrentLevel);
                UpdateExperience(experience.CurrentExperience, experience.ExperienceToNextLevel);
                experience.LevelChanged += UpdateLevel;
                experience.ExperienceChanged += UpdateExperience;
                _boundExperience = experience;
            }
        }

        private void SetCharacterName(string value)
        {
            if (characterNameText != null)
            {
                characterNameText.text = value;
            }
        }

        private void UpdateHealth(float current, float max)
        {
            if (healthText != null)
            {
                healthText.text = $"HP {Mathf.CeilToInt(current)}/{Mathf.CeilToInt(max)}";
            }
        }

        private void UpdateLevel(int level)
        {
            if (levelText != null)
            {
                levelText.text = $"LVL {level}";
            }
        }

        private void UpdateExperience(int current, int next)
        {
            if (experienceText != null)
            {
                experienceText.text = $"XP {current}/{next}";
            }
        }

        private void Reset()
        {
            foreach (TMP_Text textComponent in GetComponentsInChildren<TMP_Text>(true))
            {
                switch (textComponent.name)
                {
                    case "CharacterNameText":
                        characterNameText = textComponent;
                        break;
                    case "HealthText":
                        healthText = textComponent;
                        break;
                    case "LevelText":
                        levelText = textComponent;
                        break;
                    case "ExperienceText":
                        experienceText = textComponent;
                        break;
                }
            }
        }

        private void OnDestroy()
        {
            UnbindCurrent();
        }

        private void UnbindCurrent()
        {
            if (_boundStats != null)
            {
                _boundStats.HealthChanged -= UpdateHealth;
                _boundStats = null;
            }

            if (_boundExperience != null)
            {
                _boundExperience.LevelChanged -= UpdateLevel;
                _boundExperience.ExperienceChanged -= UpdateExperience;
                _boundExperience = null;
            }
        }
    }
}
