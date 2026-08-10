using Assets.Scripts.GameplayActions.Audio;
using GameplayActions;
using System;
using System.Collections;
using UnityEditor.PackageManager;
using UnityEngine;

namespace Assets.Scripts.Actions.VFX
{
    public class VfxInstance : MonoBehaviour
    {
        [SerializeField] private ParticleSystem[] _particleSystems;
        [SerializeField] private AudioData _audioData;

        private Action _onRelease;
        private Coroutine _releaseCoroutine;
        private float _particlesDuration;

        private void Awake()
        {
            _particlesDuration = CalculateMaxParticleDuration();
        }

        public void Play(InteractionContext context, Vector3 position, Quaternion rotation, Action onRelease)
        {
            transform.SetPositionAndRotation(position, rotation);
            _onRelease = onRelease;
            if (_releaseCoroutine != null) StopCoroutine(_releaseCoroutine);

            gameObject.SetActive(true);
            PlayParticles();

            if (_audioData != null && context != null) ActionProvider.Audio.Execute(context, _audioData, position);
            _releaseCoroutine = StartCoroutine(WaitAndReleaseRoutine(_particlesDuration));
        }

        private IEnumerator WaitAndReleaseRoutine(float waitTime)
        {
            if (waitTime > 0) yield return new WaitForSeconds(waitTime);
            ReleaseToPool();
        }

        public void ReleaseToPool()
        {
            if (_particleSystems != null)
                foreach (var ps in _particleSystems)
                    if (ps != null) ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            _onRelease?.Invoke();
            _onRelease = null;
            _releaseCoroutine = null;
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            if (_releaseCoroutine != null)
            {
                StopCoroutine(_releaseCoroutine);
                _releaseCoroutine = null;
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