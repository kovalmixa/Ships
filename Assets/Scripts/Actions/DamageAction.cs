using Assets.Common;
using Assets.Scripts.Actions;
using UnityEngine;

namespace Actions
{
    public class DamageAction : TemplateActionBase
    {
        [SerializeField] public float Radius;

        [SerializeField] public Damage Damage;

        [SerializeField] public LayerMask[] FilterLayers;

        public override void Execute(InterractionContext interractionContext, Vector3 targetPos)
        {
            if (!CanActivate(interractionContext, targetPos)) return;
            int combinedMask = 0;
            foreach (var mask in FilterLayers) combinedMask |= mask.value;
            Collider2D[] targets = Physics2D.OverlapCircleAll(targetPos, Radius, combinedMask);
            foreach (var target in targets)
                if (target.TryGetComponent(out IInteractive interactive))
                    interactive.TakeDamage(interractionContext, Damage);

            //todo add extra damage options with types
        }

        public override void Execute(InterractionContext interractionContext, IInteractive target)
        {
            target.TakeDamage(interractionContext, Damage);
        }
    }
}
