using Assets.Common;
using Assets.DataContainers;
using Assets.Entity.Controllers;
using Assets.Entity.Interfaces;
using Assets.Entity.Modifiers;
using Assets.Handlers.SceneHandlers;
using Assets.Scripts.Actions;
using Entity.Controllers;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Entity.Equipment
{
    public class Equipment : MonoBehaviour, IInteractive, IStats, IAbbility
    {
        private EntityController _entityController;
        public EquipmentContainer Data { get; private set; }
        public EquipmentAnchor EquipmentAnchor { get; set; }
        private const float _basicAngle = 90;
        public Vector2 Position
        {
            get => transform.position + _entityController.transform.position;
            set { }
        }
        public string Id { get; set; }

        #region Editor

        private void OnValidate()
        {
            if (Data == null) Data = GetComponent<EquipmentContainer>();
            if (Data == null) return;

            var statOptions = Data.statOptions;
            if (statOptions.stats != null) foreach (var stat in statOptions.stats) stat?.UpdateInspectorName();
            if (statOptions.mods != null) foreach (var mod in statOptions.mods) mod?.UpdateInspectorName();
        }

        #endregion

        #region Setup

        private void Awake()
        {
            Id = GameObjectHandler.GenerateUniqueId(name);
            Data = GetComponent<EquipmentContainer>();
        }

        public void Setup(EntityController entityController)
        {
            _entityController = entityController;
            var snapshot = entityController.GetSnapshot();
            var statOptions = Data.statOptions;
            _statModController = new(entityController.statModController, statOptions);
            foreach (var buff in statOptions.buffs) entityController.Buffs.AddBuff(buff, snapshot);

            abilitiesController = new(
                _entityController.totalAbbilitiesController,
                transform,
                _basicAngle,
                EquipmentAnchor
                );
            foreach (var ability in statOptions.abilities) abilitiesController.AddAbility(ability);
        }

        #endregion

        #region Rotation

        private float _currentLocalAngle = 0f;
        private bool _isLocalAngleInitialized = false;

        public void Rotate(Vector3 targetPos)
        {
            if (Data == null || !CanRotate()) return;

            var rotationSpeed = GetLifetimeStat(StatType.RotationSpeed);

            Vector3 localTarget = EquipmentAnchor.transform.InverseTransformPoint(targetPos);
            float targetAngle = Mathf.Atan2(localTarget.y, localTarget.x) * Mathf.Rad2Deg;

            float min = EquipmentAnchor.rotationSector.x;
            float max = EquipmentAnchor.rotationSector.y;

            if (!_isLocalAngleInitialized)
            {
                _currentLocalAngle = transform.localEulerAngles.z + _basicAngle;
                _isLocalAngleInitialized = true;
            }

            if (Mathf.Abs(max - min) >= 360f)
            {
                Quaternion targetLocalRot = Quaternion.Euler(0f, 0f, targetAngle - _basicAngle);
                transform.localRotation = Quaternion.RotateTowards(
                    transform.localRotation,
                    targetLocalRot,
                    rotationSpeed * Time.deltaTime
                );
                _currentLocalAngle = transform.localEulerAngles.z + _basicAngle;
                return;
            }

            float sectorWidth = max - min;

            float currentOffset = NormalizeAngle(_currentLocalAngle - min);
            float targetOffset = NormalizeAngle(targetAngle - min);

            float desiredOffset;
            if (targetOffset <= sectorWidth) desiredOffset = targetOffset;
            else
            {
                float distToMin = 360f - targetOffset;
                float distToMax = targetOffset - sectorWidth;
                desiredOffset = (distToMin < distToMax) ? 0f : sectorWidth;
            }

            float newOffset = Mathf.MoveTowards(currentOffset, desiredOffset, rotationSpeed * Time.deltaTime);
            newOffset = Mathf.Clamp(newOffset, 0f, sectorWidth);

            _currentLocalAngle = min + newOffset;
            transform.localRotation = Quaternion.Euler(0f, 0f, _currentLocalAngle - _basicAngle);
        }
        private float NormalizeAngle(float angle)
        {
            float result = angle % 360f;
            if (result < 0) result += 360f;
            return result;
        }

        public bool CanRotate()
        {
            if (EquipmentAnchor == null) return false;
            return EquipmentAnchor.rotationSector != Vector2.zero;
        }

        #endregion

        #region IInteractive

        public GameObject GameObject => gameObject;

        public void AddBuff(InteractionContext context)
        {
            var buff = context.ActionStruct as BuffStatus;
            if (buff == null) return;
            _entityController.Buffs.AddBuff(buff, context.SourceSnapshot);
        }

        public void TakeDamage(InteractionContext interractionContext)
        {
            throw new System.NotImplementedException();
        }

        public void TakeHeal(InteractionContext interractionContext)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region IStats

        [SerializeField] private StatModController _statModController;
        public StatModController StatModController => _statModController;
        private Dictionary<StatType, float> _lifetimeStats = new();
        public Dictionary<StatType, float> LifetimeStats => _lifetimeStats;

        private const StatLayer _statLayer = StatLayer.Equipment;
        public void ResetLifetimeStats()
        {
            _lifetimeStats.Clear();
            _lifetimeStats[StatType.RotationSpeed] = _statModController.GetStat(StatType.RotationSpeed, _statLayer);
        }

        public float GetLifetimeStat(StatType type)
        {
            if (StatModController.IsDirty) ResetLifetimeStats();
            if (LifetimeStats.TryGetValue(type, out float value)) return value;
            return 0f;
        }

        #endregion

        #region IAbbility

        public EqAbilitiesController abilitiesController;
        public IReadOnlyList<AbilityUnit> RuntimeAbilities => abilitiesController.RuntimeAbilities;

        public void AddAbility(AbilityUnit ability) => abilitiesController.AddAbility(ability);

        public bool RemoveAbility(AbilityUnit ability) => abilitiesController.RemoveAbility(ability);

        public void Activate(Vector2 targetPos, AbilityUnit abilityUnit, InteractionContext context)
        {
            if (abilitiesController.TryActivate(targetPos, abilityUnit, context)) 
                EventBrocker.Raise(new EntityInteractionEvent(context));
        }

        #endregion
    }
}
