using Assets.Common;
using Assets.Handlers;
using Assets.Handlers.Enums;
using Assets.Scripts.Actions;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayActions
{
    public enum DamageType
    {
        Physical, Fire, Explosive, Acid, Ultrasound, Electricity, Plasma,
        Slow, Freeze, Psi, Radiation, EMP, SpatialAnomaly, Flooding
    }

    [System.Serializable]
    public struct ElementalDamageData
    {
        public DamageType type;
        public float damage;
        public float critChance;
        public float critMultiplier;
    }

    [System.Serializable]
    public class DamageData : ActionData
    {
        public float value;
        public float range;
        public float splashModifier;
        public float penetration;
        public float critChance;
        public float critMultiplier;
        public LayerType targetLayer;
        public List<ElementalDamageData> elements = new();
        public AnimationCurve splashCurve;

        public DamageData GetScaledDamage(float multiplier)
        {
            DamageData clone = (DamageData)MemberwiseClone();
            clone.value = Mathf.RoundToInt(value * multiplier);
            return clone;
        }
    }

    public class DamageAction : GameplayAction<DamageData>
    {
        protected override void ExecuteAction(InteractionContext context, DamageData data, Vector2 targetPos)
        {
            Collider2D[] targets = Physics2D.OverlapCircleAll(targetPos, data.range, LayersHandler.GetPhysicsLayerMask(data.targetLayer));
            foreach (var targetCollider in targets)
            {
                if (targetCollider.TryGetComponent(out IInteractive interactive))
                {
                    if (!CanDamageLayer(data.targetLayer, interactive.Layer)) continue;
                    DamageData currentData = data;
                    if (data.splashModifier != 1 && data.range > 0)
                    {
                        Vector2 closestPoint = targetCollider.ClosestPoint(targetPos);
                        float distance = Vector2.Distance(targetPos, closestPoint);
                        float normalizedDistance = Mathf.Clamp01(distance / data.range);

                        float multiplier = data.splashCurve.Evaluate(normalizedDistance);
                        multiplier = Mathf.Max(multiplier, data.splashModifier);
                        currentData = data.GetScaledDamage(multiplier);
                    }
                    interactive.TakeDamage(context, currentData);
                }
            }
        }

        protected override void ExecuteAction(InteractionContext context, DamageData data, IInteractive target)
        {
            if (CanDamageLayer(data.targetLayer, target.Layer)) target.TakeDamage(context, data);
        }

        private bool CanDamageLayer(LayerType attackLayers, LayerType targetLayer) => (attackLayers & targetLayer) != 0;
    }
}
