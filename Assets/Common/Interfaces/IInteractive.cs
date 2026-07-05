using Assets.Entity.Controllers;
using Assets.Entity.Modifiers;
using Assets.Scripts.Actions;
using System.Collections.Generic;

namespace Assets.Common
{
    public interface IInteractive : IObject
    {
        public BuffStatController BuffController { get; }
        public Dictionary<StatType, float> LifetimeStats { get; }

        public void ResetLifetimeStats();

        public float GetLifetimeStat(StatType type);

        public void TakeDamage(InterractionContext interractionContext, Damage damage);

        public void TakeHeal(InterractionContext interractionContext, Heal heal);

        public void AddBuff(InterractionContext interractionContext, params BuffStatus[] buffs);

        public void RemoveBuff(InterractionContext interractionContext, params BuffStatus[] buffs);
    }
}
