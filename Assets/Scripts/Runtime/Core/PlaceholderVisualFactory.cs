using UnityEngine;

namespace Animalis.Core
{
    public static class PlaceholderVisualFactory
    {
        private static Sprite _cachedSquareSprite;

        public static Sprite GetSquareSprite()
        {
            if (_cachedSquareSprite != null)
            {
                return _cachedSquareSprite;
            }

            Texture2D texture = new(16, 16, TextureFormat.RGBA32, false)
            {
                name = "RuntimePlaceholderSquare"
            };

            Color[] pixels = new Color[16 * 16];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = Color.white;
            }

            texture.SetPixels(pixels);
            texture.Apply();

            _cachedSquareSprite = Sprite.Create(texture, new Rect(0f, 0f, 16f, 16f), new Vector2(0.5f, 0.5f), 16f);
            _cachedSquareSprite.name = "RuntimePlaceholderSquare";
            return _cachedSquareSprite;
        }
    }
}
