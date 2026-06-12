using Cinemachine;
using UnityEngine;

namespace Entity.Controllers
{
    public class CameraController : SingletonMonoBehaviour<CameraController>
    {
        [SerializeField] CinemachineVirtualCamera virtualCamera;

        public float CameraDistance { get; set; }
        [SerializeField] float _sensitivity;
        public float minZoom;
        public float maxZoom;
        public float SmoothSpeed { get; set; } = 5f;
        float _targetZoom;
        float _currentZoom;

        private void Awake()
        {
            base.Awake();
            _currentZoom = virtualCamera.m_Lens.OrthographicSize;
            _targetZoom = _currentZoom;
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
                _targetZoom -= Input.GetAxis("Mouse ScrollWheel") * _sensitivity;
                _targetZoom = Mathf.Clamp(_targetZoom, minZoom, maxZoom);
            }
            _currentZoom = Mathf.Lerp(_currentZoom, _targetZoom, SmoothSpeed * Time.deltaTime);
            virtualCamera.m_Lens.OrthographicSize = _currentZoom;
        }

        void Update()
        {
            Zoom();
        }

    }
}
