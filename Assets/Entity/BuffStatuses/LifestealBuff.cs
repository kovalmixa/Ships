using GameplayActions;
using Assets.Common;
using UnityEngine;

public class LifestealBuff : BuffStatus
{
    [SerializeField] private float lifestealPercentage = 0.15f;

    public override void OnApply(IInteractive owner)
    {
        base.OnApply(owner);
        EventBrocker.Subscribe<EntityInterractionEvent>(OnDamageDealt);
    }

    public override void OnRemove()
    {
        EventBrocker.Unsubscribe<EntityInterractionEvent>(OnDamageDealt);
        base.OnRemove();
    }

    private void OnDamageDealt(EntityInterractionEvent data)
    {
        // Если урон нанес не владелец этого баффа — игнорируем событие
        if (data.Source != Owner) return;

        float healAmount = data.FinalValue * lifestealPercentage;

        // Вызываем лечение на владельце баффа
        // Примечание: Убедись, что твоя система лечения не генерирует DamageDealtEvent, 
        // иначе получится бесконечная рекурсия. Либо создай отдельное событие HealEvent.
        Owner.TakeHeal(null, new Heal(healAmount));
    }
}