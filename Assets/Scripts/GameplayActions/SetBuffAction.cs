using GameplayActions;
using Assets.Common;
using Assets.Scripts.Actions;
using JetBrains.Annotations;
using UnityEngine;
using Assets.Entity.BuffStatuses;
using System.Collections.Generic;
using Assets.Entity.Modifiers;

public class BuffData : ActionData
{
    public BuffStatus[] buffs;
    public float range;
    public LayerMask[] filterLayers;
    [CanBeNull] public VfxData visualData;
}

public class SetBuffAction : GameplayAction<BuffData>
{
    protected override void ExecuteAction(InteractionContext context, BuffData data, Vector2 targetPos)
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

    protected override void ExecuteAction(InteractionContext context, BuffData data, IInteractive target)
    {
        ApplyBuffsToTarget(context, data, target);
    }

    private void ApplyBuffsToTarget(InteractionContext context, BuffData data, IInteractive target)
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