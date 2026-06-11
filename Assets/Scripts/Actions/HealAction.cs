using Assets.Common;
using Assets.Common.ActionEffectStructs;
using UnityEngine;

namespace Actions
{
    public class HealAction : ActionBase, IScalableAction
    {
        [SerializeField] public int HealValue;

        public override void Execute(ActionContext context, Vector3 targetPos)
        {
            if (!CanActivate(context, targetPos)) return;
            Debug.Log($"Healed:{HealValue}");
            //var stats = context.Target.GetComponent<CharacterStats>();
            //if (stats != null)
            //{
            //    stats.Heal(context.HealAmount.Value);
            //}
        }
        public override void Execute(ActionContext context, IInteractive target)
        {
            
            target.TakeHeal(context, new Heal());
        }

        #region IScalableAction
        public void ScaleExecute(ActionContext context, Vector3 targetPos, float scale)
        {
            throw new System.NotImplementedException();
        }

        public void ScaleExecute(ActionContext context, IInteractive target, float scale)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}
