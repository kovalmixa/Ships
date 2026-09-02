using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Handlers.SceneHandlers
{
    public class SceneController : SingletonMonoBehaviour<SceneController>
    {
        [SerializeField] private GameObject _appCoreContainer;
        [SerializeField] private string _mainMenuSceneName = "MainMenu";
        [SerializeField] private string _loadingScreenWindowName = "LoadingScreen";

        public static event Func<UniTask> OnBeforeSceneLoad;
        public static event Func<UniTask> OnAfterSceneLoad;

        #region Initialization

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(_appCoreContainer);
        }

        private void Start() => InitializeBootWindowsAsync().Forget();

        private async UniTaskVoid InitializeBootWindowsAsync()
        {
            await WindowManager.Instance.OpenWindowIndependent(_loadingScreenWindowName, delayMs: 0);
            await UniTask.Yield(PlayerLoopTiming.Update);
            await WindowManager.Instance.OpenWindowIndependent(_mainMenuSceneName, 0, freezeTime: true);
            await WindowManager.Instance.CloseWindowIndependent(_loadingScreenWindowName);
        }

        #endregion

        #region Scene Loading Logic

        public async UniTask NextLocation(string locationName)
        {
            if (!Application.CanStreamedLevelBeLoaded(locationName))
            {
                Debug.LogWarning($"[SceneController] Scene not found in Build Settings: {locationName}");
                return;
            }

            var token = this.GetCancellationTokenOnDestroy();

            try
            {
                await WindowManager.Instance.OpenWindowIndependent(_loadingScreenWindowName, delayMs: 0);
                await InvokeAsyncEvent(OnBeforeSceneLoad);

                AsyncOperation sceneLoadOperation = SceneManager.LoadSceneAsync(locationName, LoadSceneMode.Single);
                sceneLoadOperation.allowSceneActivation = false;

                var loadingWindow = WindowManager.Instance.GetWindow(_loadingScreenWindowName) as UILoadingWindow;

                while (sceneLoadOperation.progress < 0.9f)
                {
                    if (loadingWindow != null)
                    {
                        float progress = Mathf.Clamp01(sceneLoadOperation.progress / 0.9f);
                        loadingWindow.UpdateProgress(progress);
                    }
                    await UniTask.Yield(PlayerLoopTiming.Update, token);
                }

                sceneLoadOperation.allowSceneActivation = true;
                await sceneLoadOperation.WithCancellation(token);
                await InvokeAsyncEvent(OnAfterSceneLoad);

                if (loadingWindow != null) loadingWindow.UpdateProgress(1f);
                await UniTask.Delay(200, ignoreTimeScale: true, cancellationToken: token);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SceneController] Ошибка при переходе на сцену '{locationName}': {ex}");
            }
            finally
            {
                await WindowManager.Instance.CloseWindowIndependent(_loadingScreenWindowName);
                Debug.Log("[SceneController] Loading screen closed.");
            }
        }

        private async UniTask InvokeAsyncEvent(Func<UniTask> asyncEvent)
        {
            if (asyncEvent == null) return;

            var validTasks = asyncEvent.GetInvocationList()
                .Where(del =>
                {
                    if (del.Target is UnityEngine.Object unityObj) return unityObj != null;
                    return del.Target != null || del.Method.IsStatic;
                })
                .Cast<Func<UniTask>>()
                .Select(subscriber => subscriber.Invoke());

            await UniTask.WhenAll(validTasks);
        }

        #endregion
    }
}