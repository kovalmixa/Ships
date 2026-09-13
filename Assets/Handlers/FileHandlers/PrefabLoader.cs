namespace Assets.Handlers.FileHandlers
{
    using Cysharp.Threading.Tasks;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;
    using UnityEngine.ResourceManagement.ResourceLocations;

    public class PrefabLoader : SingletonMonoBehaviour<PrefabLoader>
    {
        private readonly Dictionary<string, AsyncOperationHandle<GameObject>> _loadedPrefabs = new();

        public async UniTask<GameObject> GetPrefabAsync(string id)
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
                Debug.LogWarning($"[Addressables] Key '{id}' not found in the Addressables Groups system! Check the prefab settings.");
                return null;
            }

            var handle = Addressables.LoadAssetAsync<GameObject>(id);
            _loadedPrefabs[id] = handle;
            GameObject prefab = await handle.Task;

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"[Addressables] Error loading prefab '{id}'.");
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

        public async UniTask<bool> CheckAddressableExistsAsync(string key)
        {
            var handle = Addressables.LoadResourceLocationsAsync(key);
            var locations = await handle.Task;

            bool exists = handle.Status == AsyncOperationStatus.Succeeded
                          && locations != null
                          && locations.Count > 0;
            Addressables.Release(handle);
            return exists;
        }

        public async UniTask<GameObject> InstantiatePrefabAsync(string id, Vector3 pos, Quaternion rot, Transform parent = null)
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