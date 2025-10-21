using System.Linq;
using Assets.DataContainers;
using Assets.Entity.Interfaces;
using Assets.Handlers;
using Assets.Scripts.Actions;
using Assets.Scripts.Modifiers;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Entity.Equipment
{
    public class Equipment : MonoBehaviour, IActivation, IDamageable
    {
        public EntityController EntityController;
        public EquipmentContainer EquipmentContainer;
        private const float BasicAngle = 90;
        public EquipmentAnchor EquipmentAnchor { get; set; }

        public ActionBase[] Activations;
        public ActionBase[] UpdateActions;

        public IModifier[] Modifiers;

        public Vector3 Position
        {
            get => transform.position + EntityController.transform.position;
            set{}
        }

        public void Rotate(Vector3 targetPos)
        {
            if (EquipmentContainer == null || !CanRotate()) return;
            Vector3 localTarget = EquipmentAnchor.transform.InverseTransformPoint(targetPos);
            float localAngle = Mathf.Atan2(localTarget.y, localTarget.x) * Mathf.Rad2Deg;
            float min = EquipmentAnchor.RotationSector.x;
            float max = EquipmentAnchor.RotationSector.y;
            float clampedLocal = Mathf.Clamp(localAngle, min, max);
            float finalWorldAngle = EquipmentAnchor.transform.eulerAngles.z + clampedLocal;
            float rotationSpeed = EquipmentContainer.RotationSpeed * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                Quaternion.Euler(0f, 0f, finalWorldAngle - BasicAngle),
                rotationSpeed
            );
        }

        public bool CanRotate()
        {
            if (EquipmentAnchor == null) return false;
            return EquipmentAnchor.RotationSector != Vector2.zero;
        }

        public void Activate(Vector3 targetPos, ActionBase[] actions = null)
        {
            if (actions == UpdateActions)
            {
                foreach (var activation in actions) activation.Execute(gameObject, targetPos);
                return;
            }
            var distance = Vector2.Distance(transform.position, targetPos);
            var targetPosEq =
                FunctionHandler.GetAngleDistancePoint(transform.position, transform.eulerAngles.z + BasicAngle, distance);
            foreach (var activation in Activations)
            {
                if (activation.IsPassive || activation.Delay <= 0) activation.Execute(gameObject, targetPos);
                float targetWorldAngle = Mathf.Atan2(targetPos.y - transform.position.y, targetPos.x - transform.position.x) * Mathf.Rad2Deg;
                float currentAngle = Mathf.Repeat(transform.eulerAngles.z + BasicAngle, 360f);
                float angleDiff = Mathf.DeltaAngle(currentAngle, targetWorldAngle);
                if (!(Mathf.Abs(angleDiff) < 12.5f / activation.Delay)) continue;
                if (EquipmentAnchor.ActivationSectors.Length == 0) activation.Execute(gameObject, targetPosEq);
                else
                {
                    currentAngle = Mathf.Abs(Mathf.DeltaAngle(currentAngle, EquipmentAnchor.transform.eulerAngles.z));
                    if (EquipmentAnchor.ActivationSectors.Any(sector => currentAngle >= sector.x && currentAngle <= sector.y))
                    {
                        activation.Execute(gameObject, targetPosEq);
                    }
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
        }

        public void TakeDamage(float damage)
        {
            throw new System.NotImplementedException();
        }
    }
}
