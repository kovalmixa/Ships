using Assets.Entity.BuffStatuses;
using UnityEngine;

public abstract class TickStatus : BuffStatus
{
    public float TickInterval { get; set; }
    private float _timer = 0f;

    public override bool Tick(StatusContext context)
    {
        if (base.Tick(context)) return true;
        _timer += Time.deltaTime;
        if (_timer >= TickInterval)
        {
            OnTick(context);
            _timer = 0f;
        }

        return false;
    }

    protected abstract void OnTick(StatusContext context);
}