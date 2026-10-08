using Assets.Handlers.CommonParents;
using Assets.Handlers.Enums;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Assets.Scripts.Actions.Corpse
{
    public class CorpsePoolController : SingletonPoolController<CorpsePoolController, CorpseInstance>
    {
        [SerializeField] private CorpseInstance _prefab;
        private IObjectPool<CorpseInstance> _pool;
        private readonly List<CorpseInstance> _activeCorpses = new();

        protected override void Awake()
        {
            base.Awake();
            InitializePool();
        }

        private void InitializePool()
        {
            if (_prefab == null)
            {
                Debug.LogError($"[{nameof(CorpsePoolController)}] Corpse prefab is not assigned in Inspector!");
                return;
            }

            _pool = new ObjectPool<CorpseInstance>(
                createFunc: () =>
                {
                    Transform parent = poolNode != null ? poolNode.transform : transform;
                    var instance = Instantiate(_prefab, parent);
                    instance.gameObject.SetActive(false);
                    return instance;
                },
                actionOnGet: instance =>
                {
                    if (instance == null) return;
                    instance.gameObject.SetActive(true);
                    _activeCorpses.Add(instance);
                },
                actionOnRelease: instance =>
                {
                    if (instance == null) return;
                    instance.gameObject.SetActive(false);
                    _activeCorpses.Remove(instance);
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

        public CorpseInstance SpawnCorpse(Transform source, SortingLayerType layer,
                                          Sprite hullSpriteOverride = null, float disappearTime = -1f)
        {
            if (isClearing || _pool == null || source == null) return null;
            var instance = _pool.Get();
            if (instance == null) return null;

            instance.Setup(
                source, layer, hullSpriteOverride,
                onRelease: () =>
                {
                    if (instance != null && instance.gameObject != null)
                        _pool.Release(instance);
                },
                disappearTime: disappearTime);

            return instance;
        }

        protected override async UniTask ClearOnSceneChangeAsync()
        {
            isClearing = true;
            try
            {
                var targets = _activeCorpses.ToArray();
                for (int i = 0; i < targets.Length; i++)
                {
                    if (targets[i] != null && targets[i].gameObject.activeSelf)
                        targets[i].Release();

                    if (IsIndexOverClearDelay(i)) await UniTask.Yield();
                }
                _activeCorpses.Clear();
            }
            finally
            {
                isClearing = false;
            }
        }
    }
}