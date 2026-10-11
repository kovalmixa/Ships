using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public abstract class UIWindow : MonoBehaviour
{
    [SerializeField] private float _fadeDuration = 0.25f;

    private UIDocument _uiDocument;
    private VisualElement _cachedRootVisualElement;
    private CancellationTokenSource _cts;

    protected VisualElement RootVisualElement
    {
        get
        {
            if (_cachedRootVisualElement == null)
            {
                if (_uiDocument == null) _uiDocument = GetComponent<UIDocument>();
                if (_uiDocument != null) _cachedRootVisualElement = _uiDocument.rootVisualElement;
            }
            return _cachedRootVisualElement;
        }
    }

    public bool IsOpen => RootVisualElement != null && RootVisualElement.style.opacity.value > 0;

    protected virtual void OnOpened() { }
    protected virtual void OnClosed() { }

    protected virtual void Awake()
    {
        _uiDocument = GetComponent<UIDocument>();
        if (_uiDocument != null) _cachedRootVisualElement = _uiDocument.rootVisualElement;
    }

    public async UniTask OpenAsync(int delayMs = 0, Action onComplete = null)
    {
        ResetCancellationToken();
        var token = _cts.Token;

        try
        {
            gameObject.SetActive(true);

            if (RootVisualElement == null) await UniTask.Yield(PlayerLoopTiming.Update, token);

            if (RootVisualElement == null)
            {
                Debug.LogError($"[UIWindow] RootVisualElement is still null on {gameObject.name}. Ensure a UIDocument component is attached and configured properly.", this);
                return;
            }

            RootVisualElement.style.display = DisplayStyle.Flex;
            RootVisualElement.pickingMode = PickingMode.Position;

            if (delayMs > 0)
            {
                bool isCanceled = await UniTask.Delay(delayMs, cancellationToken: token).SuppressCancellationThrow();
                if (isCanceled) return;
            }

            await FadeAsync(0f, 1f, token);

            if (!token.IsCancellationRequested)
            {
                OnOpened();
                onComplete?.Invoke();
            }
        }
        catch (OperationCanceledException) { }
    }

    public async UniTask CloseAsync(Action onComplete = null)
    {
        ResetCancellationToken();
        var token = _cts.Token;

        try
        {
            if (RootVisualElement != null)
            {
                RootVisualElement.pickingMode = PickingMode.Ignore;
                await FadeAsync(1f, 0f, token);
            }

            if (!token.IsCancellationRequested)
            {
                OnClosed();
                if (RootVisualElement != null)
                    RootVisualElement.style.display = DisplayStyle.None;
                gameObject.SetActive(false);
                onComplete?.Invoke();
            }
        }
        catch (OperationCanceledException) { }
    }

    public void SetStateImmediately(bool isOpen)
    {
        ResetCancellationToken();

        if (RootVisualElement != null)
        {
            RootVisualElement.style.opacity = isOpen ? 1f : 0f;
            RootVisualElement.style.display = isOpen ? DisplayStyle.Flex : DisplayStyle.None;
            RootVisualElement.pickingMode = isOpen ? PickingMode.Position : PickingMode.Ignore;
        }
        gameObject.SetActive(isOpen);
    }

    private async UniTask FadeAsync(float startAlpha, float endAlpha, CancellationToken token)
    {
        float elapsedTime = 0f;
        while (elapsedTime < _fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float progress = elapsedTime / _fadeDuration;

            if (RootVisualElement != null)
                RootVisualElement.style.opacity = Mathf.Lerp(startAlpha, endAlpha, progress);

            bool isCanceled = await UniTask.Yield(PlayerLoopTiming.Update, token).SuppressCancellationThrow();
            if (isCanceled) return;
        }

        if (!token.IsCancellationRequested && RootVisualElement != null)
            RootVisualElement.style.opacity = endAlpha;
    }

    private void ResetCancellationToken()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = new CancellationTokenSource();
    }

    private void OnDestroy()
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }
}