using Actions;
using Assets.Common;
using Assets.Common.ActionEffectStructs;
using Assets.Scripts.Effects;
using UnityEngine;

namespace Assets.Scripts.Actions
{

    public class SetEffectAction : ActionBase
    {
        public enum TargetType
        {
            All, Player, Friendly, Hostile
        }
        public EffectComponent[] EffectComponents { get; set; }
        public TargetType targetType = TargetType.All;
        public float Radius { get; set; }

        public override void Execute(ActionContext context, Vector3 targetPos)
        {
            //Collider2D[] targets = Physics2D.OverlapCircleAll(targetPos, Radius, combinedMask);
            //foreach (var target in targets)
            //{
            //    if (target.TryGetComponent(out IInteractive interactive))
            //        // logic to choose what type
            //        interactive.TakeDamage(context, damage);
            //}
        }

        public override void Execute(ActionContext context, IInteractive target)
        {
            //target.TakeDamage(context, damage);
        }
    }
}
