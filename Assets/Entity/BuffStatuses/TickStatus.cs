using Assets.Entity.BuffStatuses;
using Assets.Scripts.Actions;
using UnityEngine;

public abstract class TickStatus : BuffStatus
{
    public float TickInterval { get; set; }
    private float _timer = 0f;

    public override bool Tick(float deltaTime)
    {
        if (base.Tick(deltaTime)) return true;
        _timer += Time.deltaTime;
        if (_timer >= TickInterval)
        {
            OnTick();
            _timer = 0f;
        }
        return false;
    }

    protected abstract void OnTick();
}