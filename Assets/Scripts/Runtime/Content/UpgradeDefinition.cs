using UnityEngine;

namespace Animalis.Content
{
    [CreateAssetMenu(menuName = "Animalis/Data/Upgrade", fileName = "NewUpgrade")]
    public sealed class UpgradeDefinition : ContentDefinition
    {
        [Header("Offer")]
        [SerializeField] private UpgradeRarity rarity = UpgradeRarity.Common;
        [TextArea]
        [SerializeField] private string description = "Improves the current weapon.";
        [Min(1)]
        [SerializeField] private int maxLevel = 1;
        [Min(0f)]
        [SerializeField] private float offerWeight = 1f;

        [Header("Effect")]
        [SerializeField] private ElementType requiredElement = ElementType.Fire;
        [SerializeField] private UpgradeEffectType effectType = UpgradeEffectType.Damage;
        [SerializeField] private float primaryValuePerLevel = 1f;
        [SerializeField] private float secondaryValuePerLevel;

        public string UpgradeId => ContentId;
        public UpgradeRarity Rarity => rarity;
        public string Description => description;
        public int MaxLevel => maxLevel;
        public float OfferWeight => offerWeight;
        public ElementType RequiredElement => requiredElement;
        public UpgradeEffectType EffectType => effectType;
        public float PrimaryValuePerLevel => primaryValuePerLevel;
        public float SecondaryValuePerLevel => secondaryValuePerLevel;

        private void OnValidate()
        {
            ValidateIdentity();
            maxLevel = Mathf.Max(1, maxLevel);
            offerWeight = Mathf.Max(0f, offerWeight);
        }
    }
}
