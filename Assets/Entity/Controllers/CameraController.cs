using Cinemachine;
using UnityEngine;

namespace Entity.Controllers
{
    public class CameraController : SingletonMonoBehaviour<CameraController>
    {
        [SerializeField] private CinemachineVirtualCamera _virtualCamera;

        public float CameraDistance { get; set; }
        [SerializeField] float _sensitivity;
        public float minZoom;
        public float maxZoom;
        public float SmoothSpeed { get; set; } = 5f;
        float _targetZoom;
        float _currentZoom;

        protected override void Awake()
        {
            base.Awake();
            _currentZoom = _virtualCamera.m_Lens.OrthographicSize;
            _targetZoom = _currentZoom;
        }

        public void Follow(Transform targetTransform)
        {
            if (targetTransform == null)
            {
                Debug.LogError("CameraController: Попытка следовать за null трансформом!");
                return;
            }
            _virtualCamera.LookAt = targetTransform;
            _virtualCamera.Follow = targetTransform;
        }

        void Zoom()
        {
            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                _targetZoom -= Input.GetAxis("Mouse ScrollWheel") * _sensitivity;
                _targetZoom = Mathf.Clamp(_targetZoom, minZoom, maxZoom);
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
