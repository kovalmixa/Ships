using Assets.Common;
using Assets.Scripts.Actions;
using UnityEngine;

namespace GameplayActions
{
    public enum DamageType
    {
        None, Explosive, Corrosion, Energy, Fire, Radiation
    }

    public struct Damage : IActionStruct
    {
        [SerializeField] public DamageType type;

        [SerializeField] public float penetration;

        [SerializeField] public float value;

        [SerializeField] public float range;

        [SerializeField] public LayerMask[] filterLayers;

        public Damage(DamageType type, float penetration, float value, LayerMask[] filterLayers, float range = 1f)
        {
            this.type = type;
            this.penetration = penetration;
            this.value = value;
            this.range = range;
            this.filterLayers = filterLayers;
        }
    }

    public class DamageAction : GameplayAction
    {
        public override void Execute(InteractionContext interractionContext, Vector3 targetPos)
        {
            var damage = (Damage) interractionContext.ActionStruct;
            int combinedMask = 0;
            foreach (var mask in damage.filterLayers) combinedMask |= mask.value;
            Collider2D[] targets = Physics2D.OverlapCircleAll(targetPos, damage.range, combinedMask);
            foreach (var target in targets)
                if (target.TryGetComponent(out IInteractive interactive))
                    interactive.TakeDamage(interractionContext);

            //todo add extra damage options with types
        }

        public override void Execute(InteractionContext interractionContext, IInteractive target)
        {
            var damage = (Damage)interractionContext.ActionStruct;
            target.TakeDamage(interractionContext);
        }
    }
}
