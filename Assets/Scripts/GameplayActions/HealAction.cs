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

        public override void Execute(InteractionContext interractionContext, Vector3 targetPos)
        {
            //var healData = (Heal)interractionContext.ActionStruct; // doesnt work
            //Debug.Log($"Healed: {healData.value}");
            Debug.Log($"Healed");
        }

        public override void Execute(InteractionContext interractionContext, IInteractive target)
        {
            //target.TakeHeal(interractionContext);
        }

        #region IScalableAction
        public void ScaleExecute(InteractionContext interractionContext, Vector3 targetPos, float scale)
        {
            throw new System.NotImplementedException();
        }

        public void ScaleExecute(InteractionContext interractionContext, IInteractive target, float scale)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}
