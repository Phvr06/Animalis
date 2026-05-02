using System;
using Animalis.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace Animalis.Player
{
    [RequireComponent(typeof(PlayerStatsRuntime))]
    public sealed class PlayerHealth : MonoBehaviour, IDamageable
    {
        [FormerlySerializedAs("stats")]
        [SerializeField] private PlayerStatsRuntime runtimeStats;

        public event Action Died;

        public bool IsAlive => runtimeStats != null && runtimeStats.IsAlive;

        public void ApplyDamage(DamageData damageData)
        {
            if (runtimeStats == null)
            {
                return;
            }

            runtimeStats.TakeDamage(damageData.Amount);
        }

        private void Awake()
        {
            if (runtimeStats == null)
            {
                runtimeStats = GetComponent<PlayerStatsRuntime>();
            }

            runtimeStats.Died += HandleDeath;
        }

        private void OnDestroy()
        {
            if (runtimeStats != null)
            {
                runtimeStats.Died -= HandleDeath;
            }
        }

        private void Reset()
        {
            if (runtimeStats == null)
            {
                runtimeStats = GetComponent<PlayerStatsRuntime>();
            }
        }

        private void HandleDeath()
        {
            Died?.Invoke();
        }
    }
}
