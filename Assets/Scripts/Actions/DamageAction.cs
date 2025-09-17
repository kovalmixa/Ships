using Assets.Entity.Interfaces;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.Scripts.Actions
{
    public class DamageAction : ActionBase
    {
        public float Radius;

        public float Damage;

        public override void Execute(GameObject source, Vector3 targetPos)
        {
            if (!CanActivate(source, targetPos)) return;
            Collider2D[] targets = Physics2D.OverlapCircleAll(targetPos, Radius);
            foreach (var target in targets)
            {
                if (target.TryGetComponent(out IDamageable damageable))
                {
                    damageable.TakeDamage(Damage);
                }
            }
        }
    }
}
