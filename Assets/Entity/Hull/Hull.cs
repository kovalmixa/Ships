using System;
using System.Collections.Generic;
using Assets.DataContainers;
using Assets.Entity.Equipment;
using Assets.Entity.Interfaces;
using Assets.Handlers;
using Assets.Scripts.Scripts;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Entity.Hull
{
    public class Hull : MonoBehaviour, IDamageable
    {
        public int Type
        {
            get { return 0; }
            set { }
        }

        [SerializeField] public HullContainer Data;

        public List<EquipmentAnchor> EquipmentAnchors;

        public List<Equipment.Equipment> Equipments;

        private Transform _root;

        private float _targetSpeed;

        public float CurrentSpeed;

        public float SpeedLevel { get; set; }

        public int MaxSpeedLevel { get; set; } = 3;

        public int MinSpeedLevel { get; set; } = -1;

        private void Awake()
        {
            _root = transform.parent;
            CollectAnchors(transform);
        }

        public void RotateEquipment(Vector3 target)
        {
            foreach (var eq in Equipments)
            {
                eq.GetComponent<Equipment.Equipment>().Rotate(target);
            }
        }

        public void Movement(float rotationDirection)
        {
            
            switch (Type)
            {
                case 0:
                {
                    _targetSpeed = SpeedLevel * (Data.MaxSpeed / MaxSpeedLevel);
                    CurrentSpeed = MathF.Min(Mathf.MoveTowards(CurrentSpeed, _targetSpeed, Data.Acceleration * Time.deltaTime), Data.MaxSpeed);
                    _root.Rotate(Vector3.forward, -rotationDirection * Data.RotationSpeed * Time.deltaTime);
                    _root.Translate(Vector3.up * CurrentSpeed * Time.deltaTime, Space.Self);
                    break;
                }
            }
        }

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
            script?.Execute(GetComponent<EntityController>());
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
            float impulse = CurrentSpeed * rb.mass;  // Импульс игрока

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
