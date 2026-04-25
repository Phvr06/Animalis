using Animalis.Player;
using Animalis.Run;
using UnityEngine;

namespace Animalis.Pickups
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class ExperiencePickup : MonoBehaviour
    {
        [Min(1)]
        [Tooltip("XP granted when the player collects this pickup.")]
        [SerializeField] private int experienceValue = 1;

        private void Reset()
        {
            Collider2D pickupCollider = GetComponent<Collider2D>();
            pickupCollider.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
            {
                return;
            }

            PlayerExperience playerExperience = other.GetComponent<PlayerExperience>();
            if (playerExperience == null)
            {
                playerExperience = other.GetComponentInParent<PlayerExperience>();
            }

            if (playerExperience == null)
            {
                return;
            }

            playerExperience.AddExperience(experienceValue);
            Destroy(gameObject);
        }

        private void OnValidate()
        {
            experienceValue = Mathf.Max(1, experienceValue);
        }
    }
}
