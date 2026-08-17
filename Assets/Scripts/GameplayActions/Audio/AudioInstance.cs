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

    private Coroutine _stopCoroutine;
    private readonly Action _onStopped;

    public AudioInstance(InteractionContext context, AudioData data, Action onStopped)
    {
        _runner = context.SourceObject.GetComponent<MonoBehaviour>();
        _event = RuntimeManager.CreateInstance(data.sound);
        _onStopped = onStopped;
        AudioParametrsHandler.Apply(_event, data.parameters, context.AudioParameterSource);

    }

    public void Start(Vector3 position, Transform target, float timeout)
    {
        UpdatePosition(position, target);
        _event.start();
        ResetTimer(timeout);
    }

    public void Refresh(Vector3 position, Transform target, float timeout)
    {
        UpdatePosition(position, target);
        _event.getPlaybackState(out var state);

        if (state == PLAYBACK_STATE.STOPPED || state == PLAYBACK_STATE.STOPPING) _event.start();
        ResetTimer(timeout);
    }

    private void ResetTimer(float timeout)
    {
        _event.getPlaybackState(out var state);
        _event.isVirtual(out var isVirtual);
        _event.getVolume(out var volume, out var finalVolume);
        if (_stopCoroutine != null) _runner.StopCoroutine(_stopCoroutine);
        _stopCoroutine = _runner.StartCoroutine(StopRoutine(timeout));
    }

    private IEnumerator StopRoutine(float timeout)
    {
        if (timeout > 0) yield return new WaitForSeconds(timeout);

        _event.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        PLAYBACK_STATE state;

        do
        {
            yield return null;
            if (!_event.isValid()) break;
            _event.getPlaybackState(out state);

        } while (state != PLAYBACK_STATE.STOPPED);

        _event.release();
        _stopCoroutine = null;
        _onStopped?.Invoke();
    }

    private void UpdatePosition(Vector3 position, Transform target)
    {
        if (!_event.isValid()) return;
        if (target != null) _event.set3DAttributes(RuntimeUtils.To3DAttributes(target.gameObject));
        else _event.set3DAttributes(RuntimeUtils.To3DAttributes(position));
    }
}