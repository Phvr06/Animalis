using Animalis.Core;
using UnityEngine;

namespace Animalis.Combat
{
    public sealed class TrainingDummyTarget : MonoBehaviour, IDamageable
    {
        [Min(1f)]
        [Tooltip("Health of this test target.")]
        [SerializeField] private float maxHealth = 8f;
        [SerializeField] private SpriteRenderer targetRenderer;
        [SerializeField] private Color aliveColor = new(0.8f, 0.25f, 0.25f, 1f);
        [SerializeField] private Color hitColor = new(1f, 0.9f, 0.35f, 1f);

        private float _currentHealth;
        private float _hitFlashTimer;

        public bool IsAlive => _currentHealth > 0f;

        public void ApplyDamage(DamageData damageData)
        {
            if (!IsAlive)
            {
                return;
            }

            _currentHealth = Mathf.Max(0f, _currentHealth - damageData.Amount);
            _hitFlashTimer = 0.1f;

            if (_currentHealth <= 0f)
            {
                Destroy(gameObject);
            }
        }

        private void Awake()
        {
            _currentHealth = maxHealth;

            if (targetRenderer == null)
            {
                targetRenderer = GetComponentInChildren<SpriteRenderer>();
            }

            if (targetRenderer != null)
            {
                targetRenderer.color = aliveColor;
            }
        }

        private void Update()
        {
            if (targetRenderer == null)
            {
                return;
            }

            _hitFlashTimer -= Time.deltaTime;
            targetRenderer.color = _hitFlashTimer > 0f ? hitColor : aliveColor;
        }

        private void Reset()
        {
            targetRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        private void OnValidate()
        {
            maxHealth = Mathf.Max(1f, maxHealth);
        }
    }
}
