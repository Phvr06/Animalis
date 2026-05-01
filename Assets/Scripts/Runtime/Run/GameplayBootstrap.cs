using Animalis.Characters;
using Animalis.Combat;
using Animalis.Enemies;
using Animalis.Player;
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

        private void Start()
        {
            ResolveSceneReferences();

            if (configuration == null || configuration.StartingCharacter == null || configuration.PlayerPrefab == null)
            {
                Debug.LogWarning("Gameplay bootstrap is missing configuration, character data, or player prefab.", this);
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

            if (runFlow != null)
            {
                runFlow.Configure(configuration.RunDefinition);
                runFlow.RegisterPlayer(playerInstance);
            }

            if (enemySpawnDirector != null)
            {
                enemySpawnDirector.Configure(configuration, playerInstance.transform);
            }
        }

        private void Reset()
        {
            ResolveSceneReferences();
        }

        private void ResolveSceneReferences()
        {
            if (playerSpawnPoint == null)
            {
                GameObject spawn = GameObject.Find("PlayerSpawn");
                playerSpawnPoint = spawn != null ? spawn.transform : null;
            }

            if (playerParent == null)
            {
                GameObject actors = GameObject.Find("Actors");
                playerParent = actors != null ? actors.transform : null;
            }

            if (projectileParent == null)
            {
                GameObject projectiles = GameObject.Find("Projectiles");
                projectileParent = projectiles != null ? projectiles.transform : null;
            }

            if (hud == null)
            {
                hud = FindFirstObjectByType<GameplayHud>();
            }

            if (runFlow == null)
            {
                runFlow = FindFirstObjectByType<RunFlowController>();
            }

            if (enemySpawnDirector == null)
            {
                enemySpawnDirector = FindFirstObjectByType<EnemySpawnDirector>();
            }
        }

        private void OnValidate()
        {
            ResolveSceneReferences();
        }
    }
}
