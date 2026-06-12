using System.Collections.Generic;
using Entity.Controllers.GenericController;
using UnityEngine;

namespace Entity.Controllers
{
    public class PlayerController : MonoBehaviour, IEntityController
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

        private Dictionary<KeyCode, string> keyCodeActivations = new()
        {
            { KeyCode.Mouse0, "turret" },
            { KeyCode.Mouse1, "" }
        };

        private void Awake()
        {
            for (int i = 0; i <= 9; i++)
            {
                KeyCode key = (KeyCode)((int)KeyCode.Alpha0 + i);
                if (i == 1) keyCodeActivations.Add(key, "heal");
                else keyCodeActivations.Add(key, "");
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
            Debug.LogWarning("Камера не найдена!");
            return null;
        }

        public void UpdateControl(EntityController controller)
        {
            if(!controller) return;
            MoveControl(controller);
            controller.hull.RotateEquipment(Camera.ScreenToWorldPoint(Input.mousePosition));
            KeyWordControls(controller, Camera.ScreenToWorldPoint(Input.mousePosition));
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

            foreach (var entry in keyCodeActivations)
            {
                if (entry.Key.ToString().StartsWith("Mouse"))
                {
                    int mouseButton = entry.Key == KeyCode.Mouse0 ? 0 :
                        entry.Key == KeyCode.Mouse1 ? 1 : -1;
                    if (mouseButton != -1 && Input.GetMouseButton(mouseButton))
                    {
                        if (ActionIsForbidden(position, entry.Value)) return;
                        controller.ActivateCommand(position, entry.Value);
                    }
                }
                else if (Input.GetKey(entry.Key))
                {
                    controller.ActivateCommand(position, entry.Value);
                }
            }
        }

        private bool ActionIsForbidden(Vector3 position, string type)
        {
            //инвентарь панельки управления и тд. проверять пассивность/активнотсь абилки ActivationHandler.IsPassive(type);
            return false;
        }

        #region Bot redundant logic

        public void SetMovementPoint(Transform target) { }

        public void SetTargetPoint(Transform target) { }

        #endregion
    }
}