using Actions;
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
using System.Linq;
using UnityEngine;

namespace Assets.Entity.Hull
{
    public abstract class HullBase : MonoBehaviour, IHull, IInteractive, IBuffable, IAbbility
    {
        public HullContainer data;
        public List<EquipmentAnchor> equipmentAnchors;
        public List<Equipment.Equipment> equipments;
        public Transform root;
        public float currentSpeed;
        public string Id { get; set; }

        protected EntityController entityController;
        protected Rigidbody2D rigidBody2D;

        #region Setup

        private void Awake()
        {
            Id = GameObjectHandler.GenerateUniqueId(name);
            entityController = GetComponentInParent<EntityController>();
            rigidBody2D = GetComponent<Rigidbody2D>();
            data = GetComponent<HullContainer>();
            BuffController.SetupStatsMods(data.statOptions.Stats.ToDictionary(s => (s.Type, s.StatLayer), s => s.Value),
                new Modifiers.Modifiers(data.statOptions.mods));
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
        {
            foreach (var eq in equipments) eq.GetComponent<Equipment.Equipment>().Rotate(target);
        }

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
            if (collision.gameObject.layer != LayerMask.NameToLayer(data.general.layer.ToString())
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
        public void TakeDamage(InterractionContext interractionContext, Damage damage)
        {
            throw new System.NotImplementedException();
        }

        public void TakeHeal(InterractionContext interractionContext, Heal heal)
        {
            throw new System.NotImplementedException();
        }

        private string GenerateSourceId(InterractionContext context)
        {
            if (context == null) return "Unknown";
            if (!string.IsNullOrEmpty(context.AbilityId)) return context.AbilityId;
            if (context.SourceObject != null) return context.SourceObject.name;
            return context.SourceSnapshot?.Id ?? "System";
        }

        #endregion

        #region IBuffable

        [SerializeField] private BuffStatController _buffController;
        public BuffStatController BuffController => _buffController;

        private Dictionary<StatType, float> _lifetimeStats = new();
        public Dictionary<StatType, float> LifetimeStats => _lifetimeStats;

        private const StatLayer _statLayer = StatLayer.Hull;

        public void ResetLifetimeStats()
        {
            _lifetimeStats.Clear();
            _lifetimeStats[StatType.MaxMoveSpeed] = _buffController.GetStat((StatType.MaxMoveSpeed, _statLayer));
            _lifetimeStats[StatType.RotationSpeed] = _buffController.GetStat((StatType.RotationSpeed, _statLayer));
            _lifetimeStats[StatType.Acceleration] = _buffController.GetStat((StatType.Acceleration, _statLayer));
        }

        public float GetLifetimeStat(StatType type)
        {
            if (BuffController.IsDirty) ResetLifetimeStats();
            if (LifetimeStats.TryGetValue(type, out float value)) return value;
            return 0f;
        }

        public void AddBuff(InterractionContext context, params BuffStatus[] buffs)
        {
            if (_buffController == null || buffs == null) return;

            string sourceId = GenerateSourceId(context);

            foreach (var template in buffs)
            {
                if (template == null) continue;
                var instance = Instantiate(template, _buffController.transform);
                instance.Initialize(
                    buffId: template.name,
                    sourceId: sourceId,
                    duration: template.IsPermanent ? -1f : template.Duration
                );

                if (context != null && string.IsNullOrEmpty(instance.SourceId))
                    instance.SourceId = sourceId;

                _buffController.AddBuff(instance, context?.SourceSnapshot);
            }
        }

        public void RemoveBuff(InterractionContext context, params BuffStatus[] buffs)
        {
            if (_buffController == null) return;
            string sourceId = GenerateSourceId(context);
            if (buffs == null || buffs.Length == 0)
            {
                _buffController.RemoveBuffBySource(sourceId);
                return;
            }

            foreach (var buff in buffs)
            {
                if (buff == null) continue;
                _buffController.RemoveBuff(buff.BuffId, sourceId);
            }
        }

        #endregion

        #region IAbbility
        public AbilityUnit[] Abilities = System.Array.Empty<AbilityUnit>();
        private List<AbilityUnit> _runtimeAbilities;
        private readonly Dictionary<AbilityUnit, float> _abilityCooldowns = new();
        public IReadOnlyList<AbilityUnit> RuntimeAbilities
        {
            get
            {
                _runtimeAbilities ??= new List<AbilityUnit>(Abilities ?? System.Array.Empty<AbilityUnit>());
                return _runtimeAbilities;
            }
        }

        public void AddAbility(AbilityUnit ability)
        {
            if (ability == null) return;
            ((List<AbilityUnit>)RuntimeAbilities).Add(ability);
        }

        public bool RemoveAbility(AbilityUnit ability) => ((List<AbilityUnit>)RuntimeAbilities).Remove(ability);
       
        public void Activate(Vector3 targetPos, AbilityUnit abilityUnit, InterractionContext context)
        {
            throw new System.NotImplementedException();
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
