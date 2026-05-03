using Animalis.Core;
using UnityEngine;

namespace Animalis.Combat
{
    public sealed class ElementalStatusController : MonoBehaviour
    {
        private const float BurnTickInterval = 0.5f;
        private readonly Collider2D[] _explosionBuffer = new Collider2D[48];

        private IDamageable _damageable;
        private GameObject _burnSource;
        private WeaponRuntimeState _burnWeaponState;
        private float _burnDamagePerSecond;
        private float _burnRemaining;
        private float _burnTickTimer;
        private bool _deathExplosionConsumed;

        public bool IsBurning => _burnRemaining > 0f && _burnDamagePerSecond > 0f;

        public void ApplyBurn(GameObject source, WeaponRuntimeState weaponState)
        {
            if (weaponState == null || weaponState.BurnDamagePerSecond <= 0f || weaponState.BurnDuration <= 0f)
            {
                return;
            }

            ResolveReferences();
            _burnSource = source;
            _burnWeaponState = weaponState;
            _burnDamagePerSecond = Mathf.Max(_burnDamagePerSecond, weaponState.BurnDamagePerSecond);
            _burnRemaining = Mathf.Max(_burnRemaining, weaponState.BurnDuration);
            _burnTickTimer = Mathf.Min(_burnTickTimer, BurnTickInterval);
        }

        public void TryTriggerBurnDeathExplosion()
        {
            if (_deathExplosionConsumed || !IsBurning || _burnWeaponState == null || _burnWeaponState.ChainExplosionChance <= 0f)
            {
                return;
            }

            _deathExplosionConsumed = true;

            if (Random.value > _burnWeaponState.ChainExplosionChance)
            {
                return;
            }

            WeaponDefinition definition = _burnWeaponState.Definition;
            if (definition == null || definition.ChainExplosionRadius <= 0f || definition.ChainExplosionDamage <= 0f)
            {
                return;
            }

            int count = Physics2D.OverlapCircle(transform.position, definition.ChainExplosionRadius, ContactFilter2D.noFilter, _explosionBuffer);
            for (int i = 0; i < count; i++)
            {
                Collider2D candidate = _explosionBuffer[i];
                if (candidate == null || candidate.transform.root == transform.root)
                {
                    continue;
                }

                IDamageable target = candidate.GetComponentInParent<IDamageable>();
                if (target == null || !target.IsAlive)
                {
                    continue;
                }

                ElementalStatusController status = candidate.GetComponentInParent<ElementalStatusController>();
                status?.ApplyBurn(_burnSource, _burnWeaponState);

                Vector2 direction = ((Vector2)candidate.transform.position - (Vector2)transform.position).normalized;
                target.ApplyDamage(new DamageData(_burnSource, definition.ChainExplosionDamage, candidate.transform.position, direction));
            }
        }

        private void Awake()
        {
            ResolveReferences();
        }

        private void Update()
        {
            if (!IsBurning)
            {
                return;
            }

            _burnRemaining -= Time.deltaTime;
            _burnTickTimer -= Time.deltaTime;

            if (_burnTickTimer <= 0f)
            {
                _burnTickTimer = BurnTickInterval;
                ApplyBurnTick();
            }

            if (_burnRemaining <= 0f)
            {
                _burnRemaining = 0f;
                _burnDamagePerSecond = 0f;
                _burnWeaponState = null;
                _burnSource = null;
            }
        }

        private void ApplyBurnTick()
        {
            ResolveReferences();
            if (_damageable == null || !_damageable.IsAlive)
            {
                return;
            }

            float amount = _burnDamagePerSecond * BurnTickInterval;
            _damageable.ApplyDamage(new DamageData(_burnSource, amount, transform.position, Vector2.zero));
        }

        private void ResolveReferences()
        {
            _damageable ??= GetComponent<IDamageable>();
        }
    }
}
