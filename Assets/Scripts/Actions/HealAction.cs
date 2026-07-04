using Assets.Common;
using Assets.Scripts.Actions;
using UnityEngine;

namespace Actions
{
    public class HealAction : TemplateActionBase, IScalableAction
    {
        [SerializeField] public int HealValue;

        public override void Execute(InterractionContext interractionContext, Vector3 targetPos)
        {
            if (!CanActivate(interractionContext, targetPos)) return;
            Debug.Log($"Healed:{HealValue}");
            //var stats = context.Target.GetComponent<CharacterStats>();
            //if (stats != null)
            //{
            //    stats.Heal(context.HealAmount.Value);
            //}
        }
        public override void Execute(InterractionContext interractionContext, IInteractive target)
        {
            
            target.TakeHeal(interractionContext, new Heal());
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
