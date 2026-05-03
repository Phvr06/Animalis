using System.Collections.Generic;
using Animalis.Combat;
using Animalis.Content;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Animalis.Run
{
    public sealed class LevelUpController : MonoBehaviour
    {
        private readonly List<UpgradeDefinition> _offerBuffer = new();
        private readonly Button[] _optionButtons = new Button[3];

        private PlayerExperience _experience;
        private AutoWeaponController _weaponController;
        private ContentCatalog _contentCatalog;
        private Canvas _canvas;
        private GameObject _panel;
        private TMP_Text _levelText;
        private TMP_Text _promptText;
        private bool _isOffering;
        private int _queuedOffers;
        private int _latestQueuedLevel;

        public void Initialize(
            PlayerExperience experience,
            AutoWeaponController weaponController,
            ContentCatalog contentCatalog,
            Canvas canvas)
        {
            Unbind();
            _experience = experience;
            _weaponController = weaponController;
            _contentCatalog = contentCatalog;
            _canvas = canvas;

            ResolveSceneReferences();
            SetPanelVisible(false);

            if (_experience != null)
            {
                _experience.LevelGained += HandleLevelGained;
            }
        }

        private void HandleLevelGained(int level)
        {
            if (_isOffering)
            {
                _queuedOffers++;
                _latestQueuedLevel = level;
                return;
            }

            TryShowOffer(level);
        }

        private void TryShowOffer(int level)
        {
            if (_weaponController == null || _contentCatalog == null)
            {
                return;
            }

            BuildOffer();
            if (_offerBuffer.Count == 0)
            {
                return;
            }

            ShowOffer(level);
        }

        private void BuildOffer()
        {
            _offerBuffer.Clear();
            IReadOnlyList<UpgradeDefinition> upgrades = _contentCatalog.Upgrades;
            WeaponRuntimeState weaponState = _weaponController.ActiveWeaponState;

            for (int i = 0; i < upgrades.Count; i++)
            {
                UpgradeDefinition upgrade = upgrades[i];
                if (weaponState != null && weaponState.CanApply(upgrade))
                {
                    _offerBuffer.Add(upgrade);
                }
            }

            for (int i = 0; i < _offerBuffer.Count; i++)
            {
                int swapIndex = Random.Range(i, _offerBuffer.Count);
                (_offerBuffer[i], _offerBuffer[swapIndex]) = (_offerBuffer[swapIndex], _offerBuffer[i]);
            }

            while (_offerBuffer.Count > _optionButtons.Length)
            {
                _offerBuffer.RemoveAt(_offerBuffer.Count - 1);
            }
        }

        private void ShowOffer(int level)
        {
            ResolveSceneReferences();
            if (_panel == null)
            {
                Debug.LogWarning("Level up controller could not find LevelUpPanel in the scene.", this);
                return;
            }

            _isOffering = true;
            Time.timeScale = 0f;

            if (_levelText != null)
            {
                _levelText.text = $"Nivel {level}";
            }

            if (_promptText != null)
            {
                _promptText.text = "Escolha um upgrade";
            }

            for (int i = 0; i < _optionButtons.Length; i++)
            {
                UpgradeDefinition upgrade = i < _offerBuffer.Count ? _offerBuffer[i] : null;
                ConfigureButton(_optionButtons[i], upgrade);
            }

            SetPanelVisible(true);
        }

        private void ConfigureButton(Button button, UpgradeDefinition upgrade)
        {
            if (button == null)
            {
                return;
            }

            button.gameObject.SetActive(upgrade != null);
            button.onClick.RemoveAllListeners();

            if (upgrade == null)
            {
                return;
            }

            Image image = button.GetComponent<Image>();
            if (image != null)
            {
                image.color = UpgradeColor(upgrade.Rarity);
            }

            TMP_Text label = button.GetComponentInChildren<TMP_Text>(true);
            if (label != null)
            {
                label.text = BuildUpgradeLabel(upgrade);
            }

            button.onClick.AddListener(() => ApplyUpgrade(upgrade));
        }

        private string BuildUpgradeLabel(UpgradeDefinition upgrade)
        {
            int currentLevel = _weaponController.ActiveWeaponState != null
                ? _weaponController.ActiveWeaponState.GetLevel(upgrade)
                : 0;

            return $"{upgrade.DisplayName} {currentLevel + 1}/{upgrade.MaxLevel}\n{upgrade.Description}";
        }

        private static Color UpgradeColor(UpgradeRarity rarity)
        {
            return rarity switch
            {
                UpgradeRarity.Rare => new Color(0.85f, 0.34f, 0.08f, 1f),
                UpgradeRarity.Epic => new Color(0.68f, 0.2f, 0.75f, 1f),
                UpgradeRarity.Legendary => new Color(0.95f, 0.68f, 0.12f, 1f),
                _ => new Color(0.38f, 0.16f, 0.08f, 1f)
            };
        }

        private void ApplyUpgrade(UpgradeDefinition upgrade)
        {
            _weaponController.ApplyUpgrade(upgrade);
            CloseOffer();
        }

        private void CloseOffer()
        {
            SetPanelVisible(false);
            _isOffering = false;
            Time.timeScale = 1f;

            if (_queuedOffers > 0)
            {
                _queuedOffers--;
                TryShowOffer(_latestQueuedLevel);
            }
        }

        private void ResolveSceneReferences()
        {
            Transform root = _canvas != null ? _canvas.transform : null;
            _panel ??= FindGameObject(root, "LevelUpPanel");
            _levelText ??= FindComponent<TMP_Text>(root, "LevelUpTitleText");
            _promptText ??= FindComponent<TMP_Text>(root, "LevelUpPromptText");
            _optionButtons[0] ??= FindComponent<Button>(root, "UpgradeOptionButton1");
            _optionButtons[1] ??= FindComponent<Button>(root, "UpgradeOptionButton2");
            _optionButtons[2] ??= FindComponent<Button>(root, "UpgradeOptionButton3");
        }

        private void SetPanelVisible(bool visible)
        {
            if (_panel != null)
            {
                _panel.SetActive(visible);
            }
        }

        private void OnDestroy()
        {
            Unbind();
        }

        private void Unbind()
        {
            if (_experience != null)
            {
                _experience.LevelGained -= HandleLevelGained;
                _experience = null;
            }
        }

        private static GameObject FindGameObject(Transform root, string objectName)
        {
            Transform transform = FindTransform(root, objectName);
            return transform != null ? transform.gameObject : null;
        }

        private static T FindComponent<T>(Transform root, string objectName) where T : Component
        {
            Transform transform = FindTransform(root, objectName);
            return transform != null ? transform.GetComponent<T>() : null;
        }

        private static Transform FindTransform(Transform root, string objectName)
        {
            if (root != null)
            {
                Transform match = FindChildByName(root, objectName);
                if (match != null)
                {
                    return match;
                }
            }

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
    }
}
