using System;
using Animalis.Player;
using Animalis.Stage;
using TMPro;
using UnityEngine;

namespace Animalis.Run
{
    public sealed class RunFlowController : MonoBehaviour
    {
        [SerializeField] private Camera followCamera;
        [SerializeField] private TMP_Text defeatText;
        [SerializeField] private RunDefinition runDefinition;

        private PlayerHealth _playerHealth;
        private StageDefinition _currentStage;

        public event Action<bool, float> RunEnded;

        public bool IsRunActive { get; private set; } = true;
        public float ElapsedSeconds { get; private set; }

        public void Configure(StageDefinition stage, RunDefinition definition)
        {
            _currentStage = stage;
            runDefinition = definition;
            IsRunActive = true;
            ElapsedSeconds = 0f;
            Time.timeScale = 1f;

            if (defeatText != null)
            {
                defeatText.gameObject.SetActive(false);
            }
        }

        public void RegisterPlayer(GameObject player)
        {
            if (player == null)
            {
                return;
            }

            BindPlayerHealth(player.GetComponent<PlayerHealth>());
        }

        private void Awake()
        {
            Time.timeScale = 1f;

            if (followCamera == null)
            {
                Debug.LogWarning("Run flow controller requires an explicit camera reference.", this);
            }

            if (defeatText != null)
            {
                defeatText.gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            if (!IsRunActive)
            {
                return;
            }

            ElapsedSeconds += Time.deltaTime;

            if (_currentStage != null && _currentStage.HasVictoryCondition && ElapsedSeconds >= _currentStage.VictoryDurationSeconds)
            {
                EndRun(true);
            }
        }

        private void LateUpdate()
        {
            if (_playerHealth == null || followCamera == null)
            {
                return;
            }

            Transform playerTransform = _playerHealth.transform;
            Vector3 cameraPosition = followCamera.transform.position;
            cameraPosition.x = playerTransform.position.x;
            cameraPosition.y = playerTransform.position.y;
            followCamera.transform.position = cameraPosition;
        }

        private void BindPlayerHealth(PlayerHealth playerHealth)
        {
            if (_playerHealth == playerHealth)
            {
                return;
            }

            if (_playerHealth != null)
            {
                _playerHealth.Died -= HandlePlayerDied;
            }

            _playerHealth = playerHealth;

            if (_playerHealth != null)
            {
                _playerHealth.Died += HandlePlayerDied;
            }
        }

        private void HandlePlayerDied()
        {
            EndRun(false);
        }

        private void EndRun(bool victory)
        {
            if (!IsRunActive)
            {
                return;
            }

            IsRunActive = false;

            if (victory)
            {
                StageProgressionService.RegisterStageVictory(_currentStage);
            }

            Time.timeScale = 0f;
            RunEnded?.Invoke(victory, ElapsedSeconds);
        }

        private void OnDestroy()
        {
            if (_playerHealth != null)
            {
                _playerHealth.Died -= HandlePlayerDied;
            }

            Time.timeScale = 1f;
        }
    }
}
