using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILoadingWindow : UIWindow
{
    [Header("Компоненты прогресса")]
    [SerializeField] private Slider _progressBar;
    [SerializeField] private TextMeshProUGUI _progressText;

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
