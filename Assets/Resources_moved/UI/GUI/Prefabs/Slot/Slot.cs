using System;
using UnityEngine;
using UnityEngine.UIElements;

public class SlotUIConfig
{
    public Action onEnter;
    public Action onLeave;
    public Action onClick;
}


public class SlotUI : MonoBehaviour
{
    VisualElement _visualElement;

    public void Setup(VisualElement ve, SlotUIConfig config)
    {
        _visualElement = ve;
        ve.RegisterCallback<PointerEnterEvent>(evt => config.onEnter?.Invoke());
        ve.RegisterCallback<PointerLeaveEvent>(evt => config.onLeave?.Invoke());
    }
}
