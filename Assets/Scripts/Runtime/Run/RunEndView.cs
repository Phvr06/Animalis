using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Animalis.Run
{
    public sealed class RunEndView : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text detailText;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button menuButton;

        private RunFlowController _runFlow;
        private string _gameplaySceneName = "Gameplay";
        private string _menuSceneName = "MainMenu";
        private bool _initialized;

        private void Awake()
        {
            ResolveSceneReferences();
            BindButtons();
            if (!_initialized)
            {
                SetPanelVisible(false);
            }
        }

        public void Initialize(RunFlowController runFlow, string gameplaySceneName, string menuSceneName)
        {
            ResolveSceneReferences();
            BindButtons();

            _gameplaySceneName = string.IsNullOrWhiteSpace(gameplaySceneName) ? "Gameplay" : gameplaySceneName;
            _menuSceneName = string.IsNullOrWhiteSpace(menuSceneName) ? "MainMenu" : menuSceneName;
            _initialized = true;

            if (_runFlow != null)
            {
                _runFlow.RunEnded -= HandleRunEnded;
            }

            _runFlow = runFlow;
            if (_runFlow != null)
            {
                _runFlow.RunEnded += HandleRunEnded;
            }
        }

        public void Restart()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(_gameplaySceneName);
        }

        public void BackToMenu()
        {
            Time.timeScale = 1f;
            RunSelectionState.Clear();
            SceneManager.LoadScene(_menuSceneName);
        }

        private void HandleRunEnded(bool victory, float elapsedSeconds)
        {
            if (titleText != null)
            {
                titleText.text = victory ? "Vitoria" : "Derrota";
            }

            if (detailText != null)
            {
                detailText.text = victory
                    ? $"Mapa concluido em {Mathf.FloorToInt(elapsedSeconds)}s"
                    : $"Sobreviveu {Mathf.FloorToInt(elapsedSeconds)}s";
            }

            SetPanelVisible(true);
            Time.timeScale = 0f;
        }

        private void ResolveSceneReferences()
        {
            panel ??= FindGameObject("RunEndPanel");
            titleText ??= FindComponent<TMP_Text>("RunEndTitleText");
            detailText ??= FindComponent<TMP_Text>("RunEndDetailText");
            restartButton ??= FindComponent<Button>("RestartButton");
            menuButton ??= FindComponent<Button>("MenuButton");
        }

        private void BindButtons()
        {
            Bind(restartButton, Restart);
            Bind(menuButton, BackToMenu);
        }

        private void SetPanelVisible(bool visible)
        {
            if (panel != null)
            {
                panel.SetActive(visible);
            }
        }

        private void OnDestroy()
        {
            if (_runFlow != null)
            {
                _runFlow.RunEnded -= HandleRunEnded;
            }
        }

        private static void Bind(Button button, UnityEngine.Events.UnityAction action)
        {
            if (button == null)
            {
                return;
            }

            if (button.onClick.GetPersistentEventCount() > 0)
            {
                return;
            }

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(action);
        }

        private static GameObject FindGameObject(string objectName)
        {
            Transform transform = FindTransform(objectName);
            return transform != null ? transform.gameObject : null;
        }

        private static T FindComponent<T>(string objectName) where T : Component
        {
            Transform transform = FindTransform(objectName);
            return transform != null ? transform.GetComponent<T>() : null;
        }

        private static Transform FindTransform(string objectName)
        {
            Transform[] transforms = FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (int i = 0; i < transforms.Length; i++)
            {
                if (transforms[i].name == objectName)
                {
                    return transforms[i];
                }
            }

            return null;
        }
    }
}
