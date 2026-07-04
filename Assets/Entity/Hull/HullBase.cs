using Assets.Common;
using Assets.Scripts.Actions;
using Assets.DataContainers;
using Assets.Entity.Controllers;
using Assets.Entity.Equipment;
using Assets.Handlers;
using Entity.Controllers;
using Scripts;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Entity.Hull
{
    public abstract class HullBase : MonoBehaviour, IInteractive, IHull
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
            BuffController.BaseStats = data.stats;
            //_buffStatController.LocalModifiers = data.equipmentLocalModifiers;
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
        [SerializeField] private BuffStatController _buffController;
        public BuffStatController BuffController => _buffController;

        public void TakeDamage(InterractionContext interractionContext, Damage damage)
        {
            throw new System.NotImplementedException();
        }

        public void TakeHeal(InterractionContext interractionContext, Heal heal)
        {
            throw new System.NotImplementedException();
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

                _buffController.AddBuff(instance, context?.Caster);
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

        private string GenerateSourceId(InterractionContext context)
        {
            if (context == null) return "Unknown";
            if (!string.IsNullOrEmpty(context.AbilityId)) return context.AbilityId;
            if (context.SourceObject != null) return context.SourceObject.name;
            return context.Caster?.Id ?? "System";
        }
        #endregion
    }
}
