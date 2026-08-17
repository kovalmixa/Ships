
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GlobalLightHandler : SingletonMonoBehaviour<GlobalLightHandler>
{
    private Light2D _globalLight;

    protected override void Awake()
    {
        base.Awake();
        _globalLight = GetComponent<Light2D>();
    }
}
