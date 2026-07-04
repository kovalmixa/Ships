using Assets.Entity.Controllers;
using Assets.Scripts.Actions;

namespace Assets.Common
{
    public interface IInteractive : IObject
    {
        BuffStatController BuffController { get; }
        void TakeDamage(InterractionContext interractionContext, Damage damage);

        void TakeHeal(InterractionContext interractionContext, Heal heal);

        public void AddBuff(InterractionContext interractionContext, params BuffStatus[] buffs);

        public void RemoveBuff(InterractionContext interractionContext, params BuffStatus[] buffs);
    }
}
