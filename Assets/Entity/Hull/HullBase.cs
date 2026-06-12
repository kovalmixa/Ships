using Assets.Common;
using Assets.Common.ActionEffectStructs;
using Assets.DataContainers;
using Assets.Entity.Equipment;
using Assets.Handlers;
using Assets.Scripts.Effects;
using Entity.Controllers.GenericController;
using Scripts;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Entity.Hull
{
    public abstract class HullBase : MonoBehaviour, IInteractive, IHull, IModified
    {
        public HullContainer data;

        public List<EquipmentAnchor> equipmentAnchors;
        public List<Equipment.Equipment> equipments;

        public Transform root;
        protected Rigidbody2D rigidBody2D;
        public float currentSpeed;

        public List<EffectComponent> Effects { get; set; }
        public bool IsDirty { get; set; }
        protected List<EffectComponent> cachedCombinedEffects = new List<EffectComponent>();


        #region Setup

        private void Awake()
        {
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
            script?.Execute(root.GetComponent<EntityController>());
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

        #region IModified
        public List<EffectComponent> GetBuildEffects() => IsDirty ? RebuildEffects() : cachedCombinedEffects;

        public List<EffectComponent> RebuildEffects()
        {
            if (IsDirty)
            {
                cachedCombinedEffects.Clear();
                cachedCombinedEffects.AddRange(GetSets());
                //get from accessories, get from skills...
            }
            return cachedCombinedEffects;
        }

        private List<EffectComponent> GetSets()
        {
            var effects = new List<EffectComponent>();
            return effects;
        }
    }
    #endregion
}
