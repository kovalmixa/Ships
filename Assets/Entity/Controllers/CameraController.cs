using Cinemachine;
using UnityEngine;

namespace Entity.Controllers
{
    public class CameraController : MonoBehaviour
    {
        public static CameraController Instance { get; private set; }
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
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            currentZoom = virtualCamera.m_Lens.OrthographicSize;
            targetZoom = currentZoom;
        }

        public void Follow(Transform transform)
        {
            virtualCamera.LookAt = transform;
            virtualCamera.Follow = transform;
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
