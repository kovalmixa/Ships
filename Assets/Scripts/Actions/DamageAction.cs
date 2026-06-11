using Assets.Common;
using Assets.Common.ActionEffectStructs;
using UnityEngine;

namespace Actions
{
    public class DamageAction : ActionBase
    {
        [SerializeField] public float radius;

        [SerializeField] private Damage damage;

        [SerializeField] private LayerMask[] filterLayers;
        public override void Execute(ActionContext context, Vector3 targetPos)
        {
            if (!CanActivate(context, targetPos)) return;
            int combinedMask = 0;
            foreach (var mask in filterLayers) combinedMask |= mask.value;
            Collider2D[] targets = Physics2D.OverlapCircleAll(targetPos, radius, combinedMask);
            foreach (var target in targets)
                if (target.TryGetComponent(out IInteractive interactive))
                    interactive.TakeDamage(context, damage);

            //todo add extra damage options with types
        }

        public override void Execute(ActionContext context, IInteractive target)
        {
            target.TakeDamage(context, damage);
        }
    }
}
