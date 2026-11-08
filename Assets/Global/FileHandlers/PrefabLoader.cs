namespace Assets.Global.FileGlobal
{
    using Cysharp.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;

    public class PrefabLoader : SingletonMonoBehaviour<PrefabLoader>
    {
        public async UniTask<T> LoadAssetAsync<T>(string id) where T : UnityEngine.Object
        {
            if (!await CheckAddressableExistsAsync(id))
            {
                Debug.LogWarning($"[Addressables] Ключ '{id}' не найден в Addressables Groups!");
                return null;
            }

            try
            {
                var handle = Addressables.LoadAssetAsync<T>(id);
                var result = await handle.Task;

                if (handle.Status != AsyncOperationStatus.Succeeded)
                {
                    Debug.LogWarning($"[Addressables] Ошибка загрузки ассета '{id}' типа {typeof(T).Name}");
                    return null;
                }

                return result;
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[Addressables] Исключение при загрузке ассета '{id}': {ex.Message}");
                return null;
            }
        }

        public T LoadAssetSync<T>(string id) where T : Object
        {
            try
            {
                var handle = Addressables.LoadAssetAsync<T>(id);
                T result = handle.WaitForCompletion();

                if (handle.Status != AsyncOperationStatus.Succeeded)
                {
                    Debug.LogWarning($"[Addressables] Ошибка синхронной загрузки ассета '{id}' типа {typeof(T).Name}");
                    return null;
                }

                return result;
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[Addressables] Исключение при синхронной загрузке ассета '{id}': {ex.Message}");
                return null;
            }
        }

        public async UniTask<bool> CheckAddressableExistsAsync(string key)
        {
            if (string.IsNullOrEmpty(key)) return false;

            try
            {
                var handle = Addressables.LoadResourceLocationsAsync(key);
                var locations = await handle.Task;

                bool exists = handle.Status == AsyncOperationStatus.Succeeded
                              && locations != null
                              && locations.Count > 0;
                Addressables.Release(handle);
                return exists;
            }
            catch
            {
                return false;
            }
        }

        public async UniTask<T> InstantiatePrefabAsync<T>(string id, Vector3 pos, Quaternion rot, Transform parent = null) where T : class
        {
            if (!await CheckAddressableExistsAsync(id))
            {
                Debug.LogWarning($"[Addressables] Невозможно инстанцировать префаб '{id}' — ключ не найден.");
                return default;
            }

            try
            {
                var instanceHandle = Addressables.InstantiateAsync(id, pos, rot, parent);
                GameObject instance = await instanceHandle.Task;

                if (instanceHandle.Status != AsyncOperationStatus.Succeeded || instance == null)
                {
                    Debug.LogWarning($"[Addressables] Ошибка инстанцирования префаба '{id}'.");
                    return default;
                }

                var releaser = instance.AddComponent<AddressableInstanceReleaser>();
                releaser.Initialize(instanceHandle);

                return ConvertResult<T>(instance);
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[Addressables] Исключение при инстанцировании '{id}': {ex.Message}");
                return default;
            }
        }

        public T InstantiatePrefabSync<T>(string id, Vector3 pos, Quaternion rot, Transform parent = null) where T : class
        {
            var handle = Addressables.InstantiateAsync(id, pos, rot, parent);
            GameObject instance = handle.WaitForCompletion();

            if (handle.Status != AsyncOperationStatus.Succeeded || instance == null)
            {
                Debug.LogWarning($"[Addressables] Ошибка синхронного инстанцирования префаба '{id}'.");
                return default;
            }

            var releaser = instance.AddComponent<AddressableInstanceReleaser>();
            releaser.Initialize(handle);

            return ConvertResult<T>(instance);
        }

        private T ConvertResult<T>(GameObject instance) where T : class
        {
            if (typeof(T) == typeof(GameObject)) return instance as T;
            if (typeof(T).IsSubclassOf(typeof(Component))) return instance.GetComponent<T>();

            Debug.LogError($"[Addressables] Тип {typeof(T).Name} не поддерживается для возврата из InstantiatePrefab.");
            return default;
        }

        public void ReleaseInstance(GameObject instance)
        {
            if (instance == null) return;
            Addressables.ReleaseInstance(instance);
        }
    }

    public class AddressableInstanceReleaser : MonoBehaviour
    {
        private AsyncOperationHandle<GameObject> _handle;
        private bool _isReleased;

        public void Initialize(AsyncOperationHandle<GameObject> handle)
        {
            _handle = handle;
            _isReleased = false;
        }

        private void OnDestroy() => Release();

        public void Release()
        {
            if (_isReleased) return;
            _isReleased = true;

            if (_handle.IsValid()) Addressables.ReleaseInstance(_handle);
        }
    }
}