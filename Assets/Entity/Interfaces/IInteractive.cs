using Actions;
using Assets.Scripts.Actions;

namespace Assets.Common
{
    public interface IInteractive : IObject
    {
        public void AddBuff(InterractionContext interractionContext, params BuffStatus[] buffs);

        public void RemoveBuff(InterractionContext interractionContext, params BuffStatus[] buffs);

        public void TakeDamage(InterractionContext interractionContext, Damage damage);

        public void TakeHeal(InterractionContext interractionContext, Heal heal);
    }
}
