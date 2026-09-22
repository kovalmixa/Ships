
using UnityEngine.Rendering.Universal;

public class GlobalLightController : SingletonMonoBehaviour<GlobalLightController>
{
    private Light2D _globalLight;

    protected override void Awake()
    {
        base.Awake();
        _globalLight = GetComponent<Light2D>();
    }
}
