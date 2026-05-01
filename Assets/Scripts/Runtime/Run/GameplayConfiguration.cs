using Animalis.Characters;
using Animalis.Enemies;
using Animalis.Pickups;
using UnityEngine;

namespace Animalis.Run
{
    [CreateAssetMenu(menuName = "Animalis/Run/Gameplay Configuration", fileName = "GameplayConfiguration")]
    public sealed class GameplayConfiguration : ScriptableObject
    {
        [Header("Player")]
        [Tooltip("Character data used when gameplay starts. Future character selection can swap this reference.")]
        [SerializeField] private CharacterDefinition startingCharacter;
        [Tooltip("Base player prefab instantiated at runtime.")]
        [SerializeField] private GameObject playerPrefab;

        [Header("Enemies")]
        [Tooltip("Enemy data used by the spawn director.")]
        [SerializeField] private EnemyDefinition defaultEnemy;
        [Tooltip("Enemy prefab instantiated by the spawn director. If empty, a runtime placeholder enemy is created.")]
        [SerializeField] private EnemyController enemyPrefab;
        [Tooltip("Pickup prefab spawned when enemies die. If empty, a runtime placeholder pickup is created.")]
        [SerializeField] private ExperiencePickup experiencePickupPrefab;

        [Header("Run")]
        [Tooltip("Shared tuning for progression, defeat flow, and enemy spawn pacing.")]
        [SerializeField] private RunDefinition runDefinition;

        public CharacterDefinition StartingCharacter => startingCharacter;
        public GameObject PlayerPrefab => playerPrefab;
        public EnemyDefinition DefaultEnemy => defaultEnemy;
        public EnemyController EnemyPrefab => enemyPrefab;
        public ExperiencePickup ExperiencePickupPrefab => experiencePickupPrefab;
        public RunDefinition RunDefinition => runDefinition;
    }
}
