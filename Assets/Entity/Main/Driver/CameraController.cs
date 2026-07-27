using Cinemachine;
using UnityEngine;

namespace Entity.Controllers
{
    public static class CameraExtensions
    {
        public static Vector3 ScreenToWorldDelta(this Camera camera, Vector3 startScreenPos, Vector3 endScreenPos)
        {
            if (camera == null) return Vector3.zero;

            float depth = Mathf.Abs(camera.transform.position.z);
            startScreenPos.z = depth;
            endScreenPos.z = depth;

            Vector3 startWorld = camera.ScreenToWorldPoint(startScreenPos);
            Vector3 endWorld = camera.ScreenToWorldPoint(endScreenPos);

            Vector3 delta = endWorld - startWorld;
            delta.z = 0;
            return delta;
        }
    }

    public class CameraController : SingletonMonoBehaviour<CameraController>
    {
        private Camera _camera;
        private CinemachineVirtualCamera _virtualCamera;

        public Vector2 CursorPosition
        {
            get
            {
                if (_camera == null) return Vector2.zero;
                Vector3 mousePos = Input.mousePosition;
                mousePos.z = Mathf.Abs(_camera.transform.position.z);
                return _camera.ScreenToWorldPoint(mousePos);
            }
        }

        [Header("Zoom Settings")]
        public float minZoom = 2f;
        public float maxZoom = 15f;
        public float zoomSpeed = 8f;
        [SerializeField] private float _sensitivity = 2f;

        private float _targetZoom;
        private float _currentZoom;

        [Header("Pan & Inertia Settings")]
        public float panSpeed = 1f;
        public float inertiaDamping = 5f;

        private Vector3 _lastInputPosition;
        private Vector3 _panVelocity;
        private Vector3 _panOffset;

        private bool _isDragging;
        private float _distancePassed = 0f;
        private readonly float _clickThreshold = 0.05f;

        private Transform _panningTarget;
        public Transform _followTransform;

        protected override void Awake()
        {
            base.Awake();
            _camera = GetComponent<Camera>();
            if (_camera == null) _camera = Camera.main;

            _virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();

            if (_virtualCamera != null) _currentZoom = _virtualCamera.m_Lens.OrthographicSize;
            else if (_camera != null) _currentZoom = _camera.orthographicSize;

            _targetZoom = _currentZoom;

            GameObject targetObj = new GameObject("CameraPanTarget");
            targetObj.transform.parent = transform.parent;
            _panningTarget = targetObj.transform;

            Vector3 startPos = _followTransform != null ? _followTransform.position : transform.position;
            startPos.z = 0;
            _panningTarget.position = startPos;

            if (_virtualCamera != null) _virtualCamera.Follow = _panningTarget;
        }

        private void LateUpdate()
        {
            ZoomUpdate();
            MoveUpdate();
        }

        #region Zoom

        public void ZoomTo(float direction)
        {
            _targetZoom -= direction * _sensitivity;
            _targetZoom = Mathf.Clamp(_targetZoom, minZoom, maxZoom);
        }

        public void TargetZoom(float scale) => _targetZoom = Mathf.Clamp(scale, minZoom, maxZoom);

        private void ZoomUpdate()
        {
            if (Mathf.Abs(_currentZoom - _targetZoom) > 0.01f)
                _currentZoom = Mathf.Lerp(_currentZoom, _targetZoom, zoomSpeed * Time.deltaTime);
            else _currentZoom = _targetZoom;

            if (_virtualCamera != null) _virtualCamera.m_Lens.OrthographicSize = _currentZoom;
            else if (_camera != null) _camera.orthographicSize = _currentZoom;
        }

        #endregion

        #region Move & Inertia

        public void Follow(Transform targetTransform)
        {
            if (targetTransform == null)
            {
                Debug.LogError("CameraController: Attempting to follow null transform!");
                return;
            }
            _panOffset = Vector3.zero;
            _panVelocity = Vector3.zero;
            _followTransform = targetTransform;
        }

        public void ManualMove(bool isButtonDownFirstFrame, bool isButtonDown)
        {
            if (isButtonDownFirstFrame)
            {
                _lastInputPosition = Input.mousePosition;
                _distancePassed = 0f;
                _panVelocity = Vector3.zero;
                _isDragging = true;
            }

            if (isButtonDown)
            {
                Vector3 currentInputPosition = Input.mousePosition;
                Vector3 moveDelta = _camera.ScreenToWorldDelta(_lastInputPosition, currentInputPosition);

                Vector3 deltaOffset = moveDelta * panSpeed;
                _panOffset -= deltaOffset;

                if (Time.deltaTime > 0) _panVelocity = -deltaOffset / Time.deltaTime;

                _distancePassed += moveDelta.sqrMagnitude;
                _lastInputPosition = currentInputPosition;
            }
            else if (_isDragging)
            {
                _isDragging = false;
                if (_distancePassed < _clickThreshold * _currentZoom) _panOffset = _panVelocity = Vector3.zero;
            }
        }

        private void MoveUpdate()
        {
            if (_panningTarget == null) return;
            if (!_isDragging && _panVelocity.sqrMagnitude > 0.0001f)
            {
                _panOffset += _panVelocity * Time.deltaTime;
                _panVelocity = Vector3.Lerp(_panVelocity, Vector3.zero, inertiaDamping * Time.deltaTime);
            }
            Vector3 basePosition = _followTransform != null ? _followTransform.position : Vector3.zero;
            basePosition.z = 0;

            _panningTarget.position = basePosition + _panOffset;
        }

        #endregion
    }
}