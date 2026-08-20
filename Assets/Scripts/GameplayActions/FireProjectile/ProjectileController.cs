using Assets.Common.Interfaces;
using Assets.Handlers.CommonParents;
using Assets.Handlers.Enums;
using Assets.Handlers.FileHandlers;
using GameplayActions;
using System;
using System.Collections.Generic;
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

        protected override void ClearOnSceneChange()
        {
            foreach (IPoolInstance instance in _activeProjectiles) instance.ReleaseToPool();
            _activeProjectiles.Clear();
        }

        async protected override void Awake()
        {
            base.Awake();
            initialCapacity = 20;
            maxPoolSize = 200;

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

        private void PrewarmPool(IObjectPool<ProjectileInstance> pool, int amount)
        {
            var tempList = new List<ProjectileInstance>(amount);
            for (int i = 0; i < amount; i++) tempList.Add(pool.Get());
            foreach (var item in tempList) pool.Release(item);
        }

        #endregion

        public void Launch(InteractionContext interactionContext, ProjectileData data, Vector2 targetPosition)
        {
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
            float dt = Time.deltaTime;
            for (int i = _activeProjectiles.Count - 1; i >= 0; i--) _activeProjectiles[i].Tick(dt);
        }
    }
}