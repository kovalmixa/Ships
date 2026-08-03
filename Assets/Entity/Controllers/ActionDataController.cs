using Assets.Entity.Modifiers;
using Assets.Handlers;
using Assets.Handlers.Enums;
using Assets.Scripts.Actions;
using GameplayActions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Entity.Controllers
{
    public class ActionDataController : IDirty
    {
        private readonly Dictionary<Type, ActionDataSO> _dataDictionary = new();

        private static readonly Dictionary<Type, Type> _actionToDataMap = new()
        {
            { typeof(DamageAction), typeof(DamageDataSO) },
            { typeof(HealAction), typeof(HealDataSO) },
            { typeof(FireProjectileAction), typeof(ProjectileDataSO) },
            { typeof(ExplosionAction), typeof(EsplosionDataSO) },
            { typeof(SetBuffAction), typeof(BuffDataSO) }
        };

        public ActionDataSO GetActionData(AbilityType abilityType, InteractionContext context)
        {
            var action = ActionProvider.GetActionByAbility(abilityType);
            if (action == null) return null;
            return GetActionData(action.GetType(), context);
        }

        public ActionDataSO GetActionData(Type actionType, InteractionContext context)
        {
            if (context.SourceInteractive is not IStats stats) return null;

            if (!_actionToDataMap.TryGetValue(actionType, out var dataType))
            {
                Debug.LogWarning($"[ActionDataController] No data mapping for action {actionType.Name}");
                return null;
            }

            if (_dataDictionary.TryGetValue(dataType, out var data))
            {
                if (!IsDirty) return data;

                SetActionData(data, context);
                _isDirty = false;
                return data;
            }

            data = (ActionDataSO)Activator.CreateInstance(dataType);
            SetActionData(data, context);

            _dataDictionary.Add(dataType, data);
            return data;
        }

        #region Action data factory

        private void SetActionData(ActionDataSO data, InteractionContext context)
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
                    projData.damageData = (DamageDataSO)GetActionData(typeof(DamageAction), context);
                    projData.speed = stats.GetLifetimeStat(StatType.PrSpeed);
                    projData.lifeTime = stats.GetLifetimeStat(StatType.PrLifeTime);
                    projData.isHoming = stats.GetLifetimeStat(StatType.PrIsHoming) > 0f;
                    projData.isBallistic = stats.GetLifetimeStat(StatType.PrMoveType) == 1f;
                    projData.type = (ProjectileType)(int)stats.GetLifetimeStat(StatType.PrType);
                    break;
            }
        }

        private void PopulateElementalDamage(DamageDataSO damageData, IStats stats)
        {
            damageData.elements.Clear();
            foreach (var element in StatModHandler.elementalMap)
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

        #endregion

        #region IDirty

        private bool _isDirty = true;

        public bool IsDirty => _isDirty;

        public void MarkDirty() => _isDirty = true;

        #endregion
    }
}