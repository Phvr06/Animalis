using System;
using System.Collections.Generic;
using Animalis.Characters;
using Animalis.Combat;
using Animalis.Enemies;
using UnityEngine;

namespace Animalis.Content
{
    [CreateAssetMenu(menuName = "Animalis/Data/Content Catalog", fileName = "MainContentCatalog")]
    public sealed class ContentCatalog : ScriptableObject
    {
        [Header("Content")]
        [SerializeField] private List<CharacterDefinition> characters = new();
        [SerializeField] private List<WeaponDefinition> weapons = new();
        [SerializeField] private List<UpgradeDefinition> upgrades = new();
        [SerializeField] private List<EnemyDefinition> enemies = new();

        public IReadOnlyList<CharacterDefinition> Characters => characters;
        public IReadOnlyList<WeaponDefinition> Weapons => weapons;
        public IReadOnlyList<UpgradeDefinition> Upgrades => upgrades;
        public IReadOnlyList<EnemyDefinition> Enemies => enemies;

        public CharacterDefinition FindCharacter(string contentId)
        {
            return FindById(characters, contentId, definition => definition != null ? definition.CharacterId : string.Empty);
        }

        public WeaponDefinition FindWeapon(string contentId)
        {
            return FindById(weapons, contentId, definition => definition != null ? definition.WeaponId : string.Empty);
        }

        public UpgradeDefinition FindUpgrade(string contentId)
        {
            return FindById(upgrades, contentId, definition => definition != null ? definition.UpgradeId : string.Empty);
        }

        public EnemyDefinition FindEnemy(string contentId)
        {
            return FindById(enemies, contentId, definition => definition != null ? definition.EnemyId : string.Empty);
        }

        private void OnValidate()
        {
            characters.RemoveAll(character => character == null);
            weapons.RemoveAll(weapon => weapon == null);
            upgrades.RemoveAll(upgrade => upgrade == null);
            enemies.RemoveAll(enemy => enemy == null);
        }

        private static T FindById<T>(IReadOnlyList<T> entries, string contentId, Func<T, string> idSelector) where T : UnityEngine.Object
        {
            if (string.IsNullOrWhiteSpace(contentId))
            {
                return null;
            }

            for (int i = 0; i < entries.Count; i++)
            {
                T entry = entries[i];
                if (entry != null && string.Equals(idSelector(entry), contentId, StringComparison.OrdinalIgnoreCase))
                {
                    return entry;
                }
            }

            return null;
        }
    }
}
