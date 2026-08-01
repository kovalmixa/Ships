using Assets.Handlers.Events;
using Assets.Scripts.Actions;
using GameplayActions;

namespace Assets.Entity.Controllers
{
    public class InteractionController
    {
        private readonly TriggerController _triggerController;

        public InteractionController(TriggerController triggerController)
        {
            _triggerController = triggerController;
        }

        public void TakeDamage(InteractionContext context, DamageDataSO damageData)
        {
            var evt = new EntityInteractionEvent(context, damageData, damageData.value);
            _triggerController.OnTrigger(TriggerType.OnDamage, evt.Context);

            // Пример применения итогового урона после работы триггеров/баффов:
            // healthComponent.ApplyDamage(evt.FinalValue);
        }

        public void TakeHeal(InteractionContext context, HealDataSO healData)
        {
            var evt = new EntityInteractionEvent(context, healData, healData.value);
            _triggerController.OnTrigger(TriggerType.OnHeal, evt.Context);

            // healthComponent.ApplyHeal(evt.FinalValue);
        }

        public void AddBuff(InteractionContext context, BuffDataSO buffData)
        {
            var evt = new EntityInteractionEvent(context, buffData);
            _triggerController.OnTrigger(TriggerType.OnBuffed, evt.Context);

        }

        public void RemoveBuff(InteractionContext context, BuffDataSO buffData)
        {
            var evt = new EntityInteractionEvent(context, buffData);
            _triggerController.OnTrigger(TriggerType.OnBuffRemoved, evt.Context);
        }

        public void OnActivate(InteractionContext context, ActionDataSO actionData = null)
        {
            var evt = new EntityInteractionEvent(context, actionData);
            _triggerController.OnTrigger(TriggerType.OnActivate, evt.Context);
        }
    }
}