using Assets.Scripts.Actions;
using Assets.Scripts.Actions.Projectile;
using GameplayActions;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Assets.Scripts.GameplayActions.FireProjectile
{
    public class CommonProjectile : ProjectileInstance
    {
        [SerializeField] private TrailRenderer _flameTrail;
        [SerializeField] private TrailRenderer _residualTrail;
        private Light2D _light;

        private float _defaultLightIntensity;

        private Color _defaultFlameStartColor, _defaultFlameEndColor;
        private Color _defaultResidualStartColor, _defaultResidualEndColor;

        private void Awake()
        {
            _light = GetComponent<Light2D>();
            if (_light != null) _defaultLightIntensity = _light.intensity;

            if (_flameTrail != null)
            {
                _defaultFlameStartColor = _flameTrail.startColor;
                _defaultFlameEndColor = _flameTrail.endColor;
            }
            if (_residualTrail != null)
            {
                _defaultResidualStartColor = _residualTrail.startColor;
                _defaultResidualEndColor = _residualTrail.endColor;
            }
        }

        public override void Setup(InteractionContext interactionContext, ProjectileData data, Action onReturnToPool, Vector2 targetPosition)
        {
            SetTrailsEmitting(false);
            base.Setup(interactionContext, data, onReturnToPool, targetPosition);

            if (_flameTrail != null)
            {
                _flameTrail.startColor = _defaultFlameStartColor;
                _flameTrail.endColor = _defaultFlameEndColor;
            }
            if (_residualTrail != null)
            {
                _residualTrail.startColor = _defaultResidualStartColor;
                _residualTrail.endColor = _defaultResidualEndColor;
            }

            ClearTrails();
            SetTrailsEmitting(true);

            if (_light != null)
            {
                _light.intensity = _defaultLightIntensity;
                _light.enabled = true;
            }
        }

        public override bool TryExplode(bool isContinuous = false)
        {
            if (isReturned) return false;
            if (!base.TryExplode(isContinuous: true)) return false;
            if (!isContinuous)
            {
                isReturned = true;
                StartCoroutine(FadeTrailsAndReleaseRoutine());
            }

            return true;
        }

    private IEnumerator FadeTrailsAndReleaseRoutine()
    {
        SetTrailsEmitting(false);
        float fadeDuration = GetMaxTrailLifetime() / 2;

        if (fadeDuration > 0f)
        {
            float elapsed = 0f;
            float startLightIntensity = _light != null ? _light.intensity : 0f;

            Color flameStartColor = _flameTrail != null ? _flameTrail.startColor : Color.white;
            Color flameEndColor = _flameTrail != null ? _flameTrail.endColor : Color.white;
            Color residualStartColor = _residualTrail != null ? _residualTrail.startColor : Color.white;
            Color residualEndColor = _residualTrail != null ? _residualTrail.endColor : Color.white;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / fadeDuration;

                if (_light != null) _light.intensity = Mathf.Lerp(startLightIntensity, 0f, progress);
                if (_flameTrail != null)
                {
                    _flameTrail.startColor = new Color(flameStartColor.r, flameStartColor.g, flameStartColor.b, Mathf.Lerp(flameStartColor.a, 0f, progress));
                    _flameTrail.endColor = new Color(flameEndColor.r, flameEndColor.g, flameEndColor.b, Mathf.Lerp(flameEndColor.a, 0f, progress));
                }
                if (_residualTrail != null)
                {
                    _residualTrail.startColor = new Color(residualStartColor.r, residualStartColor.g, residualStartColor.b, Mathf.Lerp(residualStartColor.a, 0f, progress));
                    _residualTrail.endColor = new Color(residualEndColor.r, residualEndColor.g, residualEndColor.b, Mathf.Lerp(residualEndColor.a, 0f, progress));
                }

                yield return null;
            }
        }
            if (_light != null) _light.enabled = false;
            ReleaseToPool();
        }

        private float GetMaxTrailLifetime()
        {
            float t1 = _flameTrail != null ? _flameTrail.time : 0f;
            float t2 = _residualTrail != null ? _residualTrail.time : 0f;
            return Mathf.Max(t1, t2);
        }

        private void ClearTrails()
        {
            if (_flameTrail != null) _flameTrail.Clear();
            if (_residualTrail != null) _residualTrail.Clear();
        }

        private void SetTrailsEmitting(bool emit)
        {
            if (_flameTrail != null) _flameTrail.emitting = emit;
            if (_residualTrail != null) _residualTrail.emitting = emit;
        }
    }
}