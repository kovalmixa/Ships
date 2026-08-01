using Assets.Entity;
using Assets.Entity.Modifiers;
using Assets.Handlers.Enums;
using Assets.Scripts.Actions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayActions
{
    public static class ActionDataFactory
    {
        private static readonly Dictionary<Type, Type> _actionToDataMap = new()
        {
            { typeof(DamageAction), typeof(DamageDataSO) },
            { typeof(HealAction), typeof(HealDataSO) },
            { typeof(FireProjectileAction), typeof(ProjectileDataSO) },
            { typeof(ExplosionAction), typeof(EsplosionDataSO) },
            { typeof(SetBuffAction), typeof(BuffDataSO) }
        };

        private static readonly (DamageType type, StatType dmg, StatType critC, StatType critM)[] _elementalMap =
        {
            (DamageType.Physical, StatType.PhysicalDamage, StatType.CritChance, StatType.CritMultiplier),
            (DamageType.Fire, StatType.FireDamage, StatType.FireCritChance, StatType.FireCritMultiplier),
            (DamageType.Explosive, StatType.ExplosiveDamage, StatType.ExplosiveCritChance, StatType.ExplosiveCritMultiplier),
            (DamageType.Acid, StatType.AcidDamage, StatType.AcidCritChance, StatType.AcidCritMultiplier),
            (DamageType.Ultrasound, StatType.UltrasoundDamage, StatType.UltrasoundCritChance, StatType.UltrasoundCritMultiplier),
            (DamageType.Electricity, StatType.ElectricityDamage, StatType.ElectricityCritChance, StatType.ElectricityCritMultiplier),
            (DamageType.Plasma, StatType.PlasmaDamage, StatType.PlasmaCritChance, StatType.PlasmaCritMultiplier),
            (DamageType.Slow, StatType.SlowDamage, StatType.SlowCritChance, StatType.SlowCritMultiplier),
            (DamageType.Freeze, StatType.FreezeDamage, StatType.FreezeCritChance, StatType.FreezeCritMultiplier),
            (DamageType.Psi, StatType.PsiDamage, StatType.PsiCritChance, StatType.PsiCritMultiplier),
            (DamageType.Radiation, StatType.RadiationDamage, StatType.RadiationCritChance, StatType.RadiationCritMultiplier),
            (DamageType.EMP, StatType.EMPDamage, StatType.EMPCritChance, StatType.EMPCritMultiplier),
            (DamageType.SpatialAnomaly, StatType.SpatialAnomalyDamage, StatType.SpatialAnomalyCritChance, StatType.SpatialAnomalyCritMultiplier),
            (DamageType.Flooding, StatType.FloodingDamage, StatType.FloodingCritChance, StatType.FloodingCritMultiplier)
        };

        public static ActionDataSO CreateDynamicData(AbilityType abilityType, InteractionContext context)
        {
            var action = ActionProvider.GetActionByAbility(abilityType);
            if (action == null) return null;
            return CreateDynamicData(action.GetType(), context);
        }

        public static ActionDataSO CreateDynamicData(Type actionType, InteractionContext context)
        {
            if (!_actionToDataMap.TryGetValue(actionType, out Type dataType))
            {
                Debug.LogWarning($"[ActionDataFactory] No rule for creating data of type {actionType.Name}!");
                return null;
            }
            var dataInstance = (ActionDataSO)ScriptableObject.CreateInstance(dataType);
            InitializeData(dataInstance, context);
            return dataInstance;
        }

        private static void InitializeData(ActionDataSO data, InteractionContext context)
        {
            if (context.SourceInteractive is not IStats stats) return;
            switch (data)
            {
                case DamageDataSO damageData:
                    damageData.value = stats.GetLifetimeStat(StatType.Damage);
                    damageData.penetration = stats.GetLifetimeStat(StatType.Penetration);
                    damageData.critChance = stats.GetLifetimeStat(StatType.CritChance);
                    damageData.critMultiplier = stats.GetLifetimeStat(StatType.CritMultiplier);
                    int layerMaskValue = (int)stats.GetLifetimeStat(StatType.DamageLayer);
                    damageData.targetLayers = layerMaskValue == 0 ? LayerType.All : (LayerType)layerMaskValue;
                    PopulateElementalDamage(damageData, stats);
                    break;

                case HealDataSO healData:
                    healData.value = stats.GetLifetimeStat(StatType.Heal);
                    break;

                case ProjectileDataSO projData:
                    projData.damageData = (DamageDataSO)CreateDynamicData(typeof(DamageAction), context);
                    projData.speed = stats.GetLifetimeStat(StatType.PrSpeed);
                    projData.lifeTime = stats.GetLifetimeStat(StatType.PrLifeTime);
                    projData.isHoming = stats.GetLifetimeStat(StatType.PrIsHoming) > 0f;
                    projData.isBallistic = stats.GetLifetimeStat(StatType.PrMoveType) == 1f;
                    projData.type = (ProjectileType)(int)stats.GetLifetimeStat(StatType.PrType);
                    break;
            }
        }

        private static void PopulateElementalDamage(DamageDataSO damageData, IStats stats)
        {
            damageData.elements = new List<ElementalDamageData>();

            foreach (var element in _elementalMap)
            {
                float dmgValue = stats.GetLifetimeStat(element.dmg);
                if (dmgValue > 0f)
                {
                    damageData.elements.Add(new ElementalDamageData
                    {
                        type = element.type,
                        damage = dmgValue,
                        critChance = stats.GetLifetimeStat(element.critC),
                        critMultiplier = stats.GetLifetimeStat(element.critM)
                    });
                }
            }
        }
    }
}