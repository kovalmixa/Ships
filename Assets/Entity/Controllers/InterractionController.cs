using GameplayActions;
using Assets.Scripts.Actions;

namespace Assets.Entity.Controllers
{
    public class InterractionController
    {
        private TriggerController _triggerController;
        InterractionController(TriggerController triggerController) => _triggerController = triggerController; 

        public void AddBuff(InterractionContext interractionContext, params BuffStatus[] buffs)
        {
            _triggerController.OnTrigger(TriggerType.OnBuffed, interractionContext);
        }

        public void RemoveBuff(InterractionContext interractionContext, params BuffStatus[] buffs)
        {

        }

        public void TakeDamage(InterractionContext interractionContext, Damage damage)
        {
            _triggerController.OnTrigger(TriggerType.OnDamage, interractionContext);
        }

        public void TakeHeal(InterractionContext interractionContext, Heal heal)
        {
            _triggerController.OnTrigger(TriggerType.OnHeal, interractionContext);
        }

        public void OnActivate(InterractionContext interractionContext)
        {
            _triggerController.OnTrigger(TriggerType.OnActivate, interractionContext);
        }
    }
}
