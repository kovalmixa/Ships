using Assets.Common;
using Assets.Common.ActionEffectStructs;
using Assets.DataContainers;
using Assets.Entity.Equipment;
using Assets.Entity.Modifiers;
using Assets.Handlers;
using Entity.Controllers;
using Scripts;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Entity.Hull
{
    public abstract class HullBase : MonoBehaviour, IInteractive, IHull, IBuffStat
    {
        public HullContainer data;

        public List<EquipmentAnchor> equipmentAnchors;
        public List<Equipment.Equipment> equipments;
        public Transform root;
        public float currentSpeed;
        protected EntityController entityController;
        protected Rigidbody2D rigidBody2D;

        #region Setup

        private void Awake()
        {
            entityController = GetComponentInParent<EntityController>();
            rigidBody2D = GetComponent<Rigidbody2D>();
            data = GetComponent<HullContainer>();
            CollectAnchors(transform);
        }

        private void CollectAnchors(Transform parent)
        {
            if (parent == null) return;
            foreach (Transform child in parent)
            {
                var equipmentAnchor = child.GetComponent<EquipmentAnchor>();
                if (equipmentAnchor != null)
                {
                    equipmentAnchors.Add(equipmentAnchor);
                }
                CollectAnchors(child);
            }
        }

        #endregion

        #region Movement

        public void RotateEquipment(Vector3 target)
        {
            foreach (var eq in equipments)
            {
                eq.GetComponent<Equipment.Equipment>().Rotate(target);
            }
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
            if (collision.gameObject.layer != LayerMask.NameToLayer(
                TypeListHandler.layerTypes.ToArray()[data.general.Layer]) 
                && collision.gameObject.layer != LayerMask.NameToLayer("Markers")
                )
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
        public void TakeDamage(ActionContext context, Damage damage)
        {
            throw new System.NotImplementedException();
        }

        public void TakeHeal(ActionContext context, Heal heal)
        {
            throw new System.NotImplementedException();
        }


        #endregion

        #region IBuffStat
        public Dictionary<StatType, float> StatsDict => new();
        public Modifiers.Modifiers Modifiers
        {
            get => modifiers;
            set
            {
                modifiers = value;
                IsDirty = true;
            }
        }
        public List<BuffStatus> BuffStatuses
        {
            get => buffStatuses;
            set
            {
                buffStatuses = value;
                IsDirty = true;
            }
        }
        public bool IsDirty { get; set; }

        private Modifiers.Modifiers modifiers = new();
        private List<BuffStatus> buffStatuses = new();
        protected Dictionary<StatType, float> cachedCombinedStats = new();

        public bool TryGetCurrentStat(StatType type, out float value) => 
            (IsDirty ? RebuildCachedStats() : cachedCombinedStats).TryGetValue(type, out value);

        public void SetupStats()
        {
            throw new System.NotImplementedException();
        }

        public Dictionary<StatType, float> RebuildCachedStats()
        {
            if (IsDirty)
            {
                cachedCombinedStats.Clear();
                cachedCombinedStats.AddRange(StatsDict);

                var allMods = Modifiers;
                //get from accessories, get from skills...
                allMods.Add((entityController as IBuffStat).Modifiers);
                foreach (var mod in BuffStatuses.Select(buff => buff.Modifiers)) allMods.Add(mod);


                foreach (var stat in cachedCombinedStats)
                    cachedCombinedStats[stat.Key] = allMods.ApplyModByType(stat.Key, stat.Value);
            }
            return cachedCombinedStats;
        }

        public float TryGetCurrentStat(StatType type) => cachedCombinedStats.TryGetValue(type, out float value) ? value : 0f;

    }
    #endregion
}
