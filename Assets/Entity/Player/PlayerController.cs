using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Entity.Player
{
    public class PlayerController : MonoBehaviour, IEntityController
    {
        private Camera camera;
        public Camera Camera
        {
            get
            {
                if (camera != null) return camera;
                camera = FindMainCamera();
                return camera;
            }
            set => camera = value;
        }
        private Dictionary<KeyCode, string> keyCodeActivations = new()
        {
            { KeyCode.Mouse0, "Attack" },
            { KeyCode.Mouse1, "" }
        };

        public void SetupKeyCodeDictionary()
        {
            for (int i = 0; i <= 9; i++)
            {
                KeyCode key = (KeyCode)((int)KeyCode.Alpha0 + i);
                keyCodeActivations.Add(key, "");
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
        public void UpdateControl(EntityBody entityBody)
        {
            if(!entityBody) return;
            MoveControl(entityBody);
            RotateControl(entityBody);
            AttackControl(entityBody);
        }
        private void MoveControl(EntityBody entityBody)
        {
            if (entityBody.Type == "Sea")
            {
                if (Input.GetKeyDown(KeyCode.W) && entityBody.SpeedLevel < entityBody.MaxSpeedLevel)
                    entityBody.SpeedLevel++;
                else if (Input.GetKeyDown(KeyCode.S) && entityBody.SpeedLevel > entityBody.MinSpeedLevel)
                    entityBody.SpeedLevel--;

                float rotationInput = Input.GetAxis("Horizontal");
                entityBody.Movement(rotationInput);
            }
        }
        private void AttackControl(EntityBody entityBody)
        {
            Vector3 mouseWorldPos = Camera.ScreenToWorldPoint(Input.mousePosition);
            KeyWordControls(entityBody, mouseWorldPos);
        }
        private void RotateControl(EntityBody entityBody)
        {
            Vector3 mouseWorldPos = Camera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = mouseWorldPos - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            entityBody.RotateEquipment(angle);
        }
        public void SetMovementPoint(Transform target) { }
        public void SetTargetPoint(Transform target) { }
        private void KeyWordControls(EntityBody entityBody, Vector3 position)
        {

            foreach (var entry in keyCodeActivations)
            {
                if (entry.Key.ToString().StartsWith("Mouse"))
                {
                    int mouseButton = entry.Key == KeyCode.Mouse0 ? 0 :
                        entry.Key == KeyCode.Mouse1 ? 1 : -1;
                    if (mouseButton != -1 && Input.GetMouseButton(mouseButton))
                    {
                        if (ActionIsForbidden(entityBody, position, entry.Value)) return;
                        entityBody.ActivateCommand(position, entry.Value);
                    }
                }
                else if (Input.GetKey(entry.Key))
                {
                    entityBody.ActivateCommand(position, entry.Value);
                }
            }
        }

        private bool ActionIsForbidden(EntityBody entityBody, Vector3 position, string type)
        {
            //инвентарь панельки управления и тд. проверять пассивность/активнотсь абилки ActionHandler.IsPassive(type);
            return false;
        }
    }
}