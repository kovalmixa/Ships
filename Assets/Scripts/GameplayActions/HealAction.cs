using Assets.Common;
using Assets.Scripts.Actions;
using UnityEngine;

namespace GameplayActions
{
    [System.Serializable]
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