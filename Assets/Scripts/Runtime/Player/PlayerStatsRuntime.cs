using System;
using Animalis.Characters;
using UnityEngine;

namespace Animalis.Player
{
    public sealed class PlayerStatsRuntime : MonoBehaviour
    {
        [SerializeField, HideInInspector] private CharacterDefinition definition;

        public event Action<float, float> HealthChanged;
        public event Action Died;

        public CharacterDefinition Definition => definition;
        public float CurrentHealth { get; private set; }
        public float MaxHealth => definition != null ? definition.MaxHealth : 0f;
        public float MoveSpeed => definition != null ? definition.MoveSpeed : 0f;
        public float PickupRadius => definition != null ? definition.PickupRadius : 0f;
        public bool IsAlive => CurrentHealth > 0f;

        public void Initialize(CharacterDefinition characterDefinition)
        {
            definition = characterDefinition;
            CurrentHealth = MaxHealth;
            HealthChanged?.Invoke(CurrentHealth, MaxHealth);
        }

        public void RestoreToFull()
        {
            CurrentHealth = MaxHealth;
            HealthChanged?.Invoke(CurrentHealth, MaxHealth);
        }

        public void TakeDamage(float amount)
        {
            if (!IsAlive || amount <= 0f)
            {
                return;
            }

            CurrentHealth = Mathf.Max(0f, CurrentHealth - amount);
            HealthChanged?.Invoke(CurrentHealth, MaxHealth);

            if (CurrentHealth <= 0f)
            {
                Died?.Invoke();
            }
        }

        private void OnValidate()
        {
            if (definition == null)
            {
                return;
            }

            CurrentHealth = Mathf.Clamp(CurrentHealth <= 0f ? definition.MaxHealth : CurrentHealth, 0f, definition.MaxHealth);
        }
    }
}
