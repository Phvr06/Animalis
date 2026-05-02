using Animalis.Content;
using UnityEngine;

namespace Animalis.Combat
{
    [CreateAssetMenu(menuName = "Animalis/Data/Weapon", fileName = "NewWeapon")]
    public sealed class WeaponDefinition : ContentDefinition
    {
        [Header("Projectile")]
        [Tooltip("Prefab instantiated by the auto-weapon system.")]
        [SerializeField] private Projectile projectilePrefab;
        [Tooltip("If disabled, the projectile core sprite is hidden and only the prefab VFX remains visible.")]
        [SerializeField] private bool useProjectileSprite = true;
        [Tooltip("Optional projectile sprite override. If empty, the prefab visual keeps its current sprite.")]
        [SerializeField] private Sprite projectileSprite;
        [Tooltip("Tint applied to the projectile renderer.")]
        [SerializeField] private Color projectileColor = new(0.75f, 0.95f, 1f, 1f);
        [Tooltip("Visual scale of the projectile.")]
        [SerializeField] private Vector2 projectileScale = new(0.5f, 0.18f);
        [Min(0.1f)]
        [Tooltip("Projectile movement speed in units per second.")]
        [SerializeField] private float projectileSpeed = 12f;
        [Min(0.05f)]
        [Tooltip("How long the projectile stays alive before despawning.")]
        [SerializeField] private float projectileLifetime = 1.5f;
        [Min(0.1f)]
        [Tooltip("Damage dealt on hit.")]
        [SerializeField] private float projectileDamage = 2f;

        [Header("Firing")]
        [Min(0.05f)]
        [Tooltip("Time between automatic shots.")]
        [SerializeField] private float fireInterval = 0.45f;
        [Min(0.5f)]
        [Tooltip("Maximum range for finding targets.")]
        [SerializeField] private float targetRange = 8f;
        [Tooltip("Spawn offset of the projectile relative to the player.")]
        [SerializeField] private Vector2 muzzleOffset = new(0.45f, 0f);

        public string WeaponId => ContentId;
        public Projectile ProjectilePrefab => projectilePrefab;
        public bool UseProjectileSprite => useProjectileSprite;
        public Sprite ProjectileSprite => projectileSprite;
        public Color ProjectileColor => projectileColor;
        public Vector2 ProjectileScale => projectileScale;
        public float ProjectileSpeed => projectileSpeed;
        public float ProjectileLifetime => projectileLifetime;
        public float ProjectileDamage => projectileDamage;
        public float FireInterval => fireInterval;
        public float TargetRange => targetRange;
        public Vector2 MuzzleOffset => muzzleOffset;

        private void OnValidate()
        {
            ValidateIdentity();
            projectileScale.x = Mathf.Max(0.05f, projectileScale.x);
            projectileScale.y = Mathf.Max(0.05f, projectileScale.y);
            projectileSpeed = Mathf.Max(0.1f, projectileSpeed);
            projectileLifetime = Mathf.Max(0.05f, projectileLifetime);
            projectileDamage = Mathf.Max(0.1f, projectileDamage);
            fireInterval = Mathf.Max(0.05f, fireInterval);
            targetRange = Mathf.Max(0.5f, targetRange);
        }
    }
}
