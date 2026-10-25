using System.Collections;
using UnityEngine;

public abstract class ProgressBar : MonoBehaviour
{
    protected float lastValue = 0;
    protected float value = 0;
    protected Coroutine fillCoroutine;
    protected Material material = null;

    protected static readonly int valueProperty = Shader.PropertyToID("_Value");
    protected static readonly int deltaProperty = Shader.PropertyToID("_Delta");

    public virtual float Value
    {
        get => value;
        set
        {
            this.value = value;
            Text = value.ToString();
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

        float elapsedTime = 0f;
        float startValue = lastValue;
        float targetValue = value;

        material.SetFloat(valueProperty, targetValue);
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float currentProgress = Mathf.Lerp(startValue, targetValue, elapsedTime / duration);
            material.SetFloat(deltaProperty, currentProgress);
            yield return null;
        }
        material.SetFloat(deltaProperty, targetValue);

        lastValue = targetValue;
        fillCoroutine = null;
    }
}