using Assets.Common;
using Assets.Entity.Modifiers;
using Assets.Scripts.Actions;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayActions
{
    public class HealData : ActionData
    {
        public float value;
    }

    public class HealAction : GameplayAction<HealData>
    {
        protected override void ExecuteAction(InteractionContext context, HealData data, Vector2 targetPos)
        {
            Debug.Log($"Healed for {data.value} at position {targetPos}");
        }

        protected override void ExecuteAction(InteractionContext context, HealData data, IInteractive target)
        {
            // target.TakeHeal(context, data.value);
        }
    }
}