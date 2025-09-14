using System.Linq;
using Assets.DataContainers;
using Assets.Entity.Interfaces;
using Assets.Scripts.Actions;
using Assets.Scripts.Modifiers;
using UnityEngine;

namespace Assets.Entity.Equipment
{
    public class Equipment : MonoBehaviour, IActivation, IDamageable
    {
        private Activator _activator;
        public EntityController EntityController;
        public EquipmentContainer EquipmentContainer;
        public EquipmentAnchor EquipmentAnchor { get; set; }

        public IGameAction[] Activations;

        public IModifier[] Modifiers;

        public Vector3 Position
        {
            get => transform.position + EntityController.transform.position;
            set{}
        }

        private void Start()
        {
            _activator = gameObject.AddComponent<Activator>();
            _activator.SetActivations(Activations);
        }

        public void Rotate(Vector3 target)
        {
            if (EquipmentContainer == null) return;
            if (!CanRotate()) return;
            float targetWorldAngle = Mathf.Atan2(target.y - transform.position.y, target.x - transform.position.x) * Mathf.Rad2Deg;
            float baseAngle = EntityController.transform.eulerAngles.z + EquipmentAnchor.transform.localEulerAngles.z;
            float localTargetAngle = Mathf.DeltaAngle(baseAngle, targetWorldAngle);
            float min = EquipmentAnchor.RotationSector.x;
            float max = EquipmentAnchor.RotationSector.y;
            float clampedLocal = Mathf.Clamp(localTargetAngle, min, max);
            float finalWorldAngle = baseAngle + clampedLocal;
            float rotationSpeed = EquipmentContainer.RotationSpeed * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                Quaternion.Euler(0f, 0f, finalWorldAngle - 90f), 
                rotationSpeed
            );
        }

        public bool CanRotate()
        {
            if (EquipmentAnchor == null) return false;
            return EquipmentAnchor.RotationSector != Vector2.zero;
        }

        public void Activate(Vector3 targetPosition) => _activator.TryActivate(targetPosition);

        private void OnCollisionEnter2D(Collision2D collision)
        {
        }

        public void TakeDamage(float damage)
        {
            throw new System.NotImplementedException();
        }
    }
}
