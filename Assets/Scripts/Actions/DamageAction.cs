using Assets.Entity.Interfaces;
using UnityEditor.Timeline.Actions;
using UnityEngine;

namespace Assets.Scripts.Actions
{
        public class DamageAction : ActionBase, IGameAction
    {
            public float Radius;

            public float Damage;

            public override void Execute(GameObject source, Vector3 targetPos)
            {
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
