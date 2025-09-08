using System;
using Assets.DataContainers;
using Assets.Entity.Interfaces;
using Assets.Handlers;
using UnityEngine;

namespace Assets.Entity.Equipment
{
    public class Equipment : MonoBehaviour, IActivation, IDamageable
    {
        private Activator _activator;
        public EntityController EntityController;
        public EquipmentContainer EquipmentContainer;
        public EquipmentAnchor EquipmentAnchor { get; set; }

        public ActivationContainer[] Activations 
        { 
            get => EquipmentContainer.OnActivate;
            set => EquipmentContainer.OnActivate = value;
        }
        public Vector3 Position
        {
            get => transform.position + EntityController.transform.position;
            set{}
        }

        private void Awake()
        {
            _activator = gameObject.AddComponent<Activator>();
        }
        public void Rotate(Vector3 target)
        {
            if (EquipmentContainer == null) return;
            if (!CanRotate()) return;

            // угол к цели в мировых координатах
            Vector2 direction = target - transform.position;
            float targetWorldAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // угол якоря относительно корпуса
            float hullAngle = EntityController.transform.eulerAngles.z;
            float anchorAngle = EquipmentAnchor.transform.eulerAngles.z;

            // локальный угол турели относительно якоря
            float localAngle = Mathf.DeltaAngle(0f, targetWorldAngle - hullAngle - anchorAngle);

            float min = EquipmentAnchor.RotationSector.x;
            float max = EquipmentAnchor.RotationSector.y;
            float clampedLocal = Mathf.Clamp(localAngle, min, max);
            float finalWorldAngle = clampedLocal + anchorAngle + hullAngle;
            float rotationSpeed = EquipmentContainer.RotationSpeed * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation,
                Quaternion.Euler(0f, 0f, finalWorldAngle),
                rotationSpeed);
            if (EquipmentAnchor.Index == 1)
            Debug.Log($"{clampedLocal},{finalWorldAngle}");
        }

        public bool CanRotate()
        {
            if (EquipmentAnchor == null) return false;
            return EquipmentAnchor.RotationSector != Vector2.zero;
        }
        public void Activate(Vector3 targetPosition, string type = null) =>_activator.TryActivate(targetPosition, type);
        private void OnCollisionEnter2D(Collision2D collision)
        {
        }

        public void TakeDamage(float damage)
        {
            throw new System.NotImplementedException();
        }
    }
}
