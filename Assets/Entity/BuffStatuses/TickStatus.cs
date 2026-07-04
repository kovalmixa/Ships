using Assets.Scripts.Actions;
using UnityEngine;

public abstract class TickStatus : BuffStatus
{
    public float TickInterval { get; set; }
    private float _timer = 0f;

    public override bool Tick(InterractionContext interractionContext)
    {
        if (base.Tick(interractionContext)) return true;
        _timer += Time.deltaTime;
        if (_timer >= TickInterval)
        {
            OnTick(interractionContext);
            _timer = 0f;
        }
        return false;
    }

    protected abstract void OnTick(InterractionContext interractionContext);
}