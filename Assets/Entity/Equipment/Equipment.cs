using Assets.Entity.DataContainers;
using Assets.Entity.Interfaces;
using Assets.Handlers;
using UnityEngine;

namespace Assets.Entity.Equipment
{
    public class Equipment : InGameObject, IActivation, IDamageable
    {
        private Activator _activator;
        public EntityBody EntityBody { get; set; }
        public EquipmentContainer EquipmentContainer;
        public EquipmentAnchor EquipmentAnchor { get; set; }

        public ActivationContainer[] Activations 
        { 
            get => EquipmentContainer.OnActivate;
            set => EquipmentContainer.OnActivate = value;
        }

        public void SetEquipment(string id)
        {

        }
        public Vector3 Position
        {
            get => transform.position + EntityBody.transform.position;
            set{}
        }
        public void Rotate(Vector3 target)
        {
            if (EquipmentContainer == null) return;
            if (!CanRotate()) return;
            Vector2 direction = target - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle - 90f);
            Quaternion rotationStep = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                EquipmentContainer.RotationSpeed * Time.deltaTime
            );
            float resultWorldAngle = FunctionHandler.NormalizeAngle(rotationStep.eulerAngles.z);
            float hullRotation = FunctionHandler.NormalizeAngle(EntityBody.transform.eulerAngles.z);
            float baseRotation = FunctionHandler.NormalizeAngle(EquipmentAnchor.transform.rotation.z);
            float resultLocalAngle = FunctionHandler.NormalizeAngle(resultWorldAngle - hullRotation - baseRotation);
            Vector2 sector = EquipmentAnchor.RotationSector;
            if (IsAngleWithinSector(resultLocalAngle, sector.x, sector.y))
            {
                transform.rotation = rotationStep;
            }

        }
        private bool IsAngleWithinSector(float angle, float min, float max) => min <= angle && angle <= max;
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
