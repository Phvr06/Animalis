using Animalis.Core;
using UnityEngine;

namespace Animalis.Combat
{
    public sealed class FireTrailZone : MonoBehaviour
    {
        private const float DamageTickInterval = 0.35f;
        private readonly Collider2D[] _hitBuffer = new Collider2D[32];

        private GameObject _owner;
        private WeaponRuntimeState _weaponState;
        private float _remainingLifetime;
        private float _tickTimer;
        private float _radius;

        public static void Spawn(Vector3 position, WeaponRuntimeState weaponState, GameObject owner)
        {
            if (weaponState == null || weaponState.Definition == null || !weaponState.FireTrailEnabled)
            {
                return;
            }

            GameObject zoneObject = new("FireTrailZone");
            zoneObject.transform.position = position;
            FireTrailZone zone = zoneObject.AddComponent<FireTrailZone>();
            zone.Initialize(weaponState, owner);
        }

        private void Initialize(WeaponRuntimeState weaponState, GameObject owner)
        {
            _weaponState = weaponState;
            _owner = owner;
            _remainingLifetime = weaponState.Definition.FireTrailDuration;
            _radius = weaponState.Definition.FireTrailRadius;
            _tickTimer = 0f;

            SpriteRenderer renderer = gameObject.AddComponent<SpriteRenderer>();
            renderer.sprite = PlaceholderVisualFactory.GetSquareSprite();
            renderer.color = new Color(1f, 0.35f, 0.05f, 0.35f);
            renderer.sortingOrder = 4;
            transform.localScale = Vector3.one * (_radius * 2f);
        }

        private void Update()
        {
            _remainingLifetime -= Time.deltaTime;
            if (_remainingLifetime <= 0f)
            {
                Destroy(gameObject);
                return;
            }

            _tickTimer -= Time.deltaTime;
            if (_tickTimer > 0f)
            {
                return;
            }

            _tickTimer = DamageTickInterval;
            ApplyTick();
        }

        private void ApplyTick()
        {
            if (_weaponState == null || _weaponState.Definition == null)
            {
                return;
            }

            int count = Physics2D.OverlapCircle(transform.position, _radius, ContactFilter2D.noFilter, _hitBuffer);
            for (int i = 0; i < count; i++)
            {
                Collider2D candidate = _hitBuffer[i];
                if (candidate == null || !candidate.CompareTag("Enemy"))
                {
                    continue;
                }

                IDamageable damageable = candidate.GetComponentInParent<IDamageable>();
                if (damageable == null || !damageable.IsAlive)
                {
                    continue;
                }

                float damage = Mathf.Max(0f, _weaponState.FireTrailDamagePerSecond) * DamageTickInterval;
                if (damage > 0f)
                {
                    damageable.ApplyDamage(new DamageData(_owner, damage, candidate.transform.position, Vector2.zero));
                }

                ElementalStatusController status = candidate.GetComponentInParent<ElementalStatusController>();
                status?.ApplyBurn(_owner, _weaponState);
            }
        }
    }
}
