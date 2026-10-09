using Assets.Scripts.Actions;
using Assets.Scripts.GameplayActions.Audio;
using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using UnityEngine;

public sealed class AudioInstance
{
    private readonly MonoBehaviour _runner;
    private readonly EventInstance _event;
    private readonly AudioData _data;

    private Coroutine _stopCoroutine;
    private readonly Action _onStopped;

    public AudioInstance(InteractionContext context, AudioData data, Action onStopped)
    {
        _data = data;
        _runner = context.SourceObject.GetComponent<MonoBehaviour>();
        _onStopped = onStopped;

        // Для OneShot звуков с зацикливанием/без можно использовать стандартный механизм FMOD
        if (!data.isOneShot)
        {
            _event = RuntimeManager.CreateInstance(data.sound);
            AudioParametrsHandler.Apply(_event, data.parameters, context.AudioParameterSource);
        }
    }

    public void Start(Vector3 position, Transform target, float timeout)
    {
        // Выбираем таймаут: приоритет переданному timeout, затем loopStopTimeout из AudioData
        float effectiveTimeout = timeout > 0 ? timeout : _data.loopStopTimeout;

        if (_data.isOneShot)
        {
            PlayOneShot(position, target);
            // Если для ваншота задан таймаут, планируем только событие завершения
            if (effectiveTimeout > 0 && _runner != null)
            {
                ResetTimer(effectiveTimeout);
            }
            return;
        }

        UpdatePosition(position, target);
        _event.start();
        ResetTimer(effectiveTimeout);
    }

    public void Refresh(Vector3 position, Transform target, float timeout)
    {
        float effectiveTimeout = timeout > 0 ? timeout : _data.loopStopTimeout;

        if (_data.isOneShot)
        {
            // Для OneShot просто проигрываем повторно
            PlayOneShot(position, target);
            return;
        }

        UpdatePosition(position, target);
        _event.getPlaybackState(out var state);

        if (state == PLAYBACK_STATE.STOPPED || state == PLAYBACK_STATE.STOPPING)
        {
            _event.start();
        }

        ResetTimer(effectiveTimeout);
    }

    public void Stop(bool immediate = false)
    {
        if (_stopCoroutine != null && _runner != null)
        {
            _runner.StopCoroutine(_stopCoroutine);
            _stopCoroutine = null;
        }

        if (_event.isValid())
        {
            var stopMode = immediate ? FMOD.Studio.STOP_MODE.IMMEDIATE : FMOD.Studio.STOP_MODE.ALLOWFADEOUT;
            _event.stop(stopMode);
            _event.release();
        }

        _onStopped?.Invoke();
    }

    private void PlayOneShot(Vector3 position, Transform target)
    {
        if (target != null)
        {
            RuntimeManager.PlayOneShotAttached(_data.sound, target.gameObject);
        }
        else
        {
            RuntimeManager.PlayOneShot(_data.sound, position);
        }
    }

    private void ResetTimer(float timeout)
    {
        if (_runner == null) return;

        if (_stopCoroutine != null)
        {
            _runner.StopCoroutine(_stopCoroutine);
        }

        if (timeout > 0)
        {
            _stopCoroutine = _runner.StartCoroutine(StopRoutine(timeout));
        }
    }

    private IEnumerator StopRoutine(float timeout)
    {
        yield return new WaitForSeconds(timeout);

        if (_data.isOneShot)
        {
            _stopCoroutine = null;
            _onStopped?.Invoke();
            yield break;
        }

        if (!_event.isValid())
        {
            _stopCoroutine = null;
            _onStopped?.Invoke();
            yield break;
        }

        // Посылаем команду остановки с плавным затуханием
        _event.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        // Страховочный таймер: если звук не остановится сам за 2 секунды (из-за бесконечного лупа без FadeOut),
        // принудительно останавливаем его (IMMEDIATE)
        float maxWaitTime = 2.0f;
        float elapsedTime = 0f;
        PLAYBACK_STATE state;

        do
        {
            yield return null;
            elapsedTime += Time.deltaTime;

            if (!_event.isValid()) break;
            _event.getPlaybackState(out state);

            if (elapsedTime >= maxWaitTime && state != PLAYBACK_STATE.STOPPED)
            {
                // Принудительно глушим звук, если он застрял в цикле
                _event.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                break;
            }

        } while (state != PLAYBACK_STATE.STOPPED);

        _event.release();
        _stopCoroutine = null;
        _onStopped?.Invoke();
    }

    private void UpdatePosition(Vector3 position, Transform target)
    {
        if (!_event.isValid()) return;

        if (target != null)
            _event.set3DAttributes(RuntimeUtils.To3DAttributes(target.gameObject));
        else
            _event.set3DAttributes(RuntimeUtils.To3DAttributes(position));
    }
}