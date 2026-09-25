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
    private static readonly int _aspectProperty = Shader.PropertyToID("_Aspect");

    public float Value
    {
        get => _value;
        set
        {
            _value = value;
            if (_label != null) _label.text = value.ToString();
            if (_fillCoroutine != null) StopCoroutine(_fillCoroutine);

            _fillCoroutine = StartCoroutine(FillMaterialOverTime(3.0f));
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

        if (_material != null && _rectTransform.rect.height > 0)
        {
            float aspect = _rectTransform.rect.width / _rectTransform.rect.height;
            _material.SetFloat(_aspectProperty, aspect);
        }
    }

    private IEnumerator FillMaterialOverTime(float duration)
    {
        InitMaterial();
        UpdateAspect();

        float elapsedTime = 0f;
        float startValue = _lastValue;
        float targetValue = _value;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float currentProgress = Mathf.Lerp(startValue, targetValue, elapsedTime / duration);
            if (_material != null) _material.SetFloat(_valueProperty, currentProgress);
            yield return null;
        }

        if (_material != null) _material.SetFloat(_valueProperty, targetValue);

        _lastValue = targetValue;
        _fillCoroutine = null;
        Debug.Log("Действие завершено!");
    }
}