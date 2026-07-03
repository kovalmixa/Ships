using Assets.Entity.BuffStatuses;
using Assets.Entity.Modifiers;
using System;
using UnityEngine;

public abstract class BuffStatus : MonoBehaviour
{
    [SerializeField] public Modifiers modifiers;
    [SerializeField] public float duration;
    [SerializeField] public Action? onApply;
    [SerializeField] public Action? onRemove;

    private float _lifetime = 0f;

    public virtual bool Tick(StatusContext context)
    {
        _lifetime += Time.deltaTime;
        return _lifetime >= duration;
    }
}