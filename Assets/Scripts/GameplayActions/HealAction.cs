using Assets.Common;
using Assets.Entity.Modifiers;
using Assets.Scripts.Actions;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayActions
{
    [CreateAssetMenu(menuName = "Gameplay/Actions/Heal Data")]
    public class HealDataSO : ActionDataSO
    {
        public float value;

        public override void PopulateStatDict(Dictionary<StatType, float> targetDict)
        {
            throw new System.NotImplementedException();
        }

        public override Dictionary<StatType, float> ToStatTypeDict()
        {
            throw new System.NotImplementedException();
        }
    }

    public class HealAction : GameplayAction<HealDataSO>
    {
        protected override void ExecuteAction(InteractionContext context, HealDataSO data, Vector2 targetPos)
        {
            Debug.Log($"Healed for {data.value} at position {targetPos}");
        }

        protected override void ExecuteAction(InteractionContext context, HealDataSO data, IInteractive target)
        {
            // target.TakeHeal(context, data.value);
        }
    }
}