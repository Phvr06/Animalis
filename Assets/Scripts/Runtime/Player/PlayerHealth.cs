using System;
using Animalis.Core;
using UnityEngine;

namespace Animalis.Player
{
    [RequireComponent(typeof(PlayerStatsRuntime))]
    public sealed class PlayerHealth : MonoBehaviour, IDamageable
    {
        [SerializeField] private PlayerStatsRuntime stats;

        public event Action Died;

        public bool IsAlive => stats != null && stats.IsAlive;

        public void ApplyDamage(DamageData damageData)
        {
            if (stats == null)
            {
                return;
            }

            stats.TakeDamage(damageData.Amount);
        }

        private void Awake()
        {
            if (stats == null)
            {
                stats = GetComponent<PlayerStatsRuntime>();
            }

            stats.Died += HandleDeath;
        }

        private void OnDestroy()
        {
            if (stats != null)
            {
                stats.Died -= HandleDeath;
            }
        }

        private void HandleDeath()
        {
            Died?.Invoke();
        }
    }
}
