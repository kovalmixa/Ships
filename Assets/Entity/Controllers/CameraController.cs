using Cinemachine;
using UnityEngine;

namespace Entity.Controllers
{
    public class CameraController : SingletonMonoBehaviour<CameraController>
    {
        [SerializeField] CinemachineVirtualCamera virtualCamera;
        
        float cameraDistance;
        [SerializeField] float sensitivity;
        public float MinZoom;
        public float MaxZoom;
        public float SmoothSpeed = 5f;
        float targetZoom;
        float currentZoom;

        private void Awake()
        {
            base.Awake();
            currentZoom = virtualCamera.m_Lens.OrthographicSize;
            targetZoom = currentZoom;
        }

        public void Follow(Transform targetTransform)
        {
            if (targetTransform == null)
            {
                Debug.LogError("CameraController: Попытка следовать за null трансформом!");
                return;
            }
            virtualCamera.LookAt = targetTransform;
            virtualCamera.Follow = targetTransform;
        }

        void Zoom()
        {
            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                targetZoom -= Input.GetAxis("Mouse ScrollWheel") * sensitivity;
                targetZoom = Mathf.Clamp(targetZoom, MinZoom, MaxZoom);
            }
            currentZoom = Mathf.Lerp(currentZoom, targetZoom, SmoothSpeed * Time.deltaTime);
            virtualCamera.m_Lens.OrthographicSize = currentZoom;
        }

        void Update()
        {
            Zoom();
        }

    }
}
