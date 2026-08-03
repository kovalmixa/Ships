using Assets.Entity.BuffStatuses;
using Assets.Handlers.Enums;
using Assets.Scripts.Actions;
using GameplayActions;
using UnityEngine;

namespace Assets.Common
{
    public interface IInteractive : IObject
    {
        LayerType Layer { get; }
        public GameObject GameObject { get; }
        public void AddBuff(InteractionContext context, BuffStatus buff);

        public void TakeDamage(InteractionContext context, DamageData data);

        public void TakeHeal(InteractionContext context, HealData data);
    }
}
