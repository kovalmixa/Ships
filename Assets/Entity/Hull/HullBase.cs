using System.Collections.Generic;
using Assets.DataContainers;
using Assets.Entity.Equipment;
using Assets.Entity.Interfaces;
using Assets.Handlers;
using Entity.Controllers.GenericController;
using Scripts;
using UnityEngine;

namespace Assets.Entity.Hull
{
    public abstract class HullBase : MonoBehaviour, IDamageable, IHull
    {
        public HullContainer Data;

        public List<EquipmentAnchor> EquipmentAnchors;
        public List<Equipment.Equipment> Equipments;

        public Transform Root;
        protected Rigidbody2D Rigidbody2D;

        public float CurrentSpeed;

        private void Awake()
        {
            Rigidbody2D = GetComponent<Rigidbody2D>();
            Data = GetComponent<HullContainer>();
            CollectAnchors(transform);
        }

        public void RotateEquipment(Vector3 target)
        {
            foreach (var eq in Equipments)
            {
                eq.GetComponent<Equipment.Equipment>().Rotate(target);
            }
        }

        public abstract void AddSpeed(bool isAddition);

        public abstract void SetTargetSpeed(Vector2 directionToPoint);

        public abstract void Movement(float rotationDirection);

        private void CollectAnchors(Transform parent)
        {
            if (parent == null) return;
            foreach (Transform child in parent)
            {
                var equipmentAnchor = child.GetComponent<EquipmentAnchor>();
                if (equipmentAnchor != null)
                {
                    EquipmentAnchors.Add(equipmentAnchor);
                }
                CollectAnchors(child);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Bounce(collision);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            IScript script = other.GetComponent<IScript>();
            script?.Execute(Root.GetComponent<EntityController>());
        }

        private void Bounce(Collision2D collision)
        {
            Rigidbody2D otherRb = collision.rigidbody;
            if (otherRb == null) return;
            if (collision.gameObject.layer != LayerMask.NameToLayer(TypeListHandler.LayerTypes[Data.General.Layer]) &&
                collision.gameObject.layer != LayerMask.NameToLayer("Markers"))
            {
                CurrentSpeed = 0;
                return;
            }
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            Vector2 pushDirection = (rb.position - otherRb.position).normalized;
            float totalMass = rb.mass + otherRb.mass; // Общая масса двух объектов
            float impulse = CurrentSpeed * rb.mass * 0.1f;  // Импульс игрока

            // Передаем часть импульса другому объекту
            otherRb.AddForce(-pushDirection * (impulse * (rb.mass / totalMass)), ForceMode2D.Impulse);

            //Теряет скорость пропорционально массе другого объекта
            CurrentSpeed *= otherRb.mass / totalMass;
        }

        public void TakeDamage(float damage)
        {
            throw new System.NotImplementedException();
        }
    }
}
