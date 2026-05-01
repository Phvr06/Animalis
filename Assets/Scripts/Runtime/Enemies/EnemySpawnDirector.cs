using Animalis.Core;
using Animalis.Pickups;
using Animalis.Run;
using UnityEngine;

namespace Animalis.Enemies
{
    public sealed class EnemySpawnDirector : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameplayConfiguration configuration;
        [SerializeField] private RunFlowController runFlow;
        [SerializeField] private Transform target;
        [SerializeField] private Transform enemyParent;
        [SerializeField] private Transform pickupParent;

        private float _spawnTimer;
        private int _aliveCount;

        private RunDefinition RunDefinition => configuration != null ? configuration.RunDefinition : null;
        private EnemyDefinition DefaultEnemy => configuration != null ? configuration.DefaultEnemy : null;
        private EnemyController EnemyPrefab => configuration != null ? configuration.EnemyPrefab : null;
        private ExperiencePickup ExperiencePickupPrefab => configuration != null ? configuration.ExperiencePickupPrefab : null;
        private bool CanSpawn => target != null && DefaultEnemy != null && (runFlow == null || runFlow.IsRunActive || Time.timeScale > 0f);

        public void Configure(GameplayConfiguration gameplayConfiguration, Transform playerTarget)
        {
            configuration = gameplayConfiguration;
            target = playerTarget != null ? playerTarget : target;
        }

        private void Start()
        {
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

            float distance = GetSpawnRadius() + Random.Range(0f, GetSpawnJitter());
            Vector3 position = target.position + (Vector3)(direction * distance);
            EnemyController enemy = EnemyPrefab != null
                ? Instantiate(EnemyPrefab, position, Quaternion.identity, enemyParent)
                : CreateRuntimeEnemy(position);

            enemy.name = DefaultEnemy != null ? DefaultEnemy.DisplayName : "Enemy";
            enemy.Initialize(DefaultEnemy, target, ExperiencePickupPrefab, pickupParent);
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
            renderer.color = DefaultEnemy != null ? DefaultEnemy.WorldColor : new Color(0.78f, 0.22f, 0.18f, 1f);
            renderer.sortingOrder = 8;

            return root.AddComponent<EnemyController>();
        }

        private int GetMaxAlive(float elapsed)
        {
            RunDefinition runDefinition = RunDefinition;
            int startingMaxAlive = runDefinition != null ? runDefinition.StartingMaxAlive : 10;
            int extraAlivePerMinute = runDefinition != null ? runDefinition.ExtraAlivePerMinute : 6;
            return startingMaxAlive + Mathf.FloorToInt(elapsed / 60f) * extraAlivePerMinute;
        }

        private float GetSpawnInterval(float elapsed)
        {
            RunDefinition runDefinition = RunDefinition;
            float startingSpawnInterval = runDefinition != null ? runDefinition.StartingSpawnInterval : 2.5f;
            float minimumSpawnInterval = runDefinition != null ? runDefinition.MinimumSpawnInterval : 0.8f;
            float intervalRampPerMinute = runDefinition != null ? runDefinition.IntervalRampPerMinute : 0.55f;
            float minutes = elapsed / 60f;
            return Mathf.Max(minimumSpawnInterval, startingSpawnInterval - minutes * intervalRampPerMinute);
        }

        private float GetSpawnRadius()
        {
            return RunDefinition != null ? RunDefinition.SpawnRadius : 9f;
        }

        private float GetSpawnJitter()
        {
            return RunDefinition != null ? RunDefinition.SpawnJitter : 2f;
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
            ResolveSceneReferences();
        }

        private void OnValidate()
        {
            ResolveSceneReferences();
        }
    }
}
