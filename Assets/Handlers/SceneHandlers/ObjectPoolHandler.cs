using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

namespace Assets.Handlers.SceneHandlers
{
    public enum PoolType
    {
        None,
        Projectile,
        Effect,
    }

    public class ObjectPoolHandler : MonoBehaviour
    {
        private static Dictionary<PoolType, ObjectPoolHandler> _poolInstances;

        [SerializeField] public PoolType poolType = PoolType.None;
        [SerializeField] private GameObject prefab;
        [SerializeField] private int defaultCapacity = 100;
        [SerializeField] private int maxSize = 200;

        private List<GameObject> _activeObjects = new();
        private IObjectPool<GameObject> pool;
        
        public static void RealeasePools()
        {
            foreach (var pool in _poolInstances.Values)
                foreach (Transform child in pool.transform)
                    if (child.gameObject.activeSelf) pool.Return(child.gameObject);
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
                defaultCapacity: defaultCapacity,
                maxSize: maxSize
            );

            List<GameObject> tempList = new List<GameObject>();
            for (int i = 0; i < defaultCapacity; i++) tempList.Add(pool.Get());
            foreach (var obj in tempList) pool.Release(obj);
            _poolInstances.Add(poolType, this);
        }

        private GameObject CreateInstance() => Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);

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

        public GameObject[] GetAllActive() => _activeObjects.ToArray();

        public void Return(GameObject obj) => pool.Release(obj);
    }
}