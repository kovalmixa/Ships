using Assets.Common;
using Assets.Entity;
using Assets.Scripts.Actions;
using UnityEngine;

namespace Actions
{
    public class HealAction : TemplateActionBase, IScalableAction
    {
        [SerializeField] public int HealValue;

        public override void Execute(EntitySnapshot entitySnapshot, Vector3 targetPos)
        {
            if (!CanActivate(entitySnapshot, targetPos)) return;
            Debug.Log($"Healed:{HealValue}");
            //var stats = context.Target.GetComponent<CharacterStats>();
            //if (stats != null)
            //{
            //    stats.Heal(context.HealAmount.Value);
            //}
        }
        public override void Execute(EntitySnapshot entitySnapshot, IInteractive target)
        {
            
            target.TakeHeal(entitySnapshot, new Heal());
        }

        #region IScalableAction
        public void ScaleExecute(EntitySnapshot entitySnapshot, Vector3 targetPos, float scale)
        {
            throw new System.NotImplementedException();
        }

        public void ScaleExecute(EntitySnapshot entitySnapshot, IInteractive target, float scale)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}
