using Assets.Handlers.Enums;
using System.Collections.Generic;
using UnityEngine;

namespace Entity.Controllers
{
    public class PlayerController : MonoBehaviour, IDriver
    {
        private Camera _camera;
        public Camera Camera
        {
            get
            {
                if (_camera != null) return _camera;
                _camera = FindMainCamera();
                return _camera;
            }
            set => _camera = value;
        }

        private readonly Dictionary<KeyCode, AbilityType> _keyCodeActivations = new()
        {
            { KeyCode.Mouse0, AbilityType.FirePrimary },
            { KeyCode.Mouse1, AbilityType.FireSecondary },
        };

        private void Awake()
        {
            for (int i = 0; i <= 9; i++)
            {
                KeyCode key = (KeyCode)((int)KeyCode.Alpha0 + i);
                _keyCodeActivations[key] = i == 1 ? AbilityType.Heal : AbilityType.None;
            }
        }

        private Camera FindMainCamera()
        {
            Camera[] cameras = FindObjectsOfType<Camera>();
            foreach (Camera cam in cameras)
            {
                if (cam.enabled && cam.gameObject.activeInHierarchy)
                    return cam;
            }
            Debug.LogWarning("No active camera found.");
            return null;
        }

        public void UpdateControl(EntityController controller)
        {
            if (!controller) return;
            MoveControl(controller);
            Vector3 worldPos = Camera.ScreenToWorldPoint(Input.mousePosition);
            controller.hull.RotateEquipment(worldPos);
            KeyWordControls(controller, worldPos);
        }

        private void MoveControl(EntityController controller)
        {
            Assets.Entity.Hull.HullBase hullBase = controller.hull;
            if (Input.GetKeyDown(KeyCode.W))
                hullBase.AddSpeed(true);
            else if (Input.GetKeyDown(KeyCode.S))
                hullBase.AddSpeed(false);
            float rotationInput = Input.GetAxis("Horizontal");
            hullBase.Movement(-rotationInput);
        }

        private void KeyWordControls(EntityController controller, Vector3 position)
        {
            foreach (var entry in _keyCodeActivations)
            {
                if (entry.Value == AbilityType.None) continue;

                bool held = entry.Key == KeyCode.Mouse0 ? Input.GetMouseButton(0)
                    : entry.Key == KeyCode.Mouse1 ? Input.GetMouseButton(1)
                    : Input.GetKey(entry.Key);

                if (!held) continue;

                controller.abbilitiesController.Invoke(position, entry.Value);
            }
        }
    }
}
