using System.Collections.Generic;
using Animalis.Characters;
using Animalis.Core;
using Animalis.Player;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;

namespace Animalis.Combat
{
    [RequireComponent(typeof(PlayerStatsRuntime))]
    public sealed class AutoWeaponController : MonoBehaviour
    {
        [FormerlySerializedAs("stats")]
        [SerializeField] private PlayerStatsRuntime runtimeStats;
        [SerializeField, HideInInspector] private Transform projectileParent;

        private readonly Collider2D[] _targetBuffer = new Collider2D[32];
        private readonly List<Projectile> _activeProjectiles = new();

        private IObjectPool<Projectile> _projectilePool;
        private ContactFilter2D _targetFilter;
        private WeaponDefinition _weaponDefinition;
        private float _cooldown;

        public void Initialize(CharacterDefinition definition, Transform runtimeProjectileParent)
        {
            runtimeStats = runtimeStats != null ? runtimeStats : GetComponent<PlayerStatsRuntime>();
            projectileParent = runtimeProjectileParent;
            _weaponDefinition = ResolveWeaponDefinition(definition);

            if (_weaponDefinition == null || _weaponDefinition.ProjectilePrefab == null)
            {
                return;
            }

            _projectilePool = new ObjectPool<Projectile>(CreateProjectile, OnGetProjectile, OnReleaseProjectile, OnDestroyProjectile);
            _cooldown = _weaponDefinition.FireInterval;
        }

        private void Awake()
        {
            if (runtimeStats == null)
            {
                runtimeStats = GetComponent<PlayerStatsRuntime>();
            }

            _targetFilter = ContactFilter2D.noFilter;
        }

        private void Update()
        {
            if (_weaponDefinition == null || _projectilePool == null || runtimeStats == null || !runtimeStats.IsAlive)
            {
                return;
            }

            _cooldown -= Time.deltaTime;
            if (_cooldown > 0f)
            {
                return;
            }

            if (!TryFindNearestEnemy(out Vector2 targetPosition))
            {
                return;
            }

            FireAt(targetPosition);
            _cooldown = _weaponDefinition.FireInterval;
        }

        private bool TryFindNearestEnemy(out Vector2 targetPosition)
        {
            int count = Physics2D.OverlapCircle((Vector2)transform.position, _weaponDefinition.TargetRange, _targetFilter, _targetBuffer);
            float bestDistance = float.MaxValue;
            targetPosition = default;

            for (int i = 0; i < count; i++)
            {
                Collider2D collider = _targetBuffer[i];
                if (collider == null || !collider.CompareTag("Enemy"))
                {
                    continue;
                }

                float distance = ((Vector2)collider.transform.position - (Vector2)transform.position).sqrMagnitude;
                if (distance >= bestDistance)
                {
                    continue;
                }

                bestDistance = distance;
                targetPosition = collider.transform.position;
            }

            return bestDistance < float.MaxValue;
        }

        private void FireAt(Vector2 targetPosition)
        {
            Vector2 origin = (Vector2)transform.position;
            Vector2 direction = (targetPosition - origin).normalized;
            if (direction == Vector2.zero)
            {
                direction = Vector2.right;
            }

            origin += direction * _weaponDefinition.MuzzleOffset.x;
            origin += Vector2.up * _weaponDefinition.MuzzleOffset.y;

            Projectile projectile = _projectilePool.Get();
            projectile.Launch(_weaponDefinition, origin, direction, gameObject, ReleaseProjectile);
        }

        private Projectile CreateProjectile()
        {
            Projectile projectile = Instantiate(_weaponDefinition.ProjectilePrefab, projectileParent);
            projectile.gameObject.SetActive(false);
            return projectile;
        }

        private void OnGetProjectile(Projectile projectile)
        {
            if (!_activeProjectiles.Contains(projectile))
            {
                _activeProjectiles.Add(projectile);
            }
        }

        private void OnReleaseProjectile(Projectile projectile)
        {
            _activeProjectiles.Remove(projectile);
            if (projectile != null)
            {
                projectile.gameObject.SetActive(false);
            }
        }

        private void OnDestroyProjectile(Projectile projectile)
        {
            if (projectile != null)
            {
                Destroy(projectile.gameObject);
            }
        }

        private void ReleaseProjectile(Projectile projectile)
        {
            if (_projectilePool != null)
            {
                _projectilePool.Release(projectile);
            }
        }

        private WeaponDefinition ResolveWeaponDefinition(CharacterDefinition definition)
        {
            if (definition == null)
            {
                Debug.LogWarning("Auto weapon controller requires a character definition.", this);
                return null;
            }

            if (definition.StartingWeapon == null)
            {
                Debug.LogWarning($"Character '{definition.DisplayName}' has no starting weapon assigned.", this);
                return null;
            }

            if (definition.StartingWeapon.ProjectilePrefab == null)
            {
                Debug.LogWarning($"Weapon '{definition.StartingWeapon.DisplayName}' has no projectile prefab assigned.", this);
                return null;
            }

            return definition.StartingWeapon;
        }

        private void OnDrawGizmosSelected()
        {
            if (_weaponDefinition == null)
            {
                return;
            }

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, _weaponDefinition.TargetRange);
        }

        private void Reset()
        {
            if (runtimeStats == null)
            {
                runtimeStats = GetComponent<PlayerStatsRuntime>();
            }
        }

        private void OnValidate()
        {
            if (runtimeStats == null)
            {
                runtimeStats = GetComponent<PlayerStatsRuntime>();
            }
        }
    }
}
