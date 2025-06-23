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
        private Dictionary<KeyCode, Action<Transform>> keyCodeActivations = new()
        {
            //{ KeyCode.Alpha1, "������ 1" },
            //{ KeyCode.Alpha2, "������ 2" },
            //{ KeyCode.Alpha3, "������ 3" },
            //{ KeyCode.Alpha4, "������ 4" },
            //{ KeyCode.Alpha5, "������ 5" },
            //{ KeyCode.Alpha6, "������ 6" },
            //{ KeyCode.Alpha7, "������ 7" },
            //{ KeyCode.Alpha8, "������ 8" },
            //{ KeyCode.Alpha9, "������ 9" },
            //{ KeyCode.Alpha0, "������ 10" },
            //{ KeyCode.Mouse0, "������� (����� ������ ����)" },
            //{ KeyCode.Mouse1, "������ (������ ������ ����)" }
        };
        private Camera FindMainCamera()
        {
            Camera[] cameras = GameObject.FindObjectsOfType<Camera>();
            foreach (Camera cam in cameras)
            {
                if (cam.enabled && cam.gameObject.activeInHierarchy)
                    return cam;
            }
            Debug.LogWarning("������ �� �������!");
            return null;
        }
        public void UpdateControl(Entity entity)
        {
            if(!entity) return;
            MoveControl(entity);
            RotateControl(entity);
            AttackControl(entity);
        }
        private void MoveControl(Entity entity)
        {
            if (entity.Type == "Sea")
            {
                if (Input.GetKeyDown(KeyCode.W) && entity.SpeedLevel < entity.MaxSpeedLevel)
                    entity.SpeedLevel++;
                else if (Input.GetKeyDown(KeyCode.S) && entity.SpeedLevel > entity.MinSpeedLevel)
                    entity.SpeedLevel--;

                float rotationInput = Input.GetAxis("Horizontal");
                entity.Movement(rotationInput);
            }
        }
        private void AttackControl(Entity entity)
        {
            KeyWordControls();
            //Vector3 mouseWorldPos = Camera.ScreenToWorldPoint(Input.mousePosition);
            //Collider2D col = GetComponent<Collider2D>();
            //if (col != null && col.OverlapPoint(mouseWorldPos))
            //{
            //    Debug.Log("������ �� �������!");
            //}
        }
        private void RotateControl(Entity entity)
        {
            Vector3 mouseWorldPos = Camera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = mouseWorldPos - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            entity.RotateEquipment(angle);
        }
        public void SetMovementPoint(Transform target) { }
        public void SetTargetPoint(Transform target) { }
        private void KeyWordControls()
        {
            //foreach (var entry in keyCodeActivations)
            //{
            //    if (entry.Key.ToString().StartsWith("Mouse"))
            //    {
            //        int mouseButton = entry.Key == KeyCode.Mouse0 ? 0 :
            //            entry.Key == KeyCode.Mouse1 ? 1 : -1;

            //        if (mouseButton != -1 && Input.GetMouseButton(mouseButton))
            //        {
            //            Debug.Log($"[HOLD] {entry.Value}");
            //        }
            //    }
            //    else if (Input.GetKey(entry.Key))
            //    {
            //        Debug.Log($"[HOLD] {entry.Value}");
            //    }
            //}
        }
    }
}