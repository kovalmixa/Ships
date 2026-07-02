using Assets.Entity.Modifiers;
using System;
using UnityEngine;

public abstract class BuffStatus : MonoBehaviour
{
    [SerializeField] public Modifiers modifiers;
    [SerializeField] public float duration;
    [SerializeField] public float remainingTime;
    [SerializeField] public Action? onApply;
    [SerializeField] public Action? onRemove;
}