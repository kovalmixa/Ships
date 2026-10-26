using UnityEngine;
using UnityEngine.UIElements;

public class ProgressBarTK : ProgressBar
{
    private VisualElement _progressElement;
    private Label _labelElement;
    private VisualElement _rootContainer;
    private Material _customMaterial;
    private Texture2D _mainTex;

    protected override string Text
    {
        get => _labelElement != null ? _labelElement.text : string.Empty;
        set { if (_labelElement != null) _labelElement.text = value; }
    }

    public void Initialize(VisualElement container, Material customMaterial)
    {
        _rootContainer = container;
        _customMaterial = customMaterial;

        _progressElement = _rootContainer.Q<VisualElement>("ProgressBarElement");
        _labelElement = _rootContainer.Q<Label>();

        InitMaterial();

        if (_progressElement != null && material != null)
        {
            if (material.HasProperty("_MainTex") && material.GetTexture("_MainTex") != null)
            {
                _mainTex = material.GetTexture("_MainTex") as Texture2D;
                _progressElement.style.backgroundImage = new StyleBackground(_mainTex);
            }
            _progressElement.style.unityBackgroundImageTintColor = Color.white;
        }
    }

    protected override void InitMaterial()
    {
        if (material != null) return;

        if (_customMaterial != null) material = new Material(_customMaterial);
        else Debug.LogWarning("[ProgressBarTK] Не назначен материал шейдера!");
    }
}