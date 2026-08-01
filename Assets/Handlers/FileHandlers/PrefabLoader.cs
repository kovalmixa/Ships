using System.Collections.Generic;
using UnityEngine;

namespace Assets.Handlers.FileHandlers
{
    public class PrefabLoader : SingletonMonoBehaviour<PrefabLoader>
    {
        [SerializeField] private string _resourcesPath = "Prefabs/";
        [SerializeField] private int _cacheLimit = 40;

        private readonly Dictionary<string, GameObject> _cache = new();
        private readonly LinkedList<string> _lruOrder = new();

        public GameObject GetPrefab(string id)
        {
            if (_cache.TryGetValue(id, out GameObject prefab))
            {
                TouchLRU(id);
                return prefab;
            }
            prefab = Resources.Load<GameObject>(_resourcesPath + id);
            if (prefab == null)
            {
                Debug.LogWarning($"[PrefabManager] Prefab '{id}' not found in Resources/{_resourcesPath}");
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
            if (_cache.Count >= _cacheLimit)
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
}