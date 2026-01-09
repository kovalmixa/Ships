using Cinemachine;
using UnityEngine;

namespace Entity.Controllers
{
    public class CameraController : MonoBehaviour
    {
        public static CameraController Instance { get; private set; }
        [SerializeField] CinemachineVirtualCamera _virtualCamera;
        
        float _cameraDistance;
        [SerializeField] float _sensitivity;
        public float MinZoom;
        public float MaxZoom;
        public float SmoothSpeed = 5f;
        float _targetZoom;
        float _currentZoom;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            _currentZoom = _virtualCamera.m_Lens.OrthographicSize;
            _targetZoom = _currentZoom;
        }

        public void Follow(Transform transform)
        {
            _virtualCamera.LookAt = transform;
            _virtualCamera.Follow = transform;
        }

        void Zoom()
        {
            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                _targetZoom -= Input.GetAxis("Mouse ScrollWheel") * _sensitivity;
                _targetZoom = Mathf.Clamp(_targetZoom, MinZoom, MaxZoom);
            }
            _currentZoom = Mathf.Lerp(_currentZoom, _targetZoom, SmoothSpeed * Time.deltaTime);
            _virtualCamera.m_Lens.OrthographicSize = _currentZoom;
        }

        void Update()
        {
            Zoom();
        }

    }
}
