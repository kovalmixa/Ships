namespace Assets.Handlers.FileHandlers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;
    using UnityEngine.ResourceManagement.ResourceLocations;

    public class PrefabLoader : SingletonMonoBehaviour<PrefabLoader>
    {
        private readonly Dictionary<string, AsyncOperationHandle<GameObject>> _loadedPrefabs = new();

        public async Task<GameObject> GetPrefabAsync(string id)
        {
            if (_loadedPrefabs.TryGetValue(id, out var existingHandle))
            {
                if (existingHandle.IsDone) return existingHandle.Result;
                await existingHandle.Task;
                return existingHandle.Result;
            }

            IList<IResourceLocation> locations;
            var checkHandle = Addressables.LoadResourceLocationsAsync(id);
            locations = await checkHandle.Task;

            if (locations == null || locations.Count == 0)
            {
                Debug.LogWarning($"[Addressables] Ключ '{id}' не найден в системе Addressables Groups! Проверьте настройки префаба.");
                return null;
            }

            // 2. Загружаем префаб, если ключ существует
            var handle = Addressables.LoadAssetAsync<GameObject>(id);
            _loadedPrefabs[id] = handle;

            GameObject prefab = await handle.Task;

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"[Addressables] Ошибка при загрузке префаба '{id}'.");
                _loadedPrefabs.Remove(id);
                return null;
            }

            return prefab;
        }

        public GameObject GetPrefabSync(string id)
        {
            if (_loadedPrefabs.TryGetValue(id, out var existingHandle))
            {
                return existingHandle.Result;
            }

            var handle = Addressables.LoadAssetAsync<GameObject>(id);
            GameObject prefab = handle.WaitForCompletion();

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _loadedPrefabs[id] = handle;
                return prefab;
            }

            Debug.LogError($"[Addressables] Ошибка синхронной загрузки ID: '{id}'");
            return null;
        }

        public async Task<GameObject> InstantiatePrefabAsync(string id, Vector3 pos, Quaternion rot, Transform parent = null)
        {
            var instanceHandle = Addressables.InstantiateAsync(id, pos, rot, parent);
            return await instanceHandle.Task;
        }

        public void ReleaseInstance(GameObject instance)
        {
            if (instance == null) return;
            Addressables.ReleaseInstance(instance);
        }

        public void UnloadPrefab(string id)
        {
            if (_loadedPrefabs.TryGetValue(id, out var handle))
            {
                Addressables.Release(handle);
                _loadedPrefabs.Remove(id);
            }
        }
    }
}