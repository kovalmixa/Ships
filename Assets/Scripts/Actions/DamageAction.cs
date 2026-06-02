using System.Linq;
using Assets.Common;
using UnityEngine;

namespace Actions
{
    public class DamageAction : ActionBase
    {
        [SerializeField] public float radius;

        [SerializeField] public float damage;

        [SerializeField] public LayerMask[] filterLayers;
        public override void Execute(GameObject source, Vector3 targetPos)
        {
            if (!CanActivate(source, targetPos)) return;
            int combinedMask = 0;
            foreach (var mask in filterLayers) combinedMask |= mask.value;
            Collider2D[] targets = Physics2D.OverlapCircleAll(targetPos, radius, combinedMask);
            foreach (var target in targets)
                if (target.TryGetComponent(out IInteractive interactive))
                    interactive.TakeDamage(damage);
        }

        public override void Execute(GameObject source, IInteractive target)
        {
            target.TakeDamage(damage);
        }
    }
}
