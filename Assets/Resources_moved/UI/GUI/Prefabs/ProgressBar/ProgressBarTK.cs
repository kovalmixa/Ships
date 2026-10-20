using UnityEngine;
using UnityEngine.UIElements;

public class ProgressBarTK : ProgressBar
{
    [SerializeField] private UIDocument _uiDocument;
    [SerializeField] private string _elementName = "ProgressBarElement";
    [SerializeField] private Material _customMaterial;

    private VisualElement _progressElement;
    private Label _labelElement;

    protected override string Text
    {
        get => _labelElement != null ? _labelElement.text : string.Empty;
        set { if (_labelElement != null) _labelElement.text = value; }
    }

    protected override void Awake()
    {
        base.Awake();
        InitToolkitElements();
    }

    private void InitToolkitElements()
    {
        if (_uiDocument == null) _uiDocument = GetComponent<UIDocument>();
        if (_uiDocument != null)
        {
            var root = _uiDocument.rootVisualElement;
            if (root != null)
            {
                _progressElement = root.Q<VisualElement>(_elementName);
                _labelElement = root.Q<Label>();

                if (_progressElement != null && _customMaterial != null)
                {
                    if (_customMaterial.mainTexture != null)
                    {
                        Texture2D tex = _customMaterial.mainTexture as Texture2D;
                        if (tex != null) _progressElement.style.backgroundImage = new StyleBackground(tex);
                    }
                }
            }
        }
    }

    protected override void InitMaterial()
    {
        if (material != null) return;

        if (_customMaterial != null) material = new Material(_customMaterial);
        else material = new Material(Shader.Find("Standard"));
        if (_progressElement == null) InitToolkitElements();
    }
}