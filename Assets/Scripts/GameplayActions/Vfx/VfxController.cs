using Assets.Handlers.CommonParents;
using Assets.Handlers.FileHandlers;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        private readonly Dictionary<VfxType, Task<IObjectPool<VfxInstance>>> _loadingTasks = new();

        #region Setup

        protected override void ClearOnSceneChange()
        {
            foreach (var pool in _pools.Values) pool.Clear();
            _pools.Clear();
            _loadingTasks.Clear();
        }

        protected override void Awake()
        {
            base.Awake();
            initialCapacity = 5;
            maxPoolSize = 50;
        }

        #endregion

        public async void PlayEffect(InteractionContext context, VfxType type, Vector3 position, Quaternion rotation)
        {
            if (type == VfxType.None) return;
            if (!_pools.TryGetValue(type, out var pool))
            {
                if (!_loadingTasks.TryGetValue(type, out var loadTask))
                {
                    loadTask = CreatePoolAsync(type);
                    _loadingTasks[type] = loadTask;
                }

                pool = await loadTask;
                _loadingTasks.Remove(type);

                if (pool == null) return;
                _pools[type] = pool;
            }

            VfxInstance instance = pool.Get();
            instance.Play(
                context,
                position,
                rotation,
                onRelease: () => pool.Release(instance)
            );
        }

        private async Task<IObjectPool<VfxInstance>> CreatePoolAsync(VfxType type)
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
                actionOnGet: instance => { },
                actionOnRelease: instance => instance.gameObject.SetActive(false),
                actionOnDestroy: instance =>
                {
                    if (instance != null && instance.gameObject != null) Destroy(instance.gameObject);
                },
                collectionCheck: true,
                defaultCapacity: initialCapacity,
                maxSize: maxPoolSize
            );
        }
    }
}