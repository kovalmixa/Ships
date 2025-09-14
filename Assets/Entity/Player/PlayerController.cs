using System.Collections.Generic;
using Assets.Entity.Hull;
using Assets.Entity.Interfaces;
using UnityEngine;
namespace Assets.Entity.Player
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
            Camera[] cameras = GameObject.FindObjectsOfType<Camera>();
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
            RotateControl(controller);
            AttackControl(controller);
        }

        private void MoveControl(EntityController controller)
        {
            Hull.Hull hull = controller.Hull;
            switch (controller.Data.HullLayer)
            {
                case 0:
                {
                    {
                        if (Input.GetKeyDown(KeyCode.W) && hull.SpeedLevel < hull.MaxSpeedLevel)
                            hull.SpeedLevel++;
                        else if (Input.GetKeyDown(KeyCode.S) && hull.SpeedLevel > hull.MinSpeedLevel)
                            hull.SpeedLevel--;

                        float rotationInput = Input.GetAxis("Horizontal");
                        hull.Movement(rotationInput);
                    }
                    break;
                }
            }
        }

        private void AttackControl(EntityController controller) => KeyWordControls(controller, Camera.ScreenToWorldPoint(Input.mousePosition));

        private void RotateControl(EntityController controller) => controller.Hull.RotateEquipment(Camera.ScreenToWorldPoint(Input.mousePosition));

        public void SetMovementPoint(Transform target) { }

        public void SetTargetPoint(Transform target) { }

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
    }
}