using System;
using System.Threading;
using Assets.Handlers.SceneHandlers;
using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public abstract class UIWindow : MonoBehaviour
{
    [SerializeField] private float _fadeDuration = 0.25f;

    private CanvasGroup _canvasGroup;
    private CancellationTokenSource _cts;

    protected virtual void OnOpened() { }

    protected virtual void OnClosed() { }

    public string WindowId => GameObjectHandler.GenerateUniqueId(name);
    public bool IsOpen => CanvasGroup != null && CanvasGroup.alpha > 0;

    protected CanvasGroup CanvasGroup
    {
        get
        {
            if (_canvasGroup == null) _canvasGroup = GetComponent<CanvasGroup>();
            return _canvasGroup;
        }
    }

    private void Awake() => _ = CanvasGroup;

    public async UniTask OpenAsync(int delayMs = 0, Action onComplete = null)
    {
        ResetCancellationToken();
        var token = _cts.Token;

        try
        {
            gameObject.SetActive(true);

            if (delayMs > 0)
            {
                bool isCanceled = await UniTask.Delay(delayMs, cancellationToken: token).SuppressCancellationThrow();
                if (isCanceled) return;
            }

            CanvasGroup.interactable = true;
            CanvasGroup.blocksRaycasts = true;

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
        DisableRaycasts();
        ResetCancellationToken();
        var token = _cts.Token;

        try
        {
            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = false;

            await FadeAsync(1f, 0f, token);

            if (!token.IsCancellationRequested)
            {
                OnClosed();
                gameObject.SetActive(false);
                onComplete?.Invoke();
            }
        }
        catch (OperationCanceledException) { }
    }

    public void SetStateImmediately(bool isOpen)
    {
        ResetCancellationToken();

        CanvasGroup.alpha = isOpen ? 1f : 0f;
        CanvasGroup.interactable = isOpen;
        CanvasGroup.blocksRaycasts = isOpen;
        gameObject.SetActive(isOpen);
    }

    private async UniTask FadeAsync(float startAlpha, float endAlpha, CancellationToken token)
    {
        float elapsedTime = 0f;
        while (elapsedTime < _fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float progress = elapsedTime / _fadeDuration;
            CanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, progress);
            bool isCanceled = await UniTask.Yield(PlayerLoopTiming.Update, token).SuppressCancellationThrow();
            if (isCanceled) return;
        }

        if (!token.IsCancellationRequested) CanvasGroup.alpha = endAlpha;
    }

    private void ResetCancellationToken()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = new CancellationTokenSource();
    }

    public void DisableRaycasts()
    {
        CanvasGroup.blocksRaycasts = false;
        CanvasGroup.interactable = false;

        if (UnityEngine.EventSystems.EventSystem.current != null)
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
    }

    private void OnDestroy()
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }
}
