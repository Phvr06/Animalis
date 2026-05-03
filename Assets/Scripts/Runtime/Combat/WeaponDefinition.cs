using Animalis.Content;
using UnityEngine;

namespace Animalis.Combat
{
    [CreateAssetMenu(menuName = "Animalis/Data/Weapon", fileName = "NewWeapon")]
    public sealed class WeaponDefinition : ContentDefinition
    {
        [Header("Identity")]
        [SerializeField] private ElementType element = ElementType.Fire;
        [SerializeField] private WeaponCategory category = WeaponCategory.Elemental;

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

        [Header("Fire Status")]
        [Min(0f)]
        [SerializeField] private float burnDamagePerSecond = 1f;
        [Min(0f)]
        [SerializeField] private float burnDuration = 3f;

        [Header("Fire Upgrades")]
        [Range(0f, 1f)]
        [SerializeField] private float chainExplosionChance;
        [Min(0f)]
        [SerializeField] private float chainExplosionRadius = 2.25f;
        [Min(0f)]
        [SerializeField] private float chainExplosionDamage = 3f;
        [SerializeField] private bool fireTrailEnabled;
        [Min(0f)]
        [SerializeField] private float fireTrailDamagePerSecond = 1f;
        [Min(0.05f)]
        [SerializeField] private float fireTrailDuration = 1.1f;
        [Min(0.1f)]
        [SerializeField] private float fireTrailRadius = 0.85f;
        [Min(0.05f)]
        [SerializeField] private float fireTrailSpawnInterval = 0.18f;

        public string WeaponId => ContentId;
        public ElementType Element => element;
        public WeaponCategory Category => category;
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
        public float BurnDamagePerSecond => burnDamagePerSecond;
        public float BurnDuration => burnDuration;
        public float ChainExplosionChance => chainExplosionChance;
        public float ChainExplosionRadius => chainExplosionRadius;
        public float ChainExplosionDamage => chainExplosionDamage;
        public bool FireTrailEnabled => fireTrailEnabled;
        public float FireTrailDamagePerSecond => fireTrailDamagePerSecond;
        public float FireTrailDuration => fireTrailDuration;
        public float FireTrailRadius => fireTrailRadius;
        public float FireTrailSpawnInterval => fireTrailSpawnInterval;

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
            burnDamagePerSecond = Mathf.Max(0f, burnDamagePerSecond);
            burnDuration = Mathf.Max(0f, burnDuration);
            chainExplosionChance = Mathf.Clamp01(chainExplosionChance);
            chainExplosionRadius = Mathf.Max(0f, chainExplosionRadius);
            chainExplosionDamage = Mathf.Max(0f, chainExplosionDamage);
            fireTrailDamagePerSecond = Mathf.Max(0f, fireTrailDamagePerSecond);
            fireTrailDuration = Mathf.Max(0.05f, fireTrailDuration);
            fireTrailRadius = Mathf.Max(0.1f, fireTrailRadius);
            fireTrailSpawnInterval = Mathf.Max(0.05f, fireTrailSpawnInterval);
        }
    }
}
