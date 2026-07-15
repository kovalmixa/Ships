using GameplayActions;
using Assets.Scripts.Actions;

namespace Assets.Entity.Controllers
{
    public class InterractionController
    {
        private TriggerController _triggerController;
        InterractionController(TriggerController triggerController) => _triggerController = triggerController; 

        public void AddBuff(InteractionContext interractionContext, params BuffStatus[] buffs)
        {
            _triggerController.OnTrigger(TriggerType.OnBuffed, interractionContext);
        }

        public void RemoveBuff(InteractionContext interractionContext, params BuffStatus[] buffs)
        {

        }

        public void TakeDamage(InteractionContext interractionContext, Damage damage)
        {
            _triggerController.OnTrigger(TriggerType.OnDamage, interractionContext);
        }

        public void TakeHeal(InteractionContext interractionContext, Heal heal)
        {
            _triggerController.OnTrigger(TriggerType.OnHeal, interractionContext);
        }

        public void OnActivate(InteractionContext interractionContext)
        {
            _triggerController.OnTrigger(TriggerType.OnActivate, interractionContext);
        }
    }
}
