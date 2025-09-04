using System.Collections.Generic;
using UnityEngine;

public class GameObjectsHandler : MonoBehaviour
{
    public static GameObjectsHandler Instance { get; private set; }

    [SerializeField] private string resourcesPath = "Prefabs/";
    [SerializeField] private int cacheLimit = 20;

    private Dictionary<string, GameObject> _cache = new();
    private LinkedList<string> _lruOrder = new();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    public GameObject GetPrefab(string id)
    {
        if (_cache.TryGetValue(id, out GameObject prefab))
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
        if (_cache.Count >= cacheLimit)
        {
            string oldest = _lruOrder.Last.Value;
            _lruOrder.RemoveLast();
            _cache.Remove(oldest);
            Debug.Log($"[PrefabManager] Removed from cache: {oldest}");
        }

        _cache[id] = prefab;
        _lruOrder.AddFirst(id);
    }

    private void TouchLRU(string id)
    {
        _lruOrder.Remove(id);
        _lruOrder.AddFirst(id);
    }

}