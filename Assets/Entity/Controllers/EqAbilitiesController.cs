using Assets.Common;
using Assets.Entity.Equipment;
using Assets.Handlers;
using Assets.Scripts.Actions;
using GameplayActions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Entity.Controllers
{
    public class EqAbilitiesController : AbilitiesController
    {
        private readonly Transform _equipmentTransform;
        private readonly EquipmentAnchor _equipmentAnchor;
        private readonly float _basicAngle;

        public EqAbilitiesController(IEnumerable<AbilityUnit> abilities, TotalAbbilitiesController totalAbbilities, 
            Transform transform, float basicAngle, EquipmentAnchor equipmentAnchor)
            : base(abilities, totalAbbilities)
        {
            _equipmentTransform = transform;
            _basicAngle = basicAngle;
            _equipmentAnchor = equipmentAnchor;
        }

        public override bool TryActivate(Vector2 targetPos, AbilityUnit abilityUnit, InteractionContext context)
        {
            var action = ActionProvider.GetAction(abilityUnit.type);
            if (action == null || !CanActivate(targetPos, abilityUnit)) return false;
            if (!IsAimedAtTarget(targetPos, abilityUnit.delay, out Vector2 targetPosEq)) return false;
            if (!IsWithinActivationSector()) return false;
            action.Execute(context, targetPosEq);
            return true;
        }

        private bool IsAimedAtTarget(Vector3 targetPos, float delay, out Vector2 targetPosEq)
        {
            float distance = Vector2.Distance(_equipmentTransform.position, targetPos);
            targetPosEq = MathFuncHandler.GetAngleDistancePoint(_equipmentTransform.position, _equipmentTransform.eulerAngles.z + _basicAngle, distance);

            float targetWorldAngle = Mathf.Atan2(targetPos.y - _equipmentTransform.position.y, targetPos.x - _equipmentTransform.position.x) * Mathf.Rad2Deg;
            float currentAngle = Mathf.Repeat(_equipmentTransform.eulerAngles.z + _basicAngle, 360f);
            float angleDiff = Mathf.DeltaAngle(currentAngle, targetWorldAngle);
            return Mathf.Abs(angleDiff) < 12.5f / delay;
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
