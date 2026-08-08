using Assets.Handlers.Enums;
using Assets.Handlers.FileHandlers;
using Assets.Handlers.SceneHandlers;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Assets.Scripts.Actions.VFX
{
    public enum VfxType { None, Explosion, HitMetal, MuzzleFlash }

    public class VfxController : SingletonMonoBehaviour<VfxController>
    {
        [SerializeField] private GameObject _vfxPoolNode;
        [SerializeField] private int _initialCapacity = 10;
        [SerializeField] private int _maxPoolSize = 100;

        private Dictionary<VfxType, GameObject> _prefabDict = new();
        private Dictionary<VfxType, IObjectPool<VfxInstance>> _pools = new();

        #region Setup

        public void ClearOnSceneChange()
        {
            foreach (var pool in _pools.Values) pool.Clear();
            _pools.Clear();
        }

        private void OnEnable() => SceneController.OnBeforeSceneLoad += ClearOnSceneChange;
        private void OnDisable() => SceneController.OnBeforeSceneLoad -= ClearOnSceneChange;

        async protected override void Awake()
        {
            base.Awake();
            var prefabLoader = PrefabLoader.Instance;

            foreach (VfxType type in Enum.GetValues(typeof(VfxType)))
            {
                if (type == VfxType.None) continue;

                var typeName = type.ToString();
                var id = char.ToLower(typeName[0]) + typeName.Substring(1);

                GameObject prefab = await prefabLoader.GetPrefabAsync(id);
                if (prefab != null)
                {
                    _prefabDict[type] = prefab;
                    var pool = CreatePoolForType(prefab);
                    _pools[type] = pool;
                    PrewarmPool(pool, _initialCapacity);
                }
            }
        }

        private IObjectPool<VfxInstance> CreatePoolForType(GameObject prefab)
        {
            return new ObjectPool<VfxInstance>(
                createFunc: () =>
                {
                    Transform parent = _vfxPoolNode != null ? _vfxPoolNode.transform : transform;
                    var go = Instantiate(prefab, parent);
                    return go.GetComponent<VfxInstance>();
                },
                actionOnGet: instance => { /* ¬ключение происходит в самом Play */ },
                actionOnRelease: instance => instance.gameObject.SetActive(false),
                actionOnDestroy: instance =>
                {
                    if (instance != null && instance.gameObject != null) Destroy(instance.gameObject);
                },
                collectionCheck: true,
                defaultCapacity: _initialCapacity,
                maxSize: _maxPoolSize
            );
        }

        private void PrewarmPool(IObjectPool<VfxInstance> pool, int amount)
        {
            var tempList = new List<VfxInstance>(amount);
            for (int i = 0; i < amount; i++) tempList.Add(pool.Get());
            foreach (var item in tempList) pool.Release(item);
        }

        #endregion

        public void PlayEffect(VfxType type, Vector3 position, Quaternion rotation)
        {
            if (!_pools.TryGetValue(type, out var pool))
            {
                Debug.LogWarning($"[VfxController] ѕул дл€ эффекта {type} не найден!");
                return;
            }

            VfxInstance instance = pool.Get();
            instance.Play(
                position,
                rotation,
                onRelease: () => pool.Release(instance)
            );
        }
    }
}