using Animalis.Characters;
using Animalis.Content;
using Animalis.Enemies;
using Animalis.Pickups;
using UnityEngine;

namespace Animalis.Run
{
    [CreateAssetMenu(menuName = "Animalis/Run/Gameplay Configuration", fileName = "GameplayConfiguration")]
    public sealed class GameplayConfiguration : ScriptableObject
    {
        [Header("Catalog")]
        [Tooltip("Optional registry of all available characters, weapons and enemies. Useful for future menus, unlocks and validation.")]
        [SerializeField] private ContentCatalog contentCatalog;

        [Header("Player")]
        [Tooltip("Character data used when gameplay starts. Future character selection can swap this reference.")]
        [SerializeField] private CharacterDefinition startingCharacter;
        [Tooltip("Base player prefab instantiated at runtime.")]
        [SerializeField] private GameObject playerPrefab;

        [Header("Enemies")]
        [Tooltip("Enemy data used by the spawn director.")]
        [SerializeField] private EnemyDefinition defaultEnemy;
        [Tooltip("Enemy prefab instantiated by the spawn director.")]
        [SerializeField] private EnemyController enemyPrefab;
        [Tooltip("Pickup prefab spawned when enemies die.")]
        [SerializeField] private ExperiencePickup experiencePickupPrefab;

        [Header("Run")]
        [Tooltip("Shared tuning for progression, defeat flow, and enemy spawn pacing.")]
        [SerializeField] private RunDefinition runDefinition;

        public ContentCatalog ContentCatalog => contentCatalog;
        public CharacterDefinition StartingCharacter => startingCharacter;
        public GameObject PlayerPrefab => playerPrefab;
        public EnemyDefinition DefaultEnemy => defaultEnemy;
        public EnemyController EnemyPrefab => enemyPrefab;
        public ExperiencePickup ExperiencePickupPrefab => experiencePickupPrefab;
        public RunDefinition RunDefinition => runDefinition;
    }
}
