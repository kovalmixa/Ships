using System.Collections;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    private float _lastValue = 0;
    private float _value = 0;
    private Material _material = null;
    public float Value
    {
        get => _value;
        set
        {
            _value = value;
            StartCoroutine(FillMaterialOverTime(3.0f));
        }
    }

    private IEnumerator FillMaterialOverTime(float duration)
    {
        if (_material == null)
        {
            var objRenderer = GetComponent<Renderer>();
            if (objRenderer != null) _material = objRenderer.material;
        }

        float elapsedTime = 0f;
        float deltaValue = _value - _lastValue;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float progress = deltaValue * elapsedTime / duration;
            _material.SetFloat("Value", _lastValue + progress);

            yield return null;
        }
        _lastValue = _value;
        Debug.Log("Действие завершено!");
    }

}
