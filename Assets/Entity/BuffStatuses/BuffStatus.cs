using Assets.Entity.BuffStatuses;
using Assets.Entity.Modifiers;
using UnityEngine;

public abstract class BuffStatus : MonoBehaviour
{
    [SerializeField] public Modifiers Modifiers;
    [SerializeField] public float Duration;
    [SerializeField] public float RemainingTime;

    public virtual void OnApply(StatusContext context)
    {
    }

    public virtual void OnRemove(StatusContext context)
    {
    }

    public virtual void Update(StatusContext context, float dt)
    {
        RemainingTime -= dt;
    }
}