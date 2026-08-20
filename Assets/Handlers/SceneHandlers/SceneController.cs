using System;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Assets.Handlers.FileHandlers;
using Entity.Controllers;

namespace Assets.Handlers.SceneHandlers
{
    public class SceneController : SingletonMonoBehaviour<SceneController>
    {
        [Header("Core Container")]
        [SerializeField] private GameObject appCoreContainer;

        [Header("Player Settings")]
        public EntityController playerController;

        [Header("Save / Load Settings")]
        public string fileName;

        public static event Action OnBeforeSceneLoad;

        #region Unity Lifecycle

        protected override void Awake()
        {
            base.Awake();
            if (appCoreContainer != null) DontDestroyOnLoad(appCoreContainer);
            else DontDestroyOnLoad(gameObject);
        }

        private async void Start()
        {
            await InitializeSaveDataAsync();
        }

        #endregion

        #region Scene Loading Logic

        public async UniTask NextLocation(string locationName)
        {
            if (!Application.CanStreamedLevelBeLoaded(locationName))
            {
                Debug.LogWarning($"[SceneController] Scene not found by name: {locationName}");
                return;
            }

            OnBeforeSceneLoad?.Invoke();

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(locationName, LoadSceneMode.Single);
            while (!asyncLoad.isDone) await UniTask.Yield();
        }

        #endregion

        #region Save / Load Logic

        private async UniTask InitializeSaveDataAsync()
        {
            try
            {
                if (playerController == null)
                {
                    Debug.LogWarning("[SceneController] PlayerController is not assigned!");
                    return;
                }

                string path = Path.Combine(Application.streamingAssetsPath, "Saves", fileName);
                SaveDataBundle data = LoadData(path);

                if (data == null)
                    throw new Exception($"Data file could not be loaded from path: {path}");

                await ExtractDataAsync(data);
            }
            catch (Exception e)
            {
                Debug.LogError($"[SceneController] Failed to initialize save data: {e}");
            }
        }

        private SaveDataBundle LoadData(string path) => DataFileHandler.LoadFromJson<SaveDataBundle>(path);

        private async UniTask ExtractDataAsync(SaveDataBundle data)
        {
            if (playerController != null) await playerController.Setup(data.entityDataContainer);
        }

        #endregion

        #region Scene Search Helpers

        public static GameObject GetNodeByName(string name)
        {
            GameObject node = GameObject.Find(name);
            if (node == null)
            {
                Transform dontDestroy = GameObject.Find("DontDestroyOnLoad")?.transform;
                if (dontDestroy != null)
                {
                    Transform found = dontDestroy.Find(name);
                    if (found != null) return found.gameObject;
                }
                return null;
            }
            return node;
        }

        public static T GetNodeByType<T>() where T : Component
        {
            T node = FindAnyObjectByType<T>(FindObjectsInactive.Include);
            if (node == null)
            {
                Transform dontDestroy = GameObject.Find("DontDestroyOnLoad")?.transform;
                if (dontDestroy != null)
                    node = dontDestroy.GetComponentInChildren<T>(true);
            }

            return node;
        }

        #endregion
    }
}