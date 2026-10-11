using Assets.Entity;
using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Handlers.SceneHandlers
{
    public class SceneController : SingletonMonoBehaviour<SceneController>
    {
        [SerializeField] private GameObject _appCoreContainer;
        [SerializeField] private string _mainMenuSceneName = "MainMenu";

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
            await UniTask.Yield(PlayerLoopTiming.Update);
            await WindowHandler.Instance.OpenTab(_mainMenuSceneName, delayMs: 0, freezeTime: true);
        }

        #endregion

        #region Scene Loading Logic

        public async UniTask NextLocation(string locationName)
        {
            if (!Application.CanStreamedLevelBeLoaded(locationName)) return;

            var token = this.GetCancellationTokenOnDestroy();
            try
            {
                await WindowHandler.Instance.OpenWindowIndependent<LoadingScreen>();

                UniTask beforeLoadTask = InvokeAsyncEvent(OnBeforeSceneLoad);

                AsyncOperation sceneLoadOperation = SceneManager.LoadSceneAsync(locationName, LoadSceneMode.Single);
                sceneLoadOperation.allowSceneActivation = false;

                await ShowLoadingProgressAsync(sceneLoadOperation, beforeLoadTask, token);

                sceneLoadOperation.allowSceneActivation = true;
                await sceneLoadOperation.WithCancellation(token);

                if (EntityPoolController.Instance != null)
                    await EntityPoolController.Instance.WaitUntilAllActiveInitializedAsync(token);

                if (GameSessionHandler.Instance != null && GameSessionHandler.Instance.PlayerController == null)
                    await GameSessionHandler.Instance.SpawnPlayer();

                await InvokeAsyncEvent(OnAfterSceneLoad);

                await UniTask.Delay(100, ignoreTimeScale: true, cancellationToken: token);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SceneController] Error transitioning to scene '{locationName}': {ex}");
            }
            finally
            {
                await WindowHandler.Instance.CloseTab<LoadingScreen>();
            }
        }

        private async UniTask ShowLoadingProgressAsync(AsyncOperation operation, UniTask eventTask, CancellationToken token)
        {
            UniTask sceneProgressTask = UniTask.Create(async () =>
            {
                var _loadingWindow = WindowHandler.Instance.GetWindow<LoadingScreen>();
                while (operation.progress < 0.9f)
                {
                    if (_loadingWindow != null)
                    {
                        float progress = Mathf.Clamp01(operation.progress / 0.9f);
                        //_loadingWindow.UpdateProgress(progress);
                    }
                    await UniTask.Yield(PlayerLoopTiming.Update, token);
                }
            });

            await UniTask.WhenAll(sceneProgressTask, eventTask);
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