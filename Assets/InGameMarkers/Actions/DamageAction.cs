using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Entity;
using Assets.Entity.Interfaces;
using UnityEngine;

namespace Assets.InGameMarkers.Actions
{
        public class DamageAction : IGameAction
        {
            public bool IsPassive { get; set; } = true;

            public void Execute(ActionContext context)
            {
                if (context.Value.HasValue) return;
                float radius = 2.5f;
                float damage = context.Value.Value;

                Collider2D[] targets = Physics2D.OverlapCircleAll(context.Position, radius);
                foreach (var target in targets)
                {
                    if (target.TryGetComponent(out IDamageable damageable))
                    {
                        damageable.TakeDamage(damage);
                    }
                }
            }
        }
}
