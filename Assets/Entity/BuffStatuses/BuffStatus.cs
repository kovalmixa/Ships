using Assets.Entity.BuffStatuses;

public abstract class BuffStatus
{
    public float Duration;
    public float RemainingTime;

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