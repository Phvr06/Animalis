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
        [SerializeField, HideInInspector] private Transform target;
        [SerializeField] private Transform enemyParent;
        [SerializeField] private Transform pickupParent;

        private float _spawnTimer;
        private int _aliveCount;

        private RunDefinition RunDefinition => configuration != null ? configuration.RunDefinition : null;
        private EnemyDefinition DefaultEnemy => configuration != null ? configuration.DefaultEnemy : null;
        private EnemyDefinition[] EnemyPool => configuration != null ? configuration.EnemyPool : null;
        private EnemyController EnemyPrefab => configuration != null ? configuration.EnemyPrefab : null;
        private ExperiencePickup ExperiencePickupPrefab => configuration != null ? configuration.ExperiencePickupPrefab : null;
        private bool CanSpawn => target != null && runFlow != null && runFlow.IsRunActive && RunDefinition != null && DefaultEnemy != null && EnemyPrefab != null && ExperiencePickupPrefab != null;

        public void Configure(GameplayConfiguration gameplayConfiguration, Transform playerTarget)
        {
            configuration = gameplayConfiguration;
            target = playerTarget != null ? playerTarget : target;
        }

        private void Start()
        {
            if (configuration == null || RunDefinition == null || DefaultEnemy == null || EnemyPrefab == null || ExperiencePickupPrefab == null || runFlow == null || enemyParent == null || pickupParent == null)
            {
                Debug.LogWarning("Enemy spawn director requires configuration, run definition, enemy data, enemy prefab, pickup prefab, run flow, enemy parent, and pickup parent.", this);
                enabled = false;
                return;
            }

            _spawnTimer = 0.5f;
        }

        private void Update()
        {
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
            if (EnemyPrefab == null)
            {
                Debug.LogWarning("Enemy spawn director requires an enemy prefab in GameplayConfiguration.", this);
                enabled = false;
                return;
            }

            Vector2 direction = Random.insideUnitCircle.normalized;
            if (direction == Vector2.zero)
            {
                direction = Vector2.right;
            }

            float distance = GetSpawnRadius() + Random.Range(0f, GetSpawnJitter());
            Vector3 position = target.position + (Vector3)(direction * distance);
            EnemyController enemy = Instantiate(EnemyPrefab, position, Quaternion.identity, enemyParent);
            EnemyDefinition selectedEnemy = PickEnemyDefinition();

            enemy.name = selectedEnemy != null ? selectedEnemy.DisplayName : "Enemy";
            enemy.Initialize(selectedEnemy, target, ExperiencePickupPrefab, pickupParent);
            enemy.Died += HandleEnemyDied;
            _aliveCount++;
        }

        private EnemyDefinition PickEnemyDefinition()
        {
            if (EnemyPool == null || EnemyPool.Length == 0)
            {
                return DefaultEnemy;
            }

            int startIndex = Random.Range(0, EnemyPool.Length);
            for (int i = 0; i < EnemyPool.Length; i++)
            {
                EnemyDefinition candidate = EnemyPool[(startIndex + i) % EnemyPool.Length];
                if (candidate != null)
                {
                    return candidate;
                }
            }

            return DefaultEnemy;
        }

        private int GetMaxAlive(float elapsed)
        {
            return RunDefinition.StartingMaxAlive + Mathf.FloorToInt(elapsed / 60f) * RunDefinition.ExtraAlivePerMinute;
        }

        private float GetSpawnInterval(float elapsed)
        {
            float minutes = elapsed / 60f;
            return Mathf.Max(RunDefinition.MinimumSpawnInterval, RunDefinition.StartingSpawnInterval - minutes * RunDefinition.IntervalRampPerMinute);
        }

        private float GetSpawnRadius()
        {
            return RunDefinition.SpawnRadius;
        }

        private float GetSpawnJitter()
        {
            return RunDefinition.SpawnJitter;
        }

        private void HandleEnemyDied(EnemyController enemy)
        {
            if (enemy != null)
            {
                enemy.Died -= HandleEnemyDied;
            }

            _aliveCount = Mathf.Max(0, _aliveCount - 1);
        }
    }
}
