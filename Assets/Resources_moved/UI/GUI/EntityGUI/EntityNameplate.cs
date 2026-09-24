using Assets.Entity.Hull;
using Entity.Controllers;
using UnityEngine;

namespace UI.GUI.EntityGUI
{
    public class EntityNameplate : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private ProgressBar _hpBar;
        [SerializeField] private ProgressBar _energyBar;
        [SerializeField] public float standartSize = 50f;

        [HideInInspector] public Vector3 _offset;

        private EntityController _entityController;
        private HullBase _hull;

        #region Setup

        public void Setup(EntityController entityController)
        {
            if (entityController == null) return;

            if (TryGetComponent<Canvas>(out var canvas) && CameraController.Instance != null)
                canvas.worldCamera = CameraController.Instance.Camera;

            UnsubscribeFromEvents();

            _entityController = entityController;
            _hull = entityController.hull;

            SubscribeToEvents();
            InitVisuals();
        }

        private void OnEnable()
        {
            SubscribeToEvents();
            InitVisuals();
        }

        private void OnDisable() => UnsubscribeFromEvents();
        private void OnDestroy() => UnsubscribeFromEvents();

        #endregion

        #region Event Subscriptions

        private void SubscribeToEvents()
        {
            if (_hull != null)
            {
                _hull.OnMovement += UpdateNameplatePosition;
            }

            if (_entityController != null)
            {
                if (_entityController.StatModController != null)
                {
                    // Подписки на события статов
                }
            }
        }

        private void UnsubscribeFromEvents()
        {
            if (_hull != null) _hull.OnMovement -= UpdateNameplatePosition;

            if (_entityController != null)
            {
                if (_entityController.StatModController != null)
                {
                    // Отписки от событий статов
                }
            }
        }

        #endregion

        #region Visual Helpers

        private void InitVisuals()
        {
            UpdateNameplatePosition();
            UpdateHealthBar();
            UpdateEnergyBar();
        }

        private void UpdateHealthBar()
        {
            if (_entityController == null || _hpBar == null) return;
        }

        private void UpdateEnergyBar()
        {
            if (_entityController == null || _energyBar == null) return;
        }

        #endregion

        #region On Invocations & Public API

        public void UpdateNameplatePosition()
        {
            if (_hull == null) return;
            transform.position = _hull.transform.position + _offset;
        }

        public void OnHpSet(float normalizedValue)
        {
            if (_hpBar == null) return;
            _hpBar.Value = Mathf.Clamp01(normalizedValue);
        }

        public void OnEnergySet(float normalizedValue)
        {
            if (_energyBar == null) return;
            _energyBar.Value = Mathf.Clamp01(normalizedValue);
        }

        #endregion
    }
}