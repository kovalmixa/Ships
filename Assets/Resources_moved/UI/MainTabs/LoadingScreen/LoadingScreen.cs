using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class LoadingScreen : UIWindow
{
    private ProgressBar _progressBar;

    protected override void Awake()
    {
        base.Awake();
        //if (RootVisualElement != null)
        //    _progressBar = RootVisualElement.Q<ProgressBar>("scene-progress-bar");
    }

    protected override void OnOpened()
    {
        if (GUIHandler.Instance != null) GUIHandler.Instance.SetInputBlocked(true);
    }

    protected override void OnClosed()
    {
        gameObject.SetActive(false);
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
        if (GUIHandler.Instance != null) GUIHandler.Instance.SetInputBlocked(false);
    }

    private void OnDestroy() => OnClosed();

    public void UpdateProgress(float progress)
    {
        //if (_progressBar != null)
        //{
        //    float percentValue = progress * 100f;
        //    _progressBar.value = percentValue;

        //    _progressBar.title = $"{percentText}%";
        //}
    }
}
