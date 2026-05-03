using Animalis.Content;
using Animalis.Combat;
using UnityEngine;

namespace Animalis.Characters
{
    [CreateAssetMenu(menuName = "Animalis/Data/Character", fileName = "NewCharacter")]
    public sealed class CharacterDefinition : ContentDefinition
    {
        [Header("Gameplay")]
        [Min(1f)]
        [Tooltip("Maximum life of the character.")]
        [SerializeField] private float maxHealth = 10f;
        [Min(0.1f)]
        [Tooltip("Movement speed in world units per second.")]
        [SerializeField] private float moveSpeed = 5f;
        [Min(0f)]
        [Tooltip("Pickup magnet radius used for XP and future collectibles.")]
        [SerializeField] private float pickupRadius = 1.25f;
        [Tooltip("Elemental affinity used for starting weapons and future off-element rules.")]
        [SerializeField] private ElementType affinity = ElementType.Fire;
        [Tooltip("Weapon asset that defines projectile, damage, cooldown and range.")]
        [SerializeField] private WeaponDefinition startingWeapon;

        [Header("Placeholder Visual")]
        [Tooltip("Optional sprite for the character. If empty, a runtime square placeholder is used.")]
        [SerializeField] private Sprite worldSprite;
        [Tooltip("Tint color used on the placeholder or assigned sprite.")]
        [SerializeField] private Color worldColor = new(0.95f, 0.55f, 0.2f, 1f);
        [Tooltip("World scale of the visual child object.")]
        [SerializeField] private Vector2 worldScale = new(0.8f, 0.8f);

        public string CharacterId => ContentId;
        public float MaxHealth => maxHealth;
        public float MoveSpeed => moveSpeed;
        public float PickupRadius => pickupRadius;
        public ElementType Affinity => affinity;
        public WeaponDefinition StartingWeapon => startingWeapon;
        public Sprite WorldSprite => worldSprite;
        public Color WorldColor => worldColor;
        public Vector2 WorldScale => worldScale;

        private void OnValidate()
        {
            ValidateIdentity();
            maxHealth = Mathf.Max(1f, maxHealth);
            moveSpeed = Mathf.Max(0.1f, moveSpeed);
            pickupRadius = Mathf.Max(0f, pickupRadius);
            worldScale.x = Mathf.Max(0.1f, worldScale.x);
            worldScale.y = Mathf.Max(0.1f, worldScale.y);
        }
    }
}
