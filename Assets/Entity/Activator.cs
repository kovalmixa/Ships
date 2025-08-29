using System;
using System.Linq;
using System.Transactions;
using Assets.Entity.DataContainers;
using Assets.Handlers;
using Assets.InGameMarkers.Actions;
using UnityEngine;

namespace Assets.Entity
{
    public class Activator : MonoBehaviour
    {
        public ActivationContainer[] Activations;
        private float[] _lastActivationTimes;
        public Vector2[] HostFireSectors { get; set; }

        public void SetActivations(ActivationContainer[] activations)
        {
            Activations = activations;
            if (activations != null)
            {
                _lastActivationTimes = new float[activations.Length];
                for (int i = 0; i < _lastActivationTimes.Length; i++)
                    _lastActivationTimes[i] = -Mathf.Infinity;
            }
        }

        public void TryActivate(Vector3 position, string type = null)
        {
            if (Activations == null) return;

            float time = Time.time;
            Vector2 myPosition = transform.position;

            for (int i = 0; i < Activations.Length; i++)
            {
                ActivationContainer activation = Activations[i];
                if (type != null && activation.Type != type) continue;
                float distance = Vector2.Distance(myPosition, position);
                if (!activation.CanActivate(distance)) continue;
                if (!IsActivationWithinSector(position, activation)) continue;
                if (time - _lastActivationTimes[i] >= activation.Delay)
                {
                    Vector3 angularPosition = position;
                    if (TryGetComponent(out Equipment.Equipment equipment))
                        if (!activation.IsPassive) 
                            angularPosition = GetAngularPosition(position, transform.rotation.eulerAngles.z - 90);
                    angularPosition.x += activation.Position.x;
                    ActionContext context = new ActionContext(activation, angularPosition, gameObject);
                    ActivationHandler.Execute(activation.Type, context);
                    _lastActivationTimes[i] = time;
                }
            }
        }

        private Vector3 GetAngularPosition(Vector3 position, float eulerAnglesZ)
        {
            float radians = eulerAnglesZ * Mathf.Deg2Rad + MathF.PI;
            float distance = Vector2.Distance(position, transform.position);
            float x = transform.position.x + distance * MathF.Cos(radians);
            float y = transform.position.y + distance * MathF.Sin(radians);
            return new Vector3(x, y, 0);
        }
        private bool IsActivationWithinSector(Vector3 position, ActivationContainer activation)
        {
            if (activation.IsPassive) return true;
            Vector2 direction = (Vector2)position - (Vector2)transform.position;
            float targetLocalAngle = FunctionHandler.NormalizeAngle(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90); ;
            float currentAngle = FunctionHandler.NormalizeAngle(transform.rotation.eulerAngles.z);
            float hullRotation = 0;
            if (TryGetComponent(out Equipment.Equipment equipment))
                hullRotation = FunctionHandler.NormalizeAngle(equipment.EntityBody.transform.rotation.eulerAngles.z);
            if (Mathf.Abs(targetLocalAngle - currentAngle) % 180 >= 12.5f / activation.Delay) return false;

            bool inActivationSector = activation.FireSectors == null || activation.FireSectors.Length == 0 ||
                                      activation.FireSectors.Any(sector => FunctionHandler.IsAngleWithinSector(targetLocalAngle, sector.x + currentAngle, sector.y + currentAngle));
            
            bool inHostSector = HostFireSectors == null || HostFireSectors.Length == 0 ||
                                HostFireSectors.Any(sector => FunctionHandler.IsAngleWithinSector(targetLocalAngle, sector.x + hullRotation, sector.y + hullRotation));

            return inActivationSector && inHostSector;
        }
    }
}
