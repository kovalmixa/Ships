using Assets.Handlers.SceneHandlers;
using Cinemachine;
using UnityEngine;

namespace Entity.Controllers
{
    public static class CameraExtensions
    {
        public static Vector3 ScreenToToWorldDelta(this Camera camera, Vector3 startScreenPos, Vector3 endScreenPos)
        {
            if (camera == null) return Vector3.zero;

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

        public Vector2 CursorPosition => _camera != null
            ? _camera.ScreenToWorldPoint(Input.mousePosition)
            : Vector2.zero;

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
        public float returnSpeed = 10f;

        private Vector3 _lastMousePosition;
        private Vector3 _panVelocity;

        private bool _isDragging;
        private bool _isMovingToHost;
        private bool _isFollowing = false;

        private float _distancePassed = 0f;
        private readonly float _clickThreshold = 0.05f;

        private Transform _panningTarget;
        private Transform _followTransform;

        protected override void Awake()
        {
            base.Awake();
            _camera = GetComponent<Camera>();

            if (GameObjectHandler.playerController != null)
                _followTransform = GameObjectHandler.playerController.transform;

            if (_camera == null) _camera = Camera.main;

            _virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();

            if (_virtualCamera != null) _currentZoom = _virtualCamera.m_Lens.OrthographicSize;
            else _currentZoom = _camera.orthographicSize;

            _targetZoom = _currentZoom;

            GameObject targetObj = new GameObject("Camera_PanTarget");
            _panningTarget = targetObj.transform;
            _panningTarget.position = transform.position;

            if (_virtualCamera != null && _virtualCamera.Follow == null)
                _virtualCamera.Follow = _panningTarget;
        }

        private void Update()
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
            {
                _currentZoom = Mathf.Lerp(_currentZoom, _targetZoom, zoomSpeed * Time.deltaTime);

                if (_virtualCamera != null) _virtualCamera.m_Lens.OrthographicSize = _currentZoom;
                else if (_camera != null) _camera.orthographicSize = _currentZoom;
            }
            else
            {
                _currentZoom = _targetZoom;
                if (_virtualCamera != null) _virtualCamera.m_Lens.OrthographicSize = _currentZoom;
            }
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

            _followTransform = targetTransform;
            _isFollowing = false;
            _isMovingToHost = true;
        }

        public void ManualMove(bool isButtonDownFirstFrame, bool isButtonDown)
        {
            if (isButtonDownFirstFrame)
            {
                _lastMousePosition = Input.mousePosition;
                _distancePassed = 0f;
                _panVelocity = Vector3.zero;

                _isDragging = true;
                _isFollowing = false;
                _isMovingToHost = false;
            }

            if (isButtonDown)
            {
                Vector3 currentInputPosition = Input.mousePosition;
                Vector3 moveDelta = _camera.ScreenToToWorldDelta(_lastMousePosition, currentInputPosition);

                if (_panningTarget != null)
                    _panningTarget.position -= moveDelta * panSpeed;

                if (Time.deltaTime > 0)
                    _panVelocity = -(moveDelta * panSpeed) / Time.deltaTime;

                _distancePassed += moveDelta.sqrMagnitude;
                _lastMousePosition = currentInputPosition;
            }
            else if (_isDragging)
            {
                _isDragging = false;

                if (_distancePassed < _clickThreshold * _currentZoom)
                {
                    _panVelocity = Vector3.zero;
                    _isMovingToHost = true;
                }
            }
        }

        private void MoveUpdate()
        {
            if (_panningTarget == null) return;

            if (_isFollowing && _followTransform != null)
            {
                _panningTarget.position = _followTransform.position;
                return;
            }

            if (_isMovingToHost && _followTransform != null)
            {
                _panningTarget.position = Vector3.Lerp(_panningTarget.position, _followTransform.position, returnSpeed * Time.deltaTime);

                if (Vector3.Distance(_panningTarget.position, _followTransform.position) < 0.1f)
                {
                    _panningTarget.position = _followTransform.position;
                    _isMovingToHost = false;
                    _isFollowing = true;
                }
                return;
            }

            if (!_isDragging && _panVelocity.sqrMagnitude > 0.0001f)
            {
                _panningTarget.position += _panVelocity * Time.deltaTime;
                _panVelocity = Vector3.Lerp(_panVelocity, Vector3.zero, inertiaDamping * Time.deltaTime);
            }
        }

        #endregion
    }
}