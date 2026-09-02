using System;
using System.Threading;
using Assets.Handlers.SceneHandlers;
using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIWindow : MonoBehaviour
{
    [SerializeField] private float _fadeDuration = 0.25f;

    private CanvasGroup _canvasGroup;
    private CancellationTokenSource _cts;

    protected virtual void OnOpened()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    protected virtual void OnClosed() { }

    public string WindowId => GameObjectHandler.GenerateUniqueId(name);
    public bool IsOpen => CanvasGroup != null && CanvasGroup.alpha > 0;

    private CanvasGroup CanvasGroup
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

        gameObject.SetActive(true);

        if (delayMs > 0)
            await UniTask.Delay(delayMs, cancellationToken: token);

        CanvasGroup.interactable = true;
        CanvasGroup.blocksRaycasts = true;

        await FadeAsync(0f, 1f, token);

        OnOpened();
        onComplete?.Invoke();
    }

    public async UniTask CloseAsync(Action onComplete = null)
    {
        ResetCancellationToken();
        var token = _cts.Token;

        CanvasGroup.interactable = false;
        CanvasGroup.blocksRaycasts = false;

        await FadeAsync(1f, 0f, token);

        OnClosed();
        gameObject.SetActive(false);
        onComplete?.Invoke();
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
            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }
        CanvasGroup.alpha = endAlpha;
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