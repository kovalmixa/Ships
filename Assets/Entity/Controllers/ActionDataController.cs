using Assets.Entity.Equipment;
using Assets.Entity.Modifiers;
using Assets.Handlers;
using Assets.Handlers.Enums;
using Assets.Scripts.Actions;
using Assets.Scripts.GameplayActions;
using GameplayActions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Entity.Controllers
{
    public class ActionDataController : IDirty
    {
        private readonly Dictionary<Type, ActionData> _dataDictionary = new();

        private static readonly Dictionary<Type, Type> _actionToDataMap = new()
        {
            { typeof(DamageAction), typeof(DamageData) },
            { typeof(HealAction), typeof(HealData) },
            { typeof(FireProjectileAction), typeof(ProjectileData) },
            { typeof(ExplosionAction), typeof(ExplosionData) },
            { typeof(SpawnAction), typeof(SpawnData) },
            { typeof(SetBuffAction), typeof(BuffData) }
        };

        public ActionData GetActionData(AbilityType abilityType, InteractionContext context)
        {
            var action = ActionProvider.GetActionByAbility(abilityType);
            if (action == null) return null;
            return GetActionData(action.GetType(), context, true);
        }

        public ActionData GetActionData(Type actionType, InteractionContext context, bool getFromSnapshot)
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

                SetActionData(data, context, getFromSnapshot);
                _isDirty = false;
                return data;
            }

            data = (ActionData)Activator.CreateInstance(dataType);
            SetActionData(data, context, getFromSnapshot);

            _dataDictionary.Add(dataType, data);
            return data;
        }

        private void SetActionData(ActionData data, InteractionContext context, bool getFromSnapshot)
        {
            if (context.SourceInteractive is not IStats stats) return;

            switch (data)
            {
                case ExplosionData explosionData:
                    explosionData.damageData ??= new DamageData();
                    SetActionData(explosionData.damageData, context, getFromSnapshot);
                    break;

                case DamageData damageData:
                    damageData.value = stats.GetLifetimeStat(StatType.Damage);
                    damageData.penetration = stats.GetLifetimeStat(StatType.Penetration);
                    damageData.critChance = stats.GetLifetimeStat(StatType.CritChance);
                    damageData.critMultiplier = stats.GetLifetimeStat(StatType.CritMultiplier);
                    damageData.range = stats.GetLifetimeStat(StatType.AreaOfEffect);

                    int layerMaskValue = (int)stats.GetLifetimeStat(StatType.DamageLayer);
                    damageData.targetLayers = layerMaskValue == 0 ? LayerType.All : (LayerType)layerMaskValue;
                    
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
                    break;

                case HealData healData:
                    healData.value = stats.GetLifetimeStat(StatType.Heal);
                    break;

                case ProjectileData projData:
                    projData.damageValue = stats.GetLifetimeStat(StatType.Damage);
                    projData.speed = stats.GetLifetimeStat(StatType.PrSpeed);
                    projData.lifeTime = stats.GetLifetimeStat(StatType.PrLifeTime);
                    projData.isHoming = stats.GetLifetimeStat(StatType.PrIsHoming) > 0f;
                    projData.isBallistic = stats.GetLifetimeStat(StatType.PrMoveType) == 1f;

                    var _dataContainer = stats.GetInitialData();
                    if (_dataContainer is EquipmentDataSO eqData) projData.type = eqData.projectileType;

                    projData.elementalTypes.Clear();

                    foreach (var element in StatModHandler.elementalMap)
                    {
                        float dmgValue = stats.GetLifetimeStat(element.dmg);
                        if (dmgValue > 0f) projData.elementalTypes.Add(element.type);
                    }
                    break;

                case SpawnData spawnData:
                    var initialData = stats.GetInitialData();
                    if (initialData is ISpawnDataProvider spawnProvider)
                    {
                        spawnData.entityData = spawnProvider.EntityData;
                        spawnData.scripts = spawnProvider.Scripts;
                        spawnData.offset = spawnProvider.Offset;
                        spawnData.positionMode = spawnProvider.PositionMode;
                    }
                    break;
            }
        }

        #region IDirty

        private bool _isDirty = true;

        public bool IsDirty => _isDirty;

        public void MarkDirty() => _isDirty = true;

        #endregion
    }
}