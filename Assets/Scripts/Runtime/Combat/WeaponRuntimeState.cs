using System.Collections.Generic;
using Animalis.Content;
using UnityEngine;

namespace Animalis.Combat
{
    public sealed class WeaponRuntimeState
    {
        private readonly Dictionary<UpgradeDefinition, int> _upgradeLevels = new();

        public WeaponRuntimeState(WeaponDefinition definition)
        {
            Definition = definition;
            DirectDamage = definition != null ? definition.ProjectileDamage : 0f;
            FireInterval = definition != null ? definition.FireInterval : 1f;
            BurnDamagePerSecond = definition != null ? definition.BurnDamagePerSecond : 0f;
            BurnDuration = definition != null ? definition.BurnDuration : 0f;
            ChainExplosionChance = definition != null ? definition.ChainExplosionChance : 0f;
            FireTrailEnabled = definition != null && definition.FireTrailEnabled;
            FireTrailDamagePerSecond = definition != null ? definition.FireTrailDamagePerSecond : 0f;
        }

        public WeaponDefinition Definition { get; }
        public float DirectDamage { get; private set; }
        public float FireInterval { get; private set; }
        public float BurnDamagePerSecond { get; private set; }
        public float BurnDuration { get; private set; }
        public float ChainExplosionChance { get; private set; }
        public bool FireTrailEnabled { get; private set; }
        public float FireTrailDamagePerSecond { get; private set; }

        public bool CanApply(UpgradeDefinition upgrade)
        {
            if (upgrade == null || Definition == null)
            {
                return false;
            }

            if (upgrade.RequiredElement != ElementType.None && upgrade.RequiredElement != Definition.Element)
            {
                return false;
            }

            return GetLevel(upgrade) < upgrade.MaxLevel;
        }

        public int GetLevel(UpgradeDefinition upgrade)
        {
            return upgrade != null && _upgradeLevels.TryGetValue(upgrade, out int level) ? level : 0;
        }

        public bool ApplyUpgrade(UpgradeDefinition upgrade)
        {
            if (!CanApply(upgrade))
            {
                return false;
            }

            int nextLevel = GetLevel(upgrade) + 1;
            _upgradeLevels[upgrade] = nextLevel;

            switch (upgrade.EffectType)
            {
                case UpgradeEffectType.Damage:
                    DirectDamage += upgrade.PrimaryValuePerLevel;
                    BurnDamagePerSecond += upgrade.SecondaryValuePerLevel;
                    break;
                case UpgradeEffectType.Cooldown:
                    FireInterval = Mathf.Max(0.1f, FireInterval - upgrade.PrimaryValuePerLevel);
                    break;
                case UpgradeEffectType.Burn:
                    BurnDuration += upgrade.PrimaryValuePerLevel;
                    BurnDamagePerSecond += upgrade.SecondaryValuePerLevel;
                    break;
                case UpgradeEffectType.ChainExplosion:
                    ChainExplosionChance = Mathf.Clamp01(ChainExplosionChance + upgrade.PrimaryValuePerLevel);
                    break;
                case UpgradeEffectType.FireTrail:
                    FireTrailEnabled = true;
                    FireTrailDamagePerSecond += upgrade.PrimaryValuePerLevel;
                    BurnDamagePerSecond += upgrade.SecondaryValuePerLevel;
                    break;
            }

            return true;
        }
    }
}
