using System.Collections;
using UnityEngine;

public abstract class ProgressBar : MonoBehaviour
{
    protected float lastValue = 0;
    protected float value = 0;
    protected Coroutine fillCoroutine;
    protected Material material = null;

    // Имя свойств в Shader Graph (Unity автоматически добавляет подчеркивание к именам с большой буквы)
    protected static readonly int valueProperty = Shader.PropertyToID("_Value");
    protected static readonly int deltaProperty = Shader.PropertyToID("_Delta");

    public virtual float Value
    {
        get => value;
        set
        {
            this.value = Mathf.Clamp01(value); // Прогресс-бары обычно от 0 до 1
            Text = (this.value * 100f).ToString("F0") + "%"; // Пример вывода текста

            if (fillCoroutine != null) StopCoroutine(fillCoroutine);
            fillCoroutine = StartCoroutine(FillMaterialOverTime(0.5f));
        }
    }

    protected virtual string Text { get; set; }

    protected virtual void Awake() => InitMaterial();

    protected abstract void InitMaterial();

    protected IEnumerator FillMaterialOverTime(float duration)
    {
        InitMaterial();
        if (material == null) yield break;

        float elapsedTime = 0f;
        float startValue = lastValue;
        float targetValue = value;

        material.SetFloat(valueProperty, targetValue);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float currentProgress = Mathf.Lerp(startValue, targetValue, elapsedTime / duration);

            // Передаем параметр Delta в ваш Shader Graph
            material.SetFloat(deltaProperty, currentProgress);

            yield return null;
        }

        material.SetFloat(deltaProperty, targetValue);
        lastValue = targetValue;
        fillCoroutine = null;
    }
}