using Assets.Common;
using Assets.Scripts.Actions;
using UnityEngine;

namespace GameplayActions
{
    public struct Heal : IActionStruct
    {
        [SerializeField] public float value;

        public Heal(float value)
        {
            this.value = value;
        }
    }

    public class HealAction : GameplayAction, IScalableAction
    {
        [SerializeField] public Heal heal;

        public override void Execute(InterractionContext interractionContext, Vector3 targetPos)
        {
            Debug.Log($"Healed:{heal.value}");
            interractionContext.Type = InterractionType.Heal;
            //var stats = context.Target.GetComponent<CharacterStats>();
            //if (stats != null)
            //{
            //    stats.Heal(context.HealAmount.Value);
            //}
        }
        public override void Execute(InterractionContext interractionContext, IInteractive target)
        {
            interractionContext.Type = InterractionType.Heal;
            target.TakeHeal(interractionContext, heal);
        }

        #region IScalableAction
        public void ScaleExecute(InterractionContext interractionContext, Vector3 targetPos, float scale)
        {
            throw new System.NotImplementedException();
        }

        public void ScaleExecute(InterractionContext interractionContext, IInteractive target, float scale)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}
