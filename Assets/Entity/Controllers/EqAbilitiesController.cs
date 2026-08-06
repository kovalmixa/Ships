using Assets.Common;
using Assets.Entity.Equipment;
using Assets.Entity.Interfaces;
using Assets.Entity.Modifiers;
using Assets.Handlers;
using Assets.Scripts.Actions;
using GameplayActions;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Entity.Controllers
{
    public class EqAbilitiesController : AbilitiesController
    {
        private readonly Transform _equipmentTransform;
        private readonly EquipmentAnchor _equipmentAnchor;
        private readonly float _basicAngle;

        public EqAbilitiesController(IEnumerable<AbilityUnit> abilities, TotalAbbilitiesController totalAbbilities,
            ActionDataController actionDataController, IAbbility source, float basicAngle, EquipmentAnchor equipmentAnchor)
            : base(abilities, totalAbbilities, actionDataController, source)
        {
            _equipmentTransform = (source as IInteractive).GameObject.transform;
            _basicAngle = basicAngle;
            _equipmentAnchor = equipmentAnchor;
        }

        public override bool TryActivate(Vector2 targetPos, AbilityUnit abilityUnit)
        {
            var (action, context, data) = SetupActivationData(abilityUnit);
            if (action == null || !CanActivate(targetPos, abilityUnit) || data == null) return false;

            float activationRate = (source as IStats)?.GetLifetimeStat(StatType.ActivationRate) ?? 0f;
            float activationDelay = abilityUnit.delay / activationRate;

            float distance = Vector2.Distance(_equipmentTransform.position, targetPos);
            var targetPosEq = MathFuncHandler.GetAngleDistancePoint(
                _equipmentTransform.position, _equipmentTransform.eulerAngles.z + _basicAngle, distance);

            if (activationRate != 0 && !IsAimedAtTarget(targetPos, activationDelay)) return false;
            if (!IsWithinActivationSector()) return false;
            action.Execute(context, data, targetPosEq);
            //EventBrocker.Raise(new EntityInteractionEvent(context));
            return true;
        }

        private bool IsAimedAtTarget(Vector3 targetPos, float delay)
        {
            float targetWorldAngle = Mathf.Atan2(targetPos.y - _equipmentTransform.position.y, targetPos.x - _equipmentTransform.position.x) * Mathf.Rad2Deg;
            float currentAngle = Mathf.Repeat(_equipmentTransform.eulerAngles.z + _basicAngle, 360f);
            float angleDiff = Mathf.DeltaAngle(currentAngle, targetWorldAngle);
            return Mathf.Abs(angleDiff) < Math.Clamp(12.5f / delay, 0, 45);
        }

        private bool IsWithinActivationSector()
        {
            if (_equipmentAnchor.activationSectors.Length == 0) return true;
            float currentAngle = Mathf.Abs(Mathf.DeltaAngle(
                Mathf.Repeat(_equipmentTransform.eulerAngles.z + _basicAngle, 360f),
                _equipmentAnchor.transform.eulerAngles.z));
            return _equipmentAnchor.activationSectors.Any(sector => currentAngle >= sector.x && currentAngle <= sector.y);
        }
    }
}
