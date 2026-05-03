using System.Collections.Generic;
using System.Reflection;
using Animalis.Characters;
using Animalis.Combat;
using Animalis.Content;
using Animalis.Run;
using NUnit.Framework;
using UnityEngine;

namespace Animalis.Tests.EditMode
{
    public sealed class PlayerDay1Tests
    {
        [Test]
        public void ContentCatalogFindsCharactersWeaponsAndUpgradesById()
        {
            CharacterDefinition character = CreateContent<CharacterDefinition>("fox", "Fox");
            WeaponDefinition weapon = CreateContent<WeaponDefinition>("brasas_de_rebeliao", "Brasas de Rebeliao");
            UpgradeDefinition upgrade = CreateUpgrade("chama_mais_quente", UpgradeEffectType.Damage, 3, 0.5f, 0.25f);

            ContentCatalog catalog = ScriptableObject.CreateInstance<ContentCatalog>();
            SetField(catalog, "characters", new List<CharacterDefinition> { character });
            SetField(catalog, "weapons", new List<WeaponDefinition> { weapon });
            SetField(catalog, "upgrades", new List<UpgradeDefinition> { upgrade });

            Assert.AreSame(character, catalog.FindCharacter("FOX"));
            Assert.AreSame(weapon, catalog.FindWeapon("brasas_de_rebeliao"));
            Assert.AreSame(upgrade, catalog.FindUpgrade("CHAMA_MAIS_QUENTE"));
        }

        [Test]
        public void ExperienceEnoughForLevelRaisesLevelGained()
        {
            GameObject player = new("Player");
            PlayerExperience experience = player.AddComponent<PlayerExperience>();
            RunDefinition runDefinition = ScriptableObject.CreateInstance<RunDefinition>();

            int gainedLevel = 0;
            experience.Initialize(runDefinition);
            experience.LevelGained += level => gainedLevel = level;

            experience.AddExperience(runDefinition.GetExperienceToNextLevel(1));

            Assert.AreEqual(2, experience.CurrentLevel);
            Assert.AreEqual(2, gainedLevel);
            Object.DestroyImmediate(player);
        }

        [Test]
        public void UpgradeChangesWeaponRuntimeStatsAndRespectsMaxLevel()
        {
            WeaponDefinition weapon = CreateFireWeapon();
            UpgradeDefinition upgrade = CreateUpgrade("chama_mais_quente", UpgradeEffectType.Damage, 1, 0.65f, 0.45f);
            WeaponRuntimeState state = new(weapon);

            Assert.IsTrue(state.ApplyUpgrade(upgrade));
            Assert.AreEqual(2.65f, state.DirectDamage, 0.001f);
            Assert.AreEqual(1.45f, state.BurnDamagePerSecond, 0.001f);
            Assert.IsFalse(state.ApplyUpgrade(upgrade));
        }

        [Test]
        public void BurnApplicationUsesHighestDpsAndRenewsDuration()
        {
            WeaponDefinition weapon = CreateFireWeapon();
            WeaponRuntimeState state = new(weapon);
            GameObject enemy = new("Enemy");
            DummyDamageable damageable = enemy.AddComponent<DummyDamageable>();
            ElementalStatusController status = enemy.AddComponent<ElementalStatusController>();

            status.ApplyBurn(null, state);
            Assert.IsTrue(status.IsBurning);
            Assert.IsTrue(damageable.IsAlive);

            Object.DestroyImmediate(enemy);
        }

        private static WeaponDefinition CreateFireWeapon()
        {
            WeaponDefinition weapon = CreateContent<WeaponDefinition>("brasas_de_rebeliao", "Brasas de Rebeliao");
            SetField(weapon, "element", ElementType.Fire);
            SetField(weapon, "projectileDamage", 2f);
            SetField(weapon, "fireInterval", 1f);
            SetField(weapon, "burnDamagePerSecond", 1f);
            SetField(weapon, "burnDuration", 3f);
            return weapon;
        }

        private static UpgradeDefinition CreateUpgrade(
            string id,
            UpgradeEffectType effectType,
            int maxLevel,
            float primary,
            float secondary)
        {
            UpgradeDefinition upgrade = CreateContent<UpgradeDefinition>(id, id);
            SetField(upgrade, "requiredElement", ElementType.Fire);
            SetField(upgrade, "effectType", effectType);
            SetField(upgrade, "maxLevel", maxLevel);
            SetField(upgrade, "primaryValuePerLevel", primary);
            SetField(upgrade, "secondaryValuePerLevel", secondary);
            return upgrade;
        }

        private static T CreateContent<T>(string id, string displayName) where T : ContentDefinition
        {
            T definition = ScriptableObject.CreateInstance<T>();
            SetField(definition, "contentId", id);
            SetField(definition, "displayName", displayName);
            return definition;
        }

        private static void SetField(object target, string fieldName, object value)
        {
            FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(field, $"Missing field {fieldName} on {target.GetType().Name}");
            field.SetValue(target, value);
        }

        private sealed class DummyDamageable : MonoBehaviour, Animalis.Core.IDamageable
        {
            public bool IsAlive => true;

            public void ApplyDamage(Animalis.Core.DamageData damageData)
            {
            }
        }
    }
}
