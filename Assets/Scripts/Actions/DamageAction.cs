using Assets.Common;
using Assets.Scripts.Actions;
using UnityEngine;

namespace Actions
{
    public enum DamageType
    {
        None, Explosive, Corrosion, Energy, Fire, Radiation
    }

    public class Damage
    {
        [SerializeField] public DamageType type;

        [SerializeField] public float penetration;

        [SerializeField] public float value;
    }

    public class DamageAction : TemplateActionBase
    {
        [SerializeField] public float range;

        [SerializeField] public Damage damage;

        [SerializeField] public LayerMask[] filterLayers;

        public override void Execute(InterractionContext interractionContext, Vector3 targetPos)
        {
            int combinedMask = 0;
            foreach (var mask in filterLayers) combinedMask |= mask.value;
            Collider2D[] targets = Physics2D.OverlapCircleAll(targetPos, range, combinedMask);
            foreach (var target in targets)
                if (target.TryGetComponent(out IInteractive interactive))
                    interactive.TakeDamage(interractionContext, damage);

            //todo add extra damage options with types
        }

        public override void Execute(InterractionContext interractionContext, IInteractive target)
        {
            target.TakeDamage(interractionContext, damage);
        }
    }
}
