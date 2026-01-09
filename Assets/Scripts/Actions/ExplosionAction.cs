using JetBrains.Annotations;
using UnityEngine;

namespace Actions
{
    public class ExplosionAction : ActionBase
    {
        [SerializeField] [CanBeNull] private EffectAction effectAction;

        [SerializeField] [CanBeNull] private DamageAction damageAction;
        public override void Execute(GameObject source, Vector3 targetPos)
        {
            damageAction?.Execute(source, targetPos);
            effectAction?.Execute(source, targetPos);
        }
    }
}
