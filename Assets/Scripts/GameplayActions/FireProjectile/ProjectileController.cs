using Assets.Handlers.Enums;
using Assets.Handlers.FileHandlers;
using Assets.Handlers.SceneHandlers;
using GameplayActions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Actions.Projectile
{
    public class ProjectileController : SingletonMonoBehaviour<ProjectileController>
    {
        private ObjectPoolHandler _projectilePool;
        [SerializeField] private string _projectilePrefabFolder = "PRJT";
        private Dictionary<ProjectileType, GameObject> _prefabDict = new();

        protected override void Awake()
        {
            base.Awake();
            _projectilePool = ObjectPoolHandler.GetInstance(PoolType.Projectile);
            var prefabLoader = PrefabLoader.Instance;
            foreach (ProjectileType type in Enum.GetValues(typeof(ProjectileType)))
            {
                string id = $"{_projectilePrefabFolder}/{type.ToString()}";
                _prefabDict[type] = prefabLoader.GetPrefab(id);
            }
        }

        public void Launch(InteractionContext interactionContext, ProjectileDataSO data, Vector2 targetPosition)
        {
            if (!_prefabDict.TryGetValue(data.type, out var prefab) || prefab == null) return;
            GameObject projectileGO = ObjectPoolHandler.Get(prefab, data.startPosition, Quaternion.identity);
            if (projectileGO.TryGetComponent<ProjectileInstance>(out var instance))
            {
                var sourceTransform = interactionContext.SourceObject?.transform;
                instance.Setup(
                    interactionContext,
                    data,
                    () => ObjectPoolHandler.Release(prefab, projectileGO),
                    sourceTransform
                );
            }
        }

        private void Update()
        {
            if (_projectilePool == null) return;
            GameObject[] activeObjects = _projectilePool.GetAllActive();
            if (activeObjects == null) return;
            for (int i = 0; i < activeObjects.Length; i++)
            {
                var go = activeObjects[i];
                if (go != null && go.TryGetComponent<ProjectileInstance>(out var proj))
                    proj.Tick(Time.deltaTime);
            }
        }
    }
}
