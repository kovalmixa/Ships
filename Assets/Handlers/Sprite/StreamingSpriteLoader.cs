using System.Collections;
using System.IO;
using UnityEngine;

namespace Assets.Handlers.Sprite
{
    public static class StreamingSpriteLoader
    {
        public static IEnumerator LoadSprite(string relativePath, bool isPixel, System.Action<UnityEngine.Sprite> onLoaded)
        {
            string fullPath = Path.Combine(Application.streamingAssetsPath, relativePath);

#if UNITY_ANDROID && !UNITY_EDITOR
        // На Android StreamingAssets лежат внутри .apk, нужен UnityWebRequest
        using UnityWebRequest www = UnityWebRequest.Get(fullPath);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"[SpriteLoader] Can't load texture: {fullPath}");
            yield break;
        }
        byte[] data = www.downloadHandler.data;
#else
            if (!File.Exists(fullPath))
            {
                Debug.LogError($"[SpriteLoader] File not found: {fullPath}");
                yield break;
            }
            byte[] data = File.ReadAllBytes(fullPath);
#endif
            Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (!tex.LoadImage(data))
            {
                Debug.LogError($"[SpriteLoader] LoadImage failed: {fullPath}");
                yield break;
            }
            tex.filterMode = isPixel ? FilterMode.Point : FilterMode.Bilinear;
            UnityEngine.Sprite sprite = UnityEngine.Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f),
                100f);                                   // PPU

            onLoaded?.Invoke(sprite);
        }
    }
}