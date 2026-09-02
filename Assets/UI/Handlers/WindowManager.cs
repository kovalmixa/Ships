using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Assets.Handlers.FileHandlers;

namespace Assets.Handlers.SceneHandlers
{
    public class WindowManager : SingletonMonoBehaviour<WindowManager>
    {
        private readonly Dictionary<string, UIWindow> _activeWindows = new Dictionary<string, UIWindow>();
        private UIWindow _currentActiveTab;
        private string _currentActiveTabId;

        #region Setup & Lifecycle

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable() => SceneController.OnBeforeSceneLoad += OnBeforeSceneLoadHandler;

        private void OnDisable() => SceneController.OnBeforeSceneLoad -= OnBeforeSceneLoadHandler;

        private async UniTask OnBeforeSceneLoadHandler() => await CloseAllExcept("LoadingScreen");

        #endregion

        #region Public API

        public UIWindow GetWindow(string windowId)
        {
            if (_activeWindows.TryGetValue(windowId, out UIWindow window)) return window;
            return null;
        }

        public async UniTask SwitchWindow(string windowId, bool isTab)
        {
            var window = GetWindow(windowId);
            if (window == null || !window.IsOpen)
            {
                if (isTab) await OpenTab(windowId);
                else await OpenWindowIndependent(windowId);
                return;
            }
            if (isTab) await CloseTab(windowId);
            else await CloseWindowIndependent(windowId);
        }

        #region Tabs logic

        public async UniTask OpenTab(string windowId, int delayMs = 0, bool freezeTime = false, Action onComplete = null)
        {
            if (_currentActiveTabId == windowId && _currentActiveTab != null && _currentActiveTab.IsOpen) return;

            if (_currentActiveTab != null) await CloseWindowInternal(_currentActiveTabId, unfreezeTime: false);

            SetTimeScale(freezeTime);
            await OpenWindowInternal(windowId, delayMs, onComplete);

            if (_activeWindows.TryGetValue(windowId, out UIWindow newTab))
            {
                _currentActiveTab = newTab;
                _currentActiveTabId = windowId;
            }
        }

        public async UniTask CloseTab(string windowId, bool unfreezeTime = true, Action onComplete = null)
        {
            if (_currentActiveTabId == windowId || _activeWindows.ContainsKey(windowId))
                await CloseWindowInternal(windowId, unfreezeTime, onComplete);
        }

        #endregion

        #region Independent Windows Logic

        public async UniTask OpenWindowIndependent(string windowId, int delayMs = 0, bool freezeTime = false, Action onComplete = null)
        {
            SetTimeScale(freezeTime);
            await OpenWindowInternal(windowId, delayMs, onComplete);
        }

        public async UniTask CloseWindowIndependent(string windowId, bool unfreezeTime = true, Action onComplete = null)
        {
            await CloseWindowInternal(windowId, unfreezeTime, onComplete);
        }

        public async UniTask CloseAllExcept(params string[] ignoreWindowIds)
        {
            var ignoreSet = new HashSet<string>(ignoreWindowIds ?? Array.Empty<string>());
            var activeKeys = new List<string>(_activeWindows.Keys);
            var closeTasks = new List<UniTask>();

            foreach (var windowId in activeKeys)
            {
                if (ignoreSet.Contains(windowId)) continue;
                closeTasks.Add(CloseWindowInternal(windowId, unfreezeTime: false));
            }

            await UniTask.WhenAll(closeTasks);
            SetTimeScale(false);
        }

        #endregion

        #endregion

        #region Internal Spawning & Lifecycle

        private async UniTask OpenWindowInternal(string windowId, int delayMs, Action onComplete)
        {
            if (_activeWindows.TryGetValue(windowId, out UIWindow existingWindow))
            {
                await existingWindow.OpenAsync(delayMs, onComplete);
                return;
            }

            GameObject windowObj = await PrefabLoader.Instance.InstantiatePrefabAsync(windowId, Vector3.zero, Quaternion.identity);

            if (windowObj == null)
            {
                Debug.LogError($"[WindowManager] Не удалось загрузить или заспавнить окно: '{windowId}'");
                return;
            }

            DontDestroyOnLoad(windowObj);

            if (!windowObj.TryGetComponent<UIWindow>(out var window))
            {
                Debug.LogError($"[WindowManager] На префабе '{windowId}' отсутствует компонент UIWindow!");
                Destroy(windowObj);
                return;
            }

            _activeWindows[windowId] = window;
            ConfigureUnscaledTime(windowObj);
            await window.OpenAsync(delayMs, onComplete);
        }

        private async UniTask CloseWindowInternal(string windowId, bool unfreezeTime, Action onComplete = null)
        {
            if (!_activeWindows.TryGetValue(windowId, out UIWindow window)) return;

            await window.CloseAsync(onComplete);
            if (unfreezeTime) SetTimeScale(false);
            _activeWindows.Remove(windowId);

            if (_currentActiveTabId == windowId)
            {
                _currentActiveTab = null;
                _currentActiveTabId = null;
            }
            if (window != null && window.gameObject != null) Destroy(window.gameObject);
        }

        private void SetTimeScale(bool freeze) => Time.timeScale = freeze ? 0f : 1f;

        private void ConfigureUnscaledTime(GameObject windowObject)
        {
            var animators = windowObject.GetComponentsInChildren<Animator>(true);
            foreach (var anim in animators) anim.updateMode = AnimatorUpdateMode.UnscaledTime;
        }

        #endregion
    }
}