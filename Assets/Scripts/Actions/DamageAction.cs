using Assets.Entity.Interfaces;
using UnityEditor.Timeline.Actions;
using UnityEngine;

namespace Assets.Scripts.Actions
{
        public class DamageAction : MonoBehaviour, IGameAction
    {
            public bool IsPassive { get; set; } = true;

            public float Radius;

            public float Damage;

            public void Execute(GameObject source, Vector3 targetPos)
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
