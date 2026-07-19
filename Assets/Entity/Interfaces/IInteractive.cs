using Assets.Scripts.Actions;
using UnityEngine;

namespace Assets.Common
{
    public interface IInteractive : IObject
    {
        public GameObject GameObject { get; }
        public void AddBuff(InteractionContext interractionContext);

        public void TakeDamage(InteractionContext interractionContext);

        public void TakeHeal(InteractionContext interractionContext);
    }
}
