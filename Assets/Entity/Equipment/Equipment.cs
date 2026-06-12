using Actions;
using Assets.Common;
using Assets.Common.ActionEffectStructs;
using Assets.Entity.Hull;
using Assets.Entity.Interfaces;
using Assets.Handlers;
using Assets.Scripts.Effects;
using Entity.Controllers.GenericController;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Entity.Equipment
{
    public class Equipment : MonoBehaviour, IActivation, IInteractive, IModified
    {
        public EntityController entityController;
        public EquipmentContainer equipmentContainer;
        private const float _basicAngle = 90;
        public EquipmentAnchor EquipmentAnchor { get; set; }

        public ActionBase[] actions;
        public ActionBase[] updateActions;

        public List<EffectComponent> Effects { get; set; }
        public bool IsDirty { get; set; }
        protected List<EffectComponent> cachedCombinedEffects = new List<EffectComponent>();

        public Vector3 Position
        {
            get => transform.position + entityController.transform.position;
            set{}
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

        public void Activate(Vector3 targetPos, ActionBase[] actions = null)
        {
            var actionContext = new ActionContext(gameObject, null);
            if (actions == updateActions)
            {
                foreach (var activation in actions) activation.Execute(actionContext, targetPos);
                return;
            }
            var distance = Vector2.Distance(transform.position, targetPos);
            var targetPosEq =
                MathFuncHandler.GetAngleDistancePoint(transform.position, transform.eulerAngles.z + _basicAngle, distance);
            foreach (var activation in this.actions)
            {
                if (activation.IsPassive || activation.Delay <= 0) activation.Execute(actionContext, targetPos);
                float targetWorldAngle = Mathf.Atan2(targetPos.y - transform.position.y, targetPos.x - transform.position.x) * Mathf.Rad2Deg;
                float currentAngle = Mathf.Repeat(transform.eulerAngles.z + _basicAngle, 360f);
                float angleDiff = Mathf.DeltaAngle(currentAngle, targetWorldAngle);
                if (!(Mathf.Abs(angleDiff) < 12.5f / activation.Delay)) continue;
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
        public void TakeDamage(ActionContext context, Damage damage)
        {
            throw new System.NotImplementedException();
        }

        public void TakeHeal(ActionContext context, Heal heal)
        {
            throw new System.NotImplementedException();
        }
        #endregion

        #region IModiefied
        public List<EffectComponent> RebuildEffects()
        {
            if (IsDirty)
            {
                cachedCombinedEffects.Clear();
                if (entityController != null && entityController.GetComponent<HullBase>() != null)
                {
                    var hullEffects = entityController.GetComponent<HullBase>().GetBuildEffects();
                    if (hullEffects != null) cachedCombinedEffects.AddRange(hullEffects);
                }
                if (Effects != null)
                    cachedCombinedEffects.AddRange(Effects.OfType<EffectComponent>());
                IsDirty = false;
            }
            return cachedCombinedEffects;
        }
       
        public List<EffectComponent> GetBuildEffects() => IsDirty ? RebuildEffects() : cachedCombinedEffects;

        #endregion
    }
}
