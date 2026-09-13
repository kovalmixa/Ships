using System;
using System.Collections.Generic;
using Assets.Handlers.CommonParents;
using Assets.Handlers.FileHandlers;
using Assets.Handlers.TextHandlers;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;

namespace Assets.Scripts.Actions.VFX
{
    public enum VfxType
    {
        None = 0,
        //Bullet
        BulletLaunch = 1,
        BulletHit = 2,
        BulletHitWater = 3,
        BulletHitGround = 4,
        BulletHitAir = 5,
        //...
    }

    public class VfxController : SingletonPoolHandler<VfxController, VfxInstance>
    {
        private readonly Dictionary<VfxType, IObjectPool<VfxInstance>> _pools = new();
        private readonly Dictionary<VfxType, AsyncLazy<IObjectPool<VfxInstance>>> _loadingTasks = new();
        private readonly List<VfxInstance> _activeVfx = new();

        #region Setup

        protected override async UniTask ClearOnSceneChangeAsync()
        {
            isClearing = true;

            try
            {
                var targetsToRelease = _activeVfx.ToArray();
                for (int i = 0; i < targetsToRelease.Length; i++)
                {
                    VfxInstance instance = targetsToRelease[i];
                    if (instance != null && instance.gameObject.activeSelf)
                        instance.gameObject.SetActive(false);
                    if (IsIndexOverClearDelay(i)) await UniTask.Yield();
                }
                _activeVfx.Clear();
                _loadingTasks.Clear();
            }
            finally
            {
                isClearing = false;
            }
        }

        protected override void Awake() => base.Awake();

        protected override async UniTask PrewarmAsync()
        {
            foreach (VfxType type in Enum.GetValues(typeof(VfxType)))
            {
                if (type == VfxType.None) continue;

                if (!_pools.ContainsKey(type))
                {
                    var pool = await CreatePoolAsync(type);
                    if (pool != null)
                    {
                        _pools[type] = pool;
                        PrewarmPool(pool, initialCapacity);
                    }
                }
            }
        }

        #endregion

        public UniTask<bool> IsExist(VfxType type)
        {
            string id = StringHandler.FirstCharToLower(type.ToString());
            return PrefabLoader.Instance.CheckAddressableExistsAsync(id);
        }

        public void PlayEffect(InteractionContext context, VfxType type, Vector3 position, Quaternion rotation)
        {
            PlayEffectAsync(context, type, position, rotation).Forget();
        }

        private async UniTaskVoid PlayEffectAsync(InteractionContext context, VfxType type, Vector3 position, Quaternion rotation)
        {
            if (isClearing || type == VfxType.None) return;

            if (!_pools.TryGetValue(type, out var pool))
            {
                if (!_loadingTasks.TryGetValue(type, out var lazyLoad))
                {
                    lazyLoad = UniTask.Lazy(() => CreatePoolAsync(type));
                    _loadingTasks[type] = lazyLoad;
                }

                pool = await lazyLoad.Task;
                if (isClearing || pool == null) return;
                if (!_pools.ContainsKey(type))
                {
                    _pools[type] = pool;
                    _loadingTasks.Remove(type);
                }
            }

            if (isClearing) return;

            VfxInstance instance = pool.Get();
            if (instance == null) return;

            instance.Play(
                context,
                position,
                rotation,
                onRelease: () =>
                {
                    if (instance != null && instance.gameObject != null)
                        pool.Release(instance);
                }
            );
        }

        private async UniTask<IObjectPool<VfxInstance>> CreatePoolAsync(VfxType type)
        {
            var id = StringHandler.FirstCharToLower(type.ToString());
            GameObject prefab = await PrefabLoader.Instance.GetPrefabAsync(id);
            if (prefab == null)
            {
                Debug.LogWarning($"[VfxController] Failed to load prefab for effect: '{id}' ({type})");
                return null;
            }

            if (!prefab.TryGetComponent<VfxInstance>(out _))
            {
                Debug.LogWarning($"[VfxController] The VfxInstance script is missing from prefab '{id}' ({type})!");
                return null;
            }

            return new ObjectPool<VfxInstance>(
                createFunc: () =>
                {
                    Transform parent = poolNode != null ? poolNode.transform : transform;
                    var go = Instantiate(prefab, parent);
                    go.SetActive(false);
                    return go.GetComponent<VfxInstance>();
                },
                actionOnGet: instance =>
                {
                    if (instance == null) return;
                    instance.gameObject.SetActive(true);
                    _activeVfx.Add(instance);
                },
                actionOnRelease: instance =>
                {
                    if (instance == null) return;
                    instance.gameObject.SetActive(false);
                    _activeVfx.Remove(instance);
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
    }
}