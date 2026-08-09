using System;
using System.Collections;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

namespace Assets.Scripts.Actions.VFX
{
    public class VfxInstance : MonoBehaviour
    {
        [Header("Visuals")]
        [SerializeField] private ParticleSystem[] _particleSystems;

        [Header("Audio (FMOD)")]
        [Tooltip("Для одиночных выстрелов/взрывов")]
        [SerializeField] private EventReference _mainSound;
        [Tooltip("Для скорострельного оружия (Timeline 3D с петлей и хвостом)")]
        [SerializeField] private EventReference _secondarySound;

        [Header("Timing Settings")]
        [Tooltip("Пауза между выстрелами, после которой стрельба считается оконченной")]
        [SerializeField] private float _loopStopTimeout = 0.15f;

        private Action _onRelease;
        private EventInstance _loopingSoundInstance;
        private Coroutine _releaseCoroutine;
        private float _particlesDuration;

        private void Awake()
        {
            _particlesDuration = CalculateMaxParticleDuration();
        }

        public void Play(Vector3 position, Quaternion rotation, Action onRelease)
        {
            transform.SetPositionAndRotation(position, rotation);
            _onRelease = onRelease;

            // --- СЦЕНАРИЙ 1: Продолжение очереди выстрелов ---
            if (gameObject.activeSelf)
            {
                PlayParticles();

                if (_loopingSoundInstance.isValid())
                {
                    _loopingSoundInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
                }

                // Продлеваем время жизни цикла
                ResetReleaseTimer();
                return;
            }

            // --- СЦЕНАРИЙ 2: Первый выстрел ---
            gameObject.SetActive(true);
            PlayParticles();

            // Одиночный звук играет ТОЛЬКО если нет зацикленного события
            if (!_mainSound.IsNull && _secondarySound.IsNull)
            {
                RuntimeManager.PlayOneShot(_mainSound, transform.position);
            }

            // Запускаем 3D-петлю
            if (!_secondarySound.IsNull)
            {
                StartLoopingSound();
            }

            ResetReleaseTimer();
        }

        private void StartLoopingSound()
        {
            if (_secondarySound.IsNull) return;

            if (!_loopingSoundInstance.isValid()) _loopingSoundInstance = RuntimeManager.CreateInstance(_secondarySound);

            _loopingSoundInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
            _loopingSoundInstance.start();
        }

        private void ResetReleaseTimer()
        {
            if (_releaseCoroutine != null) StopCoroutine(_releaseCoroutine);

            float waitTime = !_secondarySound.IsNull ? _loopStopTimeout : _particlesDuration;
            _releaseCoroutine = StartCoroutine(WaitAndReleaseRoutine(waitTime));
        }

        private IEnumerator WaitAndReleaseRoutine(float waitTime)
        {
            if (waitTime > 0) yield return new WaitForSeconds(waitTime);

            if (_loopingSoundInstance.isValid())
            {
                _loopingSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

                PLAYBACK_STATE playbackState;
                _loopingSoundInstance.getPlaybackState(out playbackState);

                while (playbackState != PLAYBACK_STATE.STOPPED)
                {
                    yield return null;
                    if (!_loopingSoundInstance.isValid()) break;
                    _loopingSoundInstance.getPlaybackState(out playbackState);
                }

                _loopingSoundInstance.release();
            }

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
            if (_loopingSoundInstance.isValid())
            {
                _loopingSoundInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                _loopingSoundInstance.release();
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