using Actions;
using Assets.Common;
using Assets.Entity.Controllers;
using Assets.Entity.Interfaces;
using Assets.Handlers;
using Assets.Scripts.Actions;
using Entity.Controllers;
using System.Linq;
using UnityEngine;

namespace Assets.Entity.Equipment
{
    public class Equipment : MonoBehaviour, IActivation, IInteractive
    {
        public EntityController entityController;
        public BuffStatController buffStatController;
        public EquipmentContainer equipmentContainer;
        public EquipmentAnchor EquipmentAnchor { get; set; }
        public TemplateActionBase[] actions;
        private const float _basicAngle = 90;
        public Vector3 Position
        {
            get => transform.position + entityController.transform.position;
            set { }
        }

        public void Rotate(Vector3 targetPos)
        {
            if (equipmentContainer == null || !CanRotate()) return;
            Vector3 localTarget = EquipmentAnchor.transform.InverseTransformPoint(targetPos);
            float localAngle = Mathf.Atan2(localTarget.y, localTarget.x) * Mathf.Rad2Deg;
            float min = EquipmentAnchor.rotationSector.x;
            float max = EquipmentAnchor.rotationSector.y;
            float clampedLocal = Mathf.Clamp(localAngle, min, max);
            float finalWorldAngle = EquipmentAnchor.transform.eulerAngles.z + clampedLocal;
            float rotationSpeed = equipmentContainer.rotationSpeed * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                Quaternion.Euler(0f, 0f, finalWorldAngle - _basicAngle),
                rotationSpeed
            );
        }

        public bool CanRotate()
        {
            if (EquipmentAnchor == null) return false;
            return EquipmentAnchor.rotationSector != Vector2.zero;
        }

        public void Activate(Vector3 targetPos, TemplateActionBase[] actions = null)
        {
            var actionContext = new EntitySnapshot(entityController, entityController.data, buffStatController.BuffStatuses.ToArray());
            var distance = Vector2.Distance(transform.position, targetPos);
            var targetPosEq = MathFuncHandler.GetAngleDistancePoint(transform.position, transform.eulerAngles.z + _basicAngle, distance);
            foreach (var activation in this.actions)
            {
                if (activation.IsPassive || activation.delay <= 0) activation.Execute(actionContext, targetPos);
                float targetWorldAngle = Mathf.Atan2(targetPos.y - transform.position.y, targetPos.x - transform.position.x) * Mathf.Rad2Deg;
                float currentAngle = Mathf.Repeat(transform.eulerAngles.z + _basicAngle, 360f);
                float angleDiff = Mathf.DeltaAngle(currentAngle, targetWorldAngle);
                if (!(Mathf.Abs(angleDiff) < 12.5f / activation.delay)) continue;
                if (EquipmentAnchor.activationSectors.Length == 0) activation.Execute(actionContext, targetPosEq);
                else
                {
                    currentAngle = Mathf.Abs(Mathf.DeltaAngle(currentAngle, EquipmentAnchor.transform.eulerAngles.z));
                    if (EquipmentAnchor.activationSectors.Any(sector => currentAngle >= sector.x && currentAngle <= sector.y))
                    {
                        activation.Execute(actionContext, targetPosEq);
                    }
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
        }

        #region IInteractive
        public void TakeDamage(EntitySnapshot entitySnapshot, Damage damage)
        {
            throw new System.NotImplementedException();
        }

        public void TakeHeal(EntitySnapshot entitySnapshot, Heal heal)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}