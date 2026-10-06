using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _label;

    private float _lastValue = 0;
    private float _value = 0;
    private Material _material = null;
    private Coroutine _fillCoroutine;
    private RectTransform _rectTransform;

    private static readonly int _valueProperty = Shader.PropertyToID("_Value");
    private static readonly int _deltaProperty = Shader.PropertyToID("_Delta");

    public float Value
    {
        get => _value;
        set
        {
            _value = value;
            if (_label != null) _label.text = value.ToString();
            if (_fillCoroutine != null) StopCoroutine(_fillCoroutine);

            _fillCoroutine = StartCoroutine(FillMaterialOverTime(0.5f));
        }
    }

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        InitMaterial();
        UpdateAspect();
    }

    private void OnRectTransformDimensionsChange()
    {
        UpdateAspect();
    }

    private void InitMaterial()
    {
        if (_material != null) return;

        var graphic = GetComponent<Graphic>();
        if (graphic != null)
        {
            _material = new Material(graphic.material);
            graphic.material = _material;
        }
        else
        {
            var objRenderer = GetComponent<Renderer>();
            if (objRenderer != null) _material = objRenderer.material;
        }
    }

    private void UpdateAspect()
    {
        if (_rectTransform == null) return;
        InitMaterial();
    }

    private IEnumerator FillMaterialOverTime(float duration)
    {
        InitMaterial();
        UpdateAspect();

        float elapsedTime = 0f;
        float startValue = _lastValue;
        float targetValue = _value;

        _material.SetFloat(_valueProperty, targetValue);
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float currentProgress = Mathf.Lerp(startValue, targetValue, elapsedTime / duration);
            _material.SetFloat(_deltaProperty, currentProgress);
            yield return null;
        }
        _material.SetFloat(_deltaProperty, targetValue);

        _lastValue = targetValue;
        _fillCoroutine = null;
    }
}