using Assets.Common;
using Assets.DataContainers;
using Assets.Entity.Controllers;
using Assets.Entity.Equipment;
using Assets.Entity.Interfaces;
using Assets.Entity.Modifiers;
using Assets.Handlers.SceneHandlers;
using Assets.Scripts.Actions;
using Entity.Controllers;
using Scripts;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Entity.Hull
{
    public abstract class HullBase : MonoBehaviour, IHull, IInteractive, IStats, IAbbility
    {
        public HullContainer Data { get; private set; }
        [HideInInspector] public List<EquipmentAnchor> equipmentAnchors;
        [HideInInspector] public List<Equipment.Equipment> equipments;
        [HideInInspector] public Transform root;
        [HideInInspector] public float currentSpeed;
        public string Id { get; set; }

        protected EntityController entityController;
        protected Rigidbody2D rigidBody2D;

        #region Editor

        private void OnValidate()
        {
            if (Data == null) Data = GetComponent<HullContainer>();
            if (Data == null) return;

            var statOptions = Data.statOptions;
            if (statOptions.stats != null) foreach (var stat in statOptions.stats) stat?.UpdateInspectorName();
            if (statOptions.mods != null)foreach (var mod in statOptions.mods) mod?.UpdateInspectorName();
        }

        #endregion

        #region Setup

        private void Awake()
        {
            Id = GameObjectHandler.GenerateUniqueId(name);
            rigidBody2D = GetComponent<Rigidbody2D>();
            Data = GetComponent<HullContainer>();
        }

        public void Setup(EntityController entityController) 
        {
            this.entityController = entityController;
            abilitiesController = new(entityController.totalAbbilitiesController);

            var snapshot = entityController.GetSnapshot();
            var statOptions = Data.statOptions;
            _statModController = new(entityController.statModController, statOptions);
            foreach (var buff in statOptions.buffs) entityController.Buffs.AddBuff(buff, snapshot);
            foreach (var ability in statOptions.abilities) abilitiesController.AddAbility(ability);

            CollectAnchors(transform);
        }

        private void CollectAnchors(Transform parent)
        {
            if (parent == null) return;
            foreach (Transform child in parent)
            {
                var equipmentAnchor = child.GetComponent<EquipmentAnchor>();
                if (equipmentAnchor != null) equipmentAnchors.Add(equipmentAnchor);
                CollectAnchors(child);
            }
        }

        #endregion

        #region Movement

        public void RotateEquipment(Vector3 target)
        { foreach (var eq in equipments) eq.GetComponent<Equipment.Equipment>().Rotate(target); }

        public abstract void AddSpeed(bool isAddition);

        public abstract void SetTargetSpeed(Vector2 directionToPoint);

        public abstract void Movement(float rotationDirection);

        #endregion

        #region Triggers

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Bounce(collision);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            IScript script = other.GetComponent<IScript>();
            script?.Execute(entityController);
        }

        private void Bounce(Collision2D collision)
        {
            Rigidbody2D otherRb = collision.rigidbody;
            if (otherRb == null) return;
            if (collision.gameObject.layer != LayerMask.NameToLayer(Data.general.Layer.ToString())
                && collision.gameObject.layer != LayerMask.NameToLayer("Markers"))
            {
                currentSpeed = 0;
                return;
            }
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            Vector2 pushDirection = (rb.position - otherRb.position).normalized;
            float totalMass = rb.mass + otherRb.mass; // Общая масса двух объектов
            float impulse = currentSpeed * rb.mass * 0.1f;  // Импульс игрока

            // Передаем часть импульса другому объекту
            otherRb.AddForce(-pushDirection * (impulse * (rb.mass / totalMass)), ForceMode2D.Impulse);

            //Теряет скорость пропорционально массе другого объекта
            currentSpeed *= otherRb.mass / totalMass;
        }

        #endregion

        #region IInteractive

        public GameObject GameObject => gameObject;

        public void AddBuff(InteractionContext context)
        {
            var buff = context.ActionStruct as BuffStatus;
            if (buff == null) return;
            entityController.Buffs.AddBuff(buff, context.SourceSnapshot);
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

        private const StatLayer _statLayer = StatLayer.Hull;

        public void ResetLifetimeStats()
        {
            _lifetimeStats.Clear();
            _lifetimeStats[StatType.MaxMoveSpeed] = _statModController.GetStat(StatType.MaxMoveSpeed, _statLayer);
            _lifetimeStats[StatType.RotationSpeed] = _statModController.GetStat(StatType.RotationSpeed, _statLayer);
            _lifetimeStats[StatType.Acceleration] = _statModController.GetStat(StatType.Acceleration, _statLayer);
        }

        public float GetLifetimeStat(StatType type)
        {
            if (StatModController.IsDirty) ResetLifetimeStats();
            if (LifetimeStats.TryGetValue(type, out float value)) return value;
            return 0f;
        }

        #endregion

        #region IAbbility
        public AbilitiesController abilitiesController;
        public IReadOnlyList<AbilityUnit> RuntimeAbilities => abilitiesController.RuntimeAbilities;

        public void AddAbility(AbilityUnit ability) => abilitiesController.AddAbility(ability);

        public bool RemoveAbility(AbilityUnit ability) => abilitiesController.RemoveAbility(ability);

        public void Activate(Vector2 targetPos, AbilityUnit abilityUnit, InteractionContext context) {
            if (abilitiesController.TryActivate(targetPos, abilityUnit, context)) 
                EventBrocker.Raise(new EntityInteractionEvent(context));
        }

        #endregion
    }
}
