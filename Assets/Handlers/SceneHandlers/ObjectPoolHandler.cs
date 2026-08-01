using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Assets.Handlers.SceneHandlers
{
    public enum PoolType
    {
        Entity,
        Projectile,
        Effect
    }

    public class ObjectPoolHandler : MonoBehaviour
    {
        private static readonly Dictionary<PoolType, ObjectPoolHandler> _poolInstances = new();
        private static readonly Dictionary<GameObject, IObjectPool<GameObject>> _prefabPools = new();

        [SerializeField] public PoolType poolType;
        [SerializeField] private GameObject _prefab;
        [SerializeField] private int _defaultCapacity = 100;
        [SerializeField] private int _maxSize = 200;

        private List<GameObject> _activeObjects = new();
        private IObjectPool<GameObject> pool;

        public static void RealeasePools()
        {
            foreach (var pool in _poolInstances.Values)
                foreach (Transform child in pool.transform)
                    if (child.gameObject.activeSelf) pool.Release(child.gameObject);
        }

        public static ObjectPoolHandler GetInstance(PoolType type)
        {
            if (_poolInstances.TryGetValue(type, out ObjectPoolHandler pool)) return pool;
            return null;
        }

        void Awake()
        {
            pool = new ObjectPool<GameObject>(
                createFunc: CreateInstance,
                actionOnGet: OnGetFromPool,
                actionOnRelease: OnReleaseToPool,
                actionOnDestroy: OnDestroyPoolObject,
                collectionCheck: true,
                defaultCapacity: _defaultCapacity,
                maxSize: _maxSize
            );

            List<GameObject> tempList = new List<GameObject>();
            for (int i = 0; i < _defaultCapacity; i++) tempList.Add(pool.Get());
            foreach (var obj in tempList) pool.Release(obj);
            _poolInstances.Add(poolType, this);
        }

        private GameObject CreateInstance() => Instantiate(_prefab, Vector3.zero, Quaternion.identity, transform);

        private void OnGetFromPool(GameObject obj)
        {
            _activeObjects.Add(obj);
            obj.SetActive(true);
        }

        private void OnReleaseToPool(GameObject obj)
        {
            _activeObjects.Remove(obj);
            obj.SetActive(false);
        }

        private void OnDestroyPoolObject(GameObject obj) => Destroy(obj);

        public GameObject Get() => pool.Get();

        public static GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            if (!_prefabPools.TryGetValue(prefab, out var pool))
            {
                pool = new ObjectPool<GameObject>(
                    createFunc: () => Instantiate(prefab),
                    actionOnGet: go => go.SetActive(true),
                    actionOnRelease: go => go.SetActive(false),
                    actionOnDestroy: Destroy
                );
                _prefabPools[prefab] = pool;
            }

            var instance = pool.Get();
            instance.transform.SetPositionAndRotation(position, rotation);
            return instance;
        }

        public GameObject[] GetAllActive() => _activeObjects.ToArray();

        public void Release(GameObject obj) => pool.Release(obj);

        public static void Release(GameObject prefab, GameObject instance)
        {
            if (_prefabPools.TryGetValue(prefab, out var pool)) pool.Release(instance);
            else Destroy(instance);
        }
    }
}