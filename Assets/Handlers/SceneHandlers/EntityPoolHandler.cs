using Assets.Handlers.CommonParents;
using Cysharp.Threading.Tasks;
using Entity.Controllers;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Assets.Handlers.SceneHandlers
{
    public class EntityPoolHandler : SingletonPoolHandler<EntityPoolHandler, EntityController>
    {
        [SerializeField] private GameObject _prefab;
        private IObjectPool<EntityController> _pool;
        private readonly List<EntityController> _activeEntities = new();

        public IReadOnlyList<EntityController> ActiveEntities => _activeEntities;

        #region Setup

        protected override async UniTask ClearOnSceneChangeAsync()
        {
            isClearing = true;
            try
            {
                var targetsToRelease = _activeEntities.ToArray();
                for (int i = 0; i < targetsToRelease.Length; i++)
                {
                    if (targetsToRelease[i] != null) _pool.Release(targetsToRelease[i]);
                    if (IsIndexOverClearDelay(i)) await UniTask.Yield();
                }
                _activeEntities.Clear();
            }
            finally
            {
                isClearing = false;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            _pool = CreatePool(_prefab);
            PrewarmPool(_pool, initialCapacity);
        }

        private IObjectPool<EntityController> CreatePool(GameObject prefab)
        {
            return new ObjectPool<EntityController>(
                createFunc: () =>
                {
                    Transform parent = poolNode != null ? poolNode.transform : transform;
                    var go = Instantiate(prefab, parent);
                    return go.GetComponent<EntityController>();
                },
                actionOnGet: instance =>
                {
                    instance.gameObject.SetActive(true);
                    _activeEntities.Add(instance);
                },
                actionOnRelease: instance =>
                {
                    CleanUpEntityBeforeRelease(instance);
                    instance.gameObject.SetActive(false);
                    _activeEntities.Remove(instance);
                },
                actionOnDestroy: instance =>
                {
                    if (instance != null && instance.gameObject != null)
                        Destroy(instance.gameObject);
                },
                collectionCheck: true,
                defaultCapacity: initialCapacity,
                maxSize: maxPoolSize
            );
        }

        private void PrewarmPool(IObjectPool<EntityController> pool, int amount)
        {
            var tempList = new List<EntityController>(amount);
            for (int i = 0; i < amount; i++) tempList.Add(pool.Get());
            foreach (var item in tempList) pool.Release(item);
        }

        #endregion

        #region Public API

        public EntityController GetEntity()
        {
            if (_pool == null || isClearing) return null;
            return _pool.Get();
        }

        #endregion

        private void CleanUpEntityBeforeRelease(EntityController entity)
        {
            if (entity.hull != null)
            {
                Destroy(entity.hull.gameObject);
                entity.hull = null;
            }
            if (entity.Driver != null && entity.Driver is MonoBehaviour driverMb)
            {
                Destroy(driverMb);
                entity.Driver = null;
            }
            entity.data = null;

            // Если есть баффы или абилки - их тоже нужно сбросить
            //entity.abilitiesController?.Clear();
        }
    }
}