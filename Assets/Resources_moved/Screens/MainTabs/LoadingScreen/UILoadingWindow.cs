using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class UILoadingWindow : UIWindow
{
    [Header("Компоненты прогресса")]
    [SerializeField] private Slider _progressBar;
    [SerializeField] private TextMeshProUGUI _progressText;


    protected override void OnOpened()
    {
        if (GUIHandler.Instance != null) GUIHandler.Instance.SetInputBlocked(true);
    }

    protected override void OnClosed()
    {
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
    }

    public void UpdateProgress(float progress)
    {
        if (_progressBar != null) _progressBar.value = progress;

        if (_progressText != null)
        {
            int percent = Mathf.RoundToInt(progress * 100);
            _progressText.text = $"{percent}%";
        }
    }
}