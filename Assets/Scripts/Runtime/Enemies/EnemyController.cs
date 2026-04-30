using System;
using Animalis.Core;
using Animalis.Pickups;
using UnityEngine;

namespace Animalis.Enemies
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public sealed class EnemyController : MonoBehaviour, IDamageable
    {
        [SerializeField] private EnemyDefinition definition;
        [SerializeField] private Rigidbody2D body;
        [SerializeField] private Collider2D hitCollider;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private ExperiencePickup experiencePickupPrefab;

        private Transform _target;
        private Transform _pickupParent;
        private float _currentHealth;
        private float _contactCooldownRemaining;

        public event Action<EnemyController> Died;

        public bool IsAlive => _currentHealth > 0f;

        public void Initialize(EnemyDefinition enemyDefinition, Transform target, ExperiencePickup pickupPrefab, Transform pickupParent)
        {
            ResolveReferences();

            definition = enemyDefinition;
            _target = target;
            experiencePickupPrefab = pickupPrefab;
            _pickupParent = pickupParent;
            _currentHealth = definition != null ? definition.MaxHealth : 1f;
            _contactCooldownRemaining = 0f;

            ApplyVisuals();
            gameObject.SetActive(true);
        }

        public void ApplyDamage(DamageData damageData)
        {
            if (!IsAlive || damageData.Amount <= 0f)
            {
                return;
            }

            _currentHealth = Mathf.Max(0f, _currentHealth - damageData.Amount);
            if (_currentHealth <= 0f)
            {
                Die();
            }
        }

        private void Awake()
        {
            ResolveReferences();
            _currentHealth = definition != null ? definition.MaxHealth : 1f;
            ApplyVisuals();
        }

        private void Update()
        {
            if (_contactCooldownRemaining > 0f)
            {
                _contactCooldownRemaining -= Time.deltaTime;
            }
        }

        private void FixedUpdate()
        {
            if (!IsAlive || _target == null || body == null)
            {
                if (body != null)
                {
                    body.linearVelocity = Vector2.zero;
                }

                return;
            }

            Vector2 direction = ((Vector2)_target.position - body.position).normalized;
            float speed = definition != null ? definition.MoveSpeed : 1.5f;
            body.linearVelocity = direction * speed;
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            TryDamagePlayer(collision.collider);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            TryDamagePlayer(other);
        }

        private void TryDamagePlayer(Collider2D other)
        {
            if (!IsAlive || _contactCooldownRemaining > 0f || other == null || !other.CompareTag("Player"))
            {
                return;
            }

            IDamageable damageable = other.GetComponentInParent<IDamageable>();
            if (damageable == null || !damageable.IsAlive)
            {
                return;
            }

            Vector2 direction = ((Vector2)other.transform.position - (Vector2)transform.position).normalized;
            float damage = definition != null ? definition.ContactDamage : 1f;
            damageable.ApplyDamage(new DamageData(gameObject, damage, other.transform.position, direction));
            _contactCooldownRemaining = definition != null ? definition.ContactDamageCooldown : 0.7f;
        }

        private void Die()
        {
            body.linearVelocity = Vector2.zero;
            SpawnExperience();
            Died?.Invoke(this);
            Destroy(gameObject);
        }

        private void SpawnExperience()
        {
            int value = definition != null ? definition.ExperienceValue : 1;
            for (int i = 0; i < value; i++)
            {
                Vector2 scatter = UnityEngine.Random.insideUnitCircle * 0.25f;
                Vector3 spawnPosition = transform.position + (Vector3)scatter;

                if (experiencePickupPrefab != null)
                {
                    Instantiate(experiencePickupPrefab, spawnPosition, Quaternion.identity, _pickupParent);
                    continue;
                }

                CreateRuntimeExperiencePickup(spawnPosition);
            }
        }

        private void CreateRuntimeExperiencePickup(Vector3 spawnPosition)
        {
            GameObject pickup = new("RuntimeExperiencePickup");
            pickup.transform.SetParent(_pickupParent, false);
            pickup.transform.position = spawnPosition;

            CircleCollider2D pickupCollider = pickup.AddComponent<CircleCollider2D>();
            pickupCollider.isTrigger = true;
            pickupCollider.radius = 0.3f;

            pickup.AddComponent<ExperiencePickup>();

            GameObject visual = new("Visual");
            visual.transform.SetParent(pickup.transform, false);
            visual.transform.localScale = new Vector3(0.35f, 0.35f, 1f);

            SpriteRenderer renderer = visual.AddComponent<SpriteRenderer>();
            renderer.sprite = PlaceholderVisualFactory.GetSquareSprite();
            renderer.color = new Color(0.35f, 0.88f, 1f, 1f);
            renderer.sortingOrder = 5;
        }

        private void ApplyVisuals()
        {
            if (spriteRenderer == null)
            {
                return;
            }

            spriteRenderer.sprite = definition != null && definition.WorldSprite != null
                ? definition.WorldSprite
                : PlaceholderVisualFactory.GetSquareSprite();
            spriteRenderer.color = definition != null ? definition.WorldColor : new Color(0.78f, 0.22f, 0.18f, 1f);
            spriteRenderer.sortingOrder = 8;

            Vector2 scale = definition != null ? definition.WorldScale : new Vector2(0.72f, 0.72f);
            spriteRenderer.transform.localScale = new Vector3(scale.x, scale.y, 1f);
        }

        private void Reset()
        {
            ResolveReferences();
        }

        private void OnValidate()
        {
            ResolveReferences();
            ApplyVisuals();
        }

        private void ResolveReferences()
        {
            if (body == null)
            {
                body = GetComponent<Rigidbody2D>();
            }

            if (hitCollider == null)
            {
                hitCollider = GetComponent<Collider2D>();
            }

            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>(true);
            }

            if (body != null)
            {
                body.gravityScale = 0f;
                body.freezeRotation = true;
            }

            if (hitCollider != null)
            {
                hitCollider.isTrigger = false;
            }
        }
    }
}
