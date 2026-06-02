using Assets.Common;
using UnityEngine;

namespace Actions
{
    public class HealAction : ActionBase, IScalableAction
    {
        [SerializeField] public int HealValue;

        public override void Execute(GameObject source, Vector3 targetPos)
        {
            if (!CanActivate(source, targetPos)) return;
            Debug.Log($"Healed:{HealValue}");
            //var stats = context.Target.GetComponent<CharacterStats>();
            //if (stats != null)
            //{
            //    stats.Heal(context.HealAmount.Value);
            //}
        }
        public override void Execute(GameObject source, IInteractive target)
        {
            
            target.TakeHeal(HealValue);
        }

        #region IScalableAction
        public void ScaleExecute(GameObject source, Vector3 targetPos, float scale)
        {
            throw new System.NotImplementedException();
        }

        public void ScaleExecute(GameObject source, IInteractive target, float scale)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}
