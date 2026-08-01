using Assets.Common;
using Assets.Scripts.Actions;
using UnityEngine;

namespace GameplayActions
{
    [CreateAssetMenu(menuName = "Gameplay/Actions/Heal Data")]
    public class HealDataSO : ActionDataSO
    {
        public float value;
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