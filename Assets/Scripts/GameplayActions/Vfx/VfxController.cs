using Assets.Handlers.CommonParents;
using Assets.Handlers.FileHandlers;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Assets.Scripts.Actions.VFX
{
    public enum VfxType
    {
        None = 0,
        //Bullet
        bulletLaunch = 1,
        bulletMetalHit = 2,
        bulletGroundHit = 3,
        bulletWaterHit = 4,
        bulletFlashHit = 5,
        bulletExplosion = 6,
        //...
    }

    public class VfxController : SingletonPoolHandler<VfxController, VfxInstance>
    {
        private readonly Dictionary<VfxType, IObjectPool<VfxInstance>> _pools = new();
        private readonly Dictionary<VfxType, AsyncLazy<IObjectPool<VfxInstance>>> _loadingTasks = new();
        private readonly List<VfxInstance> _activeVfx = new();
        private bool _isClearing;

        #region Setup

        protected override async UniTask ClearOnSceneChangeAsync()
        {
            _isClearing = true;

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
                _isClearing = false;
            }
        }

        protected override void Awake() => base.Awake();

        #endregion

        public void PlayEffect(InteractionContext context, VfxType type, Vector3 position, Quaternion rotation)
        {
            PlayEffectAsync(context, type, position, rotation).Forget();
        }

        private async UniTaskVoid PlayEffectAsync(InteractionContext context, VfxType type, Vector3 position, Quaternion rotation)
        {
            if (_isClearing || type == VfxType.None) return;

            if (!_pools.TryGetValue(type, out var pool))
            {
                if (!_loadingTasks.TryGetValue(type, out var lazyLoad))
                {
                    // AsyncLazy гарантирует, что метод выполнится ровно 1 раз, сколько бы await к нему ни обратилось
                    lazyLoad = UniTask.Lazy(() => CreatePoolAsync(type));
                    _loadingTasks[type] = lazyLoad;
                }

                pool = await lazyLoad.Task;

                if (_isClearing || pool == null) return;

                if (!_pools.ContainsKey(type))
                {
                    _pools[type] = pool;
                    _loadingTasks.Remove(type);
                }
            }

            if (_isClearing) return;

            VfxInstance instance = pool.Get();
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
            var typeName = type.ToString();
            var id = char.ToLower(typeName[0]) + typeName.Substring(1);

            GameObject prefab = await PrefabLoader.Instance.GetPrefabAsync(id);
            if (prefab == null)
            {
                Debug.LogWarning($"[VfxController] Не удалось загрузить префаб для эффекта: {id}");
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
                    instance.gameObject.SetActive(true);
                    _activeVfx.Add(instance);
                },
                actionOnRelease: instance =>
                {
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