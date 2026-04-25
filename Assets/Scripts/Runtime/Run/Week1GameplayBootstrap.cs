using Animalis.Characters;
using Animalis.Combat;
using Animalis.Player;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Animalis.Run
{
    public sealed class Week1GameplayBootstrap : MonoBehaviour
    {
        private const string DefaultCharacterAssetPath = "Assets/ScriptableObjects/Characters/FoxWeek1.asset";
        private const string DefaultPlayerPrefabPath = "Assets/Prefabs/Characters/PlayerPlaceholder.prefab";

        [SerializeField] private CharacterDefinition defaultCharacter;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private Transform playerSpawnPoint;
        [SerializeField] private Transform playerParent;
        [SerializeField] private Transform projectileParent;
        [SerializeField] private GameplayHud hud;

        private void Start()
        {
            ResolveAssetReferencesInEditor();
            ResolveSceneReferences();

            if (defaultCharacter == null || playerPrefab == null)
            {
                Debug.LogWarning("Week1 bootstrap is missing character data or player prefab.", this);
                return;
            }

            Vector3 spawnPosition = playerSpawnPoint != null ? playerSpawnPoint.position : Vector3.zero;
            GameObject playerInstance = Instantiate(playerPrefab, spawnPosition, Quaternion.identity, playerParent);
            playerInstance.name = $"{defaultCharacter.DisplayName} Player";

            PlayerStatsRuntime stats = playerInstance.GetComponent<PlayerStatsRuntime>();
            PlayerAvatarView avatar = playerInstance.GetComponent<PlayerAvatarView>();
            AutoWeaponController autoWeapon = playerInstance.GetComponent<AutoWeaponController>();
            PlayerExperience experience = playerInstance.GetComponent<PlayerExperience>();

            if (stats != null)
            {
                stats.Initialize(defaultCharacter);
            }

            if (avatar != null)
            {
                avatar.Apply(defaultCharacter);
            }

            if (autoWeapon != null)
            {
                autoWeapon.Initialize(defaultCharacter, projectileParent);
            }

            if (hud != null && stats != null && experience != null)
            {
                hud.Bind(stats, experience);
            }
        }

        private void Reset()
        {
            ResolveAssetReferencesInEditor();
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
        }

        private void OnValidate()
        {
            ResolveAssetReferencesInEditor();
            ResolveSceneReferences();
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void ResolveAssetReferencesInEditor()
        {
#if UNITY_EDITOR
            if (defaultCharacter == null)
            {
                defaultCharacter = AssetDatabase.LoadAssetAtPath<CharacterDefinition>(DefaultCharacterAssetPath);
            }

            if (playerPrefab == null)
            {
                playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(DefaultPlayerPrefabPath);
            }
#endif
        }
    }
}
