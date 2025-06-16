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
    }
}