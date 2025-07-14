using System.Linq;
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
                    ActionContext context = new ActionContext(activation, position, gameObject);
                    ActivationHandler.Execute(activation.Type, context);
                    _lastActivationTimes[i] = time;
                }
            }
        }

        private bool IsActivationWithinSector(Vector3 position, ActivationContainer activation)
        {
            if (activation.IsPassive) return true;

            Vector2 direction = (Vector2)position - (Vector2)transform.position;
            float targetLocalAngle = Vector2.SignedAngle(transform.up, direction);
            float absAngle = Mathf.Abs(targetLocalAngle);

            // Проверка на прицельность (точность зависит от задержки)
            if (absAngle >= 12.5f / activation.Delay) return false;

            // Если задержка малая, разрешаем активацию
            if (activation.Delay <= 1f) return true;

            // Проверка всех секторов способности
            bool inActivationSector = activation.FireSectors != null &&
                                      activation.FireSectors.Any(sector => FunctionHandler.IsAngleWithinSector(targetLocalAngle, sector.x, sector.y));

            // Проверка всех секторов носителя (например, HullEquipmentProperties.FireSector)
            bool inHostSector = HostFireSectors == null || HostFireSectors.Length == 0 ||
                                HostFireSectors.Any(sector => FunctionHandler.IsAngleWithinSector(targetLocalAngle, sector.x, sector.y));

            return inActivationSector && inHostSector;
        }
    }
}
