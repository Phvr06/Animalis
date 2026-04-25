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

        private Action<Projectile> _release;
        private GameObject _owner;
        private float _damage;
        private float _speed;
        private float _remainingLifetime;
        private Vector2 _direction;

        public void Launch(WeaponDefinition definition, Vector2 origin, Vector2 direction, GameObject owner, Action<Projectile> release)
        {
            ResolveReferences();

            _owner = owner;
            _release = release;
            _damage = definition.ProjectileDamage;
            _speed = definition.ProjectileSpeed;
            _remainingLifetime = definition.ProjectileLifetime;
            _direction = direction.normalized;

            transform.position = origin;
            transform.right = _direction;
            transform.localScale = new Vector3(definition.ProjectileScale.x, definition.ProjectileScale.y, 1f);

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
            transform.position += (Vector3)(_direction * (_speed * Time.deltaTime));
            _remainingLifetime -= Time.deltaTime;

            if (_remainingLifetime <= 0f)
            {
                Release();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!gameObject.activeSelf)
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
        }
    }
}
