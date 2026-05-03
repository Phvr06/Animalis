using Animalis.Characters;
using Animalis.Content;
using Animalis.Enemies;
using Animalis.Pickups;
using Animalis.Stage;
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

        [Header("Stage")]
        [Tooltip("Stage data used when gameplay starts. Future map selection can swap this reference.")]
        [SerializeField] private StageDefinition startingStage;

        [Header("Enemies")]
        [Tooltip("Legacy fallback enemy data used when the selected stage does not define its own enemy setup.")]
        [SerializeField] private EnemyDefinition defaultEnemy;
        [Tooltip("Legacy fallback enemy pool used when the selected stage does not define its own enemy pool.")]
        [SerializeField] private EnemyDefinition[] enemyPool;
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
        public StageDefinition StartingStage => startingStage;
        public EnemyDefinition DefaultEnemy => startingStage != null && startingStage.DefaultEnemy != null ? startingStage.DefaultEnemy : defaultEnemy;
        public EnemyDefinition[] EnemyPool => startingStage != null && startingStage.EnemyPool != null && startingStage.EnemyPool.Length > 0 ? startingStage.EnemyPool : enemyPool;
        public EnemyController EnemyPrefab => enemyPrefab;
        public ExperiencePickup ExperiencePickupPrefab => experiencePickupPrefab;
        public RunDefinition RunDefinition => startingStage != null && startingStage.RunDefinition != null ? startingStage.RunDefinition : runDefinition;
    }
}
