using System;
using System.Collections;
using System.IO;
using UnityEngine;
using static TreeEditor.TextureAtlas;

namespace Assets.Handlers.Sprite
{
    public static class StreamingSpriteLoader
    {
        public static IEnumerator LoadSprite(string textureName, bool isPixel, System.Action<UnityEngine.Sprite> onLoaded)
        {
            GraphicElement graphicElement = GraphicElementHandler.Objects[textureName];
            if (graphicElement == null)
            {
                Debug.LogError($"Graphic element not found: {textureName}");
                yield break;
            }
            byte[] data = File.ReadAllBytes(graphicElement.SpriteAtlasPath);
            Texture2D atlasTex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (!atlasTex.LoadImage(data))
            {
                Debug.LogError($"[SpriteLoader] Failed to load atlas: {graphicElement.SpriteAtlasPath}");
                yield break;
            }
            int x = graphicElement.Frame.X;
            int y = graphicElement.Frame.Y;
            int w = graphicElement.Frame.Width;
            int h = graphicElement.Frame.Height;
            Texture2D subTex = new Texture2D(w,  h, TextureFormat.RGBA32, false);
            Color[] pixels = atlasTex.GetPixels(x, atlasTex.height - h - y, w, h);
            subTex.SetPixels(pixels);
            subTex.Apply();
            subTex.filterMode = isPixel ? FilterMode.Point : FilterMode.Bilinear;
            UnityEngine.Sprite sprite = UnityEngine.Sprite.Create(
                subTex,
                new Rect(0, 0, w, h),
                new Vector2(0.5f, 0.5f),
                100f
            );
            onLoaded?.Invoke(sprite);
        }

    }
}