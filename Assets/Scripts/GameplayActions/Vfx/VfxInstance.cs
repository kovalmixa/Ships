using System;
using System.Collections;
using UnityEngine;
using FMODUnity;

namespace Assets.Scripts.Actions.VFX
{
    public class VfxInstance : MonoBehaviour
    {
        [Header("Visuals")]
        [SerializeField] private ParticleSystem[] _particleSystems;

        [Header("Audio (FMOD)")]
        [SerializeField] private EventReference _mainSound;
        [SerializeField] private EventReference _secondarySound;

        private Action _onRelease;
        private FMOD.Studio.EventInstance _loopingSoundInstance;

        public void Play(Vector3 position, Quaternion rotation, Action onRelease)
        {
            _onRelease = onRelease;
            transform.SetPositionAndRotation(position, rotation);
            gameObject.SetActive(true);

            float maxDuration = PlayParticles();
            PlaySounds();

            StartCoroutine(WaitAndReleaseRoutine(maxDuration));
        }

        private float PlayParticles()
        {
            float maxDuration = 0f;
            if (_particleSystems == null || _particleSystems.Length == 0) return maxDuration;

            foreach (var ps in _particleSystems)
            {
                if (ps == null) continue;

                ps.Play(true);

                float duration = ps.main.duration + ps.main.startLifetime.constantMax;
                if (duration > maxDuration) maxDuration = duration;
            }

            return maxDuration;
        }

        private void PlaySounds()
        {
            if (!_mainSound.IsNull) RuntimeManager.PlayOneShot(_mainSound, transform.position);

            if (!_secondarySound.IsNull)
            {
                _loopingSoundInstance = RuntimeManager.CreateInstance(_secondarySound);
                _loopingSoundInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
                _loopingSoundInstance.start();
                _loopingSoundInstance.release();
            }
        }

        private IEnumerator WaitAndReleaseRoutine(float waitTime)
        {
            if (waitTime > 0 && waitTime < Mathf.Infinity) yield return new WaitForSeconds(waitTime);
            else yield return new WaitForSeconds(1f);

            ReleaseToPool();
        }

        public void ReleaseToPool()
        {
            if (_loopingSoundInstance.isValid()) _loopingSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

            if (_particleSystems != null)
                foreach (var ps in _particleSystems)
                    if (ps != null) ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            _onRelease?.Invoke();
            _onRelease = null;
        }

        private void OnDisable()
        {
            if (_loopingSoundInstance.isValid()) _loopingSoundInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
    }
}