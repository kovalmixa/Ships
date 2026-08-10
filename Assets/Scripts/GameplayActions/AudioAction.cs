using System.Collections;
using Assets.Common;
using Assets.Scripts.Actions;
using FMODUnity;
using FMOD.Studio;
using GameplayActions;
using UnityEngine;

namespace Assets.Scripts.GameplayActions.Audio
{
    [System.Serializable]
    public class AudioData : ActionData
    {
        [SerializeField] private EventReference _sound;
        [SerializeField] private bool _isOneShot = false;
        [SerializeField] private float _loopStopTimeout = 0.15f;

        public EventReference Sound => _sound;
        public bool IsOneShot => _isOneShot;
        public float LoopStopTimeout => _loopStopTimeout;
    }

    public class AudioAction : GameplayAction<AudioData>
    {
        private EventInstance _activeInstance;
        private Coroutine _stopCoroutine;

        protected override void ExecuteAction(InteractionContext context, AudioData data, Vector2 targetPos)
        {
            ExecuteAudio(context, data, targetPos, null);
        }

        protected override void ExecuteAction(InteractionContext context, AudioData data, IInteractive target)
        {
            Vector3 position = target != null ? target.GameObject.transform.position : context.SourceObject.transform.position;
            ExecuteAudio(context, data, position, target?.GameObject.transform);
        }

        private void ExecuteAudio(InteractionContext context, AudioData data, Vector3 position, Transform followTarget)
        {
            if (data.Sound.IsNull) return;

            if (data.IsOneShot)
            {
                RuntimeManager.PlayOneShot(data.Sound, position);
                return;
            }

            MonoBehaviour runner = context.SourceObject.GetComponent<MonoBehaviour>();
            if (runner == null)
            {
                Debug.LogError("[AudioAction] InteractionContext.Source должен быть MonoBehaviour для запуска корутин!");
                return;
            }

            if (_activeInstance.isValid() && IsInstancePlaying(_activeInstance))
            {
                Update3DAttributes(position, followTarget);
                ResetStopTimer(runner, data.LoopStopTimeout);
                return;
            }

            _activeInstance = RuntimeManager.CreateInstance(data.Sound);
            Update3DAttributes(position, followTarget);
            _activeInstance.start();

            ResetStopTimer(runner, data.LoopStopTimeout);
        }

        private void ResetStopTimer(MonoBehaviour runner, float timeout)
        {
            if (_stopCoroutine != null) runner.StopCoroutine(_stopCoroutine);
            _stopCoroutine = runner.StartCoroutine(WaitAndStopRoutine(timeout));
        }

        private IEnumerator WaitAndStopRoutine(float timeout)
        {
            if (timeout > 0) yield return new WaitForSeconds(timeout);

            if (_activeInstance.isValid())
            {
                _activeInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                PLAYBACK_STATE state;
                _activeInstance.getPlaybackState(out state);

                while (state != PLAYBACK_STATE.STOPPED)
                {
                    yield return null;
                    if (!_activeInstance.isValid()) break;
                    _activeInstance.getPlaybackState(out state);
                }

                _activeInstance.release();
            }

            _stopCoroutine = null;
        }

        private void Update3DAttributes(Vector3 position, Transform target)
        {
            if (!_activeInstance.isValid()) return;

            if (target != null) _activeInstance.set3DAttributes(RuntimeUtils.To3DAttributes(target.gameObject));
            else _activeInstance.set3DAttributes(RuntimeUtils.To3DAttributes(position));
        }

        private bool IsInstancePlaying(EventInstance instance)
        {
            PLAYBACK_STATE state;
            instance.getPlaybackState(out state);
            return state != PLAYBACK_STATE.STOPPED;
        }
    }
}