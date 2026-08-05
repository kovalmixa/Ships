using Assets.Common;
using Assets.Entity.Modifiers;
using Assets.Handlers.Enums;
using Assets.Scripts.Actions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayActions
{
    public enum DamageType
    {
        Physical, Fire, Explosive, Acid, Ultrasound, Electricity, Plasma,
        Slow, Freeze, Psi, Radiation, EMP, SpatialAnomaly, Flooding
    }

    [Serializable]
    public struct ElementalDamageData
    {
        public DamageType type;
        public float damage;
        public float critChance;
        public float critMultiplier;
    }

    public class DamageData : ActionData
    {
        public float value;
        public float penetration;
        public float critChance;
        public float critMultiplier;
        public LayerType targetLayers;
        public float range;
        public LayerMask[] filterLayers;
        public List<ElementalDamageData> elements = new();
    }

    public class DamageAction : GameplayAction<DamageData>
    {
        protected override void ExecuteAction(InteractionContext context, DamageData data, Vector2 targetPos)
        {
            int combinedMask = 0;
            if (data.filterLayers != null)
                foreach (var mask in data.filterLayers)
                    combinedMask |= mask.value;

            Collider2D[] targets = Physics2D.OverlapCircleAll(targetPos, data.range, combinedMask);

            foreach (var targetCollider in targets)
                if (targetCollider.TryGetComponent(out IInteractive interactive))
                    if (CanDamageLayer(data.targetLayers, interactive.Layer))
                        interactive.TakeDamage(context, data);
        }

        protected override void ExecuteAction(InteractionContext context, DamageData data, IInteractive target)
        {
            if (CanDamageLayer(data.targetLayers, target.Layer)) target.TakeDamage(context, data);
        }

        private bool CanDamageLayer(LayerType attackLayers, LayerType targetLayer) => (attackLayers & targetLayer) != 0;
    }
}
