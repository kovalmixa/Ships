using Assets.Common;
using Assets.Entity.BuffStatuses;
using Assets.Handlers.Events;
using Assets.Scripts.Actions;
using GameplayActions;
using UnityEngine;

public class LifestealBuff : BuffStatus
{
    [SerializeField] private float lifestealPercentage = 0.15f;

    public override void OnApply(IInteractive owner, InteractionContext context)
    {
        base.OnApply(owner, context);
        EventBrocker.Subscribe<EntityInteractionEvent>(OnDamageDealt);
    }

    public override void OnRemove()
    {
        EventBrocker.Unsubscribe<EntityInteractionEvent>(OnDamageDealt);
        base.OnRemove();
    }

    private void OnDamageDealt(EntityInteractionEvent data)
    {
        // Если урон нанес не владелец этого баффа — игнорируем событие
        if (data.Source != Owner) return;

        float healAmount = (float)data.FinalValue * lifestealPercentage;

        // Вызываем лечение на владельце баффа
        // Примечание: Убедись, что твоя система лечения не генерирует DamageDealtEvent, 
        // иначе получится бесконечная рекурсия. Либо создай отдельное событие HealEvent.
        var context = data.Context;
        context.SetTarget(data.Source.GameObject);
        HealDataSO healData = new()
        {
            value = healAmount
        };
        ActionProvider.Heal.Execute(context, healData, Owner);
    }
}