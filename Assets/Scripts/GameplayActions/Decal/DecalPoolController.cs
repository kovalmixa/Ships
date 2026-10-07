using System.Collections.Generic;
using Assets.Handlers.CommonParents;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;

namespace Assets.Scripts.Actions.Decals
{
    public class DecalPoolController : SingletonPoolController<DecalPoolController, DecalInstance>
    {
        [SerializeField] private DecalInstance _prefab;
        private IObjectPool<DecalInstance> _pool;
        private readonly List<DecalInstance> _activeDecals = new();

        protected override void Awake()
        {
            base.Awake();
            InitializePool();
        }

        private void InitializePool()
        {
            if (_prefab == null)
            {
                Debug.LogError($"[{nameof(DecalPoolController)}] DecalPrefab is not assigned in Inspector!");
                return;
            }

            _pool = new ObjectPool<DecalInstance>(
                createFunc: () =>
                {
                    Transform parent = poolNode != null ? poolNode.transform : transform;
                    DecalInstance instance = Instantiate(_prefab, parent);
                    instance.gameObject.SetActive(false);
                    return instance;
                },
                actionOnGet: instance =>
                {
                    if (instance == null) return;
                    instance.gameObject.SetActive(true);
                    _activeDecals.Add(instance);
                },
                actionOnRelease: instance =>
                {
                    if (instance == null) return;
                    instance.gameObject.SetActive(false);
                    _activeDecals.Remove(instance);
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

            PrewarmPool(_pool, initialCapacity);
        }

        public DecalInstance SpawnDecal(Sprite sprite, Vector3 position, Quaternion rotation, float disappearTime = -1f)
        {
            if (isClearing || _pool == null || sprite == null) return null;

            DecalInstance instance = _pool.Get();
            if (instance == null) return null;

            instance.transform.SetPositionAndRotation(position, rotation);

            instance.Setup(
                sprite: sprite,
                onRelease: () =>
                {
                    if (instance != null && instance.gameObject != null)
                        _pool.Release(instance);
                },
                disappearTime: disappearTime
            );

            return instance;
        }

        protected override async UniTask ClearOnSceneChangeAsync()
        {
            isClearing = true;

            try
            {
                var targetsToRelease = _activeDecals.ToArray();
                for (int i = 0; i < targetsToRelease.Length; i++)
                {
                    DecalInstance instance = targetsToRelease[i];
                    if (instance != null && instance.gameObject.activeSelf)
                        instance.Release();

                    if (IsIndexOverClearDelay(i)) await UniTask.Yield();
                }

                _activeDecals.Clear();
            }
            finally
            {
                isClearing = false;
            }
        }
    }
}