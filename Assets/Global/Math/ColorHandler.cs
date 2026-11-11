using UnityEngine;

namespace Assets.Global.Math
{
    public static class ColorHandler
    {
        public static Color fallBackColor = new Color(1f, 0.0f, 1f, 1f);
        public static Color LightUpColor(Color baseColor, float ratio)
        {
            Color result = Color.Lerp(baseColor, Color.white, ratio);
            result.a = baseColor.a;
            return result;
        }

        public static Texture2D CombineTextures(Texture2D baseTexture, Texture2D textureToOverlay, Vector2Int offset = default)
        {
            int width = baseTexture.width;
            int height = baseTexture.height;

            Texture2D combinedTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);

            Color[] basePixels = baseTexture.GetPixels();
            combinedTexture.SetPixels(basePixels);

            Color[] overlayPixels = textureToOverlay.GetPixels();
            int overlayWidth = textureToOverlay.width;
            int overlayHeight = textureToOverlay.height;

            for (int y = 0; y < overlayHeight; y++)
            {
                for (int x = 0; x < overlayWidth; x++)
                {
                    int targetX = x + offset.x;
                    int targetY = y + offset.y;

                    if (targetX >= 0 && targetX < width && targetY >= 0 && targetY < height)
                    {
                        Color overlayColor = overlayPixels[y * overlayWidth + x];

                        if (overlayColor.a <= 0f) continue;

                        int baseIndex = targetY * width + targetX;
                        Color baseColor = combinedTexture.GetPixels()[baseIndex];

                        float finalAlpha = overlayColor.a + baseColor.a * (1f - overlayColor.a);
                        Color finalColor = Color.clear;

                        if (finalAlpha > 0f)
                        {
                            finalColor.r = (overlayColor.r * overlayColor.a + baseColor.r * baseColor.a * (1f - overlayColor.a)) / finalAlpha;
                            finalColor.g = (overlayColor.g * overlayColor.a + baseColor.g * baseColor.a * (1f - overlayColor.a)) / finalAlpha;
                            finalColor.b = (overlayColor.b * overlayColor.a + baseColor.b * baseColor.a * (1f - overlayColor.a)) / finalAlpha;
                            finalColor.a = finalAlpha;
                        }

                        combinedTexture.SetPixel(targetX, targetY, finalColor);
                    }
                }
            }
            combinedTexture.Apply();
            return combinedTexture;
        }
    }
}