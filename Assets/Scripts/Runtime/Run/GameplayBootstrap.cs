using Animalis.Characters;
using Animalis.Combat;
using Animalis.Enemies;
using Animalis.Player;
using Animalis.Stage;
using UnityEngine;

namespace Animalis.Run
{
    public sealed class GameplayBootstrap : MonoBehaviour
    {
        [SerializeField] private GameplayConfiguration configuration;
        [SerializeField] private Transform playerSpawnPoint;
        [SerializeField] private Transform playerParent;
        [SerializeField] private Transform projectileParent;
        [SerializeField] private GameplayHud hud;
        [SerializeField] private RunFlowController runFlow;
        [SerializeField] private EnemySpawnDirector enemySpawnDirector;
        [SerializeField] private InfiniteChunkMap chunkMap;

        private void Start()
        {
            if (!HasCriticalReferences())
            {
                return;
            }

            Vector3 spawnPosition = playerSpawnPoint != null ? playerSpawnPoint.position : Vector3.zero;
            GameObject playerInstance = Instantiate(configuration.PlayerPrefab, spawnPosition, Quaternion.identity, playerParent);
            playerInstance.name = $"{configuration.StartingCharacter.DisplayName} Player";

            PlayerStatsRuntime stats = playerInstance.GetComponent<PlayerStatsRuntime>();
            PlayerAvatarView avatar = playerInstance.GetComponent<PlayerAvatarView>();
            AutoWeaponController autoWeapon = playerInstance.GetComponent<AutoWeaponController>();
            PlayerExperience experience = playerInstance.GetComponent<PlayerExperience>();

            if (stats != null)
            {
                stats.Initialize(configuration.StartingCharacter);
            }

            if (avatar != null)
            {
                avatar.Apply(configuration.StartingCharacter);
            }

            if (autoWeapon != null)
            {
                autoWeapon.Initialize(configuration.StartingCharacter, projectileParent);
            }

            if (experience != null)
            {
                experience.Initialize(configuration.RunDefinition);
            }

            if (hud != null && stats != null && experience != null)
            {
                hud.Bind(stats, experience);
            }
            else if (hud == null)
            {
                Debug.LogWarning("Gameplay bootstrap is missing a HUD reference. Gameplay will continue without HUD binding.", this);
            }

            if (runFlow != null)
            {
                runFlow.Configure(configuration.RunDefinition);
                runFlow.RegisterPlayer(playerInstance);
            }
            else
            {
                Debug.LogWarning("Gameplay bootstrap is missing a RunFlowController reference. Camera follow and defeat flow will be unavailable.", this);
            }

            if (enemySpawnDirector != null)
            {
                enemySpawnDirector.Configure(configuration, playerInstance.transform);
            }
            else
            {
                Debug.LogWarning("Gameplay bootstrap is missing an EnemySpawnDirector reference. Enemy spawning will be unavailable.", this);
            }

            if (chunkMap != null)
            {
                chunkMap.SetTarget(playerInstance.transform);
            }
            else
            {
                Debug.LogWarning("Gameplay bootstrap is missing an InfiniteChunkMap reference. Dynamic map generation will be unavailable.", this);
            }
        }

        private bool HasCriticalReferences()
        {
            if (configuration == null)
            {
                Debug.LogWarning("Gameplay bootstrap is missing a GameplayConfiguration asset.", this);
                return false;
            }

            if (configuration.StartingCharacter == null)
            {
                Debug.LogWarning("Gameplay bootstrap configuration is missing the starting character.", this);
                return false;
            }

            if (configuration.PlayerPrefab == null)
            {
                Debug.LogWarning("Gameplay bootstrap configuration is missing the player prefab.", this);
                return false;
            }

            if (configuration.RunDefinition == null)
            {
                Debug.LogWarning("Gameplay bootstrap configuration is missing the run definition.", this);
                return false;
            }

            if (playerSpawnPoint == null || playerParent == null || projectileParent == null)
            {
                Debug.LogWarning("Gameplay bootstrap requires explicit scene references for spawn point, actor parent, and projectile parent.", this);
                return false;
            }

            return true;
        }
    }
}
