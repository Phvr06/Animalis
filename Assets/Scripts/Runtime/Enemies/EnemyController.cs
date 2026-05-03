using System;
using Animalis.Combat;
using Animalis.Core;
using Animalis.Pickups;
using UnityEngine;

namespace Animalis.Enemies
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public sealed class EnemyController : MonoBehaviour, IDamageable
    {
        [SerializeField, HideInInspector] private EnemyDefinition definition;
        [SerializeField] private Rigidbody2D body;
        [SerializeField] private Collider2D hitCollider;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField, HideInInspector] private ElementalStatusController statusController;
        [SerializeField, HideInInspector] private ExperiencePickup experiencePickupPrefab;

        private Transform _target;
        private Transform _pickupParent;
        private float _currentHealth;
        private float _contactCooldownRemaining;
        private float _animationTime;

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
            _animationTime = UnityEngine.Random.value * 10f;

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

            AnimateWalk();
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

            if (spriteRenderer != null && Mathf.Abs(direction.x) > 0.05f)
            {
                spriteRenderer.flipX = direction.x < 0f;
            }
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
            statusController?.TryTriggerBurnDeathExplosion();
            SpawnExperience();
            Died?.Invoke(this);
            Destroy(gameObject);
        }

        private void SpawnExperience()
        {
            if (experiencePickupPrefab == null)
            {
                Debug.LogWarning("Enemy controller requires an experience pickup prefab to drop XP.", this);
                return;
            }

            int value = definition != null ? definition.ExperienceValue : 1;
            Vector2 scatter = UnityEngine.Random.insideUnitCircle * 0.25f;
            Vector3 spawnPosition = transform.position + (Vector3)scatter;
            ExperiencePickup pickup = Instantiate(experiencePickupPrefab, spawnPosition, Quaternion.identity, _pickupParent);
            pickup.SetExperienceValue(value);
        }

        private void ApplyVisuals()
        {
            if (spriteRenderer == null)
            {
                return;
            }

            Sprite sprite = GetCurrentSprite();
            spriteRenderer.sprite = sprite;
            spriteRenderer.color = definition != null ? definition.WorldColor : new Color(0.78f, 0.22f, 0.18f, 1f);
            spriteRenderer.sortingOrder = 8;

            Vector2 scale = definition != null ? definition.WorldScale : new Vector2(0.72f, 0.72f);
            spriteRenderer.transform.localScale = new Vector3(scale.x, scale.y, 1f);
            CenterVisual(sprite);
        }

        private void AnimateWalk()
        {
            if (!IsAlive || spriteRenderer == null || definition == null)
            {
                return;
            }

            Sprite[] frames = definition.WalkAnimationFrames;
            if (frames == null || frames.Length == 0)
            {
                return;
            }

            _animationTime += Time.deltaTime;
            int frame = Mathf.FloorToInt(_animationTime * definition.WalkFramesPerSecond) % frames.Length;
            Sprite sprite = frames[frame] != null ? frames[frame] : definition.WorldSprite;

            if (sprite != null && spriteRenderer.sprite != sprite)
            {
                spriteRenderer.sprite = sprite;
                CenterVisual(sprite);
            }
        }

        private Sprite GetCurrentSprite()
        {
            if (definition != null)
            {
                Sprite[] frames = definition.WalkAnimationFrames;
                if (frames != null && frames.Length > 0 && frames[0] != null)
                {
                    return frames[0];
                }

                if (definition.WorldSprite != null)
                {
                    return definition.WorldSprite;
                }
            }

            return PlaceholderVisualFactory.GetSquareSprite();
        }

        private void CenterVisual(Sprite sprite)
        {
            if (spriteRenderer == null || sprite == null)
            {
                return;
            }

            Vector3 scale = spriteRenderer.transform.localScale;
            Vector3 centerOffset = sprite.bounds.center;
            spriteRenderer.transform.localPosition = new Vector3(
                -centerOffset.x * scale.x,
                -centerOffset.y * scale.y,
                0f);
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

            if (statusController == null)
            {
                statusController = GetComponent<ElementalStatusController>();
            }

            if (statusController == null && Application.isPlaying)
            {
                statusController = gameObject.AddComponent<ElementalStatusController>();
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
