using System;
using System.Collections.Generic;
using Assets.Common.Interfaces;
using Assets.Handlers.CommonParents;
using Assets.Handlers.Enums;
using Assets.Handlers.FileHandlers;
using Cysharp.Threading.Tasks;
using GameplayActions;
using UnityEngine;
using UnityEngine.Pool;

namespace Assets.Scripts.Actions.Projectile
{
    public class ProjectileController : SingletonPoolHandler<ProjectileController, ProjectileInstance>
    {
        private Dictionary<ProjectileType, GameObject> _prefabDict = new();
        private Dictionary<ProjectileType, IObjectPool<ProjectileInstance>> _pools = new();
        private List<ProjectileInstance> _activeProjectiles = new();

        #region Setup

        protected override async UniTask ClearOnSceneChangeAsync()
        {
            isClearing = true;
            try
            {
                var targetsToRelease = _activeProjectiles.ToArray();
                for (int i = 0; i < targetsToRelease.Length; i++)
                {
                    IPoolInstance instance = targetsToRelease[i];
                    instance?.ReleaseToPool();
                    if (IsIndexOverClearDelay(i)) await UniTask.Yield();
                }
                _activeProjectiles.Clear();
            }
            finally
            {
                isClearing = false;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            initialCapacity = 20;
            maxPoolSize = 200;
        }

        protected override async UniTask PrewarmAsync()
        {
            var prefabLoader = PrefabLoader.Instance;

            foreach (ProjectileType type in Enum.GetValues(typeof(ProjectileType)))
            {
                if (type == ProjectileType.None) continue;

                var typeName = type.ToString();
                var id = char.ToLower(typeName[0]) + typeName.Substring(1);
                GameObject prefab = await prefabLoader.GetPrefabAsync(id);

                if (prefab != null)
                {
                    _prefabDict[type] = prefab;
                    var pool = CreatePoolForType(prefab);
                    _pools[type] = pool;
                    PrewarmPool(pool, initialCapacity);
                }
            }
        }

        private IObjectPool<ProjectileInstance> CreatePoolForType(GameObject prefab)
        {
            return new ObjectPool<ProjectileInstance>(
                createFunc: () =>
                {
                    Transform parent = poolNode != null ? poolNode.transform : transform;
                    var go = Instantiate(prefab, parent);
                    return go.GetComponent<ProjectileInstance>();
                },
                actionOnGet: instance =>
                {
                    instance.gameObject.SetActive(true);
                    _activeProjectiles.Add(instance);
                },
                actionOnRelease: instance =>
                {
                    instance.gameObject.SetActive(false);
                    _activeProjectiles.Remove(instance);
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

        public void Launch(InteractionContext interactionContext, ProjectileData data, Vector2 targetPosition)
        {
            if (isClearing || !isPrewarmed)
            {
                Debug.LogWarning("[ProjectileController] Launch skipped: pool is not ready or clearing in progress.");
                return;
            }

            if (!_pools.TryGetValue(data.type, out var pool)) return;

            ProjectileInstance instance = pool.Get();
            instance.Setup(
                interactionContext,
                data,
                () => pool.Release(instance),
                targetPosition
            );
        }

        private void Update()
        {
            if (isClearing) return;

            float dt = Time.deltaTime;
            for (int i = _activeProjectiles.Count - 1; i >= 0; i--)
                _activeProjectiles[i].Tick(dt);
        }
    }
}