using Animalis.Core;
using Animalis.Pickups;
using Animalis.Run;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Animalis.Enemies
{
    public sealed class EnemySpawnDirector : MonoBehaviour
    {
        private const string DefaultEnemyAssetPath = "Assets/ScriptableObjects/Enemies/FarmhandWeek1.asset";
        private const string DefaultPickupPrefabPath = "Assets/Prefabs/Pickups/ExperiencePickup.prefab";

        [Header("References")]
        [SerializeField] private RunFlowController runFlow;
        [SerializeField] private Transform target;
        [SerializeField] private Transform enemyParent;
        [SerializeField] private Transform pickupParent;
        [SerializeField] private EnemyDefinition defaultEnemy;
        [SerializeField] private EnemyController enemyPrefab;
        [SerializeField] private ExperiencePickup experiencePickupPrefab;

        [Header("Spawn Timing")]
        [Min(0.2f)]
        [SerializeField] private float startingSpawnInterval = 2.5f;
        [Min(0.2f)]
        [SerializeField] private float minimumSpawnInterval = 0.8f;
        [Min(0f)]
        [SerializeField] private float intervalRampPerMinute = 0.55f;
        [Min(1)]
        [SerializeField] private int startingMaxAlive = 10;
        [Min(0)]
        [SerializeField] private int extraAlivePerMinute = 6;

        [Header("Spawn Area")]
        [Min(1f)]
        [SerializeField] private float spawnRadius = 9f;
        [Min(1f)]
        [SerializeField] private float spawnJitter = 2f;

        private float _spawnTimer;
        private int _aliveCount;

        private bool CanSpawn => target != null && (runFlow == null || runFlow.IsRunActive || Time.timeScale > 0f);

        private void Start()
        {
            ResolveAssetReferencesInEditor();
            ResolveSceneReferences();
            _spawnTimer = 0.5f;
        }

        private void Update()
        {
            ResolveRuntimeTarget();

            if (!CanSpawn)
            {
                return;
            }

            if (_aliveCount == 0)
            {
                SpawnEnemy();
                _spawnTimer = GetSpawnInterval(runFlow != null ? runFlow.ElapsedSeconds : Time.timeSinceLevelLoad);
                return;
            }

            _spawnTimer -= Time.deltaTime > 0f ? Time.deltaTime : Time.unscaledDeltaTime;
            if (_spawnTimer > 0f)
            {
                return;
            }

            float elapsed = runFlow != null ? runFlow.ElapsedSeconds : Time.timeSinceLevelLoad;
            if (_aliveCount < GetMaxAlive(elapsed))
            {
                SpawnEnemy();
            }

            _spawnTimer = GetSpawnInterval(elapsed);
        }

        private void SpawnEnemy()
        {
            Vector2 direction = Random.insideUnitCircle.normalized;
            if (direction == Vector2.zero)
            {
                direction = Vector2.right;
            }

            float distance = spawnRadius + Random.Range(0f, spawnJitter);
            Vector3 position = target.position + (Vector3)(direction * distance);
            EnemyController enemy = enemyPrefab != null
                ? Instantiate(enemyPrefab, position, Quaternion.identity, enemyParent)
                : CreateRuntimeEnemy(position);

            enemy.name = defaultEnemy != null ? defaultEnemy.DisplayName : "Farmhand";
            enemy.Initialize(defaultEnemy, target, experiencePickupPrefab, pickupParent);
            enemy.Died += HandleEnemyDied;
            _aliveCount++;
        }

        private EnemyController CreateRuntimeEnemy(Vector3 position)
        {
            GameObject root = new("RuntimeEnemy");
            root.transform.SetParent(enemyParent, false);
            root.transform.position = position;
            root.tag = "Enemy";

            Rigidbody2D body = root.AddComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            body.freezeRotation = true;

            CircleCollider2D collider = root.AddComponent<CircleCollider2D>();
            collider.radius = 0.35f;
            collider.isTrigger = false;

            GameObject visual = new("Visual");
            visual.transform.SetParent(root.transform, false);

            SpriteRenderer renderer = visual.AddComponent<SpriteRenderer>();
            renderer.sprite = PlaceholderVisualFactory.GetSquareSprite();
            renderer.color = defaultEnemy != null ? defaultEnemy.WorldColor : new Color(0.78f, 0.22f, 0.18f, 1f);
            renderer.sortingOrder = 8;

            return root.AddComponent<EnemyController>();
        }

        private int GetMaxAlive(float elapsed)
        {
            return startingMaxAlive + Mathf.FloorToInt(elapsed / 60f) * extraAlivePerMinute;
        }

        private float GetSpawnInterval(float elapsed)
        {
            float minutes = elapsed / 60f;
            return Mathf.Max(minimumSpawnInterval, startingSpawnInterval - minutes * intervalRampPerMinute);
        }

        private void HandleEnemyDied(EnemyController enemy)
        {
            if (enemy != null)
            {
                enemy.Died -= HandleEnemyDied;
            }

            _aliveCount = Mathf.Max(0, _aliveCount - 1);
        }

        private void ResolveRuntimeTarget()
        {
            if (target != null)
            {
                return;
            }

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            target = player != null ? player.transform : null;
        }

        private void ResolveSceneReferences()
        {
            if (runFlow == null)
            {
                runFlow = FindFirstObjectByType<RunFlowController>();
            }

            if (enemyParent == null)
            {
                GameObject enemies = GameObject.Find("Enemies");
                enemyParent = enemies != null ? enemies.transform : transform;
            }

            if (pickupParent == null)
            {
                GameObject pickups = GameObject.Find("Pickups");
                pickupParent = pickups != null ? pickups.transform : null;
            }
        }

        private void Reset()
        {
            ResolveAssetReferencesInEditor();
            ResolveSceneReferences();
        }

        private void OnValidate()
        {
            startingSpawnInterval = Mathf.Max(0.2f, startingSpawnInterval);
            minimumSpawnInterval = Mathf.Max(0.2f, minimumSpawnInterval);
            startingMaxAlive = Mathf.Max(1, startingMaxAlive);
            spawnRadius = Mathf.Max(1f, spawnRadius);
            spawnJitter = Mathf.Max(1f, spawnJitter);
            ResolveAssetReferencesInEditor();
            ResolveSceneReferences();
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void ResolveAssetReferencesInEditor()
        {
#if UNITY_EDITOR
            if (defaultEnemy == null)
            {
                defaultEnemy = AssetDatabase.LoadAssetAtPath<EnemyDefinition>(DefaultEnemyAssetPath);
            }

            if (experiencePickupPrefab == null)
            {
                GameObject pickupPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(DefaultPickupPrefabPath);
                experiencePickupPrefab = pickupPrefab != null ? pickupPrefab.GetComponent<ExperiencePickup>() : null;
            }
#endif
        }
    }
}
