using System.Collections.Generic;
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
        [SerializeField] private RunEndView runEndView;
        [SerializeField] private string gameplaySceneName = "Gameplay";
        [SerializeField] private string menuSceneName = "MainMenu";

        private bool _runStarted;

        private void Start()
        {
            if (!HasCriticalReferences())
            {
                return;
            }

            CharacterDefinition selectedCharacter = RunSelectionState.SelectedCharacter != null
                ? RunSelectionState.SelectedCharacter
                : configuration.StartingCharacter;

            StartRun(selectedCharacter);
        }

        public void StartRun(CharacterDefinition selectedCharacter)
        {
            if (_runStarted)
            {
                return;
            }

            CharacterDefinition character = selectedCharacter != null ? selectedCharacter : configuration.StartingCharacter;
            if (character == null)
            {
                Debug.LogWarning("Gameplay bootstrap cannot start without a selected character.", this);
                return;
            }

            _runStarted = true;

            StageDefinition currentStage = configuration.StartingStage;
            if (currentStage != null)
            {
                StageProgressionService.EnsureStageRegistered(currentStage);
            }

            Vector3 spawnPosition = playerSpawnPoint != null ? playerSpawnPoint.position : Vector3.zero;
            GameObject playerInstance = Instantiate(configuration.PlayerPrefab, spawnPosition, Quaternion.identity, playerParent);
            playerInstance.name = $"{character.DisplayName} Player";

            PlayerStatsRuntime stats = playerInstance.GetComponent<PlayerStatsRuntime>();
            PlayerAvatarView avatar = playerInstance.GetComponent<PlayerAvatarView>();
            AutoWeaponController autoWeapon = playerInstance.GetComponent<AutoWeaponController>();
            PlayerExperience experience = playerInstance.GetComponent<PlayerExperience>();

            if (stats != null)
            {
                stats.Initialize(character);
            }

            if (avatar != null)
            {
                avatar.Apply(character);
            }

            if (autoWeapon != null)
            {
                autoWeapon.Initialize(character, projectileParent);
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

            Canvas canvas = ResolveCanvas();
            if (experience != null && autoWeapon != null)
            {
                LevelUpController levelUpController = playerInstance.GetComponent<LevelUpController>();
                if (levelUpController == null)
                {
                    levelUpController = playerInstance.AddComponent<LevelUpController>();
                }

                levelUpController.Initialize(experience, autoWeapon, configuration.ContentCatalog, canvas);
            }

            if (runFlow != null)
            {
                runFlow.Configure(currentStage, configuration.RunDefinition);
                runFlow.RegisterPlayer(playerInstance);
                RunEndView endView = ResolveRunEndView(canvas);
                if (endView != null)
                {
                    endView.Initialize(runFlow, gameplaySceneName, menuSceneName);
                }
                else
                {
                    Debug.LogWarning("Gameplay bootstrap could not find a RunEndView or RunEndPanel in the scene.", this);
                }
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
                chunkMap.Configure(currentStage);
                chunkMap.SetTarget(playerInstance.transform);
            }
            else
            {
                Debug.LogWarning("Gameplay bootstrap is missing an InfiniteChunkMap reference. Dynamic map generation will be unavailable.", this);
            }
        }

        private Canvas ResolveCanvas()
        {
            if (hud != null)
            {
                Canvas hudCanvas = hud.GetComponentInParent<Canvas>();
                if (hudCanvas != null)
                {
                    return hudCanvas;
                }
            }

            return FindFirstObjectByType<Canvas>();
        }

        private RunEndView ResolveRunEndView(Canvas canvas)
        {
            if (runEndView != null)
            {
                return runEndView;
            }

            RunEndView sceneView = FindFirstObjectByType<RunEndView>(FindObjectsInactive.Include);
            if (sceneView != null)
            {
                return sceneView;
            }

            Transform panel = canvas != null ? FindChildByName(canvas.transform, "RunEndPanel") : null;
            return panel != null ? panel.GetComponent<RunEndView>() : null;
        }

        private static Transform FindChildByName(Transform root, string childName)
        {
            if (root == null)
            {
                return null;
            }

            if (root.name == childName)
            {
                return root;
            }

            for (int i = 0; i < root.childCount; i++)
            {
                Transform match = FindChildByName(root.GetChild(i), childName);
                if (match != null)
                {
                    return match;
                }
            }

            return null;
        }

        private bool HasCriticalReferences()
        {
            if (configuration == null)
            {
                Debug.LogWarning("Gameplay bootstrap is missing a GameplayConfiguration asset.", this);
                return false;
            }

            if (configuration.ContentCatalog == null)
            {
                Debug.LogWarning("Gameplay bootstrap configuration is missing the content catalog.", this);
                return false;
            }

            if (!HasSelectableCharacter())
            {
                Debug.LogWarning("Gameplay bootstrap requires at least one character in the catalog or a starting character fallback.", this);
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

        private bool HasSelectableCharacter()
        {
            if (configuration.StartingCharacter != null)
            {
                return true;
            }

            IReadOnlyList<CharacterDefinition> characters = configuration.ContentCatalog != null ? configuration.ContentCatalog.Characters : null;
            if (characters == null)
            {
                return false;
            }

            for (int i = 0; i < characters.Count; i++)
            {
                if (characters[i] != null)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
