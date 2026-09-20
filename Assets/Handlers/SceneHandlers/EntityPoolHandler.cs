using Assets.Handlers.CommonParents;
using Cysharp.Threading.Tasks;
using Entity.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
                    if (targetsToRelease[i] != null) _pool?.Release(targetsToRelease[i]);
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
            InitPoolIfNeeded();
        }

        private void InitPoolIfNeeded()
        {
            if (_pool == null && _prefab != null) _pool = CreatePool(_prefab);
        }

        protected override async UniTask PrewarmAsync()
        {
            InitPoolIfNeeded();
            if (_pool != null) PrewarmPool(_pool, initialCapacity);
            isPrewarmed = true;
            await UniTask.CompletedTask;
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
                    instance.ResetInitializationState();
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

        #endregion

        #region Public API

        public async UniTask WaitUntilAllActiveInitializedAsync(CancellationToken token)
        {
            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, cancellationToken: token);

            if (_activeEntities.Count == 0) return;

            var tasks = _activeEntities
                .Where(entity => entity != null && !entity.IsInitialized)
                .Select(entity => entity.WaitUntilInitializedAsync(token));

            await UniTask.WhenAll(tasks);
        }

        public EntityController GetEntity()
        {
            InitPoolIfNeeded();
            if (_pool == null || isClearing)
            {
                Debug.LogWarning($"[EntityPoolHandler] Cannot get entity. Pool null: {_pool == null}, IsClearing: {isClearing}");
                return null;
            }

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
        }
    }
}