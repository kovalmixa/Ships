using GameplayActions;
using Assets.Common;
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
        public EntityController entityController;
        [SerializeField] private EquipmentContainer _equipmentContainer;
        public EquipmentContainer EquipmentContainer => _equipmentContainer;

        public EquipmentAnchor EquipmentAnchor { get; set; }

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
            abilitiesController = new(entityController.totalAbbilitiesController, transform, _basicAngle, EquipmentAnchor);
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

        [SerializeField] private StatModController _buffController;
        public StatModController BuffController => _buffController;
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

        public EqAbilitiesController abilitiesController;
        public IReadOnlyList<AbilityUnit> RuntimeAbilities => abilitiesController.RuntimeAbilities;

        public void AddAbility(AbilityUnit ability) => abilitiesController.AddAbility(ability);

        public bool RemoveAbility(AbilityUnit ability) => abilitiesController.RemoveAbility(ability);

        public void Activate(Vector2 targetPos, AbilityUnit abilityUnit, InterractionContext context)
        {
            if (abilitiesController.TryActivate(targetPos, abilityUnit, context)) 
                EventBrocker.Raise(new EntityInterractionEvent(context));
        }

        #endregion
    }
}
