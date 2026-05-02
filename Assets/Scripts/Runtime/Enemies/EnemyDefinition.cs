using Animalis.Content;
using UnityEngine;

namespace Animalis.Enemies
{
    [CreateAssetMenu(menuName = "Animalis/Data/Enemy", fileName = "NewEnemy")]
    public sealed class EnemyDefinition : ContentDefinition
    {
        [Header("Combat")]
        [Min(1f)]
        [SerializeField] private float maxHealth = 4f;
        [Min(0.1f)]
        [SerializeField] private float moveSpeed = 2f;
        [Min(0.1f)]
        [SerializeField] private float contactDamage = 1f;
        [Min(0.05f)]
        [SerializeField] private float contactDamageCooldown = 0.7f;
        [Min(1)]
        [SerializeField] private int experienceValue = 1;

        [Header("Placeholder Visual")]
        [SerializeField] private Sprite worldSprite;
        [SerializeField] private Color worldColor = new(0.78f, 0.22f, 0.18f, 1f);
        [SerializeField] private Vector2 worldScale = new(0.72f, 0.72f);

        public string EnemyId => ContentId;
        public float MaxHealth => maxHealth;
        public float MoveSpeed => moveSpeed;
        public float ContactDamage => contactDamage;
        public float ContactDamageCooldown => contactDamageCooldown;
        public int ExperienceValue => experienceValue;
        public Sprite WorldSprite => worldSprite;
        public Color WorldColor => worldColor;
        public Vector2 WorldScale => worldScale;

        private void OnValidate()
        {
            ValidateIdentity();
            maxHealth = Mathf.Max(1f, maxHealth);
            moveSpeed = Mathf.Max(0.1f, moveSpeed);
            contactDamage = Mathf.Max(0.1f, contactDamage);
            contactDamageCooldown = Mathf.Max(0.05f, contactDamageCooldown);
            experienceValue = Mathf.Max(1, experienceValue);
            worldScale.x = Mathf.Max(0.1f, worldScale.x);
            worldScale.y = Mathf.Max(0.1f, worldScale.y);
        }
    }
}
