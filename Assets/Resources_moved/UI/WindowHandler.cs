using Assets.Global.FileGlobal;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Global.SceneHandlers
{
    public class WindowHandler : SingletonMonoBehaviour<WindowHandler>
    {
        private readonly Dictionary<string, UIWindow> _activeWindows = new();

        [Header("Tabs State")]
        [SerializeField] private UIWindow _currentActiveTab;
        private string _currentActiveTabId;

        public bool IsOverUI
        {
            get
            {
                foreach (var pair in _activeWindows)
                {
                    var window = pair.Value;
                    string windowName = pair.Key;

                    if (window == null || !window.IsOpen) continue;

                    if (window.PointerColElements == null)
                    {
                        Debug.LogWarning($"[IsOverUI] У открытого окна '{windowName}' коллекция PointerColElements равна null!");
                        continue;
                    }

                    if (window.PointerColElements.Count > 0)
                    {
                        Debug.Log($"[IsOverUI] Курсор НАД UI! Окно: '{windowName}', элементов под курсором: {window.PointerColElements.Count}");
                        return true;
                    }
                }
                return false;
            }
        }

        #region Setup & Lifecycle

        protected override void Awake()
        {
            base.Awake();

            var allWindows = GetComponentsInChildren<UIWindow>(true);
            foreach (var uiWindow in allWindows)
            {
                if (uiWindow == null) continue;
                string windowName = uiWindow.GetType().Name;

                if (!_activeWindows.ContainsKey(windowName))
                    _activeWindows.Add(windowName, uiWindow);
            }

            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable() => SceneController.OnBeforeSceneLoad += OnBeforeSceneLoadHandler;
        private void OnDisable() => SceneController.OnBeforeSceneLoad -= OnBeforeSceneLoadHandler;

        private async UniTask OnBeforeSceneLoadHandler() => await CloseAllExcept("LoadingScreen");

        #endregion

        #region Public API (String-based)

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

        #endregion

        #region Public API (Type-based / Generic)

        public T GetWindow<T>() where T : UIWindow
        {
            string windowId = typeof(T).Name;
            return GetWindow(windowId) as T;
        }

        public async UniTask SwitchWindow<T>(bool isTab) where T : UIWindow
        {
            await SwitchWindow(typeof(T).Name, isTab);
        }

        public async UniTask OpenTab<T>(int delayMs = 0, bool freezeTime = false, Action onComplete = null) where T : UIWindow
        {
            await OpenTab(typeof(T).Name, delayMs, freezeTime, onComplete);
        }

        public async UniTask CloseTab<T>(bool unfreezeTime = true, Action onComplete = null) where T : UIWindow
        {
            await CloseTab(typeof(T).Name, unfreezeTime, onComplete);
        }

        public async UniTask OpenWindowIndependent<T>(int delayMs = 0, bool freezeTime = false, Action onComplete = null) where T : UIWindow
        {
            await OpenWindowIndependent(typeof(T).Name, delayMs, freezeTime, onComplete);
        }

        public async UniTask CloseWindowIndependent<T>(bool unfreezeTime = true, Action onComplete = null) where T : UIWindow
        {
            await CloseWindowIndependent(typeof(T).Name, unfreezeTime, onComplete);
        }

        #endregion

        #region Tabs Logic (Exclusive Fullscreen/Menu screens)

        public async UniTask OpenTab(string windowId, int delayMs = 0, bool freezeTime = false, Action onComplete = null)
        {
            if (_currentActiveTabId == windowId && _currentActiveTab != null && _currentActiveTab.IsOpen)
                return;

            if (_currentActiveTab != null && !string.IsNullOrEmpty(_currentActiveTabId))
                await CloseTabInternal(_currentActiveTabId, unfreezeTime: false);

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
            if (_activeWindows.ContainsKey(windowId)) await CloseTabInternal(windowId, unfreezeTime, onComplete);
            else Debug.LogWarning($"[WindowHandler] Не удалось закрыть таб '{windowId}', так как он отсутствует в _activeWindows.");
        }

        private async UniTask CloseTabInternal(string windowId, bool unfreezeTime = true, Action onComplete = null)
        {
            if (!_activeWindows.TryGetValue(windowId, out UIWindow window)) return;

            await window.CloseAsync(onComplete);

            if (window == null || window.IsOpen) return;
            if (unfreezeTime) SetTimeScale(false);
            if (_currentActiveTabId == windowId)
            {
                _currentActiveTab = null;
                _currentActiveTabId = null;
            }

            HandleWindowCachingOrDestruction(windowId, window);
        }

        #endregion

        #region Independent Windows Logic (Inventory, Console, etc.)

        public async UniTask OpenWindowIndependent(string windowId, int delayMs = 0, bool freezeTime = false, Action onComplete = null)
        {
            SetTimeScale(freezeTime);
            await OpenWindowInternal(windowId, delayMs, onComplete);
        }

        public async UniTask CloseWindowIndependent(string windowId, bool unfreezeTime = true, Action onComplete = null)
        {
            if (!_activeWindows.TryGetValue(windowId, out UIWindow window)) return;

            await window.CloseAsync(onComplete);

            if (window == null || window.IsOpen) return;
            if (unfreezeTime) SetTimeScale(false);
            if (_currentActiveTabId == windowId)
            {
                _currentActiveTab = null;
                _currentActiveTabId = null;
            }

            HandleWindowCachingOrDestruction(windowId, window);
        }

        #endregion

        #region Common Window Management & Internal Spawning

        public async UniTask CloseAllExcept(params string[] ignoreWindowIds)
        {
            var ignoreSet = new HashSet<string>(ignoreWindowIds ?? Array.Empty<string>());
            var activeKeys = new List<string>(_activeWindows.Keys);
            var closeTasks = new List<UniTask>();

            foreach (var windowId in activeKeys)
            {
                if (ignoreSet.Contains(windowId)) continue;
                if (_activeWindows.TryGetValue(windowId, out var win))
                    closeTasks.Add(CloseWindowGenericInternal(windowId, win, unfreezeTime: false));
            }

            await UniTask.WhenAll(closeTasks);

            if (!string.IsNullOrEmpty(_currentActiveTabId) && !ignoreSet.Contains(_currentActiveTabId))
            {
                _currentActiveTab = null;
                _currentActiveTabId = null;
            }

            SetTimeScale(false);
        }

        private async UniTask OpenWindowInternal(string windowId, int delayMs, Action onComplete)
        {
            if (_activeWindows.TryGetValue(windowId, out UIWindow existingWindow))
            {
                if (existingWindow != null)
                {
                    if (!existingWindow.gameObject.activeSelf)
                        existingWindow.gameObject.SetActive(true);

                    await existingWindow.OpenAsync(delayMs, onComplete);
                }
                return;
            }

            GameObject windowObj = await PrefabLoader.Instance.InstantiatePrefabAsync<GameObject>(windowId, Vector3.zero, Quaternion.identity);

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

        private async UniTask CloseWindowGenericInternal(string windowId, UIWindow window, bool unfreezeTime, Action onComplete = null)
        {
            await window.CloseAsync(onComplete);
            if (window == null || window.IsOpen) return;
            if (unfreezeTime) SetTimeScale(false);

            HandleWindowCachingOrDestruction(windowId, window);
        }

        private void HandleWindowCachingOrDestruction(string windowId, UIWindow window)
        {
            if (window.IsCached)
            {
                if (window.gameObject != null && window.gameObject.activeSelf)
                    window.gameObject.SetActive(false);
            }
            else
            {
                _activeWindows.Remove(windowId);
                if (window != null && window.gameObject != null)
                    Destroy(window.gameObject);
            }
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