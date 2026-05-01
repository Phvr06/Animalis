using System;
using Animalis.Core;
using UnityEngine;

namespace Animalis.Combat
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class Projectile : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Collider2D hitCollider;
        [SerializeField] private ProjectileParticleVisual particleVisual;
        [Min(0f)]
        [SerializeField] private float releaseDelay = 1.1f;

        private Action<Projectile> _release;
        private GameObject _owner;
        private float _damage;
        private float _speed;
        private float _remainingLifetime;
        private float _releaseTimer;
        private Vector2 _direction;
        private bool _isReleasing;

        public void Launch(WeaponDefinition definition, Vector2 origin, Vector2 direction, GameObject owner, Action<Projectile> release)
        {
            ResolveReferences();

            _owner = owner;
            _release = release;
            _damage = definition.ProjectileDamage;
            _speed = definition.ProjectileSpeed;
            _remainingLifetime = definition.ProjectileLifetime;
            _releaseTimer = 0f;
            _direction = direction.normalized;
            _isReleasing = false;

            if (hitCollider != null)
            {
                hitCollider.enabled = true;
            }

            transform.position = origin;
            transform.right = _direction;
            float zScale = Mathf.Max(definition.ProjectileScale.x, definition.ProjectileScale.y);
            transform.localScale = new Vector3(definition.ProjectileScale.x, definition.ProjectileScale.y, zScale);

            if (spriteRenderer != null)
            {
                if (definition.ProjectileSprite != null)
                {
                    spriteRenderer.sprite = definition.ProjectileSprite;
                }
                else if (spriteRenderer.sprite == null)
                {
                    spriteRenderer.sprite = PlaceholderVisualFactory.GetSquareSprite();
                }

                spriteRenderer.color = definition.ProjectileColor;
            }

            gameObject.SetActive(true);
        }

        private void Awake()
        {
            ResolveReferences();

            if (hitCollider != null)
            {
                hitCollider.isTrigger = true;
            }
        }

        private void Update()
        {
            if (_isReleasing)
            {
                _releaseTimer -= Time.deltaTime;
                if (_releaseTimer <= 0f)
                {
                    ReleaseNow();
                }

                return;
            }

            transform.position += (Vector3)(_direction * (_speed * Time.deltaTime));
            _remainingLifetime -= Time.deltaTime;

            if (_remainingLifetime <= 0f)
            {
                Release();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!gameObject.activeSelf || _isReleasing)
            {
                return;
            }

            if (_owner != null && (other.gameObject == _owner || other.transform.root.gameObject == _owner))
            {
                return;
            }

            IDamageable damageable = other.GetComponentInParent<IDamageable>();
            if (damageable == null || !damageable.IsAlive)
            {
                return;
            }

            damageable.ApplyDamage(new DamageData(_owner, _damage, transform.position, _direction));
            Release();
        }

        private void Release()
        {
            if (_isReleasing)
            {
                return;
            }

            _isReleasing = true;
            _releaseTimer = releaseDelay;

            if (hitCollider != null)
            {
                hitCollider.enabled = false;
            }

            particleVisual?.StopEmitting();

            if (_releaseTimer <= 0f)
            {
                ReleaseNow();
            }
        }

        private void ReleaseNow()
        {
            gameObject.SetActive(false);
            _release?.Invoke(this);
        }

        private void Reset()
        {
            ResolveReferences();
        }

        private void OnValidate()
        {
            ResolveReferences();
        }

        private void ResolveReferences()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>(true);
            }

            if (hitCollider == null)
            {
                hitCollider = GetComponent<Collider2D>();
            }

            if (particleVisual == null)
            {
                particleVisual = GetComponent<ProjectileParticleVisual>();
            }
        }
    }
}
