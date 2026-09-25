using Assets.Entity.Hull;
using Assets.Entity.Modifiers;
using Entity.Controllers;
using System.Collections;
using TMPro;
using UnityEngine;

namespace UI.GUI.EntityGUI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class EntityNameplate : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private ProgressBar _hpBar;
        [SerializeField] private ProgressBar _energyBar;
        [SerializeField] private TextMeshProUGUI _nameLabel;
        [SerializeField] public float standartSize = 50f;

        [Header("Fade Settings")]
        [Tooltip("Задержка перед началом затухания (в секундах)")]
        [SerializeField] private float _fadeDelay = 3f;
        [Tooltip("Длительность самого процесса затухания (в секундах)")]
        [SerializeField] private float _fadeDuration = 0.5f;

        [HideInInspector] public Vector3 _offset;

        private CanvasGroup _canvasGroup;
        private EntityController _entityController;
        private HullBase _hull;
        private Coroutine _fadeCoroutine;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        #region Setup

        public void Setup(EntityController entityController)
        {
            if (entityController == null) return;
            if (TryGetComponent<Canvas>(out var canvas) && CameraController.Instance != null)
                canvas.worldCamera = CameraController.Instance.Camera;

            UnsubscribeFromEvents();

            _entityController = entityController;
            _hull = entityController.Hull;
            SetupNameLabel(entityController);
            SetupProgressBars(entityController);

            SubscribeToEvents();
            InitVisuals();
        }

        private void SetupNameLabel(EntityController entityController)
        {
            var data = entityController.data;
            string name = data.name;
            if (string.IsNullOrEmpty(name)) _nameLabel.gameObject.SetActive(false);
            else
            {
                string hexColor = ColorUtility.ToHtmlStringRGB(Color.gray);
                _nameLabel.text = $"<color=#{hexColor}>{data}</color> {name}";
            }
        }

        private void SetupProgressBars(EntityController entityController)
        {
            //OnHpSet(entityController.GetLifetimeStat(StatType.Hp));
            //OnEnergySet(entityController.GetLifetimeStat(StatType.Energy));
        }

        private void OnEnable()
        {
            SubscribeToEvents();
            InitVisuals();
        }

        private void OnDisable()
        {
            UnsubscribeFromEvents();
            StopFadeCoroutine();
        }

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
            ResetFadeTimer();
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

            ResetFadeTimer();
        }

        public void OnEnergySet(float normalizedValue)
        {
            if (_energyBar == null) return;
            _energyBar.Value = Mathf.Clamp01(normalizedValue);

            ResetFadeTimer();
        }

        #endregion

        #region Fade Logic

        public void Show() => _canvasGroup.alpha = 1f;

        public void Hide() => ResetFadeTimer();

        public void ResetFadeTimer()
        {
            StopFadeCoroutine();
            if (_canvasGroup != null) _canvasGroup.alpha = 1f;
            if (gameObject.activeInHierarchy) _fadeCoroutine = StartCoroutine(FadeRoutine());
        }

        private void StopFadeCoroutine()
        {
            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
                _fadeCoroutine = null;
            }
        }

        private IEnumerator FadeRoutine()
        {
            yield return new WaitForSeconds(_fadeDelay);
            float startAlpha = _canvasGroup.alpha;
            float time = 0f;

            while (time < _fadeDuration)
            {
                time += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, time / _fadeDuration);
                yield return null;
            }

            _canvasGroup.alpha = 0f;
            _fadeCoroutine = null;
        }

        #endregion
    }
}