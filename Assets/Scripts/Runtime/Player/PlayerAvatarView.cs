using Animalis.Characters;
using Animalis.Core;
using UnityEngine;

namespace Animalis.Player
{
    public sealed class PlayerAvatarView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer targetRenderer;
        [SerializeField] private Transform visualRoot;

        public void Apply(CharacterDefinition definition)
        {
            ResolveReferences();

            if (definition == null || targetRenderer == null)
            {
                return;
            }

            if (definition.WorldSprite != null)
            {
                targetRenderer.sprite = definition.WorldSprite;
            }
            else if (targetRenderer.sprite == null)
            {
                targetRenderer.sprite = PlaceholderVisualFactory.GetSquareSprite();
            }

            targetRenderer.color = definition.WorldColor;

            if (visualRoot != null)
            {
                visualRoot.localScale = new Vector3(definition.WorldScale.x, definition.WorldScale.y, 1f);
            }
        }

        private void Awake()
        {
            ResolveReferences();
        }

        private void OnValidate()
        {
            ResolveReferences();
        }

        private void Reset()
        {
            ResolveReferences();
        }

        private void ResolveReferences()
        {
            if (targetRenderer == null)
            {
                targetRenderer = GetComponentInChildren<SpriteRenderer>(true);
            }

            if (visualRoot == null)
            {
                visualRoot = targetRenderer != null ? targetRenderer.transform : transform;
            }
        }
    }
}
