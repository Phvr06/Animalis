using Animalis.Content;
using Animalis.Enemies;
using Animalis.Run;
using UnityEngine;

namespace Animalis.Stage
{
    [CreateAssetMenu(menuName = "Animalis/Stage/Stage Definition", fileName = "NewStage")]
    public sealed class StageDefinition : ContentDefinition
    {
        [Header("Flow")]
        [Tooltip("Run tuning used while this stage is active.")]
        [SerializeField] private RunDefinition runDefinition;
        [Min(10f)]
        [Tooltip("Automatic victory time for the stage in seconds.")]
        [SerializeField] private float victoryDurationSeconds = 120f;
        [Tooltip("Whether this stage should be available by default before the unlock flow is implemented.")]
        [SerializeField] private bool startsUnlocked = true;
        [Tooltip("Optional next stage unlocked when this stage is cleared.")]
        [SerializeField] private StageDefinition nextStage;

        [Header("Enemies")]
        [Tooltip("Fallback enemy used when no pool entry is available.")]
        [SerializeField] private EnemyDefinition defaultEnemy;
        [Tooltip("Enemy pool used by this stage. If empty, Default Enemy is used.")]
        [SerializeField] private EnemyDefinition[] enemyPool;

        [Header("Map")]
        [Tooltip("Visual generation profile for the infinite map while this stage is active.")]
        [SerializeField] private MapVisualDefinition visualDefinition;

        public string StageId => ContentId;
        public RunDefinition RunDefinition => runDefinition;
        public float VictoryDurationSeconds => victoryDurationSeconds;
        public bool StartsUnlocked => startsUnlocked;
        public StageDefinition NextStage => nextStage;
        public EnemyDefinition DefaultEnemy => defaultEnemy;
        public EnemyDefinition[] EnemyPool => enemyPool;
        public MapVisualDefinition VisualDefinition => visualDefinition;
        public bool HasVictoryCondition => victoryDurationSeconds > 0f;

        private void OnValidate()
        {
            ValidateIdentity();
            victoryDurationSeconds = Mathf.Max(10f, victoryDurationSeconds);

            if (enemyPool == null || enemyPool.Length == 0)
            {
                return;
            }

            int validCount = 0;
            for (int i = 0; i < enemyPool.Length; i++)
            {
                if (enemyPool[i] != null)
                {
                    validCount++;
                }
            }

            if (validCount == enemyPool.Length)
            {
                if (defaultEnemy == null)
                {
                    defaultEnemy = enemyPool[0];
                }

                return;
            }

            EnemyDefinition[] compactedPool = new EnemyDefinition[validCount];
            int index = 0;
            for (int i = 0; i < enemyPool.Length; i++)
            {
                if (enemyPool[i] != null)
                {
                    compactedPool[index++] = enemyPool[i];
                }
            }

            enemyPool = compactedPool;
            if (defaultEnemy == null && enemyPool.Length > 0)
            {
                defaultEnemy = enemyPool[0];
            }
        }
    }
}
