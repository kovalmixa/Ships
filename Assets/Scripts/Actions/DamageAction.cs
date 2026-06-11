using Assets.Common;
using Assets.Common.ActionEffectStructs;
using UnityEngine;

namespace Actions
{
    public class DamageAction : ActionBase
    {
        [SerializeField] public float Radius;

        [SerializeField] public Damage Damage;

        [SerializeField] public LayerMask[] FilterLayers;
        public override void Execute(ActionContext context, Vector3 targetPos)
        {
            if (!CanActivate(context, targetPos)) return;
            int combinedMask = 0;
            foreach (var mask in FilterLayers) combinedMask |= mask.value;
            Collider2D[] targets = Physics2D.OverlapCircleAll(targetPos, Radius, combinedMask);
            foreach (var target in targets)
                if (target.TryGetComponent(out IInteractive interactive))
                    interactive.TakeDamage(context, Damage);

            //todo add extra damage options with types
        }

        public override void Execute(ActionContext context, IInteractive target)
        {
            target.TakeDamage(context, Damage);
        }
    }
}
