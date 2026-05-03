using System.Collections.Generic;
using Animalis.Characters;
using Animalis.Content;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Animalis.Run
{
    public sealed class MainMenuController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private ContentCatalog contentCatalog;
        [SerializeField] private string gameplaySceneName = "Gameplay";

        [Header("Scene UI")]
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private GameObject characterPanel;
        [SerializeField] private GameObject creditsPanel;
        [SerializeField] private Button playButton;
        [SerializeField] private Button creditsButton;
        [SerializeField] private Button characterBackButton;
        [SerializeField] private Button creditsBackButton;
        [SerializeField] private Button firstCharacterButton;
        [SerializeField] private TMP_Text firstCharacterLabel;

        private CharacterDefinition _firstCharacter;

        private void Awake()
        {
            Time.timeScale = 1f;
            RunSelectionState.Clear();

            ResolveSceneReferences();
            BindFirstCharacter();
            BindButtons();
            ShowMain();
        }

        private void ResolveSceneReferences()
        {
            mainPanel ??= FindGameObject("MainPanel");
            characterPanel ??= FindGameObject("CharacterPanel");
            creditsPanel ??= FindGameObject("CreditsPanel");
            playButton ??= FindComponent<Button>("JogarButton");
            creditsButton ??= FindComponent<Button>("CreditosButton");
            characterBackButton ??= FindComponent<Button>("CharacterBackButton");
            creditsBackButton ??= FindComponent<Button>("CreditsBackButton");
            firstCharacterButton ??= FindComponent<Button>("FoxButton");

            if (firstCharacterLabel == null && firstCharacterButton != null)
            {
                firstCharacterLabel = firstCharacterButton.GetComponentInChildren<TMP_Text>(true);
            }
        }

        private void BindButtons()
        {
            Bind(playButton, ShowCharacters);
            Bind(creditsButton, ShowCredits);
            Bind(characterBackButton, ShowMain);
            Bind(creditsBackButton, ShowMain);
        }

        private void BindFirstCharacter()
        {
            _firstCharacter = FindFirstAvailableCharacter();

            if (firstCharacterLabel != null)
            {
                firstCharacterLabel.text = _firstCharacter != null
                    ? $"{_firstCharacter.DisplayName}\nAfinidade: {_firstCharacter.Affinity}"
                    : "Nenhum personagem disponivel";
            }

            if (firstCharacterButton == null)
            {
                return;
            }

            firstCharacterButton.interactable = _firstCharacter != null;
            Bind(firstCharacterButton, SelectFirstCharacter);
        }

        private CharacterDefinition FindFirstAvailableCharacter()
        {
            IReadOnlyList<CharacterDefinition> characters = contentCatalog != null ? contentCatalog.Characters : null;
            if (characters == null)
            {
                return null;
            }

            for (int i = 0; i < characters.Count; i++)
            {
                if (characters[i] != null)
                {
                    return characters[i];
                }
            }

            return null;
        }

        private void SelectCharacter(CharacterDefinition character)
        {
            if (character == null)
            {
                return;
            }

            RunSelectionState.SetCharacter(character);
            SceneManager.LoadScene(gameplaySceneName);
        }

        public void SelectFirstCharacter()
        {
            SelectCharacter(_firstCharacter);
        }

        public void ShowMain()
        {
            SetActive(mainPanel, true);
            SetActive(characterPanel, false);
            SetActive(creditsPanel, false);
        }

        public void ShowCharacters()
        {
            SetActive(mainPanel, false);
            SetActive(characterPanel, true);
            SetActive(creditsPanel, false);
        }

        public void ShowCredits()
        {
            SetActive(mainPanel, false);
            SetActive(characterPanel, false);
            SetActive(creditsPanel, true);
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

        private static void SetActive(GameObject target, bool active)
        {
            if (target != null)
            {
                target.SetActive(active);
            }
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
