using Actions;
using Assets.Common;
using Assets.Entity.Controllers;
using Assets.Entity.Interfaces;
using Assets.Entity.Modifiers;
using Assets.Handlers;
using Assets.Handlers.Enums;
using Assets.Handlers.SceneHandlers;
using Assets.Scripts.Actions;
using Entity.Controllers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Entity.Equipment
{
    public class Equipment : MonoBehaviour, IInteractive, IBuffable, IAbbility
    {
        public EntityController entityController;
        [SerializeField] private EquipmentContainer _equipmentContainer;
        public EquipmentContainer EquipmentContainer => _equipmentContainer;

        public EquipmentAnchor EquipmentAnchor { get; set; }
        public EquipmentSubType Type;

        private const float _basicAngle = 90;
        public Vector2 Position
        {
            get => transform.position + entityController.transform.position;
            set { }
        }
        public string Id { get; set; }

        private void Awake()
        {
            Id = GameObjectHandler.GenerateUniqueId(name);
        }

        public void Rotate(Vector3 targetPos)
        {
            var rotationSpeed = GetLifetimeStat(StatType.RotationSpeed);

            if (_equipmentContainer == null || !CanRotate()) return;
            Vector3 localTarget = EquipmentAnchor.transform.InverseTransformPoint(targetPos);
            float localAngle = Mathf.Atan2(localTarget.y, localTarget.x) * Mathf.Rad2Deg;
            float min = EquipmentAnchor.rotationSector.x;
            float max = EquipmentAnchor.rotationSector.y;
            float clampedLocal = Mathf.Clamp(localAngle, min, max);
            float finalWorldAngle = EquipmentAnchor.transform.eulerAngles.z + clampedLocal;
            float rotationSpeedDelta = rotationSpeed * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                Quaternion.Euler(0f, 0f, finalWorldAngle - _basicAngle),
                rotationSpeedDelta
            );
        }

        public bool CanRotate()
        {
            if (EquipmentAnchor == null) return false;
            return EquipmentAnchor.rotationSector != Vector2.zero;
        }

        #region IInteractive

        public void TakeDamage(InterractionContext interractionContext, Damage damage)
        {
            throw new System.NotImplementedException();
        }

        public void TakeHeal(InterractionContext interractionContext, Heal heal)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region IBuffable

        [SerializeField] private BuffStatController _buffController;
        public BuffStatController BuffController => _buffController;
        private Dictionary<StatType, float> _lifetimeStats = new();
        public Dictionary<StatType, float> LifetimeStats => _lifetimeStats;

        private const StatLayer _statLayer = StatLayer.Equipment;
        public void ResetLifetimeStats()
        {
            _lifetimeStats.Clear();
            _lifetimeStats[StatType.RotationSpeed] = _buffController.GetStat((StatType.RotationSpeed, _statLayer));
        }

        public float GetLifetimeStat(StatType type)
        {
            if (BuffController.IsDirty) ResetLifetimeStats();
            if (LifetimeStats.TryGetValue(type, out float value)) return value;
            return 0f;
        }

        public void AddBuff(InterractionContext interractionContext, params BuffStatus[] buffs)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveBuff(InterractionContext interractionContext, params BuffStatus[] buffs)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region IAbbility

        private List<AbilityUnit> _runtimeAbilities;
        private readonly Dictionary<AbilityUnit, float> _abilityCooldowns = new();
        public AbilityUnit[] Abilities = System.Array.Empty<AbilityUnit>();
        public IReadOnlyList<AbilityUnit> RuntimeAbilities
        {
            get
            {
                EnsureRuntimeAbilities();
                return _runtimeAbilities;
            }
        }

        private void EnsureRuntimeAbilities()
        {
            if (_runtimeAbilities != null) return;
            _runtimeAbilities = new List<AbilityUnit>();
            if (_equipmentContainer != null && _equipmentContainer.statOptions.abilities != null)
                _runtimeAbilities.AddRange(_equipmentContainer.statOptions.abilities);
            if (Abilities != null)
                _runtimeAbilities.AddRange(Abilities);
        }

        public void AddAbility(AbilityUnit ability)
        {
            if (ability == null) return;
            EnsureRuntimeAbilities();
            _runtimeAbilities.Add(ability);
            entityController?.abbilitiesController?.MarkDirty();
        }

        public bool RemoveAbility(AbilityUnit ability)
        {
            EnsureRuntimeAbilities();
            bool removed = _runtimeAbilities.Remove(ability);
            if (removed) entityController?.abbilitiesController?.MarkDirty();
            return removed;
        }

        public void Activate(Vector3 targetPos, AbilityUnit abilityUnit, InterractionContext context)
        {
            var action = abilityUnit.action;
            if (action == null) return;
            if (CanActivate(targetPos, abilityUnit))
            {
                action.Execute(context, targetPos);
                return;
            }
            if (!IsAimedAtTarget(targetPos, abilityUnit.delay, out Vector3 targetPosEq)) return;
            if (!IsWithinActivationSector()) return;
            action.Execute(context, targetPosEq);
        }

        private bool IsAimedAtTarget(Vector3 targetPos, float delay, out Vector3 targetPosEq)
        {
            float distance = Vector2.Distance(transform.position, targetPos);
            targetPosEq = MathFuncHandler.GetAngleDistancePoint(transform.position, transform.eulerAngles.z + _basicAngle, distance);

            float targetWorldAngle = Mathf.Atan2(targetPos.y - transform.position.y, targetPos.x - transform.position.x) * Mathf.Rad2Deg;
            float currentAngle = Mathf.Repeat(transform.eulerAngles.z + _basicAngle, 360f);
            float angleDiff = Mathf.DeltaAngle(currentAngle, targetWorldAngle);
            return Mathf.Abs(angleDiff) < 12.5f / delay;
        }

        private bool IsWithinActivationSector()
        {
            if (EquipmentAnchor.activationSectors.Length == 0) return true;
            float currentAngle = Mathf.Abs(Mathf.DeltaAngle(
                Mathf.Repeat(transform.eulerAngles.z + _basicAngle, 360f),
                EquipmentAnchor.transform.eulerAngles.z));
            return EquipmentAnchor.activationSectors.Any(sector => currentAngle >= sector.x && currentAngle <= sector.y);
        }

        public bool CanActivate(Vector3 targetPos, AbilityUnit abilityUnit)
        {
            float time = Time.time;
            float delay = abilityUnit.delay;
            if (delay <= 0 || abilityUnit.isPassive) return true;
            _abilityCooldowns.TryGetValue(abilityUnit, out float lastActivationTime);
            if (time - lastActivationTime < delay) return false;
            _abilityCooldowns[abilityUnit] = time;
            return true;
        }

        #endregion
    }
}
