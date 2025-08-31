using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static TreeEditor.TextureAtlas;

namespace Assets.Handlers.Sprite
{
    public static class StreamingSpriteLoader
    {
        private static readonly Dictionary<string, Texture2D> Cache = new();

        public static IEnumerator LoadSprite(GraphicElement graphicElement, int index, bool isPixel, Action<UnityEngine.Sprite> onLoaded)
        {
            if (index >= graphicElement.Quantity)
            {
                Debug.LogError($"Index is out of bounds: {graphicElement.Filename}, index: {index}");
                yield break;
            }
            if (!Cache.TryGetValue(graphicElement.SpriteAtlasPath, out Texture2D atlasTex))
            {
                byte[] data;
                try
                {
                    data = File.ReadAllBytes(graphicElement.SpriteAtlasPath);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[SpriteLoader] Cannot read atlas file: {e.Message}");
                    yield break;
                }
                atlasTex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                if (!atlasTex.LoadImage(data))
                {
                    Debug.LogError($"[SpriteLoader] Failed to load atlas: {graphicElement.SpriteAtlasPath}");
                    yield break;
                }
                Cache[graphicElement.SpriteAtlasPath] = atlasTex;
            }
            int w = graphicElement.Frame.Width / graphicElement.Quantity;
            int h = graphicElement.Frame.Height;
            int x = graphicElement.Frame.X + w * index;
            int y = graphicElement.Frame.Y;
            //Debug.Log($"name:{graphicElement.Filename},w:{w},h:{h},x:{x},y:{y}");
            Texture2D subTex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            Color[] pixels = atlasTex.GetPixels(x, atlasTex.height - h - y, w, h);
            subTex.SetPixels(pixels);
            subTex.Apply();
            subTex.filterMode = isPixel ? FilterMode.Point : FilterMode.Bilinear;
            var sprite = UnityEngine.Sprite.Create(subTex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 100f);
            onLoaded?.Invoke(sprite);
        }
    }
}