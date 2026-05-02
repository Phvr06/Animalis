using UnityEngine;
using UnityEngine.Serialization;

namespace Animalis.Content
{
    public abstract class ContentDefinition : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("Stable id used by saves, unlocks and lookups.")]
        [FormerlySerializedAs("characterId")]
        [FormerlySerializedAs("weaponId")]
        [FormerlySerializedAs("enemyId")]
        [SerializeField] private string contentId = "new_content";
        [Tooltip("Name shown in UI and debug labels.")]
        [SerializeField] private string displayName = "New Content";

        public string ContentId => contentId;
        public string DisplayName => displayName;

        protected void ValidateIdentity()
        {
            string fallbackName = string.IsNullOrWhiteSpace(name) ? "New Content" : name;

            if (string.IsNullOrWhiteSpace(displayName))
            {
                displayName = fallbackName;
            }

            if (string.IsNullOrWhiteSpace(contentId))
            {
                contentId = ToId(fallbackName);
            }
        }

        private static string ToId(string source)
        {
            string trimmed = source.Trim().ToLowerInvariant();
            System.Text.StringBuilder builder = new(trimmed.Length);
            bool lastWasSeparator = false;

            foreach (char character in trimmed)
            {
                if (char.IsLetterOrDigit(character))
                {
                    builder.Append(character);
                    lastWasSeparator = false;
                }
                else if (!lastWasSeparator)
                {
                    builder.Append('_');
                    lastWasSeparator = true;
                }
            }

            string sanitized = builder.ToString().Trim('_');
            return string.IsNullOrWhiteSpace(sanitized) ? "new_content" : sanitized;
        }
    }
}
