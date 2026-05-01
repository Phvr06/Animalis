using System;
using Animalis.Player;
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

        public event Action<bool, float> RunEnded;

        public bool IsRunActive { get; private set; } = true;
        public float ElapsedSeconds { get; private set; }

        public void Configure(RunDefinition definition)
        {
            runDefinition = definition;
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
                followCamera = Camera.main;
            }

            if (defeatText == null)
            {
                GameObject defeatTextObject = GameObject.Find("DefeatText");
                defeatText = defeatTextObject != null ? defeatTextObject.GetComponent<TMP_Text>() : null;
            }

            if (defeatText != null)
            {
                defeatText.gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            TryFindPlayer();

            if (!IsRunActive)
            {
                return;
            }

            ElapsedSeconds += Time.deltaTime;
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

        private void TryFindPlayer()
        {
            if (_playerHealth != null)
            {
                return;
            }

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                BindPlayerHealth(player.GetComponent<PlayerHealth>());
            }
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

            if (!victory && defeatText != null)
            {
                defeatText.text = $"DERROTA\nSobreviveu {Mathf.FloorToInt(ElapsedSeconds)}s";
                defeatText.gameObject.SetActive(true);
            }

            if ((runDefinition == null || runDefinition.PauseOnDefeat) && !victory)
            {
                Time.timeScale = 0f;
            }

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
