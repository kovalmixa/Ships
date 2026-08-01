using GameplayActions;
using Assets.Common;
using Assets.Scripts.Actions;
using JetBrains.Annotations;
using UnityEngine;
using Assets.Entity.BuffStatuses;

public class BuffDataSO : ActionDataSO
{
    public BuffStatus[] buffs;
    public float range;
    public LayerMask[] filterLayers;
    [CanBeNull] public EffectDataSO visualData;
}

public class SetBuffAction : GameplayAction<BuffDataSO>
{
    protected override void ExecuteAction(InteractionContext context, BuffDataSO data, Vector2 targetPos)
    {
        if (data.visualData != null) ActionProvider.Effect.Execute(context, data.visualData, targetPos);

        int combinedMask = 0;
        if (data.filterLayers != null)
            foreach (var layer in data.filterLayers) combinedMask |= layer.value;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(targetPos, data.range, combinedMask);
        foreach (var col in colliders)
            if (col.TryGetComponent(out IInteractive target))
                ApplyBuffsToTarget(context, data, target);
    }

    protected override void ExecuteAction(InteractionContext context, BuffDataSO data, IInteractive target)
    {
        ApplyBuffsToTarget(context, data, target);
    }

    private void ApplyBuffsToTarget(InteractionContext context, BuffDataSO data, IInteractive target)
    {
        if (data.buffs == null) return;

        foreach (var buffTemplate in data.buffs)
        {
            var instance = new BuffStatus
            {
                Id = buffTemplate.Id,
                SourceId = context?.AbilityId ?? context?.SourceSnapshot?.Id,
                Duration = buffTemplate.Duration,
                Scope = buffTemplate.Scope,
                Policy = buffTemplate.Policy,
                modifiers = buffTemplate.modifiers
            };
            target.AddBuff(context, instance);
        }
    }
}