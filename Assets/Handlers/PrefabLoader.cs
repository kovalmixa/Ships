using Assets.Handlers.SceneHandlers;
using System.Collections.Generic;
using UnityEngine;

public class PrefabLoader : SingletonMonoBehaviour<PrefabLoader>
{
    [SerializeField] private string resourcesPath = "Prefabs/";
    [SerializeField] private int cacheLimit = 20;

    private Dictionary<string, GameObject> cache = new();
    private LinkedList<string> lruOrder = new();

    public GameObject GetPrefab(string id)
    {
        if (cache.TryGetValue(id, out GameObject prefab))
        {
            TouchLRU(id);
            return prefab;
        }
        prefab = Resources.Load<GameObject>(resourcesPath + id);
        if (prefab == null)
        {
            Debug.LogError($"[PrefabManager] Prefab '{id}' not found in Resources/{resourcesPath}");
            return null;
        }
        AddToCache(id, prefab);
        return prefab;
    }

    public GameObject InstantiatePrefab(string id, Vector3 pos, Quaternion rot, Transform parent = null)
    {
        GameObject prefab = GetPrefab(id);
        if (prefab == null) return null;
        return Instantiate(prefab, pos, rot, parent);
    }

    private void AddToCache(string id, GameObject prefab)
    {
        if (cache.Count >= cacheLimit)
        {
            string oldest = lruOrder.Last.Value;
            lruOrder.RemoveLast();
            cache.Remove(oldest);
            Debug.Log($"[PrefabManager] Removed from cache: {oldest}");
        }

        cache[id] = prefab;
        lruOrder.AddFirst(id);
    }

    private void TouchLRU(string id)
    {
        lruOrder.Remove(id);
        lruOrder.AddFirst(id);
    }
}