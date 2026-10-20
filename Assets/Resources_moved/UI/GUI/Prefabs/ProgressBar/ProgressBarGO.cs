using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarGO : ProgressBar
{
    [SerializeField] private TextMeshProUGUI _label;

    protected override string Text 
    {
        get => _label != null ? _label.text : string.Empty;
        set { if (_label != null) _label.text = value; }
    }

    protected override void InitMaterial()
    {
        if (material != null) return;

        if (TryGetComponent<Graphic>(out var graphic))
        {
            material = new Material(graphic.material);
            graphic.material = material;
        }
        else
        {
            var objRenderer = GetComponent<Renderer>();
            if (objRenderer != null) material = objRenderer.material;
        }
    }
}