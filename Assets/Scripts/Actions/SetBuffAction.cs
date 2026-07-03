using Actions;
using Assets.Common;
using Assets.Entity;
using UnityEngine;

namespace Assets.Scripts.Actions
{

    public class SetBuffAction : TemplateActionBase, IScalableAction
    {
        public enum TargetType
        {
            All, Player, Friendly, Hostile
        }
        public BuffStatus[] BuffStatuses { get; set; }
        public TargetType targetType = TargetType.All;
        public float Radius { get; set; }

        public override void Execute(EntitySnapshot entitySnapshot, Vector3 targetPos)
        {
            //Collider2D[] targets = Physics2D.OverlapCircleAll(targetPos, Radius, combinedMask);
            //foreach (var target in targets)
            //{
            //    if (target.TryGetComponent(out IInteractive interactive))
            //        // logic to choose what type
            //        interactive.TakeDamage(context, damage);
            //}
        }

        public override void Execute(EntitySnapshot entitySnapshot, IInteractive target)
        {
            //target.TakeDamage(context, damage);
        }

        public void ScaleExecute(EntitySnapshot entitySnapshot, Vector3 targetPos, float scale)
        {
            throw new System.NotImplementedException();
        }

        public void ScaleExecute(EntitySnapshot entitySnapshot, IInteractive target, float scale)
        {
            throw new System.NotImplementedException();
        }
    }
}
