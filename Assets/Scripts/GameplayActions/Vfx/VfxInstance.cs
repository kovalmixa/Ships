using Assets.Scripts.GameplayActions.Audio;
using GameplayActions;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Assets.Scripts.Actions.VFX
{
    public class VfxInstance : MonoBehaviour
    {
        [SerializeField] private ParticleSystem[] _particleSystems;
        [SerializeField] private AudioData _audioData;

        [Header("Light tools")]
        [SerializeField] private Light2D _light;
        [SerializeField] private float _time = 0.5f;
        [SerializeField]
        private AnimationCurve _lightIntencityCurve = new AnimationCurve(
            new Keyframe(0f, 1f), new Keyframe(1f, 0f)
        );

        private Action _onRelease;
        private Coroutine _releaseCoroutine;
        private Coroutine _lightCoroutine;

        private float _particlesDuration;
        private float _defaultLightIntensity;

        private void Awake()
        {
            _particlesDuration = CalculateMaxParticleDuration();
            if (_light != null) _defaultLightIntensity = _light.intensity;
        }

        public void Play(InteractionContext context, Vector3 position, Quaternion rotation, Action onRelease)
        {
            transform.SetPositionAndRotation(position, rotation);
            _onRelease = onRelease;

            StopActiveCoroutines();

            gameObject.SetActive(true);
            PlayParticles();

            if (_light != null && _time > 0f)
                _lightCoroutine = StartCoroutine(AnimateLightRoutine());

            if (_audioData != null && context != null)
                ActionProvider.Audio.Execute(context, _audioData, position);

            float totalLifetime = Mathf.Max(_particlesDuration, _time);
            _releaseCoroutine = StartCoroutine(WaitAndReleaseRoutine(totalLifetime));
        }

        private IEnumerator AnimateLightRoutine()
        {
            _light.enabled = true;
            float elapsed = 0f;

            while (elapsed < _time)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / _time);
                _light.intensity = _lightIntencityCurve.Evaluate(progress) * _defaultLightIntensity;

                yield return null;
            }

            _light.intensity = _lightIntencityCurve.Evaluate(1f) * _defaultLightIntensity;
            _light.enabled = false;
        }

        private IEnumerator WaitAndReleaseRoutine(float waitTime)
        {
            if (waitTime > 0) yield return new WaitForSeconds(waitTime);
            ReleaseToPool();
        }

        public void ReleaseToPool()
        {
            StopActiveCoroutines();

            if (_particleSystems != null)
                foreach (var ps in _particleSystems)
                    if (ps != null) ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            if (_light != null)
            {
                _light.enabled = false;
                _light.intensity = _defaultLightIntensity;
            }

            _onRelease?.Invoke();
            _onRelease = null;
            gameObject.SetActive(false);
        }

        private void OnDisable() => StopActiveCoroutines();

        private void StopActiveCoroutines()
        {
            if (_releaseCoroutine != null)
            {
                StopCoroutine(_releaseCoroutine);
                _releaseCoroutine = null;
            }
            if (_lightCoroutine != null)
            {
                StopCoroutine(_lightCoroutine);
                _lightCoroutine = null;
            }
        }

        private float CalculateMaxParticleDuration()
        {
            float maxDuration = 0f;
            if (_particleSystems == null || _particleSystems.Length == 0) return maxDuration;
            foreach (var ps in _particleSystems)
            {
                if (ps == null) continue;
                float duration = ps.main.duration + ps.main.startLifetime.constantMax;
                if (duration > maxDuration) maxDuration = duration;
            }
            return maxDuration;
        }

        private void PlayParticles()
        {
            if (_particleSystems == null) return;
            foreach (var ps in _particleSystems) if (ps != null) ps.Play(true);
        }
    }
}